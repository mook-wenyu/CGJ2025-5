using System.Collections;
using UnityEngine;

public class Robot : MonoBehaviour
{

    [Header("图片")]
    public Sprite[] imgs;

    private GameObject partsObj;

    private SpriteRenderer sr;

    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;

    private FurnitureData furniture;

    public Transform startPoint;
    [Header("所有目标位置")]
    public Transform[] positions;

    [Header("移动速度")]
    public float speed = 10f;

    private Coroutine launchCoroutine = null;
    private Coroutine sweepLoop = null;
    private Coroutine stateTickLoop = null;

    private HidePoint orderCtrl = null;
    private int lastIndex = -1;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();

        var p = transform.Find("Parts");
        if (p) partsObj = p.gameObject;
        if (partsObj) partsObj.SetActive(false);

        GetComponent<SpriteButton>().onClick.AddListener(OnClicked);
    }

    void Start()
    {
        SwitchStatus(status);
    }

    public void Launch(FurnitureData f)
    {
        furniture = f;
        Reset();

        currentAnger = furniture.startanger;
        SwitchStatus(status);

        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    IEnumerator LaunchCoroutine()
    {
        float waitTime = furniture.waitTime / GameMgr.timeScale;
        float elapsedTime = 0f;

        // 扫地机器人
        sweepLoop = StartCoroutine(SweepLoop());

        while (elapsedTime < waitTime)
        {
            // 在暂停时持续等待，直到游戏恢复
            if (!GameMgr.IsTimePaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        if (sweepLoop != null)
        {
            StopCoroutine(sweepLoop);
            sweepLoop = null;
            yield return null;
        }

        StartCoroutine(SwitchToSpecial(0));
    }

    IEnumerator SweepLoop()
    {
        while (true)
        {
            if (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            // 在Normal状态下随机在positions点之间移动
            if (status == FurnitureStatus.Normal && positions.Length > 0)
            {
                // 随机选择一个位置点
                int newIndex;
                do
                {
                    newIndex = Random.Range(0, positions.Length);
                } while (positions.Length > 1 && newIndex == lastIndex); // 避免和上次一样

                lastIndex = newIndex;
                Transform target = positions[newIndex];

                // 移动到目标位置
                while (Mathf.Abs(transform.position.x - target.position.x) > 0.05f)
                {
                    if (GameMgr.IsTimePaused)
                    {
                        yield return null;
                        continue;
                    }

                    if (status != FurnitureStatus.Normal)
                    {
                        break; // 如果状态改变，退出移动循环
                    }

                    // 只移动X轴
                    float newX = Mathf.MoveTowards(
                        transform.position.x,
                        target.position.x,
                        speed * GameMgr.timeScale * Time.deltaTime
                    );
                    transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                    yield return null;
                }

                // 在每个位置短暂停留
                if (status == FurnitureStatus.Normal)
                {
                    float pauseTime = 1.0f / GameMgr.timeScale;
                    float elapsedTime = 0f;
                    while (elapsedTime < pauseTime && status == FurnitureStatus.Normal)
                    {
                        if (!GameMgr.IsTimePaused)
                        {
                            elapsedTime += Time.deltaTime;
                        }
                        yield return null;
                    }
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 在目标位置隐藏
    /// </summary>
    /// <returns>隐藏协程</returns>
    private IEnumerator HideAtTarget()
    {
        if (positions.Length == 0) yield break;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, positions.Length);
        } while (positions.Length > 1 && newIndex == lastIndex); // 避免和上次一样

        lastIndex = newIndex;
        Transform target = positions[newIndex];

        // 移动到目标位置
        while (Mathf.Abs(transform.position.x - target.position.x) > 0.05f)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            // 只移动X轴
            float newX = Mathf.MoveTowards(
                transform.position.x,
                target.position.x,
                speed * GameMgr.timeScale * Time.deltaTime
            );
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            Debug.Log("隐藏 当前X:" + transform.position.x + " 目标X:" + target.position.x);
            yield return null;
        }

        Debug.Log("已到达目标点，进入状态1");
        orderCtrl = target.GetComponent<HidePoint>();
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToBottom();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }

        status = FurnitureStatus.Special;
        SwitchStatus(status);

        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
        }
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
            if (stateTickLoop != null)
            {
                StopCoroutine(stateTickLoop);
            }
            stateTickLoop = null;
            Debug.Log("进入状态3：扫地机器人失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            if (partsObj) partsObj.SetActive(true);
            Debug.Log("进入状态2：扫地机器人黑化");
            return;
        }
    }

    private void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
            sweepLoop = StartCoroutine(SweepLoop());

            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            StartCoroutine(SwitchToSpecial(delay));
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

        if (sweepLoop != null)
        {
            StopCoroutine(sweepLoop);
            sweepLoop = null;
            yield return null;
        }
        StartCoroutine(HideAtTarget());
    }

    void SwitchToNormal()
    {
        if (sweepLoop != null)
        {
            StopCoroutine(sweepLoop);
            sweepLoop = null;
        }

        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
            stateTickLoop = null;
        }

        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        currentAnger = furniture.startanger;

        if (partsObj) partsObj.SetActive(false);
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToTop();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }

        Debug.Log("进入状态0：扫地机器人恢复正常");
    }

    void Reset()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
        }

        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
            stateTickLoop = null;
        }

        transform.position = new Vector2(startPoint.position.x, transform.position.y);
        SwitchToNormal();

        if (sweepLoop != null)
        {
            StopCoroutine(sweepLoop);
            sweepLoop = null;
        }
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }

}
