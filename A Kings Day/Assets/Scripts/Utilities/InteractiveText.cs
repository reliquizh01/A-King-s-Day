using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using UnityEngine.EventSystems;
public enum TextStates
{
    Normal,
    FastBlink,
    SlowFadeInOut,
    Hide,
    Deleted,
    TimerText, // To be Implemented
}
public class InteractiveText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public bool clickable = false;
    private bool isClicked = false;
    public TextStates currentState;
    public TextStates clickedState;
    public TextStates initialState;
    public TextStates afterClickState;
    public bool initialized = false;
    public bool disableOnClick = false;

    // Numeric Data
    [SerializeField] private float currentAlpha = 0;
    [SerializeField] private float targetTextAlpha = 0;
    [SerializeField] private float fadeSpd = 0.55f;
    [SerializeField] private float slowFadeSpd = 0.55f;
    [SerializeField] private int currentBlinkCount = 0;
    [SerializeField] private int blinkCount = 15;
    [SerializeField] private float currentblinkInterval = 0f;
    [SerializeField] private float blinkInterval = 0.35f;
    // Callbacks
    private List<Action> callbacks = new List<Action>();
    private Action hoverCallback, exitCallback;

    public void Awake()
    {
        if(text == null)
        {
            if(this.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                text = this.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }
        else
        {
            initialized = true;
        }
    }
    public void SetHoverCallback(Action newCallBack)
    {
        hoverCallback = newCallBack;
    }
    public void SetExitCallback(Action newCallBack)
    {
        exitCallback = newCallBack;
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(exitCallback != null)
        {
            exitCallback();
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (hoverCallback != null)
        {
            hoverCallback();
        }
    }
    public void Update()
    {
        if(currentState == TextStates.SlowFadeInOut)
        {
            fadeSpd = slowFadeSpd;
            if (targetTextAlpha == 1)
            {
                FadeIn();
            }
            else if(targetTextAlpha == 0)
            {
                FadeOut();
            }
        }
        else if(currentState == TextStates.FastBlink)
        {

            currentblinkInterval += Time.deltaTime * 2.5f;
            if(currentblinkInterval > blinkInterval && currentBlinkCount < blinkCount)
            {
                if(targetTextAlpha == 1)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                    targetTextAlpha = 0;
                }
                else if(targetTextAlpha == 0)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                    targetTextAlpha = 1;
                }
                // Interval per blink
                currentblinkInterval = 0;

                if (currentBlinkCount < blinkCount)
                {
                    currentBlinkCount += 1;
                }
            }
            if(currentBlinkCount >= blinkCount)
            {
                CheckNextState();
            }
        }
    }

    public void ResetInteraction()
    {
        isClicked = false;
    }
    public void AddTransition(Action callback)
    {
        callbacks.Add(callback);
    }

    public void CallTransition()
    {
        if(callbacks != null && callbacks.Count > 0)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Invoke();
            }
        }
    }
    public void GoToClickState()
    {
        if(!isClicked)
        {
            isClicked = true;
            currentState = clickedState;
        }
    }
    public void GoToAfterClickState()
    {
        CallTransition();
        currentState = afterClickState;
        if(disableOnClick)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            isClicked = false;
        }
        CheckNextState();
    }
    private void FadeIn()
    {
        currentAlpha += Time.deltaTime * fadeSpd;
        text.color = new Color(text.color.r, text.color.g, text.color.b, currentAlpha);
        if(currentAlpha >= targetTextAlpha)
        {
            CheckNextState();
        }
        
    }
    private void FadeOut()
    {
        currentAlpha -= Time.deltaTime * fadeSpd;
        text.color = new Color(text.color.r, text.color.g, text.color.b, currentAlpha);
        if (currentAlpha <= targetTextAlpha)
        {
            CheckNextState();
        }
    }
    
    private void CheckNextState()
    {
        switch (currentState)
        {
            case TextStates.SlowFadeInOut:
                if (currentAlpha >= 1)
                {
                    targetTextAlpha = 0;
                }
                else if(currentAlpha <= 0.01f)
                {
                    targetTextAlpha = 1;
                }
                break;


            case TextStates.FastBlink:
               // Debug.Log("[BUTTON EVENT] " + this.gameObject.name + " is blinking");
                if (currentBlinkCount < blinkCount)
                {
                    currentBlinkCount += 1;
                }
                else
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                    targetTextAlpha = 1;
                    currentBlinkCount = 0;
                }
                break;
            case TextStates.Normal:
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                break;
            case TextStates.Hide:
                //Debug.Log("[BUTTON EVENT] " + this.gameObject.name + " is hiding");
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                break;
        }
        if(currentState == clickedState && clickable)
        {
            GoToAfterClickState();
        }
    }
    public void ResetText()
    {
        this.gameObject.SetActive(true);
        isClicked = false;
        currentState = initialState;
    }
}
