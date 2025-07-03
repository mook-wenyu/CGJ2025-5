using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake
{
    private MonoBehaviour monoBehaviour;
    private Vector3 originalPosition;
    private bool isShaking = false;
    private Coroutine shakeCoroutine = null;
    private float baseShakeAmount = 0.1f;       // 基础抖动幅度
    private float baseShakeSpeed = 10f;         // 基础抖动速度
    private float specialShakeMultiplier = 1f;  // 特殊状态抖动倍率
    private float darkShakeMultiplier = 1.5f;   // 黑化状态抖动倍率
    private float currentShakeMultiplier = 1f;   // 当前抖动倍率

    public Shake(MonoBehaviour monoBehaviour, Vector3 originalPosition)
    {
        this.monoBehaviour = monoBehaviour;
        this.originalPosition = originalPosition;
    }

    /// <summary>
    /// 开始抖动
    /// </summary>
    public void StartShaking()
    {
        if (isShaking)
        {
            // 如果已经在抖动，只需更新抖动参数
            return;
        }

        isShaking = true;
        if (shakeCoroutine != null)
        {
            monoBehaviour.StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = monoBehaviour.StartCoroutine(ShakeCoroutine());
    }

    /// <summary>
    /// 停止抖动
    /// </summary>
    public void StopShaking()
    {
        if (isShaking)
        {
            isShaking = false;
            if (shakeCoroutine != null)
            {
                monoBehaviour.StopCoroutine(shakeCoroutine);
                shakeCoroutine = null;
            }
            monoBehaviour.transform.localPosition = originalPosition;
        }
    }

    /// <summary>
    /// 设置抖动幅度
    /// </summary>
    /// <param name="amount"></param>
    public void SetShakeAmount(float amount)
    {
        baseShakeAmount = amount;
    }

    /// <summary>
    /// 设置抖动倍率
    /// </summary>
    /// <param name="multiplier"></param>
    public void SetShakeMultiplier(float multiplier)
    {
        currentShakeMultiplier = multiplier;
    }

    /// <summary>
    /// 设置特殊状态抖动倍率
    /// </summary>
    public void SetSpecialShakeMultiplier()
    {
        currentShakeMultiplier = specialShakeMultiplier;
    }

    /// <summary>
    /// 设置黑化状态抖动倍率
    /// </summary>
    public void SetDarkShakeMultiplier()
    {
        currentShakeMultiplier = darkShakeMultiplier;
    }

    /// <summary>
    /// 抖动协程
    /// </summary>
    private IEnumerator ShakeCoroutine()
    {
        while (isShaking)
        {
            if (!GameMgr.IsTimePaused)
            {
                // 根据状态和timeScale计算当前抖动参数
                float currentShakeAmount = baseShakeAmount * currentShakeMultiplier;

                // 计算随机偏移
                float offsetX = Random.Range(-currentShakeAmount, currentShakeAmount);
                float offsetY = Random.Range(-currentShakeAmount, currentShakeAmount);

                // 应用偏移
                monoBehaviour.transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
            }

            // 等待时间考虑状态和timeScale
            float waitTime = 1f / (baseShakeSpeed * currentShakeMultiplier * GameMgr.timeScale);
            yield return new WaitForSeconds(waitTime);
        }

        // 恢复原始位置
        monoBehaviour.transform.localPosition = originalPosition;
    }
}
