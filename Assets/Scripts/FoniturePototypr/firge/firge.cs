using System.Collections;
using UnityEngine;

public class firge : MonoBehaviour
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

    private bool hasStartedDelay = false;
    private bool isCoolingDown = false;
    private bool isPaused = false;     // �Ƿ���ͣ����
    private bool isReady = false;
    private bool isFirst_time_to_wait=false;

    [Header("��ʼ�ȴ�ʱ��")]
    public float waittime = 10f;

    [Header("��������")]
    public GameObject sayObj;          // ������ say ������
    public GameObject dialogueUI;

    void Start()
    {
        type = 0;
        anger = startanger;

        if (sayObj != null)
            sayObj.SetActive(false); // ��ʼ����
        dialogueUI.SetActive(false);

        StartCoroutine(StartDelayed());
    }
    IEnumerator StartDelayed()
    {
        Debug.Log($"�ȴ� {waittime} ���ʼ����");
        yield return new WaitForSeconds(waittime);

        // �ȴ�������ʼִ���߼�
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
        isFirst_time_to_wait = true;
        isReady=true;

    }
    void Update()
    {
        if (!isReady) return;  // û�ȹ�ʱ�䣬�������߼�

        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == startanger)
        {
            hasStartedDelay = true;
            float delay = Random.Range(starttime, endtime);
            if(isFirst_time_to_wait=true)
            {
                delay = 0f;
            }
            Debug.Log($"״̬0������ {delay:F1} ������״̬1");
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
                Debug.Log($"״̬1����ŭֵ = {anger}");
                if (anger >= 60)
                {
                    type = 2;
                    Debug.Log("����״̬2���Ҿ߿�ʼ��");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"״̬2����ŭֵ = {anger}");
                if (anger >= 100)
                {
                    type = 3;
                    Debug.Log("����״̬3���Ҿ߱��ߣ�");
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

    //  ������Ϊ��
    public void PauseAngerGrowth()
    {
        isPaused = true;
        Debug.Log(isPaused ? "����ͣŭ������" : "ŭ���ָ�����");
    }

    public void ResetToCalm()
    {
        Debug.Log("��� say��ŭ�����㣬�ص�״̬0");

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
