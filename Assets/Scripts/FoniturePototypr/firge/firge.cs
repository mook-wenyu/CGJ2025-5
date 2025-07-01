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
    public GameObject dialogueUI;

    private SpriteRenderer sr;
    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;              // 愤怒值

    private Furniture furniture;

    private bool hasStartedDelay = false;
    private bool isPaused = false;     // 是否暂停增长
    private bool isReady = false;
    private bool isFirstWait = false;

    private Coroutine launchCoroutine = null;

    public List<ColdJokeConfig> dialogueContentList = new List<ColdJokeConfig>();

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();

        if (dialogueContentList.Count == 0)
        {
            CSVMgr.GetAll<ColdJokeConfig>().ForEach(config =>
            {
                dialogueContentList.Add(config);
            });
        }
    }

    void Start()
    {
        SwitchStatus(status);

        if (sayObj != null)
            sayObj.SetActive(false); // 初始隐藏
        dialogueUI.SetActive(false);
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
            Debug.Log($"状态0(正常)：冰箱将在 {furniture.waitTime:F1} 秒后进入状态1(特殊)");
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
        if (isPaused) return;


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
            dialogueUI.SetActive(false);

            SwitchStatus(status);
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            Debug.Log("进入状态3：冰箱失控");
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            Debug.Log("进入状态2：冰箱黑化");
            return;
        }
    }

    IEnumerator SwitchToSpecial(float delay)
    {
        yield return new WaitForSeconds(delay);

        status = FurnitureStatus.Special;
        SwitchStatus(status);
        hasStartedDelay = false;
        Debug.Log("进入状态1：冰箱进入特殊状态");
    }

    //  交互行为：
    public void PauseAngerGrowth()
    {
        isPaused = true;
        Debug.Log(isPaused ? "已暂停怒气增长" : "怒气恢复增长");
    }

    public void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        currentAnger = furniture.startanger;

        hasStartedDelay = false;
        isPaused = false;

        if (sayObj != null)
            sayObj.SetActive(false);
        dialogueUI.SetActive(false);

        Debug.Log("进入状态0：冰箱恢复正常");
    }

    public void ClickClose()
    {
        isPaused = false;

        if (sayObj != null)
        {
            sayObj.SetActive(true);
            sayObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        dialogueUI.SetActive(false);
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
