using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    protected Animator animator;
    protected GameObject partsObj;
    protected Shake shake;

    protected FurnitureData furniture;
    protected FurnitureStatus status = FurnitureStatus.Normal;
    protected int currentAnger = 0;

    // 是否是第一次等待
    protected bool isFirstWait = false;
    // 是否准备好
    protected bool isReady = false;
    // 有开始时延时
    protected bool hasStartedDelay = false;

    protected Coroutine launchCoroutine = null;
    protected Coroutine stateTickLoop = null;

    void Awake()
    {
        StartFurniture();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFurniture();
    }

    protected virtual void StartFurniture()
    {
        animator = GetComponent<Animator>();
        shake = new Shake(this, transform.localPosition);
        GetComponent<SpriteButton>().onClick.AddListener(OnClicked);
        SwitchStatus(status);
        var p = transform.Find("Parts");
        if (p) partsObj = p.gameObject;
        if (partsObj) partsObj.SetActive(false);
    }

    /// <summary>
    /// 更新
    /// </summary>
    protected virtual void UpdateFurniture()
    {
        if (!isReady) return;
        if (!hasStartedDelay && status == FurnitureStatus.Normal && currentAnger == furniture.startanger)
        {
            hasStartedDelay = true;
            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            if (isFirstWait)
            {
                isFirstWait = false;
                delay = 0f;
            }
            StartCoroutine(SwitchToSpecial(delay));
            return;
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="f"></param>
    public virtual void Launch(FurnitureData f)
    {
        furniture = f;
        Reset();

        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    /// <summary>
    /// 启动协程
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator LaunchCoroutine()
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

    /// <summary>
    /// 状态更新循环
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator StateTickLoop()
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

    /// <summary>
    /// 状态更新
    /// </summary>
    protected virtual void StateTick()
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
    protected virtual void AngerTick()
    {
        currentAnger++;
        if (currentAnger >= furniture.stageCrazy && status != FurnitureStatus.Crazy)
        {
            status = FurnitureStatus.Crazy;
            SwitchStatus(status);
            Debug.Log("进入状态3：失控");
            LevelProgressPanel.Instance.ShowFailPanel(furniture.name);
            return;
        }
        if (currentAnger >= furniture.stageDark && status != FurnitureStatus.Dark)
        {
            status = FurnitureStatus.Dark;
            SwitchStatus(status);
            GameMgr.Instance.SetTimeScale(GameMgr.TIME_SCALE_DARK);
            if (partsObj) partsObj.SetActive(true);
            Debug.Log("进入状态2：黑化");
            return;
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    protected virtual void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        if (status == FurnitureStatus.Special || status == FurnitureStatus.Dark)
        {
            SwitchToNormal();
        }
    }

    /// <summary>
    /// 切换到正常状态
    /// </summary>
    protected virtual void SwitchToNormal()
    {
        currentAnger = furniture.startanger;
        status = FurnitureStatus.Normal;
        SwitchStatus(status);
        if (partsObj) partsObj.SetActive(false);
        GameMgr.Instance.SetTimeScale(1f);
        hasStartedDelay = false;
        Debug.Log("进入状态0：恢复正常");
    }

    /// <summary>
    /// 切换到特殊状态
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    protected virtual IEnumerator SwitchToSpecial(float delay)
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
        Debug.Log("进入状态1：进入特殊状态");
    }

    /// <summary>
    /// 重置
    /// </summary>
    protected virtual void Reset()
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

    /// <summary>
    /// 切换状态
    /// </summary>
    protected virtual void SwitchStatus(FurnitureStatus newStatus)
    {
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
