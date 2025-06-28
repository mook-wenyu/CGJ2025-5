using UnityEngine;

public class plant : MonoBehaviour
{
    public float rotateSpeedType1 = 50f;
    public float rotateSpeedType2 = 150f;

    private bool isRotating = false;
    private float currentSpeed = 0f;

    public void StartRotate(int type)
    {
        if (type == 1)
            currentSpeed = rotateSpeedType1;
        else if (type == 2)
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
        if (isRotating)
        {
            transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
        }
    }
}
