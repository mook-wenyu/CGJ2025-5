using System.Collections;
using UnityEngine;

public enum CockroachStatus
{
    Move,
    Fly,
    Die,
    FlyDie
}

public class Cockroach : MonoBehaviour
{
    public Transform[] positions;         // 路径点数组
    public float speed = 2f;              // 移动速度
    public float rotationSpeed = 360f;    // 旋转速度（度/秒）
    public CockroachStatus status = CockroachStatus.Move;
    public Sprite[] imgs;

    private SpriteRenderer sr;

    public int index = 0;

    private Coroutine moveCoroutine;

    private bool isfly = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<SpriteButton>().onClick.AddListener(OnClicked);
    }

    void Start()
    {
        SwitchStatus(status);

        if (positions.Length > 0)
        {
            transform.position = positions[0].position;
            moveCoroutine = StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (isfly)
            {
                status = CockroachStatus.Fly;
                SwitchStatus(status);
            }
            else
            {
                isfly = Random.value <= 0.2 * i;
            }
            Vector3 target = positions[i].position;

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                while (GameMgr.IsTimePaused)
                {
                    yield return null;
                }

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * GameMgr.timeScale * Time.deltaTime
                );

                Vector3 direction = (target - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                    if (status != CockroachStatus.Fly)
                        targetRotation *= Quaternion.Euler(0, 0, 90);
                    else
                        targetRotation *= Quaternion.Euler(0, 0, 45);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * GameMgr.timeScale * Time.deltaTime
                    );
                }

                yield return null;
            }

            float waitTime = 0.1f / GameMgr.timeScale;
            float elapsedTime = 0f;
            
            while (elapsedTime < waitTime)
            {
                if (!GameMgr.IsTimePaused)
                {
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }
        }

        Debug.Log("所有路径点已到达");
    }

    private void OnClicked()
    {
        if (GameMgr.IsTimePaused) return;

        //if (airconditioning.isdie)
        //    return;
        Debug.Log("点击了蟑螂，将在1.5秒后销毁");

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (status == CockroachStatus.Move)
        {
            status = CockroachStatus.Die;
            SwitchStatus(status);
        }
        else if (status == CockroachStatus.Fly)
        {
            status = CockroachStatus.FlyDie;
            SwitchStatus(status);
        }

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        float waitTime = delay / GameMgr.timeScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < waitTime)
        {
            if (!GameMgr.IsTimePaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
        
        Destroy(gameObject);
    }

    void SwitchStatus(CockroachStatus newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
