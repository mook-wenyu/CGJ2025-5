using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class WorldSceneRoot : MonoBehaviour
{
    [Header("World")]
    public GameObject bedroomRoot;
    public GameObject kitchenRoot;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {

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
}
