using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimerUI : MonoBehaviour
{
    public bool startTimer = false;
    public bool isClockLike = true;
    public TextMeshProUGUI timerText;
    [Header("Time Information")]
    public int minute;
    public int seconds;

    private float curSecond;
    private Action afterTimeCallback;
    public void Update()
    {
        if(startTimer)
        {
            curSecond += Time.deltaTime;

            if (curSecond >= 1)
            {
                if(seconds > 0)
                {
                    seconds -= 1;
                }
                else
                {
                    if(minute > 0)
                    {
                        minute -= 1;
                        seconds = 59;
                    }
                    else
                    {
                        startTimer = false;
                        afterTimeCallback();
                    }
                }
                curSecond = 0;
                if(isClockLike)
                {
                    UpdateClockText();
                }
                else
                {
                    UpdateCountText();
                }
            }
        }
    }
    public void StartTimer(int newMin, int newSec, Action afterTimerCallback = null)
    {
        minute = newMin;
        seconds = newSec;

        startTimer = true;

        afterTimeCallback = afterTimerCallback;
        if(isClockLike)
        {
            UpdateClockText();
        }
        else
        {
            UpdateCountText();
        }
    }

    public void PauseTimer()
    {
        startTimer = false;
    }

    public void ResetTimer()
    {
        minute = 0;
        seconds = 0;

        startTimer = false;

        afterTimeCallback = null;
    }
    public void UpdateCountText()
    {
        int currentCount = minute * 60;
        currentCount += seconds;

        if(currentCount < 10)
        {
            timerText.text = "0" + currentCount.ToString();
        }
        else
        {
            timerText.text = currentCount.ToString();
        }
    }
    public void UpdateClockText()
    {
        if(seconds >= 10)
        {
            timerText.text = minute.ToString() + ":" + seconds.ToString();
        }
        else
        {
            timerText.text = minute.ToString() + ":0" + seconds.ToString();
        }
    }
}
