using System.Collections;
using UnityEngine;

public class bin : MonoBehaviour
{
    public int type = 0;               // 家具状态
    public int anger = 0;              // 愤怒值
    public float negativespeed = 1f;   // 情绪积累速度（秒）

    private bool hasStartedDelay = false;
    private bool isCoolingDown = false;

    private int directionChangeCount = 0;
    private Vector3 lastMousePos;
    private int lastDirection = 0; // -1:左, 1:右, 0:未定

    void Start()
    {
        type = 0;
        anger = 0;
        InvokeRepeating(nameof(StateTick), 0f, negativespeed);
    }

    void Update()
    {
        if (!hasStartedDelay && !isCoolingDown && type == 0 && anger == 0)
        {
            hasStartedDelay = true;
            float delay = Random.Range(1f, 10f);
            Debug.Log($"状态0：将在 {delay:F1} 秒后进入状态1");
            StartCoroutine(DelayToState1(delay));
        }
    }

    void StateTick()
    {
        if (isCoolingDown) return;
        if (type == 3) return;

        switch (type)
        {
            case 1:
                anger++;
                Debug.Log($"状态1：愤怒值 = {anger}");
                if (anger >= 60)
                {
                    type = 2;
                    Debug.Log("进入状态2：家具开始震动");
                }
                break;

            case 2:
                anger++;
                Debug.Log($"状态2：愤怒值 = {anger}");
                if (anger >= 100)
                {
                    type = 3;
                    Debug.Log("进入状态3：家具暴走！");
                }
                break;
        }
    }

    IEnumerator DelayToState1(float delay)
    {
        yield return new WaitForSeconds(delay);
        type = 1;
        Debug.Log("状态0倒计时结束，进入状态1：开始积累愤怒");
    }

    void CoolDownToZero()
    {
        isCoolingDown = true;
        Debug.Log("滑动变向3次，怒气立即清零...");

        type = 0;
        anger = 0;
        directionChangeCount = 0;
        lastDirection = 0;
        isCoolingDown = false;
        hasStartedDelay = false;

        Debug.Log("怒气清零，状态回到0，等待重新进入状态1");
    }

    void OnMouseOver()
    {
        if (type >= 1 && type <= 2 && !isCoolingDown)
        {
            if (Input.GetMouseButton(0)) 
            {
                Vector3 currentMousePos = Input.mousePosition;
                float dx = currentMousePos.x - lastMousePos.x;

                int currentDirection = 0;
                if (Mathf.Abs(dx) > 20f) // 滑动超过20像素才判定
                {
                    currentDirection = dx > 0 ? 1 : -1;

                    if (lastDirection != 0 && currentDirection != lastDirection)
                    {
                        directionChangeCount++;
                        Debug.Log($"滑动方向改变，第 {directionChangeCount} 次");
                    }

                    lastDirection = currentDirection;
                    lastMousePos = currentMousePos;

                    if (directionChangeCount >= 3)
                    {
                        CoolDownToZero();
                    }
                }
            }
        }
    }

    void OnMouseEnter()
    {
        lastMousePos = Input.mousePosition;
        directionChangeCount = 0;
        lastDirection = 0;
    }
}
