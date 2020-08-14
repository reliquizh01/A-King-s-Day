using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public enum ButtonActionChangeType
{
    Down,
    Up,
    Enter,
    Exit,
}
public class ButtonChangeUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler 
{
    public Image buttonIcon;

    public Sprite normalIcon;
    public Sprite onClickIcon;
    public Sprite onHoverIcon;

    [Header("Resize Mechanic")]
    public float clickedSize = 0.95f;
    public float normalSize = 1.0f;
    public bool isClicked = false;

    private List<Action> onClickCallbacks;
    private List<Action> onUpCallbacks;
    private List<Action> onEnterCallbacks;
    private List<Action> onExitCallbacks;

    public void Awake()
    {
        if (onClickCallbacks == null)
        {
            onClickCallbacks = new List<Action>();
        }

        if (onUpCallbacks == null)
        {
            onUpCallbacks = new List<Action>();
        }

        if (onEnterCallbacks == null)
        {
            onEnterCallbacks = new List<Action>();
        }

        if (onExitCallbacks == null)
        {
            onExitCallbacks = new List<Action>();
        }
    }
    public void Start()
    {

        normalIcon = buttonIcon.sprite;

        onEnterCallbacks.Add(OnEnterVisualEffects);
        onClickCallbacks.Add(OnClickVisualEffects);
        onUpCallbacks.Add(OnUpVisualEffects);
        onExitCallbacks.Add(OnExitVisualEffects);
    }


    public void AddActionCallback(ButtonActionChangeType thisType, Action thisAction)
    {
        switch (thisType)
        {
            case ButtonActionChangeType.Down:
                onClickCallbacks.Add(thisAction);
                break;
            case ButtonActionChangeType.Up:
                onUpCallbacks.Add(thisAction);
                break;
            case ButtonActionChangeType.Enter:
                onEnterCallbacks.Add(thisAction);
                break;
            case ButtonActionChangeType.Exit:
                onExitCallbacks.Add(thisAction);
                break;
            default:
                break;
        }
    }
    public void OnClickVisualEffects()
    {
        isClicked = true;
        if(isClicked)
        {
            ButtonIsClicked();
            buttonIcon.sprite = onClickIcon;
        }
    }

    public void OnUpVisualEffects()
    {
        if(isClicked)
        {
            isClicked = false;
            ButtonIsNormal();
        }
    }

    public void OnEnterVisualEffects()
    {
        buttonIcon.sprite = onHoverIcon;
    }

    public void OnExitVisualEffects()
    {
        ButtonIsNormal();
        buttonIcon.sprite = normalIcon;
    }

    public void ButtonIsClicked()
    {
        buttonIcon.transform.localScale = new Vector3(clickedSize, clickedSize, clickedSize);
    }

    public void ButtonIsNormal()
    {
        buttonIcon.transform.localScale = new Vector3(normalSize, normalSize, normalSize);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUpCallbacks != null && onUpCallbacks.Count > 0)
        {
            for (int i = 0; i < onUpCallbacks.Count; i++)
            {
                onUpCallbacks[i]();
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(onClickCallbacks != null && onClickCallbacks.Count > 0)
        {
            for (int i = 0; i < onClickCallbacks.Count; i++)
            {
                onClickCallbacks[i]();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnterCallbacks != null && onEnterCallbacks.Count > 0)
        {
            for (int i = 0; i < onEnterCallbacks.Count; i++)
            {
                onEnterCallbacks[i]();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExitCallbacks != null && onExitCallbacks.Count > 0)
        {
            for (int i = 0; i < onExitCallbacks.Count; i++)
            {
                onExitCallbacks[i]();
            }
        }
    }


}
