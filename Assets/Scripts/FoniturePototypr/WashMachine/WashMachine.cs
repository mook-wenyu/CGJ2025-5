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

    private SpriteRenderer fsr;

    private SpriteRenderer sr;

    private Furniture furniture;

    private FurnitureStatus status = FurnitureStatus.Normal;

    private int currentAnger = 0;

    private bool isFirstWait = false;
    private bool isReady = false;

    private bool hasStartedDelay = false;

    private Coroutine launchCoroutine = null;

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        fsr = face.GetComponent<SpriteRenderer>();
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
            Debug.Log("进入状态2：滚筒洗衣机黑化");
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

    IEnumerator SwitchToSpecial(float delay)
    {
        yield return new WaitForSeconds(delay);
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

        Debug.Log("进入状态0：滚筒洗衣机恢复正常");
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
