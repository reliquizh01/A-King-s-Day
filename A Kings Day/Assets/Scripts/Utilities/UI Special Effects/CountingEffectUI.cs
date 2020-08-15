﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountingEffectUI : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public string numberColorText;

    public int curCount;
    public int targetCount;
    public int increment = 1;

    private float intervalPerChange = 0.05f;
    private float curInterval = 0;
    public bool startUpdating = false;
    public bool enableColor = false;

    [Header("Before and After Count Messages")]
    public string preCountMesg;
    public string postCountMesg;
    public void Update()
    {
        if(curCount != targetCount && startUpdating)
        {
            curInterval += Time.deltaTime;
            if(curInterval >= intervalPerChange)
            {
                curInterval = 0;
                int dif = Mathf.Abs(curCount - targetCount);
                AdjustIncrement(dif);

                if(curCount > targetCount)
                {
                    curCount -= increment;
                }
                else
                {
                    curCount += increment;
                }
            }
            if(string.IsNullOrEmpty(numberColorText))
            {
                countText.text = preCountMesg + " " + curCount.ToString() + " " + postCountMesg;
            }
            else
            {
                countText.text = preCountMesg + " " + numberColorText + curCount.ToString() + "</color>" + " " + postCountMesg;
            }
        }
    }
    public void SetNumberColor(string newColor)
    {
        numberColorText = newColor;
    }

    public void SetTargetCount(int newTarget, bool startQuickly = true)
    {
        targetCount = newTarget;

        if (enableColor)
        {
            if (curCount < targetCount)
            {
                countText.color = Color.green;
            }
            else if(curCount == targetCount)
            {
                countText.color = Color.white;
            }
            else
            {
                countText.color = Color.red;
            }
        }
        if(startQuickly)
        {
            startUpdating = true;
        }

        Debug.Log("From: " + postCountMesg);
    }
    public void AdjustIncrement(int difference)
    {
        if(difference > 500)
        {
            increment = 100;
        }
        else if(difference > 100)
        {
            increment = 25;
        }
        else if(difference > 30)
        {
            increment = 10;
        }
        else if(difference > 10)
        {
            increment = 2;
        }
        else
        {
            increment = 1;
        }
    }
}
