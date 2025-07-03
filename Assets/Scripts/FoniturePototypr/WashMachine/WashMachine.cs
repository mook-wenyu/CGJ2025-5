using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashMachine : MonoBehaviour
{
    [Header("图片")]
    public Sprite[] imgs;

    [Header("附加装饰")]
    public Sprite[] faceimg;
    public WMPlant plantObj;
    public GameObject face;

    private GameObject partsObj;

    private Shake shake;

    private SpriteRenderer fsr;

    private SpriteRenderer sr;

    private FurnitureData furniture;

    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private Coroutine launchCoroutine = null;
    private Coroutine stateTickLoop = null;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        fsr = face.GetComponent<SpriteRenderer>();
        shake = new Shake(this, transform.localPosition);
        var p = transform.Find("Parts");
        if (p) partsObj = p.gameObject;
        if (partsObj) partsObj.SetActive(false);

        GetComponent<SpriteButton>().onClick.AddListener(OnClicked);
    }

    void Start()
    {
        SwitchStatus(status);
        if (plantObj != null)
            plantObj.gameObject.SetActive(false);  // 默认不显示
        face.SetActive(false);
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
            Debug.Log($"状态0(正常)：滚筒洗衣机将在 {furniture.waitTime:F1} 秒后进入状态1(特殊)");
        }

        if (plantObj != null)
        {
            if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
            {
                if (!plantObj.gameObject.activeSelf)
                    plantObj.StartRotate(status);  // 显身并开始旋转
            }
            else
            {
                if (plantObj.gameObject.activeSelf)
                    plantObj.StopRotate();       // 隐身并停止旋转
            }
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
        float waitTime = furniture.waitTime / GameMgr.timeScale;
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
            face.SetActive(false);
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);
            Debug.Log("进入状态3：滚筒洗衣机失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            fsr.sprite = faceimg[1];
            face.SetActive(true);
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            if (partsObj) partsObj.SetActive(true);
            Debug.Log("进入状态2：滚筒洗衣机黑化");
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
        fsr.sprite = faceimg[0];
        face.SetActive(true);
        Debug.Log("进入状态1：滚筒洗衣机进入特殊状态");
    }

    void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        face.SetActive(false);
        if (plantObj != null)
            plantObj.StopRotate();

        currentAnger = furniture.startanger;
        hasStartedDelay = false;
        if (partsObj) partsObj.SetActive(false);
        Debug.Log("进入状态0：滚筒洗衣机恢复正常");
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
