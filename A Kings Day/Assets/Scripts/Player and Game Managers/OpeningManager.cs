﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using TMPro;
using Kingdoms;

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
        if(appFirstStart && TransitionManager.GetInstance.previousScene == SceneType.Courtroom)
        {
            startBtn.ResetText();
            startBtn.AddTransition(TransitionToKingdomCreation);
            TransitionManager.GetInstance.SetAsCurrentManager(gameView);
            startBtn.gameObject.SetActive(true);
            appFirstStart = false;
        }
        else
        {
            startBtn.gameObject.SetActive(false);
        }
    }
    public void TransitionToKingdomCreation()
    {
        Debug.Log("Transitioning to Kingdom Creation");
        TransitionManager.GetInstance.TransitionToNextGameView(GameViews.KingdomCreationView);
    }
    public override void CloseManager()
    {
        base.CloseManager();
        startBtn.gameObject.SetActive(false);
    }
}