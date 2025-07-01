using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : MonoBehaviour
{
    [Header("图片")]
    public Sprite[] imgs;

    public GameObject lidObj;

    private SpriteRenderer sr;

    private Furniture furniture;

    private int currentAnger = 0;
    private FurnitureStatus status = FurnitureStatus.Normal;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private KettleHead lid;

    private Coroutine launchCoroutine = null;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        lid = lidObj.GetComponent<KettleHead>();
        lid.SetOriginalPos(lidObj.transform.position);
        Debug.Log("originalPos: " + lidObj.transform.position);
    }

    void Start()
    {
        SwitchStatus(status);
        lidObj.SetActive(false);
    }

    void Update()
    {
        if (!isReady) return;

        // 状态恢复逻辑
        if (!hasStartedDelay && status == FurnitureStatus.Normal && currentAnger == furniture.startanger)
        {
            hasStartedDelay = true;
            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            if (isFirstWait)
            {
                delay = 0f;
                isFirstWait = false;
            }
            StartCoroutine(SwitchToSpecial(delay));
            Debug.Log($"状态0(正常)：烧水壶将在 {furniture.waitTime:F1} 秒后进入状态1(特殊)");
        }
    }

    public void Launch(Furniture f)
    {
        furniture = f;
        Reset();
        currentAnger = furniture.startanger;
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    IEnumerator LaunchCoroutine()
    {
        yield return new WaitForSeconds(furniture.waitTime);
        isReady = true;
        isFirstWait = true;
        InvokeRepeating(nameof(StateTick), 0f, furniture.angerSpeed);
    }

    void StateTick()
    {
        if (!isReady) return;

        switch (status)
        {
            case FurnitureStatus.Special:
                AngerTick();
                break;

            case FurnitureStatus.Dark:
                AngerTick();
                break;
        }
    }

    void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);

            lid.StopShaking();

            Debug.Log("进入状态3：烧水壶失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);

            lidObj.SetActive(true);
            lid.StartShaking(status);

            Debug.Log("进入状态2：烧水壶黑化");
            return;
        }
    }


    private void OnMouseDown()
    {
        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }

    IEnumerator SwitchToSpecial(float delay)
    {
        yield return new WaitForSeconds(delay);
        status = FurnitureStatus.Special;
        SwitchStatus(status);

        lidObj.SetActive(true);
        lid.StartShaking(status);

        hasStartedDelay = false;

        Debug.Log("进入状态1：烧水壶进入特殊状态");
    }

    void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        currentAnger = furniture.startanger;
        hasStartedDelay = false;

        lid.StopShaking();

        Debug.Log("进入状态0：烧水壶恢复正常");
    }

    void Reset()
    {
        SwitchToNormal();

        isReady = false;
        isFirstWait = false;

        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            CancelInvoke(nameof(StateTick));
        }
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
