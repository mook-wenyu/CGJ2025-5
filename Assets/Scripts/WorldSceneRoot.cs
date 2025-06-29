using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class WorldSceneRoot : MonoSingleton<WorldSceneRoot>
{
    public Slider gameTimeSlider;

    [Header("World")]
    public GameObject kitchenRoot;
    public GameObject firgeGO;


    public GameObject bedroomRoot;

    public Button startBtn;

    private Camera mainCamera;

    public Coroutine gameTimeCoroutine;

    private Firge firge;

    void Awake()
    {
        mainCamera = Camera.main;
        gameTimeSlider.value = 0;
        gameTimeSlider.gameObject.SetActive(false);

        firge = firgeGO.transform.GetComponentInChildren<Firge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (mainCamera.transform.position.x >= 19.2f)
            {
                Tween.PositionX(mainCamera.transform, 0f, 0.3f);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (mainCamera.transform.position.x <= 0f)
            {
                Tween.PositionX(mainCamera.transform, 19.2f, 0.3f);
            }
        }
    }

    public void ResetWorld(int level)
    {
        Utils.currentLevel = level;

        gameTimeSlider.gameObject.SetActive(true);
        gameTimeSlider.value = 0;

        var levelData = CSVMgr.Get<LevelConfigConfig>(level.ToString());
        var time = levelData.time;
        gameTimeSlider.maxValue = time;

        if (gameTimeCoroutine != null)
        {
            StopCoroutine(gameTimeCoroutine);
        }

        gameTimeCoroutine = StartCoroutine(GameTimeCoroutine(time));

        // 初始化家具配置
        for (int i = 0; i < levelData.content.Length; i++)
        {
            var content = levelData.content[i];
            var furniture = new Furniture()
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

            switch (content)
            {
                case "冰箱":
                    firge.Launch(furniture);
                    break;
            }
        }
    }

    IEnumerator GameTimeCoroutine(int time)
    {
        gameTimeSlider.gameObject.SetActive(true);
        gameTimeSlider.value = 0;

        while (true)
        {
            yield return new WaitForSeconds(1f);
            Tween.UISliderValue(gameTimeSlider, gameTimeSlider.value + 1, 1f);
            if (gameTimeSlider.value >= time)
            {
                break;
            }
        }

        gameTimeSlider.gameObject.SetActive(false);
        if (Utils.currentLevel < Utils.MAX_LEVEL)
        {
            LevelProgressPanel.Instance.ShowVictoryPanel();
        }
        else
        {
            LevelProgressPanel.Instance.ShowEndPanel();
        }
    }

}
