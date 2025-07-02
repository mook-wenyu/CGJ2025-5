using UnityEngine;

public class WMPlant : MonoBehaviour
{
    public float rotateSpeedType1 = 50f;
    public float rotateSpeedType2 = 150f;

    private bool isRotating = false;
    private float currentSpeed = 0f;

    public void StartRotate(FurnitureStatus status)
    {
        if (status == FurnitureStatus.Special)
            currentSpeed = rotateSpeedType1;
        else if (status == FurnitureStatus.Dark)
            currentSpeed = rotateSpeedType2;

        isRotating = true;
        gameObject.SetActive(true);
    }

    public void StopRotate()
    {
        isRotating = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isRotating && !GameMgr.IsTimePaused)
        {
            transform.Rotate(0, 0, currentSpeed * GameMgr.timeScale * Time.deltaTime);
        }
    }
}
