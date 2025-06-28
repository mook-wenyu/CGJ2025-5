using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    private void Awake()
    {
        PrimeTweenConfig.warnZeroDuration = false;
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.defaultEase = Ease.Linear;

        Utils.isInitGame = true;
        CSVMgr.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
