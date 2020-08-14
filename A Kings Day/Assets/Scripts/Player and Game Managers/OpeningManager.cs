using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using TMPro;
using Kingdoms;
using UnityEngine.UI;
using SaveData;
using System.Linq;

public class OpeningManager : BaseManager
{
    #region Singleton
    private static OpeningManager instance;
    public static OpeningManager GetInstance
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public InteractiveText startBtn;
    public bool appFirstStart = true;
    public override void Start()
    {
        base.Start();
        if(appFirstStart)
        {
            TransitionManager.GetInstance.SetAsCurrentManager(gameView);
            appFirstStart = false;

            AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.openingTheme);
        }
    }

    public override void PreOpenManager()
    {
        base.PreOpenManager();
        panelBehaviour.PlayOpenAnimation();
    }
    public void OpenOptions()
    {
        if(TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.ShowOptions(true);
        }
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void TransitionToKingdomCreation()
    {
        if(SaveLoadManager.GetInstance != null)
        {
            if(SaveLoadManager.GetInstance.saveDataList != null
                && SaveLoadManager.GetInstance.saveDataList.Count > 0)
            {
                TransitionManager.GetInstance.TransitionToNextGameView(SceneType.Creation);
            }
            else
            {
                TransitionManager.GetInstance.LoadScene(SceneType.Creation);
            }
        }
        else
        {
            TransitionManager.GetInstance.LoadScene(SceneType.Creation);
        }
        panelBehaviour.PlayCloseAnimation();

    }

    public void TransitionToCustomBattle()
    {
        panelBehaviour.PlayCloseAnimation();
        TransitionManager.GetInstance.LoadScene(SceneType.Battlefield);
        PlayThisBackGroundMusic(BackgroundMusicType.battlefieldPreparation1);
    }
    public override void PreCloseManager()
    {
        base.PreCloseManager();
    }
    public override void CloseManager()
    {
        base.CloseManager();
    }

    public void MouseHoverOnOptions(GameObject thisOption)
    {
        if(!thisOption.GetComponent<Button>().interactable)
        {
            return;
        }
        if(AudioManager.GetInstance != null)
        {
            AudioManager.GetInstance.PlayDecisionHover();
        }
        thisOption.transform.localScale = new Vector3(1.15f, 1.15f, 1.0f);
    }
    public void MouseExitOnOptions(GameObject thisOption)
    {
        thisOption.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

}
