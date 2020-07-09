using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Buildings;
using Utilities;
using Battlefield;
using System;

namespace Managers
{
    public class BattlefieldSceneManager : BaseSceneManager
    {
        #region Singleton
        private static BattlefieldSceneManager instance;
        public static BattlefieldSceneManager GetInstance
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

        [Header("Battlefield Information")]
        public BattlefieldSpawnManager spawnManager;
        public BattlefieldUIHandler battleUIInformation;
        public GameObject customizePanel;
        public bool isCampaignMode;
        public int warDays;

        public override void Start()
        {
            base.Start();
            if(TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsNewSceneManager(this);
            }
        }

        public override void PreOpenManager()
        {
            base.PreOpenManager();
            if(PlayerGameManager.GetInstance != null && !string.IsNullOrEmpty(PlayerGameManager.GetInstance.playerData.kingdomsName))
            {
                isCampaignMode = true;
            }
            else
            {
                ShowCustomBattlePanel();
            }
            Loaded = true;
            if(TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.RemoveLoading();
            }
        }

        public void ShowCustomBattlePanel()
        {
            customizePanel.gameObject.SetActive(true);
        }
        public void HideCustomBattlePanel()
        {
            customizePanel.gameObject.SetActive(false);
        }
        public void PreBattleStart()
        {
            if(customizePanel.activeInHierarchy)
            {
                HideCustomBattlePanel();
            }

            InitializeArea();
        }
        public void InitializeArea()
        {
            battleUIInformation.SetUnitPanels(spawnManager.attackingCommander, spawnManager.defendingCommander);
            BattlefieldSystemsManager.GetInstance.StartDay();
        }


        public void EndTodaysBattle()
        {
            // Add Cutscenes here
            BattlefieldSystemsManager.GetInstance.unitsInCamp = true;
            BattlefieldSpawnManager.GetInstance.UpdateCommanderResources();
            battleUIInformation.ShowdailyReportPanel();
        }
        public void CheckCommanderReadiness()
        {
            if(!isCampaignMode)
            {
                battleUIInformation.CheckReadiness();
            }
        }
    }
}