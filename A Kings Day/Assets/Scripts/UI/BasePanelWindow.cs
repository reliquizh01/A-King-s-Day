using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanelWindow : MonoBehaviour
{
    public bool enabledBased = false;

    public void OnEnable()
    {
        if(enabledBased)
        {
            OpenWindow();
        }
    }
    public virtual void OpenWindow()
    {
        if(PanelWindowManager.GetInstance != null)
        {
            PanelWindowManager.GetInstance.AddWindow(this);
        }
    }


    public virtual void CloseWindow()
    {
        if(PanelWindowManager.GetInstance != null)
        {
            PanelWindowManager.GetInstance.CloseWindow(this);
            if(enabledBased)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
