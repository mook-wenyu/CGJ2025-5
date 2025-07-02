using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Robot : MonoBehaviour
{

    [Header("图片")]
    public Sprite[] imgs;

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
    private Coroutine stateTickLoop = null;

    private bool isFirstWait = false;
    private Coroutine moveCoroutine;
    private HidePoint orderCtrl = null;
    private int lastIndex = -1;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
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
        
        isFirstWait = true;
        StartCoroutine(IdleAndMoveLoop());
    }

    IEnumerator IdleAndMoveLoop()
    {
        // 记录上一次的索引

        while (true)
        {
            if (status != FurnitureStatus.Normal) yield break;

            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            if (isFirstWait)
            {
                delay = 0f;
                isFirstWait = false;
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < delay / GameMgr.timeScale)
            {
                // 在暂停时持续等待，直到游戏恢复
                if (!GameMgr.IsTimePaused)
                {
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }

            if (positions.Length == 0) yield break;

            int newIndex;
            do
            {
                newIndex = Random.Range(0, positions.Length);
            } while (positions.Length > 1 && newIndex == lastIndex); // 避免和上次一样

            lastIndex = newIndex;
            Transform target = positions[newIndex];
            moveCoroutine = StartCoroutine(MoveToTarget(target));
            yield break;
        }
    }


    IEnumerator MoveToTarget(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }
            
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * GameMgr.timeScale * Time.deltaTime
            );
            yield return null;
        }
        yield return new WaitForSeconds(0.5f / GameMgr.timeScale);
        Debug.Log("已到达目标点，进入状态1");
        orderCtrl = target.GetComponent<HidePoint>();
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToBottom();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }

        status = FurnitureStatus.Special;
        SwitchStatus(status);

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
        }
    }

    void SwitchToNormal()
    {
        if (stateTickLoop != null)
        {
            StopCoroutine(stateTickLoop);
        }
        stateTickLoop = null;

        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        currentAnger = furniture.startanger;

        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToTop();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        StartCoroutine(IdleAndMoveLoop());

        Debug.Log("进入状态0：扫地机器人恢复正常");
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

        transform.position = new Vector2(startPoint.position.x, transform.position.y);
        SwitchToNormal();
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }

}
