using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ShakingEffectUI : BaseEffectUI
{
    public RectTransform myRect;
    public TextMeshProUGUI shakingText;

    public float shakingSpeed = 30.5f;
    private bool goingLeft = false;
    public float targetRotZ = 15f;
    private float origTargetRotZ = 0;
    public float curRotZ = 0.0f;

    public float curDuration = 0.0f;
    public float sfxDuration = 2f;

    // Interval
    public bool enableInterval = false;
    public float curInterval = 0;
    public float interval = 3;

    // Font Increase/Decrease
    public bool enableFontBeat = false;
    public float originalFont;
    public float fontSizeSpeed = 3.0f;
    public float curFont = 0;
    public float targetFont = 30.0f;
    private float incDuration = 0;
    private float decDuration = 0;
    private bool isIncreasing = true;
    public void Start()
    {
        if (myRect == null)
        {
            myRect = this.GetComponent<RectTransform>();
        }
        origTargetRotZ = targetRotZ;

        if (shakingText == null)
        {
            shakingText = this.GetComponent<TextMeshProUGUI>();
        }

        if (shakingText != null)
        {

            originalFont = shakingText.fontSize;
            curFont = originalFont;
            targetFont = curFont + targetFont;

            incDuration = sfxDuration / 2;
            decDuration = incDuration;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (enableInterval && curDuration >= sfxDuration)
        {
            curInterval += Time.deltaTime;
            if (curInterval >= interval)
            {
                curInterval = 0;
                curDuration = 0;
            }
        }
        if (curDuration < sfxDuration)
        {
            if (enableFontBeat)
            {
                if (isIncreasing)
                {
                    curFont += fontSizeSpeed;
                    shakingText.fontSize = curFont;
                }
                else
                {
                    curFont -= fontSizeSpeed;
                    shakingText.fontSize = curFont;
                }
                if (curFont > targetFont)
                {
                    isIncreasing = false;
                }
                if (curFont <= originalFont && !isIncreasing)
                {
                    isIncreasing = true;
                    curFont = originalFont;
                }

            }
            curDuration += Time.deltaTime;
            if (goingLeft)
            {
                curRotZ += shakingSpeed;
                myRect.rotation = Quaternion.Euler((new Vector3(0, 0, curRotZ)));
            }
            else
            {
                curRotZ -= shakingSpeed;
                myRect.rotation = Quaternion.Euler((new Vector3(0, 0, curRotZ)));
            }

            if (curRotZ >= targetRotZ)
            {
                targetRotZ = -origTargetRotZ;
                goingLeft = false;
            }
            else
            {
                targetRotZ = origTargetRotZ;
                goingLeft = true;

            }
        }
        else
        {
            curRotZ = 0;
            myRect.rotation = Quaternion.Euler((new Vector3(0, 0, 0)));

            if (shakingText != null)
            {
                shakingText.fontSize = originalFont;
                curFont = originalFont;
            }
        }
    }
}
