using System;

public class LevelConfigConfig : BaseConfig
{
    /// <summary>
    /// 名称
    /// </summary>
    public string name;
    /// <summary>
    /// 关卡时长
    /// </summary>
    public int time;
    /// <summary>
    /// 内容
    /// </summary>
    public string[] content;
    /// <summary>
    /// 首次触发时间（第n秒）
    /// </summary>
    public int[] first_show;
    /// <summary>
    /// 触发间隔最小值(s)
    /// </summary>
    public int[] show_mingap;
    /// <summary>
    /// 触发间隔最大值(s)
    /// </summary>
    public int[] show_maxgap;
    /// <summary>
    /// 状态基线值
    /// </summary>
    public float[] baseline;
    /// <summary>
    /// 负面情绪积累速度(s/1)
    /// </summary>
    public float[] velocity;
    /// <summary>
    /// 黑化状态阈值
    /// </summary>
    public float[] stage_2_threshold;
    /// <summary>
    /// 失控状态阈值
    /// </summary>
    public float[] stage_3_threshold;
}
