using System.Collections;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("状态类型")]
    public int type = 0;

    [Header("现在的愤怒值")]
    public int anger = 0;

    [Header("愤怒值增长速度(x秒增长1)")]
    public float negativespeed = 1f;

    [Header("阶段阈值")]
    public int stage2 = 50;
    public int stage3 = 100;

    [Header("开始等待时间")]
    public float waittime = 10f;

    [Header("愤怒值基线")]
    public int startanger = 0;

    [Header("所有目标位置")]
    public Transform[] positions;

    [Header("移动速度")]
    public float speed = 2f;

    private bool isFirstTimeWait = false;
    private Coroutine moveCoroutine;
    private Object orderCtrl = null;
    private int lastIndex = -1;

    void Start()
    {
        anger = startanger;
        StartCoroutine(InitialWait());
    }

    IEnumerator InitialWait()
    {
        Debug.Log($"首次启动，等待 {waittime} 秒");
        yield return new WaitForSeconds(waittime);
        isFirstTimeWait = true;
        StartCoroutine(IdleAndMoveLoop());
    }

    IEnumerator IdleAndMoveLoop()
    {
         // 记录上一次的索引

        while (true)
        {
            if (type != 0) yield break;

            float delay = Random.Range(1f, 10f);
            if (isFirstTimeWait)
            {
                delay = 0f;
                isFirstTimeWait = false;
            }
            Debug.Log($"空闲状态：等待 {delay:F1} 秒后移动");
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
        orderCtrl = target.GetComponent<Object>();
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToBottom();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }

        type = 1;
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
    }

    private void OnMouseDown()
    {
        if (type == 1 || type == 2)
        {
            Debug.Log("点击家具：重置为状态0");
            ResetToIdle();
        }
    }

    void StateTick()
    {
        if (type == 1)
        {
            anger++;
            Debug.Log($"状态1：愤怒值 = {anger}");
            if (anger >= stage2)
            {
                type = 2;
                Debug.Log("进入状态2：家具震动");
            }
        }
        else if (type == 2)
        {
            anger++;
            Debug.Log($"状态2：愤怒值 = {anger}");
            if (anger >= stage3)
            {
                type = 3;
                Debug.Log("进入状态3：家具暴走");
                CancelInvoke(nameof(StateTick));
            }
        }
    }

    void ResetToIdle()
    {
        CancelInvoke(nameof(StateTick));
        type = 0;
        anger = startanger;

        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToTop();
            Debug.Log("调用 SetSonOrderToBottom 成功");
        }
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        StartCoroutine(IdleAndMoveLoop());
    }
}
