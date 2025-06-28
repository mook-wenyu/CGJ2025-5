using System.Collections;
using UnityEngine;

public class bin : MonoBehaviour
{
    [Header("状态类型")]
    public int type = 0;               // 家具状态

    [Header("现在的愤怒值")]
    public int anger = 0;              // 愤怒值

    [Header("愤怒值增长速度(x秒增长1)")]
    public float negativespeed = 1f;   // 情绪积累速度（秒）

    [Header("触发间隔")]
    public float starttime = 1f;
    public float endtime = 10f;

    [Header("状态基线值")]
    public int startanger = 0;

    [Header("阶段阈值")]
    public int stage2 = 50;
    public int stage3 = 100;

    [Header("开始等待时间")]
    public float waittime = 10f;
    private bool isFirst_time_to_wait=false;
    private bool isReady = false;

    [Header("引用物体")]
    public Collider2D position1;
    public Collider2D position2;

    private bool isCoolingDown = false;
    private bool hasStartedDelay = false;

    private string startArea = ""; // 记录从哪里开始滑动
    private bool isMouseHeld = false;

    void Start()
    {
        type = 0;
        anger = startanger;
        StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        Debug.Log($"等待 {waittime} 秒后开始运行");
        yield return new WaitForSeconds(waittime);

        // 等待结束后开始执行逻辑
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
        isFirst_time_to_wait = true;
        isReady = true;

    }

    void Update()
    {
        if (!isReady) return;
        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(starttime, endtime);
            if (isFirst_time_to_wait)
            {
                delay= 0f;
                isFirst_time_to_wait= false;
            }
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(DelayToState1(delay));
        }

        HandleSwipeBetweenAreas();
    }

    void HandleSwipeBetweenAreas()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (position1.OverlapPoint(mouseWorldPos))
            {
                startArea = "position1";
                isMouseHeld = true;
            }
        }

        if (Input.GetMouseButton(0) && isMouseHeld && startArea == "position1")
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (position2.OverlapPoint(mouseWorldPos))
            {
                Debug.Log("从 position1 滑到了 position2，清空怒气");
                CoolDownToZero();
                isMouseHeld = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            startArea = "";
            isMouseHeld = false;
        }
    }

    void StateTick()
    {
        if (isCoolingDown) return;
        if (type == 3) return;

        if (type == 1 || type == 2)
        {
            anger++;
            Debug.Log($"愤怒值 = {anger}");
            if (anger >= stage2 && type == 1) type = 2;
            if (anger >= stage3 && type == 2) type = 3;
        }
    }

    IEnumerator DelayToState1(float delay)
    {
        yield return new WaitForSeconds(delay);
        type = 1;
        Debug.Log("进入状态1：开始积累愤怒");
    }

    void CoolDownToZero()
    {
        type = 0;
        anger = 0;
        isCoolingDown = false;
        hasStartedDelay = false;
        Debug.Log("怒气清空，回到状态0");
    }
}
