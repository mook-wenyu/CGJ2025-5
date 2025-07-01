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
            Vector2 offset = new(
                Random.Range(-shakeAmount * type, shakeAmount * type),
                Random.Range(-shakeAmount * type, shakeAmount * type)
            );

            transform.position = originalPos + offset;

            yield return new WaitForSeconds(interval);
        }
    }
}
