using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class AliveRedPiont : MonoSingleton<AliveRedPiont>
{
    public Camera mainCamera;
    public GameObject redPointObjRight;
    public GameObject redPointObjLeft;

    private int rightCount;
    private int leftCount;

    private Coroutine rightRotateCoroutine;
    private Coroutine leftRotateCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;
        redPointObjRight.SetActive(false);
        redPointObjLeft.SetActive(false);
    }

    public void AddRedPoint(bool isRight)
    {
        if (isRight)
        {
            rightCount += 1;
            if (!redPointObjRight.activeSelf)
            {
                redPointObjRight.SetActive(true);
                rightRotateCoroutine = StartCoroutine(RedPointRightRotate());
            }
        }
        else
        {
            leftCount += 1;
            if (!redPointObjLeft.activeSelf)
            {
                redPointObjLeft.SetActive(true);
                leftRotateCoroutine = StartCoroutine(RedPointLeftRotate());
            }
        }
    }

    public void RemoveRedPoint(bool isRight)
    {
        if (isRight)
        {
            rightCount -= 1;
            if (rightCount <= 0)
            {
                if (rightRotateCoroutine != null)
                {
                    StopCoroutine(rightRotateCoroutine);
                    rightRotateCoroutine = null;
                }
                rightCount = 0;
                redPointObjRight.SetActive(false);
            }
        }
        else
        {
            leftCount -= 1;
            if (leftCount <= 0)
            {
                if (leftRotateCoroutine != null)
                {
                    StopCoroutine(leftRotateCoroutine);
                    leftRotateCoroutine = null;
                }
                leftCount = 0;
                redPointObjLeft.SetActive(false);
            }
        }
    }

    public void ClearRedPoint()
    {
        if (rightRotateCoroutine != null)
        {
            StopCoroutine(rightRotateCoroutine);
            rightRotateCoroutine = null;
        }
        if (leftRotateCoroutine != null)
        {
            StopCoroutine(leftRotateCoroutine);
            leftRotateCoroutine = null;
        }
        rightCount = 0;
        leftCount = 0;
        redPointObjRight.SetActive(false);
        redPointObjLeft.SetActive(false);
    }

    IEnumerator RedPointRightRotate()
    {
        while (redPointObjRight.activeSelf)
        {
            yield return Tween.LocalRotation(redPointObjRight.transform, new Vector3(0, 0, 25), 0.5f, Ease.InOutSine).ToYieldInstruction();
            yield return Tween.LocalRotation(redPointObjRight.transform, new Vector3(0, 0, -25), 0.5f, Ease.InOutSine).ToYieldInstruction();
        }
    }

    IEnumerator RedPointLeftRotate()
    {
        while (redPointObjLeft.activeSelf)
        {
            yield return Tween.LocalRotation(redPointObjLeft.transform, new Vector3(0, 0, -25), 0.5f, Ease.InOutSine).ToYieldInstruction();
            yield return Tween.LocalRotation(redPointObjLeft.transform, new Vector3(0, 0, 25), 0.5f, Ease.InOutSine).ToYieldInstruction();
        }
    }
}
