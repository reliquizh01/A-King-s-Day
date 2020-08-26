using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using TMPro;
public class MouseTooltip : MonoBehaviour
{
    public Canvas parentCanvas;
    public bool isShowing = false;
    public bool allowShowing = true;
    public TextMeshProUGUI mesgText;
    public Image bg;

    public float offsetX = 200f;
    public float offsetY = 50f;
    public void Awake()
    {
        if(!EventBroadcaster.Instance.HasObserverForEvent(EventNames.SHOW_TOOLTIP_MESG))
        {
            EventBroadcaster.Instance.AddObserver(EventNames.SHOW_TOOLTIP_MESG, ShowToolTip);
            EventBroadcaster.Instance.AddObserver(EventNames.HIDE_TOOLTIP_MESG, HideToolTip);
            EventBroadcaster.Instance.AddObserver(EventNames.ENABLE_TOOLTIP_MESG, EnableTooltip);
            EventBroadcaster.Instance.AddObserver(EventNames.DISABLE_TOOLTIP_MESG, DisableTooltip);
        }
    }

    public void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SHOW_TOOLTIP_MESG, ShowToolTip);
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.HIDE_TOOLTIP_MESG, HideToolTip);
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ENABLE_TOOLTIP_MESG, EnableTooltip);
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.DISABLE_TOOLTIP_MESG, DisableTooltip);
    }

    public void DisableTooltip(Parameters p = null)
    {
        allowShowing = false;
        HideToolTip();
    }

    public void EnableTooltip(Parameters p = null)
    {
        allowShowing = true;
    }

    public void Start()
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out pos);
    }

    public void Update()
    {
        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);
        if(Input.mousePosition.x >= Screen.width *0.90)
        {
            movePos = new Vector2(movePos.x - offsetX, movePos.y + offsetY);
        }
        else
        {
            movePos = new Vector2(movePos.x+ offsetX, movePos.y+ offsetY);
        }
        transform.position = parentCanvas.transform.TransformPoint(movePos);
    }
    public void ShowToolTip(Parameters p = null)
    {
        if(!allowShowing)
        {
            return;
        }
        isShowing = true;
        string mesg = p.GetWithKeyParameterValue("Mesg", "");

        mesgText.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);

        mesgText.text = mesg;
    }
    public void HideToolTip(Parameters p = null)
    {
        mesgText.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
        isShowing = false;
    }
}
