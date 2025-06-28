using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Button playGameBtn;
    public Button zhiZuoRenBtn;
    public Button exitGameBtn;

    public GameObject maskBg;

    private void Awake()
    {
        PrimeTweenConfig.warnZeroDuration = false;
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.defaultEase = Ease.Linear;

        Utils.isInitGame = true;

        playGameBtn.onClick.AddListener(OnPlayGame);
        zhiZuoRenBtn.onClick.AddListener(OnZhiZuoRen);
        exitGameBtn.onClick.AddListener(OnExitGame);

        CSVMgr.Init();
        AudioMgr.Instance.Init();

        maskBg.SetActive(false);
    }

    void Start()
    {
        Utils.currentLevel = 1;

        AudioMgr.Instance.PlayMusic("bgm1");
    }

    public void OnPlayGame()
    {
        maskBg.SetActive(true);
        var image = maskBg.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
        Tween.Alpha(image, 1, 0.2f).OnComplete(() =>
        {
            StartCoroutine(WaitForMaskBg());
        });
    }

    public void OnZhiZuoRen()
    {

    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator WaitForMaskBg()
    {
#if UNITY_EDITOR
        yield return null;
#else
        yield return new WaitForSeconds(3f);
#endif
        SceneManager.LoadScene("MainScene");
    }
}
