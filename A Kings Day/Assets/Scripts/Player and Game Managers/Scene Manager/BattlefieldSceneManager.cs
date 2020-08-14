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
        public CustomBattlePanelHandler customBattlePanel;
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

            Debug.Log("Checkmate");
            if(TransitionManager.GetInstance != null && TransitionManager.GetInstance.previousScene != SceneType.Opening)
            {
                Debug.Log("Dolce");
                isCampaignMode = true;

                if (TransitionManager.GetInstance != null)
                {
                    HideCustomBattlePanel();
                    TransitionManager.GetInstance.RemoveLoading(InitializeCampaignBattles);
                }
            }
            else
            {
                Debug.Log("FUCKING PUSSY");
                ShowCustomBattlePanel();
            }
            Loaded = true;
        }

        public void InitializeCampaignBattles()
        {
            if (!TransitionManager.GetInstance.isEngagedWithTraveller &&
                !TransitionManager.GetInstance.isEngagedWithMapPoint)
            {
                return;
            }

            List<TroopsInformation> tmp = new List<TroopsInformation>();
            for (int i = 0; i < PlayerGameManager.GetInstance.playerData.troopsList.Count; i++)
            {
                tmp.Add(PlayerGameManager.GetInstance.playerData.troopsList[i]);
            }
            spawnManager.SetupPlayerCommander(tmp, TransitionManager.GetInstance.isPlayerAttacker);

            BattlefieldCommander enemyCommander = new BattlefieldCommander();

            if(!string.IsNullOrEmpty(TransitionManager.GetInstance.attackedTravellerData.travellersName))
            {
                Debug.Log("Initializing Battle Thru Traveller");
                enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(TransitionManager.GetInstance.attackedTravellerData);
            }
            else if(!string.IsNullOrEmpty(TransitionManager.GetInstance.attackedPointInformationData.pointName))
            {
                Debug.Log("Initializing Battle Thru Map Point");
                enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(TransitionManager.GetInstance.attackedPointInformationData);
            }

            if(TransitionManager.GetInstance.isPlayerAttacker)
            {
                SwitchAttackerControls(PlayerControlType.PlayerOne);
                SwitchDefenderControls(PlayerControlType.Computer);
                BattlefieldSystemsManager.GetInstance.playerTeam = TeamType.Attacker;

                spawnManager.ImplementTechnology(spawnManager.attackingCommander);
                spawnManager.SetupDefendingCommander(enemyCommander);
            }
            else
            {
                SwitchAttackerControls(PlayerControlType.Computer);
                SwitchDefenderControls(PlayerControlType.PlayerOne);
                BattlefieldSystemsManager.GetInstance.playerTeam = TeamType.Defender;

                spawnManager.ImplementTechnology(spawnManager.defendingCommander);
                spawnManager.SetupAttackingCommander(enemyCommander);


            }

            InitializeArea();
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
            Debug.Log("[Ending Todays Battle]");
            // Add Cutscenes here
            BattlefieldSystemsManager.GetInstance.unitsInCamp = true;
            BattlefieldSpawnManager.GetInstance.UpdateCommanderResources();
            battleUIInformation.ShowdailyReportPanel();
        }

        public void SwitchAttackerControls(PlayerControlType newControlType)
        {
            battleUIInformation.attackerPanel.SetControlType(newControlType, true);
            battleUIInformation.attackerReportPanel.SetControlType(newControlType);
        }

        public void SwitchDefenderControls(PlayerControlType newControlType)
        {
            battleUIInformation.defenderPanel.SetControlType(newControlType, false);
            battleUIInformation.defenderReportPanel.SetControlType(newControlType);
        }

        public void CheckCommanderReadiness()
        {
             battleUIInformation.CheckReadiness();
        }
    }
}