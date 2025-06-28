using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashMachine : MonoBehaviour
{
    [Header("״̬����")]
    public int type = 0;               // �Ҿ�״̬

    [Header("���ڵķ�ŭֵ")]
    public int anger = 0;              // ��ŭֵ

    [Header("��ŭֵ�����ٶ�(x������1)")]
    public float negativespeed = 1f;   // ���������ٶȣ��룩

    [Header("�������")]
    public float starttime = 1f;
    public float endtime = 10f;

    [Header("״̬����ֵ")]
    public int startanger = 0;

    [Header("�׶���ֵ")]
    public int stage2 = 50;
    public int stage3 = 100;

    [Header("��ʼ�ȴ�ʱ��")]
    public float waittime = 10f;
    private bool isFirst_time_to_wait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    void Start()
    {
        type = 0;
        anger = startanger;
        StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        Debug.Log($"�ȴ� {waittime} ���ʼ����");
        yield return new WaitForSeconds(waittime);

        // �ȴ�������ʼִ���߼�
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
        isFirst_time_to_wait = true;
        isReady = true;

    }
    void Update()
    {
        if (!isReady) return;
        // ״̬�ָ��߼�
        if (!hasStartedDelay && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(1f, 10f);
            if(isFirst_time_to_wait)
            {
                delay = 0f;
                isFirst_time_to_wait= false;
            }
            Debug.Log($"״̬0������ {delay:F1} ������״̬1");
            StartCoroutine(DelayToState1(delay));
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
                Debug.Log($"״̬1����ŭֵ = {anger}");
                if (anger >= stage2)
                {
                    type = 2;
                    Debug.Log("����״̬2���Ҿ߿�ʼ��");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"״̬2����ŭֵ = {anger}");
                if (anger >= stage3)
                {
                    type = 3;
                    Debug.Log("����״̬3���Ҿ߱��ߣ�");
                    return;
                }
                break;


        }
    }

    IEnumerator DelayToState1(float delay)
    {
        yield return new WaitForSeconds(delay);
        type = 1;
        Debug.Log("״̬0����ʱ����������״̬1����ʼ���۷�ŭ");
    }

    void CoolDownToZero()
    {
        type = 0;
        anger = 0;
        hasStartedDelay = false;
    }
    public void InteractEvent()
    {
        CoolDownToZero();  
    }
}
