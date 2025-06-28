using UnityEngine;

public class airconditioning : MonoBehaviour
{
    public int type = 0;

    [Header("�ж�·����")]
    public Transform p2; // ״̬2������
    public Transform p3; // ״̬3������
    public float positionTolerance = 0.1f;

    public static bool isdie=false;

    void Update()
    {
        GameObject[] roaches = GameObject.FindGameObjectsWithTag("Cockroach");

        // û����� �� ״̬0
        if (roaches.Length == 0)
        {
            if (type != 0)
            {
                type = 0;
                Debug.Log("�������� �� ״̬0");
            }
            return;
        }

        // ������������ǰ��״̬0 �� ״̬1
        if (type == 0)
        {
            type = 1;
            Debug.Log("������ �� ״̬1");
        }

        // ״̬1 �� ״̬2��������뵽�� position2
        if (type == 1)
        {
            foreach (GameObject roach in roaches)
            {
                if (Vector3.Distance(roach.transform.position, p2.position) <= positionTolerance)
                {
                    type = 2;
                    Debug.Log("����뵽�� position2 �� ״̬2");
                    break;
                }
            }
        }

        // ״̬2 �� ״̬3��������뵽�� position3
        if (type == 2)
        {
            foreach (GameObject roach in roaches)
            {
                if (Vector3.Distance(roach.transform.position, p3.position) <= positionTolerance)
                {
                    type = 3;
                    Debug.Log("����뵽�� position3 �� ״̬3");
                    isdie = true;
                    break;
                }
            }
        }
    }
}
