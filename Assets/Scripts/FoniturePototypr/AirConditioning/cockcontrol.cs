using System.Collections;
using UnityEngine;

public class CockroachSpawner : MonoBehaviour
{
    public GameObject cockroachPrefab;
    [Header("·����")]
    public Transform[] pathPoints; // ͬһ��·����
    [Header("���������")]
    public float startTime = 5f;
    public float endTime = 10f;
    [Header("��ʼ�ȴ�ʱ��")]
    public float waitTime = 1f;


    void Start()
    {
        airconditioning.isdie = false;
        StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        Debug.Log($"�ȴ� {waitTime} ���ʼ����");
        yield return new WaitForSeconds(waitTime);
        // �ȴ�������ʼִ���߼�
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
            Debug.Log($"���� {delay:F1} �������");
            yield return new WaitForSeconds(delay);

            GameObject roach = Instantiate(cockroachPrefab, pathPoints[0].position, Quaternion.identity);
            cockroach script = roach.GetComponent<cockroach>();
            script.positions = pathPoints;

           
        }
    }
}
