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
    public bool allowMesgControl = false;
    public bool isDialogueMesg = false;
    [Header("Interval Mechanics")]
    public DialogueDeliveryType deliveryType;
    public string currentMesg;
    public int mesgLength;
    public int letterIdx;
    public float intervalPerLetter;
    public float intervalCounter;

    [Header("Dramatic Mechanics")]
    public bool dramaticPauseDetected = false;
    public float dramaticPause = 1.25f;
    public float dramaticPauseCounter;
    public List<int> characterIndexPauses;
    public Action afterMessageCallback;
    public Action lastLetterCallback;
    public void Update()
    {       
        if (startTyping)
        {
            intervalCounter += (isDialogueMesg) ? Time.deltaTime: 0.025f;
            if (intervalCounter >= intervalPerLetter)
            {
                if (characterIndexPauses != null && characterIndexPauses.Count > 0)
                {
                    if (characterIndexPauses.Contains(letterIdx))
                    {
                        dramaticPauseCounter += Time.deltaTime;
                        if (dramaticPauseCounter > dramaticPause)
                        {
                            dramaticPauseCounter = 0;
                            intervalCounter = 0;
                            letterIdx += 2;
                            AddLetter();
                        }
                    }
                    else
                    {
                        intervalCounter = 0;
                        AddLetter();
                    }
                }
                else
                {
                    intervalCounter = 0;
                    AddLetter();
                }
            }

            if (allowMesgControl)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    if(isDialogueMesg && Time.timeScale == 0)
                    {
                        return;
                    }

                    startTyping = false;
                    intervalCounter = 0;
                    letterIdx = (mesgLength - 1);
                    mesgText.text = currentMesg.Replace("||", "");

                    if(lastLetterCallback != null)
                    {
                        lastLetterCallback();
                        lastLetterCallback = null;
                    }
                }
            }
        }
        else if (fadeAfter && letterIdx >= mesgLength - 1)
        {
            FadingMessage();
        }
        else
        {
            if (deliveryType == DialogueDeliveryType.EnterAfterEachSpeech)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    if (!isDialogueMesg && Time.timeScale == 0)
                    {
                        return;
                    }

                    if(afterMessageCallback != null)
                    {
                        afterMessageCallback();
                        if(isDialogueMesg)
                        {
                            afterMessageCallback = null;
                        }
                    }
                }
            }
        }
    }

    public void FadingMessage()
    {
        mesgText.color = new Color(mesgText.color.r, mesgText.color.g, mesgText.color.b, mesgText.color.a - Time.deltaTime);
    }

    public void SetMessageAsFade()
    {
        mesgText.color = new Color(mesgText.color.r, mesgText.color.g, mesgText.color.b, 0.95f);
    }

    public void SetMessageAsFadest()
    {
        mesgText.color = new Color(mesgText.color.r, mesgText.color.g, mesgText.color.b, 0.65f);

    }
    public void SetMessageColor(Color newColor)
    {
        mesgText.color = newColor;
    }

    public void SetTypeWriterMessage(string newMessage, bool startNow = true, Action newCallback = null, Action endingCallback = null)
    {
        ClearCurrentMessage();
        afterMessageCallback = newCallback;
        currentMesg = newMessage;
        mesgLength = currentMesg.Length;
        CheckForDramaticPauses();
        SwitchTyping(startNow);
        lastLetterCallback = endingCallback;
    }

    public void CheckForDramaticPauses()
    {
        characterIndexPauses = new List<int>();

        if(currentMesg.Contains("||"))
        {
            characterIndexPauses.Add(currentMesg.IndexOf("||"));
            currentMesg.Replace("||", "");
        }

        if(characterIndexPauses.Count > 0)
        {
            dramaticPauseDetected = true;
        }
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
        afterMessageCallback = null;
        letterIdx = 0;
        mesgText.text = "";
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
            if(lastLetterCallback != null)
            {
                lastLetterCallback();
            }
            if(deliveryType != DialogueDeliveryType.EnterAfterEachSpeech)
            {
                if(afterMessageCallback != null)
                {
                    afterMessageCallback();
                    afterMessageCallback = null;
                }
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
