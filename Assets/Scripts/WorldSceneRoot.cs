using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldSceneRoot : MonoSingleton<WorldSceneRoot>
{
    public Slider gameTimeSlider;

    [Header("World")]
    public GameObject kitchenRoot;
    public GameObject bedroomRoot;

    public GameObject firgeGO;
    public GameObject binGO;
    public GameObject robotGO;
    public GameObject washMachineGO;
    public GameObject kettleGO;
    public GameObject airconditioningGO;
    public GameObject clockGO;

    private Camera mainCamera;

    public Coroutine gameTimeCoroutine;

    private Firge firge;
    private Bin bin;
    private Robot robot;
    private WashMachine washMachine;
    private Kettle kettle;
    private Airconditioning airconditioning;
    private Clock clock;

    private int levelTime = 0;

    // 记录手机屏幕拖动起始点
    Vector2? dragStartPos = null;
    float dragThreshold = 100f; // 拖动阈值，单位像素

    void Awake()
    {
        mainCamera = Camera.main;
        gameTimeSlider.value = 0;
        gameTimeSlider.gameObject.SetActive(false);

        firge = firgeGO.transform.GetComponentInChildren<Firge>();
        bin = binGO.transform.GetComponent<Bin>();
        robot = robotGO.transform.GetComponent<Robot>();
        washMachine = washMachineGO.transform.GetComponent<WashMachine>();
        kettle = kettleGO.transform.GetComponent<Kettle>();
        airconditioning = airconditioningGO.transform.GetComponent<Airconditioning>();
        clock = clockGO.transform.GetComponent<Clock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.isFailed || GameMgr.isVictory) return;

        // 同时支持方向键、手机屏幕拖动和鼠标拖动

        // 方向键支持
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (mainCamera.transform.position.x >= 19.2f)
            {
                Tween.PositionX(mainCamera.transform, 0f, 0.3f);
            }
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (mainCamera.transform.position.x <= 0f)
            {
                Tween.PositionX(mainCamera.transform, 19.2f, 0.3f);
            }
        }

        // 手机屏幕拖动支持
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // 检查触摸点是否在UI上
                if (!IsPointerOverUI(touch.position))
                {
                    dragStartPos = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended && dragStartPos.HasValue)
            {
                Vector2 dragEndPos = touch.position;
                float deltaX = dragEndPos.x - dragStartPos.Value.x;
                HandleSwipe(deltaX);
                dragStartPos = null;
            }
        }

        // 鼠标拖动支持
        if (Input.touchCount == 0) // 避免和触屏冲突
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 检查鼠标点击是否在UI上
                if (!IsPointerOverUI(Input.mousePosition))
                {
                    dragStartPos = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && dragStartPos.HasValue)
            {
                Vector2 dragEndPos = Input.mousePosition;
                float deltaX = dragEndPos.x - dragStartPos.Value.x;
                HandleSwipe(deltaX);
                dragStartPos = null;
            }
        }
    }

    // 检查指针是否在UI元素上
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        // 使用EventSystem检查是否点击在UI上
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = screenPosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        return false;
    }

    // 处理滑动手势
    private void HandleSwipe(float deltaX)
    {
        if (Mathf.Abs(deltaX) > dragThreshold)
        {
            // 向左拖动，切换到右侧（因为内容向左移动）
            if (deltaX < 0 && mainCamera.transform.position.x <= 0f)
            {
                Tween.PositionX(mainCamera.transform, 19.2f, 0.3f);
            }
            // 向右拖动，切换到左侧（因为内容向右移动）
            else if (deltaX > 0 && mainCamera.transform.position.x >= 19.2f)
            {
                Tween.PositionX(mainCamera.transform, 0f, 0.3f);
            }
        }
    }

    /// <summary>
    /// 根据关卡配置初始化世界
    /// </summary>
    /// <param name="level"></param>
    public void ResetWorld(int level)
    {
        // level = 2;
        GameMgr.currentLevel = level;

        gameTimeSlider.gameObject.SetActive(true);
        gameTimeSlider.value = 0;

        var levelData = CSVMgr.Get<LevelConfigConfig>(GameMgr.currentLevel.ToString());
        levelTime = levelData.time;
        gameTimeSlider.maxValue = levelTime;

        GameMgr.ResumeTime();
        if (gameTimeCoroutine != null)
        {
            StopCoroutine(gameTimeCoroutine);
        }

        gameTimeCoroutine = StartCoroutine(GameTimeCoroutine(levelTime));

        // 初始化家具配置
        for (int i = 0; i < levelData.content.Length; i++)
        {
            var content = levelData.content[i];
            var furniture = new FurnitureData()
            {
                name = content,
                waitTime = levelData.first_show[i],
                minInterval = levelData.show_mingap[i],
                maxInterval = levelData.show_maxgap[i],
                startanger = (int)levelData.baseline[i],
                angerSpeed = levelData.velocity[i],
                stageDark = (int)levelData.stage_2_threshold[i],
                stageCrazy = (int)levelData.stage_3_threshold[i],
            };

            // if (content != "扫地机器人") continue;

            switch (content)
            {
                case "冰箱":
                    firge.Launch(furniture);
                    break;
                case "智能垃圾桶":
                    bin.Launch(furniture);
                    break;
                case "扫地机器人":
                    robot.Launch(furniture);
                    break;
                case "滚筒洗衣机":
                    washMachine.Launch(furniture);
                    break;
                case "烧水壶":
                    kettle.Launch(furniture);
                    break;
                case "空调":
                    airconditioning.Launch(furniture);
                    break;
                case "时钟":
                    clock.Launch(furniture);
                    break;
            }
        }
    }

    /// <summary>
    /// 游戏时间流逝
    /// </summary>
    /// <param name="totalTime"></param>
    IEnumerator GameTimeCoroutine(int totalTime)
    {
        while (true)
        {
            // 在暂停时持续等待，直到游戏恢复
            while (GameMgr.IsTimePaused)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f / GameMgr.timeScale);
            Tween.UISliderValue(gameTimeSlider, gameTimeSlider.value + 1, 1f / GameMgr.timeScale);
            if (gameTimeSlider.value >= totalTime && GameMgr.gameMode != GameMode.Endless)
            {
                break;
            }
        }
        GameMgr.PauseTime();
        gameTimeSlider.gameObject.SetActive(false);

        if (GameMgr.gameMode == GameMode.Level)
        {
            if (GameMgr.currentLevel < GameMgr.MAX_LEVEL)
            {
                LevelProgressPanel.Instance.ShowVictoryPanel();
            }
            else
            {
                LevelProgressPanel.Instance.ShowEndPanel();
            }
        }
        else
        {
            LevelProgressPanel.Instance.ShowEndPanel();
        }
    }
}
