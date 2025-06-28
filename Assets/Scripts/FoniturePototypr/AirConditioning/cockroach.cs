using System.Collections;
using UnityEngine;

public class cockroach : MonoBehaviour
{
    public Transform[] positions;         // ·��������
    public float speed = 2f;              // �ƶ��ٶ�
    public float rotationSpeed = 360f;    // ��ת�ٶȣ���/�룩

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

        Debug.Log("����·�����ѵ���");
    }

    void OnMouseDown()
    {
        if (airconditioning.isdie)
            return;
        Debug.Log("�������룬����1.5�������");

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
