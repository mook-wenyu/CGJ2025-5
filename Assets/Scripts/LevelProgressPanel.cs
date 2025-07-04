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
    public Sprite[] victorySprites;
    public Image victoryImg;
    public TextMeshProUGUI victoryText;

    public Scrollbar levelProgressScrollbar;
    public TextMeshProUGUI levelProgressText;

    public GameObject failPanel;
    public Sprite[] failSprites;
    public Image failImg;
    public TextMeshProUGUI failText;

    public Button tryAgainBtn;
    public Button backMainMenuBtn;

    public GameObject endPanel;

    private readonly List<float> progress = new List<float>();

    private Dictionary<string, Sprite> homeAliveSprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        progress.Add(0);
        progress.Add(0.17f);
        progress.Add(0.337f);
        progress.Add(0.503f);
        progress.Add(0.668f);
        progress.Add(0.834f);
        progress.Add(1f);

        homeAliveSprites.Add("冰箱", failSprites[0]);
        homeAliveSprites.Add("智能垃圾桶", failSprites[1]);
        homeAliveSprites.Add("扫地机器人", failSprites[2]);
        homeAliveSprites.Add("滚筒洗衣机", failSprites[3]);
        homeAliveSprites.Add("烧水壶", failSprites[4]);
        homeAliveSprites.Add("空调", failSprites[5]);
        homeAliveSprites.Add("时钟", failSprites[6]);

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
        if (GameMgr.currentState != GameState.Playing)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.currentState = GameState.Victory;

        victoryPanel.transform.localPosition = new Vector2(1920, 0);
        fgPanel.transform.localPosition = new Vector2(2660, 0);
        victoryPanel.SetActive(true);
        fgPanel.SetActive(true);

        if (GameMgr.currentLevel < GameMgr.MAX_LEVEL)
        {
            levelProgressScrollbar.value = progress[GameMgr.currentLevel];
            levelProgressText.text = GetLevelProgressText(GameMgr.currentLevel + 1);

            victoryImg.sprite = victorySprites[GameMgr.currentLevel];
            victoryText.text = CSVMgr.Get<LevelConfigConfig>((GameMgr.currentLevel + 1).ToString()).introduce;
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
        if (GameMgr.currentState != GameState.Playing)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.currentState = GameState.End;

        endPanel.transform.localPosition = new Vector2(1920, 0);
        fgPanel.transform.localPosition = new Vector2(2660, 0);
        endPanel.SetActive(true);
        fgPanel.SetActive(true);

        Tween.LocalPositionX(endPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
        Tween.LocalPositionX(fgPanel.transform, 0, 0.5f, ease: Ease.InOutCirc);
    }

    public void ShowFailPanel(string name)
    {
        if (GameMgr.currentState != GameState.Playing)
        {
            return;
        }

        GameMgr.PauseTime();
        GameMgr.currentState = GameState.Failed;

        if (WorldSceneRoot.Instance.gameTimeCoroutine != null)
        {
            StopCoroutine(WorldSceneRoot.Instance.gameTimeCoroutine);
        }

        failPanel.SetActive(true);
        failImg.sprite = homeAliveSprites[name];
        failText.text = CSVMgr.Get<HomeAliveConfig>(name).failure_lines;
    }

    void OnTryAgainBtn()
    {
        failPanel.SetActive(false);
        GameMgr.currentState = GameState.Playing;
        WorldSceneRoot.Instance.ResetWorld(GameMgr.currentLevel);
    }

    void OnBackMainMenuBtn()
    {
        GameMgr.currentState = GameState.Playing;
        SceneManager.LoadScene("StartScene");
    }

    void OnEndPanel()
    {
        SceneManager.LoadScene("StartScene");
    }

    void NextLevel()
    {
        GameMgr.currentLevel++;
        GameMgr.currentState = GameState.Playing;
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
