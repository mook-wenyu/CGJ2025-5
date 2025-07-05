using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Firge : MonoBehaviour
{
    public Sprite[] imgs;

    [Header("引用物体")]
    public GameObject sayObj;          // 子物体 say 的引用
    public GameObject dialogueUIObj;

    private GameObject partsObj;

    private Shake shake;

    private SpriteRenderer sr;
    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;              // 愤怒值

    private FurnitureData furniture;

    private bool hasStartedDelay = false;
    private bool isReady = false;
    private bool isFirstWait = false;

    private Coroutine launchCoroutine = null;
    private Coroutine stateTickLoop = null;

    private DialogueUI dialogueUI;

    public List<ColdJokeConfig> dialogueContentList = new List<ColdJokeConfig>();

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        shake = new Shake(this, transform.localPosition);
        var p = transform.Find("Parts");
        if (p) partsObj = p.gameObject;
        if (partsObj) partsObj.SetActive(false);

        if (dialogueContentList.Count == 0)
        {
            ConfigMgr.GetAll<ColdJokeConfig>().ForEach(config =>
            {
                dialogueContentList.Add(config);
            });
        }

        dialogueUI = dialogueUIObj.GetComponent<DialogueUI>();
        if (sayObj != null)
        {
            sayObj.SetActive(true);
            sayObj.GetComponent<SpriteButton>().onClick.AddListener(sayObj.GetComponent<SayClick>().OnClicked);
        }
    }

    void Start()
    {
        SwitchStatus(status);

        if (sayObj != null)
            sayObj.SetActive(false); // 初始隐藏
        dialogueUI.HideDialogue();
    }

    void Update()
    {
        if (!isReady) return;  // 没等够时间

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
        if (GameMgr.IsTimePaused || !isReady) return;

        switch (status)
        {
            case FurnitureStatus.Normal:
                if (sayObj != null && sayObj.activeSelf)
                    sayObj.SetActive(false);
                break;

            case FurnitureStatus.Special:
                if (sayObj != null && !sayObj.activeSelf)
                {
                    sayObj.SetActive(true);
                    sayObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }

                AngerTick();
                break;

            case FurnitureStatus.Dark:
                AngerTick();
                break;
        }
    }

    /// <summary>
    /// 愤怒值更新
    /// </summary>
    void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;

            if (sayObj != null)
                sayObj.SetActive(false);
            dialogueUI.HideDialogue();

            SwitchStatus(status);
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            Debug.Log("进入状态3：冰箱失控");
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            if (partsObj) partsObj.SetActive(true);
            Debug.Log("进入状态2：冰箱黑化");
            return;
        }
    }

    IEnumerator SwitchToSpecial(float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
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
        if (sayObj != null && !sayObj.activeSelf)
        {
            sayObj.SetActive(true);
            sayObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        hasStartedDelay = false;
        Debug.Log("进入状态1：冰箱进入特殊状态");
    }

    //  交互行为：
    public void PauseAngerGrowth()
    {
        GameMgr.PauseTime();
    }

    public void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        currentAnger = furniture.startanger;

        hasStartedDelay = false;
        GameMgr.ResumeTime();

        if (sayObj != null)
            sayObj.SetActive(false);
        dialogueUI.HideDialogue();
        if (partsObj) partsObj.SetActive(false);
        Debug.Log("进入状态0：冰箱恢复正常");
    }

    public void ClickClose()
    {
        GameMgr.ResumeTime();

        if (sayObj != null)
        {
            sayObj.SetActive(true);
            sayObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        dialogueUI.HideDialogue();
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
