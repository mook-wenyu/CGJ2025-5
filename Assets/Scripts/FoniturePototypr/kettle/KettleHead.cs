using System.Collections;
using PrimeTween;
using UnityEngine;

public class KettleHead : MonoBehaviour
{
    public float shakeAmount = 0.1f;
    public float speedState1 = 30f;
    public float speedState2 = 60f;

    private Vector2 originalPos;
    private Coroutine shakeCoroutine;

    public void SetOriginalPos(Vector2 pos)
    {
        originalPos = pos;
    }

    public void StartShaking(FurnitureStatus type)
    {
        gameObject.SetActive(true);

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        float interval = 1f / (type == FurnitureStatus.Special ? speedState1 : speedState2);
        shakeCoroutine = StartCoroutine(ShakeLoop(interval, (int)type));

    }

    public void StopShaking()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;
        transform.position = originalPos;
        gameObject.SetActive(false);
    }

    public IEnumerator ShakeLoop(float interval, int type)
    {
        // float finalPos = 0f;
        while (true)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            Vector2 offset = new(
                Random.Range(-shakeAmount * type, shakeAmount * type),
                Random.Range(-shakeAmount * type, shakeAmount * type)
            );

            transform.position = originalPos + offset;

            float waitTime = interval / GameMgr.timeScale;
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
        }
    }
}
