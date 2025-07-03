using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePoint : MonoBehaviour
{
    public GameObject son;

    public void SetSonOrderToTop()
    {
        if (son != null)
        {
            SpriteRenderer sr = son.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 9;
            }
        }
    }

    public void SetSonOrderToBottom()
    {
        if (son != null)
        {
            SpriteRenderer sr = son.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 13;
            }
        }
    }
}
