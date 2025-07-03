using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : MonoBehaviour
{
    [Header("图片")]
    public Sprite[] imgs;

    public GameObject lidObj;

    private GameObject partsObj;

    private Shake shake;

    private SpriteRenderer sr;

    private FurnitureData furniture;

    private int currentAnger = 0;
    private FurnitureStatus status = FurnitureStatus.Normal;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private KettleHead lid;

    private Coroutine launchCoroutine = null;
    private Coroutine stateTickLoop = null;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        shake = new Shake(this, transform.localPosition);
        var p = transform.Find("Parts");
        if (p) partsObj = p.gameObject;
        if (partsObj) partsObj.SetActive(false);

        lid = lidObj.GetComponent<KettleHead>();
        lid.SetOriginalPos(lidObj.transform.position);
        GetComponent<SpriteButton>().onClick.AddListener(OnClicked);
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

    public void Launch(FurnitureData f)
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
        float waitTime = furniture.waitTime;
        float elapsedTime = 0f;
        
        while (elapsedTime < waitTime)
        {
            // 在暂停时持续等待，直到游戏恢复
            if (!GameMgr.IsTimePaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
        
        isReady = true;
        isFirstWait = true;
        stateTickLoop = StartCoroutine(StateTickLoop());
    }

    IEnumerator StateTickLoop()
    {
        while (true)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            yield return new WaitForSeconds(furniture.angerSpeed / GameMgr.timeScale);
            StateTick();
        }
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

            if (partsObj) partsObj.SetActive(true);

            Debug.Log("进入状态2：烧水壶黑化");
            return;
        }
    }


    private void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }

    IEnumerator SwitchToSpecial(float delay)
    {
        float waitTime = delay / GameMgr.timeScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < waitTime)
        {
            // 在暂停时持续等待，直到游戏恢复
            if (!GameMgr.IsTimePaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
        
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

        if (partsObj) partsObj.SetActive(false);
        Debug.Log("进入状态0：烧水壶恢复正常");
    }

    void Reset()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = null;

        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
        }
        stateTickLoop = null;

        SwitchToNormal();
        shake.StopShaking();

        isReady = false;
        isFirstWait = false;
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];

        // 根据状态控制抖动
        switch (newStatus)
        {
            case FurnitureStatus.Normal:
                shake.StopShaking();
                break;
            case FurnitureStatus.Special:
                shake.SetSpecialShakeMultiplier();
                shake.StartShaking();
                break;
            case FurnitureStatus.Dark:
                shake.SetDarkShakeMultiplier();
                shake.StartShaking();
                break;
            case FurnitureStatus.Crazy:
                // Crazy状态停止抖动
                shake.StopShaking();
                break;
        }
    }
}
