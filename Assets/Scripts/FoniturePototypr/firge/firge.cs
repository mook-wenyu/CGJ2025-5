using System.Collections;
using UnityEngine;

public class firge : MonoBehaviour
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

    private bool hasStartedDelay = false;
    private bool isCoolingDown = false;
    private bool isPaused = false;     // 是否暂停增长
    private bool isReady = false;
    private bool isFirst_time_to_wait=false;

    [Header("开始等待时间")]
    public float waittime = 10f;

    [Header("引用物体")]
    public GameObject sayObj;          // 子物体 say 的引用
    public GameObject dialogueUI;

    void Start()
    {
        type = 0;
        anger = startanger;

        if (sayObj != null)
            sayObj.SetActive(false); // 初始隐藏
        dialogueUI.SetActive(false);

        StartCoroutine(StartDelayed());
    }
    IEnumerator StartDelayed()
    {
        Debug.Log($"等待 {waittime} 秒后开始运行");
        yield return new WaitForSeconds(waittime);

        // 等待结束后开始执行逻辑
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
        isFirst_time_to_wait = true;
        isReady=true;

    }
    void Update()
    {
        if (!isReady) return;  // 没等够时间，不运行逻辑

        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == startanger)
        {
            hasStartedDelay = true;
            float delay = Random.Range(starttime, endtime);
            if(isFirst_time_to_wait=true)
            {
                delay = 0f;
            }
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(DelayToState1(delay));
        }
    }

    void StateTick()
    {
        if (isCoolingDown || isPaused) return;  
        if (type == 3) return;

        switch (type)
        {
            case 0:
                if (sayObj != null && sayObj.activeSelf)
                    sayObj.SetActive(false);
                break;

            case 1:
                if (sayObj != null && !sayObj.activeSelf)
                    sayObj.SetActive(true);

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

    //  交互行为：
    public void PauseAngerGrowth()
    {
        isPaused = true;
        Debug.Log(isPaused ? "已暂停怒气增长" : "怒气恢复增长");
    }

    public void ResetToCalm()
    {
        Debug.Log("点击 say：怒气清零，回到状态0");

        type = 0;
        anger = startanger;
        isCoolingDown = false;
        hasStartedDelay = false;
        isPaused = false;

        if (sayObj != null)
            sayObj.SetActive(false);
        dialogueUI.SetActive(false);
    }
}
