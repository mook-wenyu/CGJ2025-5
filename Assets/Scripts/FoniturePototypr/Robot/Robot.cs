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

    private Furniture furniture;


    public Transform startPoint;
    [Header("所有目标位置")]
    public Transform[] positions;

    [Header("移动速度")]
    public float speed = 2f;

    private Coroutine launchCoroutine = null;

    private bool isFirstWait = false;
    private Coroutine moveCoroutine;
    private HidePoint orderCtrl = null;
    private int lastIndex = -1;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SwitchStatus(status);
    }

    public void Launch(Furniture f)
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
        yield return new WaitForSeconds(furniture.waitTime);
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
            Debug.Log($"状态0(正常)：扫地机器人将在 {furniture.waitTime:F1} 秒后进入状态1(特殊)");
            yield return new WaitForSeconds(delay);

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
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Debug.Log("已到达目标点，进入状态1");
        orderCtrl = target.GetComponent<HidePoint>();
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToBottom();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }

        status = FurnitureStatus.Special;
        SwitchStatus(status);

        InvokeRepeating(nameof(StateTick), 0f, furniture.angerSpeed);
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
            CancelInvoke(nameof(StateTick));
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

    private void OnMouseDown()
    {
        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }


    void SwitchToNormal()
    {
        CancelInvoke(nameof(StateTick));
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
        transform.position = new Vector2(startPoint.position.x, transform.position.y);

        SwitchToNormal();


        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
    }

    void SwitchStatus(FurnitureStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
