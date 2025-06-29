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

    private Furniture furniture;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private Coroutine launchCoroutine = null;

    private BtnEntity btn;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        btn = transform.Find("lid").GetComponent<BtnEntity>();
        isReady = false;
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
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(SwitchToSpecial(delay));
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
                Debug.Log($"状态1：愤怒值 = {currentAnger}");
                break;
            case FurnitureStatus.Dark:
                AngerTick();
                Debug.Log($"状态2：愤怒值 = {currentAnger}");
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
            btn.OnClicked -= OnClicked;

            Debug.Log("进入状态3：家具暴走！");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            Debug.Log("进入状态2：家具开始震动");
            return;
        }

    }

    void OnClicked()
    {
        SwitchToNormal();
    }

    IEnumerator SwitchToSpecial(float delay)
    {
        yield return new WaitForSeconds(delay);
        status = FurnitureStatus.Special;
        SwitchStatus(status);
        btn.SetSprite(lidOpen);
        btn.SetAlpha(0.01f);
        btn.OnClicked += OnClicked;
        Debug.Log("进入状态1：开始积累愤怒");
    }

    void SwitchToNormal()
    {
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        btn.SetSprite(lidClose);
        btn.SetAlpha(1f);
        btn.OnClicked -= OnClicked;

        currentAnger = furniture.startanger;

        hasStartedDelay = false;

        Debug.Log("怒气清空，回到状态0");
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
