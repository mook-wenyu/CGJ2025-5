using System.Collections;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [Header("图片")]
    public Sprite[] imgs;

    public GameObject lid;
    public Sprite lidOpen;
    public Sprite lidClose;

    private SpriteRenderer sr;

    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;              // 愤怒值

    private FurnitureData furniture;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private Coroutine launchCoroutine = null;
    private Coroutine stateTickLoop = null;

    private SpriteButton btn;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        btn = transform.Find("lid").GetComponent<SpriteButton>();
    }

    void Start()
    {
        SwitchStatus(status);
        lid.SetActive(true);
        btn.SetSprite(lidClose);
        btn.SetAlpha(1f);
    }

    void Update()
    {
        if (!isReady) return;
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

    /// <summary>
    /// 愤怒值更新
    /// </summary>
    void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);

            btn.SetSprite(lidOpen);
            btn.SetAlpha(0.001f);
            btn.onClick.RemoveAllListeners();

            Debug.Log("进入状态3：垃圾桶失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            Debug.Log("进入状态2：垃圾桶黑化");
            return;
        }

    }

    private void OnClicked()
    {
        SwitchToNormal();
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
        btn.SetSprite(lidOpen);
        btn.SetAlpha(0.01f);
        btn.onClick.AddListener(OnClicked);
        Debug.Log("进入状态1：垃圾桶进入特殊状态");
    }

    void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        btn.SetSprite(lidClose);
        btn.SetAlpha(1f);
        btn.onClick.RemoveAllListeners();

        currentAnger = furniture.startanger;

        hasStartedDelay = false;

        Debug.Log("进入状态0：垃圾桶恢复正常");
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

        isReady = false;
        isFirstWait = false;
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
