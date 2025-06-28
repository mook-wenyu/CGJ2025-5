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

    [Header("判断路径点")]
    public Transform p2; // 状态2触发点
    public Transform p3; // 状态3触发点
    public float positionTolerance = 0.1f;
    [Header("图片")]
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

        // 没有蟑螂 → 状态0
        if (roaches.Length == 0)
        {
            if (type != 0)
            {
                type = 0;
                status = Status.S0;
                SwitchStatus(status);
                smoke.SetActive(false);
                Debug.Log("无蟑螂存在 → 状态0");
            }
            return;
        }

        // 有蟑螂出生，当前是状态0 → 状态1
        if (type == 0)
        {
            type = 1;
            status = Status.S1;
            SwitchStatus(status);
            smoke.SetActive(false);
            Debug.Log("蟑螂出生 → 状态1");
        }

        // 状态1 → 状态2：任意蟑螂到达 position2
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
                    Debug.Log("有蟑螂到达 position2 → 状态2");
                    break;
                }
            }
        }

        // 状态2 → 状态3：任意蟑螂到达 position3
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
                    Debug.Log("有蟑螂到达 position3 → 状态3");
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
