using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Kingdoms;

public class ResourceWarningHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI notifMesg;
    public TextMeshProUGUI textMesg;
    public bool isShowing = false;

    public void ShowWarning(string mesg)
    {
        notifMesg.gameObject.SetActive(true);
        textMesg.text = mesg;
        isShowing = true;
    }
    public void HideWarning()
    {
        notifMesg.gameObject.SetActive(false);
        isShowing = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isShowing)
        {
            textMesg.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isShowing)
        {
            textMesg.gameObject.SetActive(false);
        }
    }
}
