using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class BaseManager : MonoBehaviour
{
    public BasePanelBehavior panelBehaviour;
    public GameViews gameView;
    public bool Loaded = false;
    public bool isViewManager = true;
    public virtual void Start()
    {
        if(TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.AddManager(this);
        }


    }

    public virtual void OnDestroy()
    {
        if(TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.managerList.RemoveAll(x => x.thisManager == this);
        }
    }
    // START
    public virtual void PreOpenManager()
    {
        //Debug.Log("PreOpening Manager View :" + gameView);
        if (panelBehaviour != null && this.gameObject.activeInHierarchy && !string.IsNullOrEmpty(panelBehaviour.openAnimationName))
        {
            StartCoroutine(panelBehaviour.WaitAnimationForAction(panelBehaviour.openAnimationName, StartManager));
        }
        else
        {
          //  Debug.Log("Manager has no Animation, Overriding Start");
            StartManager();
        }
    }
    public virtual void StartManager()
    {

        Debug.Log("Starting Manager View :" + gameView);
        Loaded = true;
    }

    public void CallBackTransitionOff()
    {
        TransitionManager.GetInstance.inTransition = false;
        Loaded = true;
    }

    // CLOSING
    public virtual void PreCloseManager()
    {
        if (panelBehaviour != null && !string.IsNullOrEmpty(panelBehaviour.closeAnimationName))
        {
            StartCoroutine(panelBehaviour.WaitAnimationForAction(panelBehaviour.closeAnimationName, CloseManager));
        }
        else
        {

            CloseManager();
        }
    }
    public virtual void CloseManager()
    {
        //Debug.Log("Closing Manager View : " + gameView);
        CallBackTransitionOff();
    }

    public virtual void CheckAndOnCamera()
    {

    }

    public virtual void PlayThisBackGroundMusic(BackgroundMusicType thisType)
    {
        if(AudioManager.GetInstance != null)
        {
            AudioManager.GetInstance.PlayThisBackGroundMusic(thisType);
        }
    }
}
