﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountingEffectUI : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public int curCount;
    public int targetCount;

    public int increment = 1;

    private float intervalPerChange = 0.05f;
    private float curInterval = 0;
    public bool startUpdating = false;
    public bool enableColor = false;

    public void Update()
    {
        if(curCount != targetCount && startUpdating)
        {
            curInterval += Time.deltaTime;
            if(enableColor)
            {
                if(curCount < targetCount)
                {
                    countText.color = Color.green;
                }
                else
                {
                    countText.color = Color.red;
                }
            }
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
            countText.text = curCount.ToString();
        }
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