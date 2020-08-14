using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class OverheadDialogue2DUI : MonoBehaviour
{
    public TextMeshProUGUI mTextOverHead;
    public Transform mTransform;
    public Transform mTextOverTransform;

    [Header("Dialogue Mechanics")]
    public bool showText = false;
    public string currentMesg;
    public float curDuration;
    private float durationCounter;
    void Awake()
    {
        mTransform = transform;
        mTextOverTransform = mTextOverHead.transform;
    }

    public void Update()
    {

    }
    void LateUpdate()
    {
        if(showText)
        {
            durationCounter += Time.deltaTime;

            if(durationCounter >= curDuration)
            {
                showText = false;
                HideText();
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(mTransform.position);
            // add a tiny bit of height?
            screenPos.y += 2; // adjust as you see fit.
            mTextOverTransform.position = screenPos;
            
        }
    }

    public void ShowDialogue(string newDialogue, float duration = 2)
    {
        showText = true;
        durationCounter = 0;
        curDuration = duration;
        mTextOverHead.text = "'"+newDialogue+ "'";
        mTextOverHead.gameObject.SetActive(true);
    }
    public void HideText()
    {
        showText = false;
        mTextOverHead.gameObject.SetActive(false);
    }
}
