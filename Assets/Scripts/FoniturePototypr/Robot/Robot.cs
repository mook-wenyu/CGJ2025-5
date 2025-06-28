using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Robot : MonoBehaviour
{

    private SpriteRenderer sr;

    [Header("ͼƬ")]
    public Sprite[] imgs;

    [Header("״̬����")]
    public int type = 0;

    [Header("���ڵķ�ŭֵ")]
    public int anger = 0;

    [Header("��ŭֵ�����ٶ�(x������1)")]
    public float negativespeed = 1f;

    [Header("�׶���ֵ")]
    public int stage2 = 50;
    public int stage3 = 100;

    [Header("��ʼ�ȴ�ʱ��")]
    public float waittime = 10f;

    [Header("��ŭֵ����")]
    public int startanger = 0;

    [Header("����Ŀ��λ��")]
    public Transform[] positions;

    [Header("�ƶ��ٶ�")]
    public float speed = 2f;

    private bool isFirstTimeWait = false;
    private Coroutine moveCoroutine;
    private Object orderCtrl = null;
    private int lastIndex = -1;

    void Start()
    {
        type = 0;
        anger = startanger;
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = imgs[0];
        SwitchStatus(type);
        StartCoroutine(InitialWait());
    }

    IEnumerator InitialWait()
    {
        Debug.Log($"�״��������ȴ� {waittime} ��");
        yield return new WaitForSeconds(waittime);
        isFirstTimeWait = true;
        StartCoroutine(IdleAndMoveLoop());
    }

    IEnumerator IdleAndMoveLoop()
    {
         // ��¼��һ�ε�����

        while (true)
        {
            if (type != 0) yield break;

            float delay = Random.Range(1f, 10f);
            if (isFirstTimeWait)
            {
                delay = 0f;
                isFirstTimeWait = false;
            }
            Debug.Log($"����״̬���ȴ� {delay:F1} ����ƶ�");
            yield return new WaitForSeconds(delay);

            if (positions.Length == 0) yield break;

            int newIndex;
            do
            {
                newIndex = Random.Range(0, positions.Length);
            } while (positions.Length > 1 && newIndex == lastIndex); // ������ϴ�һ��

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
        Debug.Log("�ѵ���Ŀ��㣬����״̬1");
        orderCtrl = target.GetComponent<Object>();
        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToBottom();
            Debug.Log("���� SetSonOrderToBottom �ɹ�");
        }

        type = 1;
        SwitchStatus(type);
        sr.sprite = imgs[0];
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
    }

    private void OnMouseDown()
    {
        if (type == 1 || type == 2)
        {
            Debug.Log("����Ҿߣ�����Ϊ״̬0");
            ResetToIdle();
        }
    }

    void StateTick()
    {
        if (type == 1)
        {
            anger++;
            Debug.Log($"״̬1����ŭֵ = {anger}");
            if (anger >= stage2)
            {
                type = 2;
                SwitchStatus(type);
                sr.sprite = imgs[0];
                Debug.Log("����״̬2���Ҿ���");
            }
        }
        else if (type == 2)
        {
            anger++;
            Debug.Log($"״̬2����ŭֵ = {anger}");
            if (anger >= stage3)
            {
                type = 3;
                SwitchStatus(type);
                sr.sprite = imgs[0];
                Debug.Log("����״̬3���Ҿ߱���");
                CancelInvoke(nameof(StateTick));
            }
        }
    }

    void ResetToIdle()
    {
        CancelInvoke(nameof(StateTick));
        type = 0;
        SwitchStatus(type);
        sr.sprite = imgs[0];
        anger = startanger;

        if (orderCtrl != null)
        {
            orderCtrl.SetSonOrderToTop();
            Debug.Log("���� SetSonOrderToBottom �ɹ�");
        }
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        StartCoroutine(IdleAndMoveLoop());
    }
    void SwitchStatus(int newStatus)
    {
        sr.sprite = imgs[newStatus];
    }
}
