using System.Collections;
using UnityEngine;

public class bin : MonoBehaviour
{
    public int type = 0;               // �Ҿ�״̬
    public int anger = 0;              // ��ŭֵ
    public float negativespeed = 1f;   // ���������ٶȣ��룩

    private bool hasStartedDelay = false;
    private bool isCoolingDown = false;

    private int directionChangeCount = 0;
    private Vector3 lastMousePos;
    private int lastDirection = 0; // -1:��, 1:��, 0:δ��

    void Start()
    {
        type = 0;
        anger = 0;
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
    }

    void Update()
    {
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
        if (type == 3) return;

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
        Debug.Log("��������3�Σ�ŭ����������...");

        type = 0;
        anger = 0;
        directionChangeCount = 0;
        lastDirection = 0;
        isCoolingDown = false;
        hasStartedDelay = false;

        Debug.Log("ŭ�����㣬״̬�ص�0���ȴ����½���״̬1");
    }

    void OnMouseOver()
    {
        if (type >= 1 && type <= 2 && !isCoolingDown)
        {
            if (Input.GetMouseButton(0)) 
            {
                Vector3 currentMousePos = Input.mousePosition;
                float dx = currentMousePos.x - lastMousePos.x;

                int currentDirection = 0;
                if (Mathf.Abs(dx) > 20f) // ��������20���ز��ж�
                {
                    currentDirection = dx > 0 ? 1 : -1;

                    if (lastDirection != 0 && currentDirection != lastDirection)
                    {
                        directionChangeCount++;
                        Debug.Log($"��������ı䣬�� {directionChangeCount} ��");
                    }

                    lastDirection = currentDirection;
                    lastMousePos = currentMousePos;

                    if (directionChangeCount >= 3)
                    {
                        CoolDownToZero();
                    }
                }
            }
        }
    }

    void OnMouseEnter()
    {
        lastMousePos = Input.mousePosition;
        directionChangeCount = 0;
        lastDirection = 0;
    }
}
