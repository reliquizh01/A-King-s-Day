using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Kingdoms;
using GameResource;

public class ResourceWarningHandler : MonoBehaviour
{
    public TextMeshProUGUI notifMesg;
    public TextMeshProUGUI textMesg;
    public bool isShowing = false;
    public bool isMultiple = false;
    public List<WarningMessageClass> shownWarningDatas;
    public float curSwitchCounter = 0.0f;
    public float curMesgDuration = 2.5f;
    public int warningIdx = 0;

    public void Update()
    {
        if(isMultiple)
        {
            curSwitchCounter += Time.deltaTime;
            if(curSwitchCounter >= curMesgDuration)
            {
                curSwitchCounter = 0;
                if(warningIdx < shownWarningDatas.Count-1)
                {
                    warningIdx += 1;
                }
                else
                {
                    warningIdx = 0;
                }

                ShowWarning(shownWarningDatas[warningIdx].message);
            }
        }
    }
    public void SetupWarningDatas(List<WarningMessageClass> newData)
    {
        if(shownWarningDatas == null)
        {
            shownWarningDatas = new List<WarningMessageClass>();
        }

        shownWarningDatas.Clear();
        isMultiple = false;
        warningIdx = 0;

        shownWarningDatas.AddRange(newData);

        if(shownWarningDatas.Count > 1)
        {
            isMultiple = true;
        }

        ShowWarning(shownWarningDatas[warningIdx].message);
    }

    public void ShowWarning(string mesg)
    {
        notifMesg.gameObject.SetActive(true);
        textMesg.gameObject.SetActive(true);
        textMesg.text = mesg;
        isShowing = true;
    }
    public void HideWarning()
    {
        notifMesg.gameObject.SetActive(false);
        textMesg.gameObject.SetActive(false);
        isShowing = false;
        shownWarningDatas.Clear();
    }
}
