using System.Collections;
using UnityEngine;

public class CockroachSpawner : MonoBehaviour
{
    public GameObject cockroachPrefab;
    [Header("路径点")]
    public Transform[] pathPoints; // 同一组路径点
    [Header("生成蟑螂间隔")]
    public float startTime = 5f;
    public float endTime = 10f;
    [Header("初始等待时间")]
    public float waitTime = 1f;


    void Start()
    {
        airconditioning.isdie = false;
        StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        Debug.Log($"等待 {waitTime} 秒后开始运行");
        yield return new WaitForSeconds(waitTime);
        // 等待结束后开始执行逻辑
        StartCoroutine(SpawnCockroachLoop());
        GameObject roach = Instantiate(cockroachPrefab, pathPoints[0].position, Quaternion.identity);
        cockroach script = roach.GetComponent<cockroach>();
        script.positions = pathPoints;

    }

    IEnumerator SpawnCockroachLoop()
    {
        while (true)
        {
            if(airconditioning.isdie)
            {
                break;
            }
            float delay = Random.Range(startTime, endTime);
            Debug.Log($"将在 {delay:F1} 秒后生成");
            yield return new WaitForSeconds(delay);

            GameObject roach = Instantiate(cockroachPrefab, pathPoints[0].position, Quaternion.identity);
            cockroach script = roach.GetComponent<cockroach>();
            script.positions = pathPoints;

           
        }
    }
}
