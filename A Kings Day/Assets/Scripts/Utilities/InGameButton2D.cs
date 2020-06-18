using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A Personalized Form of In Game Button
/// </summary>
public class InGameButton2D : MonoBehaviour
{
    public Action onClickAction;
    public bool clickHeld;
    public Animation anim;
    public bool btnEnabled;
    public SpriteRenderer spriteRend;

    public void Update()
    {
        if (clickHeld)
        {
#if UNITY_ANDROID
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                //We transform the touch position into word space from screen space and store it.
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                //We now raycast with this information. If we have hit something we can process it.
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                if (hitInformation.collider != null)
                {
                    if (hitInformation.transform != this)
                    {
                        clickHeld = false;
                    }
                }
            }
#else
            if (Input.GetMouseButtonUp(0))
            {
                //We transform the touch position into word space from screen space and store it.
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                //We now raycast with this information. If we have hit something we can process it.
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                if (hitInformation.collider != null)
                {
                    if (hitInformation.transform != this)
                    {
                        clickHeld = false;
                    }
                }
            }
#endif

        }
    }
    public void SwitchButtonInteraction(bool newState)
    {
        btnEnabled = newState;
        if(!btnEnabled)
        {
            spriteRend.color = new Color(1, 1, 1, 0.5f);
        }
    }
    public void OnMouseDown()
    {
        if(btnEnabled)
        {
            clickHeld = true;
        }
    }

    public void OnMouseUp()
    {
        if(clickHeld)
        {
            if(onClickAction != null)
            {   
                onClickAction();
            }
        }
    }
}
