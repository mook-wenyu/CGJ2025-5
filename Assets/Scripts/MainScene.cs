using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public GameObject mainMenuUIPanel;
    public GameObject bgPanel;
    public GameObject fgPanel;
    public GameObject mainMenu;
    public GameObject maskBg;

    public GameObject initStartPanel;
    public Image startImg;

    private Coroutine fadeInOutLoop;


    private void Awake()
    {
        if (!GameMgr.isInitGame)
        {
            SceneManager.LoadScene("StartScene");
        }

        maskBg.SetActive(true);
        bgPanel.SetActive(true);
        mainMenu.SetActive(true);
        fgPanel.transform.localPosition = new Vector2(1920, 0);
        fgPanel.SetActive(true);
        initStartPanel.GetComponent<Button>().onClick.AddListener(OnStartClick);
        initStartPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        var image = maskBg.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 1f);
        Tween.Alpha(image, 0, 0.2f).OnComplete(() =>
        {
            Tween.LocalPositionX(mainMenu.transform, -1600, 0.8f, ease: Ease.InOutCirc).OnComplete(() =>
            {
                mainMenu.SetActive(false);
            });
            Tween.LocalPositionX(fgPanel.transform, 0, 0.8f, ease: Ease.InOutCirc).OnComplete(() =>
            {
                bgPanel.SetActive(false);

                initStartPanel.SetActive(true);
                fadeInOutLoop = StartCoroutine(FadeInOutLoop());
            });
            maskBg.SetActive(false);
        });
    }

    IEnumerator FadeInOutLoop()
    {
        while (true)
        {
            Tween.Alpha(startImg, 0, 0.8f);
            yield return new WaitForSeconds(0.8f);
            Tween.Alpha(startImg, 1, 0.8f);
            yield return new WaitForSeconds(0.8f);
        }
    }

    void OnStartClick()
    {
        StopCoroutine(fadeInOutLoop);
        fadeInOutLoop = null;

        Tween.LocalPositionX(fgPanel.transform, -2660f, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            fgPanel.SetActive(false);
        });
        Tween.LocalPositionX(initStartPanel.transform, -Screen.width, 0.5f, ease: Ease.InOutCirc).OnComplete(() =>
        {
            initStartPanel.SetActive(false);

            WorldSceneRoot.Instance.ResetWorld(GameMgr.currentLevel);
        });
    }
}
