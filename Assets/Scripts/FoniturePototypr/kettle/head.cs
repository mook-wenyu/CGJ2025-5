using System.Collections;
using UnityEngine;

public class head : MonoBehaviour
{
    public float shakeAmount = 0.1f;
    public float speedState1 = 2f;
    public float speedState2 = 8f;

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    void Start()
    {
        originalPos = transform.localPosition;
        gameObject.SetActive(false); // ≥ı º≤ªœ‘ æ
    }

    public void StartShaking(int type)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        float interval = 1f / (type == 1 ? speedState1 : speedState2);
        shakeCoroutine = StartCoroutine(ShakeLoop(interval,type));
        gameObject.SetActive(true);
    }

    public void StopShaking()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;
        transform.localPosition = originalPos;
        gameObject.SetActive(false);
    }

    public IEnumerator ShakeLoop(float interval,int type)
    {
        while (true)
        {
            Vector3 offset = new Vector3(
                Random.Range(-shakeAmount*type, shakeAmount * type),
                Random.Range(-shakeAmount * type, shakeAmount * type),
                0f
            );
            transform.localPosition = originalPos + offset;
            yield return new WaitForSeconds(interval);
        }
    }
}
