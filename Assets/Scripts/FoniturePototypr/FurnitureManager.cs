using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    [Header("开始控制器")]
    public static bool isStart;
    [Header("暂停控制器")]
    public static bool isPaused;
    [Header("结束控制器")]
    public static bool isEnd;
    [Header("增长速度倍率")]
    public static float growth_Rate;

    private void Start()
    {
        isStart = false;
        isPaused = false;
        isEnd = false;
        growth_Rate = 1f;
    }
}
