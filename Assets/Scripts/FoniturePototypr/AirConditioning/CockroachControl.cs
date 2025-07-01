using System.Collections;
using UnityEngine;

public class CockroachControl : MonoBehaviour
{
    public GameObject cockroachPrefab;
    [Header("路径点")]
    public Transform[] pathPoints; // 同一路径点

    public Furniture furniture;

    [HideInInspector]
    public int currentIndex = 0;

    private Coroutine launchCoroutine = null;

    void Start()
    {
        //airconditioning.isdie = false;
    }

    public void Launch(Furniture f)
    {
        furniture = f;
        Reset();

        launchCoroutine = StartCoroutine(SpawnCockroachLoop());
        GenerateCockroach(0);
    }

    IEnumerator SpawnCockroachLoop()
    {
        while (true)
        {
            //if(airconditioning.isdie)
            //{
            //    break;
            //}
            float delay = Random.Range(furniture.minInterval, furniture.maxInterval);
            Debug.Log($"生成蟑螂 {delay:F1} 秒后");
            yield return new WaitForSeconds(delay);

            GenerateCockroach(currentIndex + 1);
        }
    }

    void GenerateCockroach(int index)
    {
        currentIndex = index;
        GameObject roach = Instantiate(cockroachPrefab, pathPoints[0].position, Quaternion.identity);
        Cockroach script = roach.GetComponent<Cockroach>();
        script.positions = pathPoints;
        script.index = index;
    }

    public void Reset()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = null;

        foreach (GameObject roach in GameObject.FindGameObjectsWithTag("Cockroach"))
        {
            Destroy(roach);
        }
    }
}
