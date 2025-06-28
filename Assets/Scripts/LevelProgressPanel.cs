using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelProgressPanel : MonoSingleton<LevelProgressPanel>
{
    public GameObject fgPanel;

    public GameObject victoryPanel;
    public GameObject failPanel;
    public Button tryAgainBtn;
    public Button backMainMenuBtn;

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

        victoryPanel.SetActive(false);
        failPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Show()
    {
        fgPanel.transform.localPosition = new Vector2(2660, 0);
        fgPanel.SetActive(true);
        Tween.LocalPositionX(fgPanel.transform, 0, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            victoryPanel.SetActive(true);
        });
    }

    void OnVictoryPanel()
    {
        Tween.LocalPositionX(fgPanel.transform, -2660, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            fgPanel.SetActive(false);
        });
        Tween.LocalPositionX(victoryPanel.transform, -1920, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            victoryPanel.SetActive(false);
        });
    }

    void OnTryAgainBtn()
    {
        ResetLevel();
    }

    void OnBackMainMenuBtn()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void ResetLevel()
    {

    }

    public void NextLevel()
    {
        Utils.currentLevel++;
    }
}
