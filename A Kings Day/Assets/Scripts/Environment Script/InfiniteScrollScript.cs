using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScrollScript : MonoBehaviour
{
    public float scrollSpeed = -5f;
    private float topLocalPos = 1.2f;
    private float btmLocalPos = -2.4f;

    Vector2 startPos;
    public bool startmoving = true;
    private void Start()
    {
        startPos = transform.position;
    }


    public void Update()
    {
        if(startmoving)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + (Time.deltaTime *scrollSpeed));
        
            if(transform.localPosition.y <= btmLocalPos)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, topLocalPos);
                startPos = transform.localPosition;
            }

        }
    }
}
