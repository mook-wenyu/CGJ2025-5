using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashMachine : MonoBehaviour
{
    
    private SpriteRenderer sr;

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

    [Header("附加装饰")]
    public plant plantObj;  // 拖入 plant GameObject
    public GameObject face;
    public SpriteRenderer fsr;
    public Sprite[] faceimg;

    private bool hasStartedDelay = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        type = 0;
        SwitchStatus(type);
        anger = startanger;
        fsr=face.GetComponent<SpriteRenderer>();
        if (plantObj != null)
            plantObj.gameObject.SetActive(false);  // 默认不显示
        face.gameObject.SetActive(false);
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

        if (plantObj != null)
        {
            if (type == 1 || type == 2)
            {
                if (!plantObj.gameObject.activeSelf)
                    plantObj.StartRotate(type);  // 显身并开始旋转
            }
            else
            {
                if (plantObj.gameObject.activeSelf)
                    plantObj.StopRotate();       // 隐身并停止旋转
            }
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
                    fsr.sprite = faceimg[1];
                    face.gameObject.SetActive(true);
                    SwitchStatus(type);
                    Debug.Log("进入状态2：家具开始震动");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"状态2：愤怒值 = {anger}");
                if (anger >= stage3)
                {
                    type = 3;
                    face.gameObject.SetActive(false);
                    SwitchStatus(type);
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
        SwitchStatus(type);
        fsr.sprite = faceimg[0];
        face.gameObject.SetActive(true);
        Debug.Log("状态0倒计时结束，进入状态1：开始积累愤怒");
    }

    void CoolDownToZero()
    {
        type = 0;
        SwitchStatus(type);
        anger = 0;
        face.gameObject.SetActive(false);
        if (plantObj != null)
            plantObj.StopRotate();
        hasStartedDelay = false;
    }
    public void InteractEvent()
    {
        CoolDownToZero();  
    }
    void SwitchStatus(int newStatus)
    {
        sr.sprite = imgs[newStatus];
    }
}
