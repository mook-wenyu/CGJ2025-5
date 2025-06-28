using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poto : MonoBehaviour
{
    public int type = 0;               // �Ҿ�״̬
    public int anger = 0;              // ��ŭֵ
    public float negativespeed = 1f;   // ���������ٶȣ��룩

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
        // ״̬1~3ʱ������
        if (!isCoolingDown && type >= 1 && type <= 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InteractEvent(); // ���������¼�
            }
        }

        // ״̬�ָ��߼�
        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(1f, 10f);
            Debug.Log($"״̬0������ {delay:F1} ������״̬1");
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
        isCoolingDown = true;
        Debug.Log("���3�Σ�ŭ����������...");

        type = 0;
        anger = 0;

        isCoolingDown = false;
        hasStartedDelay = false;

        Debug.Log("ŭ�����㣬״̬�ص�0���ȴ����½���״̬1");
    }
    public void InteractEvent()
    {
        if (clickTimer <= 0f)
        {
            // ��һ�ε����������ʱ��
            clickTimer = 3f;
            clickCount = 1;
            Debug.Log("��ʼ��ʱ�����1��");
        }
        else
        {
            clickCount++;
            Debug.Log($"�ѵ�� {clickCount} ��");

            if (clickCount >= 3)
            {
                clickCount = 0;
                clickTimer = 0f;
                CoolDownToZero();
                return;
            }
        }

        // ��������ʱ��ֻҪ�����ִ�У�
        StartCoroutine(ClickTimerCountdown());
    }
    private IEnumerator ClickTimerCountdown()
    {
        float currentTime = clickTimer;

        while (clickTimer > 0f)
        {
            yield return null;
            clickTimer -= Time.deltaTime;

            // ��ǰ�˳��������������
            if (clickCount == 0) yield break;
        }

        // ʱ�䵽�����δ��� �� ���õ����
        clickCount = 0;
        clickTimer = 0f;
        Debug.Log("�����ʱ�����õ������");
    }
}
