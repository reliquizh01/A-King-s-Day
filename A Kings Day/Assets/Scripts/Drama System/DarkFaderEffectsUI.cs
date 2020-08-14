using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DarkFaderEffectsUI : MonoBehaviour
{
    public Image imageToFade;
    public bool isFading;
    public float curAlpha;
    public float targetAlpha;
    public float fadeSpd = 0.015f;

    public Action afterFadeCallback;
    public void Update()
    {
        if(isFading)
        {
            if(targetAlpha >= 1)
            {
                curAlpha += fadeSpd;
                if(curAlpha >= targetAlpha)
                {
                    curAlpha = targetAlpha;
                    isFading = false;
                    if (afterFadeCallback != null)
                    {
                        afterFadeCallback();
                    }
                }
            }
            else
            {
                curAlpha -= fadeSpd;
                if (curAlpha <= targetAlpha)
                {
                    curAlpha = targetAlpha;
                    isFading = false;
                    imageToFade.enabled = false;
                    if (afterFadeCallback != null)
                    {
                        afterFadeCallback();
                    }
                }
            }
            imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, curAlpha);
        }
    }
    public void FadeToClear(Action newAfterFadeAction = null)
    {
        targetAlpha = 0;
        isFading = true;
        afterFadeCallback = newAfterFadeAction;
    }

    public void FadeToDark(Action newAfterFadeAction = null)
    {
        targetAlpha = 1;
        isFading = true;
        imageToFade.enabled = true;
        afterFadeCallback = newAfterFadeAction;
    }
}
