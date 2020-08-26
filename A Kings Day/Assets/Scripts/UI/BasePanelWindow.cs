using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BasePanelWindow : MonoBehaviour
{
    public Transform origParent;
    public bool enabledBased = false;
    public bool transferEnabled = true;

    public Action parentOpenCallback;
    public Action parentCloseCallback;
    public void Awake()
    {
        if(transform.parent != null)
        {
            origParent = transform.parent;
        }
    }
    public void OnEnable()
    {
        if(enabledBased)
        {
            OpenWindow();
        }
    }

    public void OnDisable()
    {
        CloseWindow();
    }
    public virtual void OpenWindow()
    {
        if(PanelWindowManager.GetInstance != null)
        {
            PanelWindowManager.GetInstance.AddWindow(this);
        }

        if(parentOpenCallback != null)
        {
            parentOpenCallback();
        }
    }


    public virtual void CloseWindow()
    {
        if(PanelWindowManager.GetInstance != null)
        {
            PanelWindowManager.GetInstance.CloseWindow(this);
        }

        if (enabledBased)
        {
            this.gameObject.SetActive(false);
        }

        if(parentCloseCallback != null)
        {
            parentCloseCallback();
        }
    }
}
