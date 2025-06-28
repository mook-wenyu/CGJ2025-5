using System.Collections;
using UnityEngine;

public class cockroach : MonoBehaviour
{
    public Transform[] positions;         // 路径点数组
    public float speed = 2f;              // 移动速度
    public float rotationSpeed = 360f;    // 旋转速度（度/秒）

    private Coroutine moveCoroutine;

    void Start()
    {
        if (positions.Length > 0)
        {
            transform.position = positions[0].position;
            moveCoroutine = StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        for (int i = 1; i < positions.Length; i++)
        {
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

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
