using Unity.VisualScripting;
using UnityEngine;

public class airconditioning : MonoBehaviour
{
    public enum Status
    {
        S0,
        S1, 
        S2, 
        S3
    }
    public int type = 0;

    [Header("�ж�·����")]
    public Transform p2; // ״̬2������
    public Transform p3; // ״̬3������
    public float positionTolerance = 0.1f;
    [Header("ͼƬ")]
    public Sprite[] imgs;
    public GameObject smoke;

    private SpriteRenderer sr;
    private Status status;

    //public static bool isdie=false;

    private void Start()
    {
        sr=gameObject.GetComponent<SpriteRenderer>();
        status = Status.S0;
        SwitchStatus(status);
        smoke.SetActive(false);
    }

    void Update()
    {
        GameObject[] roaches = GameObject.FindGameObjectsWithTag("Cockroach");

        // û����� �� ״̬0
        if (roaches.Length == 0)
        {
            if (type != 0)
            {
                type = 0;
                status = Status.S0;
                SwitchStatus(status);
                smoke.SetActive(false);
                Debug.Log("�������� �� ״̬0");
            }
            return;
        }

        // ������������ǰ��״̬0 �� ״̬1
        if (type == 0)
        {
            type = 1;
            status = Status.S1;
            SwitchStatus(status);
            smoke.SetActive(false);
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
                    status = Status.S2;
                    SwitchStatus(status);
                    smoke.SetActive(true);
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
                    status = Status.S3;
                    SwitchStatus(status);
                    smoke.SetActive(false);
                    Debug.Log("����뵽�� position3 �� ״̬3");
                    //isdie = true;
                    break;
                }
            }
        }
    }

    void SwitchStatus(Status newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
