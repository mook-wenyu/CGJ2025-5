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

    private Furniture furniture;

    private bool isReady = false;
    private bool isFirstWait = false;
    private bool hasStartedDelay = false;
    private Coroutine launchCoroutine = null;

    void Awake()
    {
        clockH = transform.Find("ClockH");
        clockM = transform.Find("ClockM");
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchStatus(status);
    }

    void Update()
    {
        if (!isReady)
        {
            return;
        }
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
            Debug.Log($"状态0(正常)：时钟将在 {furniture.waitTime:F1} 秒后进入状态1(特殊)");
            return;
        }

    }

    private void OnMouseDown()
    {
        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Launch(Furniture f)
    {
        furniture = f;

        Reset();

        currentAnger = furniture.startanger;
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
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
        InvokeRepeating(nameof(StateTick), 0f, furniture.angerSpeed);
    }

    /// <summary>
    /// 状态更新
    /// </summary>
    void StateTick()
    {
        if (!isReady)
            return;

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
        yield return new WaitForSeconds(delay);

        status = FurnitureStatus.Special;
        SwitchStatus(status);
        hasStartedDelay = false;
        Debug.Log("进入状态1：时钟进入特殊状态");
    }

    void SwitchToNormal()
    {
        currentAnger = furniture.startanger;
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        hasStartedDelay = false;
        Debug.Log("进入状态0：时钟恢复正常");
    }

    void Reset()
    {
        SwitchToNormal();

        isReady = false;
        isFirstWait = false;

        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            CancelInvoke(nameof(StateTick));
        }
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
