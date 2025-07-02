using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Sprite[] imgs;

    [HideInInspector]
    public Transform clockH, clockM;

    private SpriteRenderer sr;

    private FurnitureStatus status = FurnitureStatus.Normal;

    /// <summary>
    /// 当前愤怒值
    /// </summary>
    private int currentAnger = 0;

    private FurnitureData furniture;

    private bool isReady = false;
    private bool isFirstWait = false;
    private bool hasStartedDelay = false;
    private Coroutine launchCoroutine = null;
    private Coroutine stateTickLoop = null;
    private SpriteButton sBtn;

    // 时针旋转速度（度/秒）
    private float hourRotateSpeed = 5f;
    // 分针旋转速度（度/秒）
    private float minuteRotateSpeed = 60f;
    // 特殊状态下的速度倍率
    private float specialSpeedMultiplier = 20f;
    // 黑化状态下的速度倍率
    private float darkSpeedMultiplier = 60f;
    // 是否反向旋转
    private bool reverseRotation = false;
    // 随机方向变化计时器
    private float directionChangeTimer = 0f;
    // 随机方向变化间隔
    private float directionChangeInterval = 4f;

    void Awake()
    {
        clockH = transform.Find("ClockH");
        clockM = transform.Find("ClockM");
        sr = GetComponent<SpriteRenderer>();
        sBtn = GetComponent<SpriteButton>();
        sBtn.onClick.AddListener(OnClicked);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchStatus(status);
    }

    void Update()
    {
        if (GameMgr.IsTimePaused) return;

        // 更新时钟指针旋转
        UpdateClockHands();

        if (!isReady) return;

        if (!hasStartedDelay && status == FurnitureStatus.Normal && currentAnger == furniture.startanger)
        {
            hasStartedDelay = true;
            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            if (isFirstWait)
            {
                isFirstWait = false;
                delay = 0f;
            }
            StartCoroutine(SwitchToSpecial(delay));
            return;
        }
    }

    /// <summary>
    /// 更新时钟指针旋转
    /// </summary>
    private void UpdateClockHands()
    {
        float hourSpeed = hourRotateSpeed;
        float minuteSpeed = minuteRotateSpeed;

        // 根据状态调整旋转速度和行为
        switch (status)
        {
            case FurnitureStatus.Normal:
                // 正常状态下，正常旋转
                reverseRotation = false;
                break;

            case FurnitureStatus.Special:
                // 特殊状态下，加快旋转
                hourSpeed *= specialSpeedMultiplier;
                minuteSpeed *= specialSpeedMultiplier;

                // 随机切换旋转方向
                directionChangeTimer += Time.deltaTime;
                if (directionChangeTimer >= directionChangeInterval)
                {
                    directionChangeTimer = 0f;
                    reverseRotation = !reverseRotation;
                }
                break;

            case FurnitureStatus.Dark:
                // 黑化状态下，更快旋转
                hourSpeed *= darkSpeedMultiplier;
                minuteSpeed *= darkSpeedMultiplier;

                // 更频繁地随机切换旋转方向
                directionChangeTimer += Time.deltaTime;
                if (directionChangeTimer >= directionChangeInterval / 2f)
                {
                    directionChangeTimer = 0f;
                    reverseRotation = !reverseRotation;
                }
                break;

            case FurnitureStatus.Crazy:
                // 失控状态下，指针停止转动
                return;
        }

        // 应用旋转
        float direction = reverseRotation ? -1f : 1f;
        if (clockH != null)
        {
            clockH.Rotate(0, 0, -hourSpeed * direction * Time.deltaTime);
        }

        if (clockM != null)
        {
            clockM.Rotate(0, 0, -minuteSpeed * direction * Time.deltaTime);
        }
    }

    private void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Launch(FurnitureData f)
    {
        furniture = f;
        Reset();

        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    /// <summary>
    /// 启动协程
    /// </summary>
    /// <returns>协程</returns>
    IEnumerator LaunchCoroutine()
    {
        yield return new WaitForSeconds(furniture.waitTime);
        isReady = true;
        isFirstWait = true;
        stateTickLoop = StartCoroutine(StateTickLoop());
    }

    IEnumerator StateTickLoop()
    {
        while (true)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            yield return new WaitForSeconds(furniture.angerSpeed / GameMgr.timeScale);
            StateTick();
        }
    }

    /// <summary>
    /// 状态更新
    /// </summary>
    void StateTick()
    {
        if (GameMgr.IsTimePaused || !isReady) return;

        switch (status)
        {
            case FurnitureStatus.Special:
                AngerTick();
                break;
            case FurnitureStatus.Dark:
                AngerTick();
                break;
        }
    }

    /// <summary>
    /// 愤怒值更新
    /// </summary>
    void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);
            Debug.Log("进入状态3：时钟失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            GameMgr.Instance.SetTimeScale(GameMgr.TIME_SCALE_DARK);

            Debug.Log("进入状态2：时钟黑化");
            return;
        }
    }

    /// <summary>
    /// 切换到特殊状态
    /// </summary>
    /// <param name="delay">等待时间</param>
    /// <returns>协程</returns>
    IEnumerator SwitchToSpecial(float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            // 在暂停时持续等待，直到游戏恢复
            if (!GameMgr.IsTimePaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        status = FurnitureStatus.Special;
        SwitchStatus(status);

        GameMgr.Instance.SetTimeScale(GameMgr.TIME_SCALE_SPECIAL);
        hasStartedDelay = false;

        Debug.Log("进入状态1：时钟进入特殊状态");
    }

    void SwitchToNormal()
    {
        currentAnger = furniture.startanger;
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        GameMgr.Instance.SetTimeScale(1f);
        hasStartedDelay = false;
        directionChangeTimer = 0f;
        reverseRotation = false;
        Debug.Log("进入状态0：时钟恢复正常");
    }

    void Reset()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = null;
        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
        }
        stateTickLoop = null;

        SwitchToNormal();
        isReady = false;
        isFirstWait = false;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newStatus">新状态</param>
    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }

}
