using System.Collections;
using UnityEngine;

public class Airconditioning : MonoBehaviour
{
    [Header("图片")]
    public Sprite[] imgs;
    public GameObject smoke;

    private SpriteRenderer sr;

    private FurnitureData furniture;

    private FurnitureStatus status = FurnitureStatus.Normal;

    [Header("判断路径点")]
    public Transform p2; // 状态2触发点
    public Transform p3; // 状态3触发点

    public float positionTolerance = 0.1f;

    public CockroachControl cockcontrol;

    private Coroutine launchCoroutine = null;

    //public static bool isdie=false;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SwitchStatus(status);
        smoke.SetActive(false);
    }

    void Update()
    {
        GameObject[] roaches = GameObject.FindGameObjectsWithTag("Cockroach");

        // 没有蟑螂 → 状态0
        if (roaches.Length == 0)
        {
            if (status != FurnitureStatus.Normal)
            {
                status = FurnitureStatus.Normal;
                SwitchStatus(status);
                smoke.SetActive(false);
                Debug.Log("无蟑螂存在 → 状态0");
            }
            return;
        }

        // 有蟑螂出生，当前是状态0 → 状态1
        if (status == FurnitureStatus.Normal)
        {
            status = FurnitureStatus.Special;
            SwitchStatus(status);
            smoke.SetActive(false);
            Debug.Log("蟑螂出生 → 状态1");
        }

        // 状态1 → 状态2：任意蟑螂到达 position2
        if (status == FurnitureStatus.Special)
        {
            foreach (GameObject roach in roaches)
            {
                if (Vector3.Distance(roach.transform.position, p2.position) <= positionTolerance)
                {
                    status = FurnitureStatus.Dark;
                    SwitchStatus(status);
                    smoke.SetActive(true);
                    Debug.Log("有蟑螂到达 position2 → 状态2");
                    break;
                }
            }
        }

        // 状态2 → 状态3：任意蟑螂到达 position3
        if (status == FurnitureStatus.Dark)
        {
            foreach (GameObject roach in roaches)
            {
                if (Vector3.Distance(roach.transform.position, p3.position) <= positionTolerance)
                {
                    status = FurnitureStatus.Crazy;
                    SwitchStatus(status);
                    smoke.SetActive(false);
                    LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
                    Debug.Log("有蟑螂到达 position3 → 状态3");
                    //isdie = true;
                    break;
                }
            }
        }
    }

    public void Launch(FurnitureData f)
    {
        furniture = f;
        Reset();

        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    IEnumerator LaunchCoroutine()
    {
        yield return new WaitForSeconds(furniture.waitTime / GameMgr.timeScale);

        cockcontrol.Launch(furniture);
    }

    void Reset()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = null;

        cockcontrol.Reset();
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
