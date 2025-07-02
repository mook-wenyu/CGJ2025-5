using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 家具状态
/// </summary>
public enum FurnitureStatus
{
    /// <summary>
    /// 正常状态
    /// </summary>
    Normal,
    /// <summary>
    /// 特殊状态
    /// </summary>
    Special,
    /// <summary>
    /// 黑化状态
    /// </summary>
    Dark,
    /// <summary>
    /// 失控状态
    /// </summary>
    Crazy
}

/// <summary>
/// 家具
/// </summary>
public class FurnitureData
{
    /// <summary>
    /// 家具ID
    /// </summary>
    public string id = string.Empty;

    /// <summary>
    /// 家具名称
    /// </summary>
    public string name = string.Empty;

    /// <summary>
    /// 开始等待时间(秒)
    /// </summary>
    public float waitTime = 0f;

    /// <summary>
    /// 最小触发间隔(秒)
    /// </summary>
    public float minInterval = 0f;
    /// <summary>
    /// 最大触发间隔(秒)
    /// </summary>
    public float maxInterval = 0f;

    /// <summary>
    /// 愤怒值基线
    /// </summary>
    public int startanger = 0;

    /// <summary>
    /// 愤怒值增长速度(x秒增长1)
    /// </summary>
    public float angerSpeed = 0f;

    /// <summary>
    /// 黑化阶段阈值(愤怒值达到该值时，进入黑化阶段)
    /// </summary>
    public int stageDark = 0;
    /// <summary>
    /// 失控阶段阈值(愤怒值达到该值时，进入失控阶段)
    /// </summary>
    public int stageCrazy = 0;



}
