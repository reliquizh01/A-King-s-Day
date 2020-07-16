using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class TypeWriterEffectUI : MonoBehaviour
{
    public TextMeshProUGUI mesgText;
    public bool startTyping;
    public bool fadeAfter;
    [Header("Interval Mechanics")]
    public string currentMesg;
    public int mesgLength;
    public int letterIdx;
    public float intervalPerLetter;
    public float intervalCounter;

    Action afterMessageCallback;

    public void Update()
    {
        if(startTyping)
        {
            intervalCounter += Time.deltaTime;
            if(intervalCounter >= intervalPerLetter)
            {
                intervalCounter = 0;
                AddLetter();
            }
        }
        else if(fadeAfter && letterIdx >= mesgLength -1)
        {
            FadeMessage();
        }
    }

    public void FadeMessage()
    {
        mesgText.color = new Color(mesgText.color.r, mesgText.color.g, mesgText.color.b, mesgText.color.a - Time.deltaTime);
    }
    public void SetMessageColor(Color newColor)
    {
        mesgText.color = newColor;
    }

    public void SetTypeWriterMessage(string newMessage, bool startNow = true, Action newCallback = null)
    {
        ClearCurrentMessage();
        afterMessageCallback = newCallback;
        currentMesg = newMessage;
        mesgLength = currentMesg.Length;

        SwitchTyping(startNow);

    }

    public void ExtendCurrentMessage(string addedMesg, bool startNow = true, Action afterExtendCallback = null)
    {
        afterMessageCallback = afterExtendCallback;

        currentMesg = string.Concat(currentMesg, addedMesg);
        mesgLength = currentMesg.Length;

        SwitchTyping(startNow);
    }
    public void SwitchTyping(bool startNow)
    {
        startTyping = startNow;

    }

    public void ClearCurrentMessage()
    {
        letterIdx = 0;
        if (startTyping)
        {
            mesgText.text = "";
            mesgText.color = new Color(mesgText.color.r, mesgText.color.g, mesgText.color.b, 1.0f);
        }
    }
    public void AddLetter()
    {
        if(letterIdx > mesgLength-1)
        {
            startTyping = false;
            intervalCounter = 0;
            if(afterMessageCallback != null)
            {
                afterMessageCallback();
            }
        }
        else
        {
            mesgText.text = mesgText.text + currentMesg[letterIdx];

            if(letterIdx <= mesgLength-1)
            {
                letterIdx += 1;
            }
        }
    }

}
