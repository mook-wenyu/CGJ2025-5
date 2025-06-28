using System.Collections;
using UnityEngine;

public enum Status
{
    Move,
    Fly,
    Die,
    FlyDie
}

public class cockroach : MonoBehaviour
{
    public Transform[] positions;         // 路径点数组
    public float speed = 2f;              // 移动速度
    public float rotationSpeed = 360f;    // 旋转速度（度/秒）
    public Status status = Status.Move;
    public Sprite[] imgs;

    private SpriteRenderer sr;

    private Coroutine moveCoroutine;

    private bool isfly = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        status = Status.Move;
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
                status = Status.Fly;
                SwitchStatus(status);
            }
            else
            {
                isfly = Random.value <= 0.2 * i;
            }
            Vector3 target = positions[i].position;

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                if (airconditioning.isdie)
                {
                    break;
                }
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );

                Vector3 direction = (target - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("所有路径点已到达");
    }

    void OnMouseDown()
    {
        if (airconditioning.isdie)
            return;
        Debug.Log("点击了蟑螂，将在1.5秒后销毁");

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        if(status==Status.Move)
        {
            status = Status.Die;
            SwitchStatus(status);
        }
        else if (status==Status.Fly)
        {
            status = Status.FlyDie;
            SwitchStatus(status);
        }

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void SwitchStatus(Status newStatus)
    {
        sr.sprite = imgs[(int)newStatus];
    }
}
