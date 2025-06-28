using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : MonoBehaviour
{
    public enum Status
    {
        S0,
        S1,
        S2,
        S3
    }
    private SpriteRenderer sr;
    private Status status;

    [Header("图片")]
    public Sprite[] imgs;

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
    private bool isFirst_time_to_wait = false;
    private bool isReady = false;

    public head lid;

    private bool hasStartedDelay = false;

    void Start()
    {
        type = 0;
        sr = GetComponent<SpriteRenderer>();
        status = Status.S0;
        SwitchStatus(status);
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
        // 状态恢复逻辑
        if (!hasStartedDelay && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(1f, 10f);
            if(isFirst_time_to_wait)
            {
                delay = 0f;
                isFirst_time_to_wait= false;
            }
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(DelayToState1(delay));
        }
        // 控制盖子的显隐和抖动
        if (type == 1 || type == 2)
        {
            lid.StartShaking(type);
        }
        else
        {
            lid.StopShaking();
        }

    }

    private void OnMouseDown()
    {
        if(type >= 1 && type <= 2)
        {
            InteractEvent();
        }
    }


    void StateTick()
    {
        if (type == 3)
            return;
        switch (type)
        {
            case 1:
                anger++;
                Debug.Log($"状态1：愤怒值 = {anger}");
                if (anger >= stage2)
                {
                    type = 2;
                    status = Status.S2;
                    SwitchStatus(status);
                    Debug.Log("进入状态2：家具开始震动");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"状态2：愤怒值 = {anger}");
                if (anger >= stage3)
                {
                    type = 3;
                    status = Status.S3;
                    SwitchStatus(status);
                    Debug.Log("进入状态3：家具暴走！");
                    return;
                }
                break;


        }
    }

    IEnumerator DelayToState1(float delay)
    {
        yield return new WaitForSeconds(delay);
        type = 1;
        status = Status.S1;
        SwitchStatus(status);
        Debug.Log("状态0倒计时结束，进入状态1：开始积累愤怒");
    }

    void CoolDownToZero()
    {
        type = 0;
        anger = 0;
        status = Status.S0;
        SwitchStatus(status);
        hasStartedDelay = false;
    }
    public void InteractEvent()
    {
        CoolDownToZero();  
    }

    void SwitchStatus(Status newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
