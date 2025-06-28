using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poto : MonoBehaviour
{
    public int type = 0;               // 家具状态
    public int anger = 0;              // 愤怒值
    public float negativespeed = 1f;   // 情绪积累速度（秒）

    private bool hasStartedDelay = false;
    private int clickCount = 0;
    private float clickTimer = 0f;
    private bool isCoolingDown = false;

    void Start()
    {
        type = 0;
        anger = 0;
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
    }

    void Update()
    {
        // 状态1~3时允许交互
        if (!isCoolingDown && type >= 1 && type <= 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InteractEvent(); // 触发交互事件
            }
        }

        // 状态恢复逻辑
        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(1f, 10f);
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(DelayToState1(delay));
        }
    }


    void StateTick()
    {
        if (isCoolingDown) return;
        if (type == 3)
            return;
        switch (type)
        {
            case 1:
                anger++;
                Debug.Log($"状态1：愤怒值 = {anger}");
                if (anger >= 60)
                {
                    type = 2;
                    Debug.Log("进入状态2：家具开始震动");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"状态2：愤怒值 = {anger}");
                if (anger >= 100)
                {
                    type = 3;
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
        Debug.Log("状态0倒计时结束，进入状态1：开始积累愤怒");
    }




    void CoolDownToZero()
    {
        isCoolingDown = true;
        Debug.Log("点击3次，怒气立即清零...");

        type = 0;
        anger = 0;

        isCoolingDown = false;
        hasStartedDelay = false;

        Debug.Log("怒气清零，状态回到0，等待重新进入状态1");
    }
    public void InteractEvent()
    {
        if (clickTimer <= 0f)
        {
            // 第一次点击，启动计时器
            clickTimer = 3f;
            clickCount = 1;
            Debug.Log("开始计时，点击1次");
        }
        else
        {
            clickCount++;
            Debug.Log($"已点击 {clickCount} 次");

            if (clickCount >= 3)
            {
                clickCount = 0;
                clickTimer = 0f;
                CoolDownToZero();
                return;
            }
        }

        // 继续倒计时（只要点击就执行）
        StartCoroutine(ClickTimerCountdown());
    }
    private IEnumerator ClickTimerCountdown()
    {
        float currentTime = clickTimer;

        while (clickTimer > 0f)
        {
            yield return null;
            clickTimer -= Time.deltaTime;

            // 提前退出如果点击数被清空
            if (clickCount == 0) yield break;
        }

        // 时间到，点击未完成 → 重置点击数
        clickCount = 0;
        clickTimer = 0f;
        Debug.Log("点击超时，重置点击次数");
    }
}
