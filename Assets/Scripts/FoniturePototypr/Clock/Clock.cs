using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Clock : MonoBehaviour
{
    public Sprite[] imgs;

    public Transform clockH, clockM;

    private SpriteRenderer sr;

    private FurnitureStatus status = FurnitureStatus.Normal;

    /// <summary>
    /// 当前愤怒值
    /// </summary>
    private int currentAnger = 0;

    private Furniture furniture;

    private bool isPlaying = false;
    private bool isFirstWait = false;
    private bool hasStartedDelay = false;
    private Coroutine launchCoroutine = null;
    private Coroutine switchToSpecialCoroutine = null;

    void Awake()
    {
        clockH = transform.Find("ClockH");
        clockM = transform.Find("ClockM");
        sr = GetComponent<SpriteRenderer>();
        isPlaying = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchStatus(status);
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }
        if (!hasStartedDelay && status == FurnitureStatus.Normal && currentAnger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            if (isFirstWait)
            {
                isFirstWait = false;
                delay = 0f;
            }
            if (switchToSpecialCoroutine != null)
            {
                StopCoroutine(switchToSpecialCoroutine);
            }
            switchToSpecialCoroutine = StartCoroutine(SwitchToSpecial(delay));
            return;
        }

    }

    private void OnMouseDown()
    {
        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            InteractEvent();
        }
    }

    /// <summary>
    /// 交互事件
    /// </summary>
    void InteractEvent()
    {
        currentAnger = 0;
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
    }

    /// <summary>
    /// 启动
    /// </summary>
    public void Launch()
    {
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
        isPlaying = true;
        isFirstWait = true;
        InvokeRepeating(nameof(StateTick), 0f, furniture.angerSpeed);
    }

    /// <summary>
    /// 状态更新
    /// </summary>
    void StateTick()
    {
        if (!isPlaying)
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
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
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
