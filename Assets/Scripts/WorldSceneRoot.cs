using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class WorldSceneRoot : MonoSingleton<WorldSceneRoot>
{
    public Slider gameTimeSlider;

    [Header("World")]
    public GameObject bedroomRoot;
    public GameObject kitchenRoot;

    private Camera mainCamera;

    private Coroutine gameTimeCoroutine;

    void Awake()
    {
        mainCamera = Camera.main;
        gameTimeSlider.value = 0;
        gameTimeSlider.gameObject.SetActive(false);
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
    }

    IEnumerator GameTimeCoroutine(int time)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            // yield return new WaitForSeconds(1f);
            Tween.UISliderValue(gameTimeSlider, gameTimeSlider.value + 1, 1f);
            if (gameTimeSlider.value >= time)
            {
                break;
            }
        }

        if (Random.Range(0, 100) < 50)
        {
            gameTimeSlider.gameObject.SetActive(false);
            if (Utils.currentLevel < 7)
            {
                LevelProgressPanel.Instance.ShowVictoryPanel();
            }
            else
            {
                // TODO: 显示通关界面
            }
        }
        else
        {
            LevelProgressPanel.Instance.ShowFailPanel();
        }
    }

}
