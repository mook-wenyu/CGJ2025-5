using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class WorldSceneRoot : MonoBehaviour
{
    [Header("UI")]
    public GameObject bedroomUIRoot;
    public GameObject kitchenUIRoot;

    [Header("World")]
    public GameObject bedroomRoot;
    public GameObject kitchenRoot;

    private float kitchenRootX;

    void Awake()
    {
        kitchenUIRoot.transform.localPosition = new Vector2(Screen.width, 0);

        kitchenRootX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;
        kitchenRoot.transform.position = new Vector3(kitchenRootX, kitchenRoot.transform.position.y, kitchenRoot.transform.position.z);

        kitchenUIRoot.SetActive(false);
        kitchenRoot.SetActive(false);
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
            if (kitchenRoot.transform.position.x <= 0)
            {
                bedroomUIRoot.SetActive(true);
                bedroomRoot.SetActive(true);

                Tween.LocalPositionX(bedroomUIRoot.transform, 0, 0.3f);
                Tween.PositionX(bedroomRoot.transform, 0, 0.3f);

                Tween.LocalPositionX(kitchenUIRoot.transform, Screen.width, 0.3f);
                Tween.PositionX(kitchenRoot.transform, kitchenRootX, 0.3f).OnComplete(() =>
                {
                    kitchenUIRoot.SetActive(false);
                    kitchenRoot.SetActive(false);
                });
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (kitchenRoot.transform.position.x >= kitchenRootX)
            {
                kitchenUIRoot.SetActive(true);
                kitchenRoot.SetActive(true);

                Tween.LocalPositionX(kitchenUIRoot.transform, 0, 0.3f);
                Tween.PositionX(kitchenRoot.transform, 0, 0.3f);

                Tween.LocalPositionX(bedroomUIRoot.transform, -Screen.width, 0.3f);
                Tween.PositionX(bedroomRoot.transform, -kitchenRootX, 0.3f).OnComplete(() =>
                {
                    bedroomUIRoot.SetActive(false);
                    bedroomRoot.SetActive(false);
                });

            }
        }
    }
}
