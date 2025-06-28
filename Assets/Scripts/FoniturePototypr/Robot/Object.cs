using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public GameObject son;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSonOrderToTop()
    {
        if (son != null)
        {
            SpriteRenderer sr = son.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 1;
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
                sr.sortingOrder = 3;
            }
        }
    }
}
