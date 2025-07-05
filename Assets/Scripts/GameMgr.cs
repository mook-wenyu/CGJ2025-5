using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

/// <summary>
/// 游戏模式
/// </summary>
public enum GameMode
{
    /// <summary>
    /// 关卡模式（默认）
    /// </summary>
    Level,
    /// <summary>
    /// 挑战模式
    /// </summary>
    Challenge,
    /// <summary>
    /// 无尽模式
    /// </summary>
    Endless,
}

public enum GameState
{
    /// <summary>
    /// 游戏进行中
    /// </summary>
    Playing,
    /// <summary>
    /// 游戏胜利
    /// </summary>
    Victory,
    /// <summary>
    /// 游戏失败
    /// </summary>
    Failed,
    /// <summary>
    /// 游戏结束
    /// </summary>
    End,
}

public class GameMgr : MonoSingleton<GameMgr>
{
    public GameObject[] objs;

    /// <summary>
    /// 是否初始化游戏
    /// </summary>
    public static bool isInitGame = false;
    /// <summary>
    /// 当前关卡
    /// </summary>
    public static int currentLevel = 1;
    /// <summary>
    /// 当前游戏状态
    /// </summary>
    public static GameState currentState = GameState.Playing;
    /// <summary>
    /// 最大关卡
    /// </summary>
    public const int MAX_LEVEL = 7;

    /// <summary>
    /// 游戏模式
    /// </summary>
    public static GameMode gameMode = GameMode.Level;

    /// <summary>
    /// 游戏时间
    /// </summary>
    public static float gameTime = 0;
    /// <summary>
    /// 游戏时间缩放
    /// </summary>
    public static float timeScale = 1;

    public const float TIME_SCALE_SPECIAL = 2f;
    public const float TIME_SCALE_DARK = 4f;

    /// <summary>
    /// 游戏时间暂停事件
    /// </summary>
    public static event Action OnGameTimePaused;
    /// <summary>
    /// 游戏时间恢复事件
    /// </summary>
    public static event Action OnGameTimeResumed;

    /// <summary>
    /// 时间是否暂停
    /// </summary>
    private static bool _isTimePaused;

    public static bool IsTimePaused => _isTimePaused;

    /// <summary>
    /// 暂停时间
    /// </summary>
    public static void PauseTime()
    {
        _isTimePaused = true;
        OnGameTimePaused?.Invoke();
    }

    /// <summary>
    /// 恢复时间
    /// </summary>
    public static void ResumeTime()
    {
        _isTimePaused = false;
        OnGameTimeResumed?.Invoke();
    }

    public void SetTimeScale(float timeScale)
    {
        GameMgr.timeScale = timeScale;
    }

}
