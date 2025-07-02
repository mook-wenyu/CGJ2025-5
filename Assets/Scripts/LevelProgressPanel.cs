using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelProgressPanel : MonoSingleton<LevelProgressPanel>
{
    public GameObject fgPanel;

    public GameObject victoryPanel;
    public Scrollbar levelProgressScrollbar;
    public TextMeshProUGUI levelProgressText;

    public GameObject failPanel;
    public Button tryAgainBtn;
    public Button backMainMenuBtn;

    public GameObject endPanel;

    private readonly List<float> progress = new List<float>();

    void Awake()
    {
        progress.Add(0);
        progress.Add(0.17f);
        progress.Add(0.337f);
        progress.Add(0.503f);
        progress.Add(0.668f);
        progress.Add(0.834f);
        progress.Add(1f);

        victoryPanel.GetComponent<Button>().onClick.AddListener(OnVictoryPanel);
        tryAgainBtn.GetComponent<Button>().onClick.AddListener(OnTryAgainBtn);
        backMainMenuBtn.GetComponent<Button>().onClick.AddListener(OnBackMainMenuBtn);
        endPanel.GetComponent<Button>().onClick.AddListener(OnEndPanel);

        victoryPanel.SetActive(false);
        failPanel.SetActive(false);
        endPanel.SetActive(false);
    }

    public void ShowVictoryPanel()
    {
        if (GameMgr.isFailed || GameMgr.isVictory)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.isVictory = true;
        GameMgr.isFailed = false;

        victoryPanel.transform.localPosition = new Vector2(1920, 0);
        fgPanel.transform.localPosition = new Vector2(2660, 0);
        victoryPanel.SetActive(true);
        fgPanel.SetActive(true);

        if (GameMgr.currentLevel < GameMgr.MAX_LEVEL)
        {
            levelProgressScrollbar.value = progress[GameMgr.currentLevel];
            levelProgressText.text = GetLevelProgressText(GameMgr.currentLevel + 1);
        }
        Tween.LocalPositionX(victoryPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
        Tween.LocalPositionX(fgPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
    }

    void OnVictoryPanel()
    {
        Tween.LocalPositionX(fgPanel.transform, -2660, 0.5f, ease: Ease.InOutCirc);
        Tween.LocalPositionX(victoryPanel.transform, -1920, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            fgPanel.SetActive(false);
            victoryPanel.SetActive(false);
            NextLevel();
        });
    }

    public void ShowEndPanel()
    {
        if (GameMgr.isFailed || GameMgr.isVictory)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.isVictory = true;
        GameMgr.isFailed = false;

        endPanel.transform.localPosition = new Vector2(1920, 0);
        fgPanel.transform.localPosition = new Vector2(2660, 0);
        endPanel.SetActive(true);
        fgPanel.SetActive(true);

        Tween.LocalPositionX(endPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
        Tween.LocalPositionX(fgPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
    }

    public void ShowFailPanel(string name)
    {
        if (GameMgr.isFailed || GameMgr.isVictory)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.isVictory = false;
        GameMgr.isFailed = true;

        if (WorldSceneRoot.Instance.gameTimeCoroutine != null)
        {
            StopCoroutine(WorldSceneRoot.Instance.gameTimeCoroutine);
        }

        failPanel.SetActive(true);
    }

    void OnTryAgainBtn()
    {
        failPanel.SetActive(false);
        GameMgr.isFailed = false;
        GameMgr.isVictory = false;
        WorldSceneRoot.Instance.ResetWorld(GameMgr.currentLevel);
    }

    void OnBackMainMenuBtn()
    {
        GameMgr.isFailed = false;
        GameMgr.isVictory = false;
        SceneManager.LoadScene("StartScene");
    }

    void OnEndPanel()
    {
        SceneManager.LoadScene("StartScene");
    }

    void NextLevel()
    {
        GameMgr.currentLevel++;
        GameMgr.isFailed = false;
        GameMgr.isVictory = false;
        WorldSceneRoot.Instance.ResetWorld(GameMgr.currentLevel);
    }

    string GetLevelProgressText(int level)
    {
        return level switch
        {
            1 => "第一关",
            2 => "第二关",
            3 => "第三关",
            4 => "第四关",
            5 => "第五关",
            6 => "第六关",
            7 => "第七关",
            _ => "",
        };
    }
}
