using System.Collections;
using UnityEngine;

public class CockroachControl : MonoBehaviour
{
    public GameObject cockroachPrefab;
    [Header("路径点")]
    public Transform[] pathPoints; // 同一路径点

    public FurnitureData furniture;

    [HideInInspector]
    public int currentIndex = 0;

    private Coroutine launchCoroutine = null;
    private Coroutine spawnCockroachLoop = null;

    void Start()
    {
        //airconditioning.isdie = false;
    }

    public void Launch(FurnitureData f)
    {
        furniture = f;
        Reset();

        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    IEnumerator LaunchCoroutine()
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
        
        spawnCockroachLoop = StartCoroutine(SpawnCockroachLoop());
        GenerateCockroach(0);
    }

    IEnumerator SpawnCockroachLoop()
    {
        while (true)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            float delay = Random.Range(furniture.minInterval, furniture.maxInterval + 1);
            
            float elapsedTime = 0f;
            while (elapsedTime < delay / GameMgr.timeScale)
            {
                // 在暂停时持续等待，直到游戏恢复
                if (!GameMgr.IsTimePaused)
                {
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }

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

        if (spawnCockroachLoop != null)
        {
            StopCoroutine(spawnCockroachLoop);
        }
        spawnCockroachLoop = null;

        foreach (GameObject roach in GameObject.FindGameObjectsWithTag("Cockroach"))
        {
            Destroy(roach);
        }
    }
}
