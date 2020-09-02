using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Buildings;
using Utilities;
using Battlefield;
using System;
using Kingdoms;

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

        public BattlefieldTutorialController battlefieldTutorial;
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

            if(TransitionManager.GetInstance.isNewGame)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
                BattlefieldPathManager.GetInstance.SetupFieldPaths();
                InitializeCampaignBattles(true);
                battlefieldTutorial.StartBattlefieldTutorial(true);
                isCampaignMode = true;
            }
            else
            {
                if(TransitionManager.GetInstance != null && TransitionManager.GetInstance.previousScene != SceneType.Opening)
                {

                    isCampaignMode = true;

                    if (TransitionManager.GetInstance != null)
                    {
                        HideCustomBattlePanel();
                        TransitionManager.GetInstance.RemoveLoading(()=> InitializeCampaignBattles());
                    }
                }
                else
                {
                    ShowCustomBattlePanel();
                }
            }
            Loaded = true;
        }

        public void InitializeCampaignBattles(bool isFromCreationScene = false)
        {
            if (!TransitionManager.GetInstance.isEngagedWithTraveller &&
                !TransitionManager.GetInstance.isEngagedWithMapPoint)
            {
                return;
            }

            BattlefieldCommander enemyCommander = new BattlefieldCommander();

            BaseTravellerData tmp = new BaseTravellerData();
            tmp = PlayerGameManager.GetInstance.unitsToSend;
            

            // If He's the defender
            if (!TransitionManager.GetInstance.isPlayerAttacker)
            {
                // MAP NODE
                if(TransitionManager.GetInstance.isEngagedWithMapPoint)
                {
                    if(!string.IsNullOrEmpty(TransitionManager.GetInstance.attackedPointInformationData.pointName))
                    {
                        tmp.troopsCarried.AddRange(TransitionManager.GetInstance.attackedPointInformationData.troopsStationed);
                        tmp.leaderUnit.Add(TransitionManager.GetInstance.attackedPointInformationData.leaderUnit);
                    }

                    if (TransitionManager.GetInstance.attackedPointInformationData.travellersOnPoint != null &&
                       TransitionManager.GetInstance.attackedPointInformationData.travellersOnPoint.Count > 0)
                    {
                        enemyCommander = ObtainCampaignMapPointEnemyCommander();
                    }
                }   
                // TRAVELLER
                else
                {
                    if(TransitionManager.GetInstance.attackedTravellerData.ObtainTotalUnitCount() > 0)
                    {
                        Debug.Log("Initializing Battle Thru Traveller");
                        enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(TransitionManager.GetInstance.attackedTravellerData);
                    }
                }
            }
            else //  If He's the attacker
            {
                // MAP NODE
                if(TransitionManager.GetInstance.isEngagedWithMapPoint)
                {
                    if (!string.IsNullOrEmpty(TransitionManager.GetInstance.attackedPointInformationData.pointName))
                    {
                        Debug.Log("Initializing Battle Thru Map Point");
                        enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(TransitionManager.GetInstance.attackedPointInformationData);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(TransitionManager.GetInstance.attackedTravellerData.travellersName))
                    {
                        Debug.Log("Initializing Battle Thru Traveller");
                        enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(TransitionManager.GetInstance.attackedTravellerData);
                    }
                }
            }

            spawnManager.SetupPlayerCommander(tmp, TransitionManager.GetInstance.isPlayerAttacker);




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

            InitializeArea(isFromCreationScene);
        }
        public BattlefieldCommander ObtainCampaignMapPointEnemyCommander()
        {
            BattlefieldCommander enemyCommander = new BattlefieldCommander();
            List<BaseTravellerData> enemyOnPointTravellerList = new List<BaseTravellerData>();
            enemyOnPointTravellerList.AddRange(TransitionManager.GetInstance.attackedPointInformationData.travellersOnPoint.FindAll(x => x.relationship < 0));
            enemyCommander.teamAffiliation = TransitionManager.GetInstance.attackedPointInformationData.ownedBy;

            // CONVERT ALL HATEFUL UNITS IN THE POINT TO 1 TRAVELLER
            BaseTravellerData enemyTravellers = new BaseTravellerData();
            enemyTravellers.troopsCarried = new List<TroopsInformation>();
            enemyTravellers.leaderUnit = new List<BaseHeroInformationData>();
            if (enemyOnPointTravellerList.Count > 0)
            {
                for (int i = 0; i < enemyOnPointTravellerList.Count; i++)
                {
                    enemyTravellers.leaderUnit.AddRange(enemyOnPointTravellerList[i].leaderUnit);

                    for (int x = 0; x < enemyOnPointTravellerList[i].troopsCarried.Count; x++)
                    {
                        if (enemyTravellers.troopsCarried.Count > 0)
                        {
                            int idx = -1;
                            idx = enemyTravellers.troopsCarried.FindIndex(y => y.unitInformation.unitName == enemyOnPointTravellerList[i].troopsCarried[i].unitInformation.unitName);
                            if (idx != -1)
                            {
                                enemyTravellers.troopsCarried[idx].totalUnitCount += enemyOnPointTravellerList[i].troopsCarried[i].totalUnitCount;
                            }
                            else
                            {
                                enemyTravellers.troopsCarried.Add(enemyOnPointTravellerList[i].troopsCarried[i]);
                            }
                        }
                        else
                        {
                            enemyTravellers.troopsCarried.Add(enemyOnPointTravellerList[i].troopsCarried[i]);
                        }
                    }
                }
            }
            enemyCommander = BattlefieldCommander.ConvertTravellerToCommander(enemyTravellers);

            return enemyCommander;
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
        public void InitializeArea(bool isFromCreationScene = false)
        {
            battleUIInformation.SetUnitPanels(spawnManager.attackingCommander, spawnManager.defendingCommander);

            if(spawnManager.attackingCommander.heroesCarried != null && spawnManager.attackingCommander.heroesCarried.Count > 0)
            {
                battleUIInformation.SetAttackerLeaderSkills(spawnManager.attackingCommander.heroesCarried[0]);
            }

            if (spawnManager.defendingCommander.heroesCarried != null && spawnManager.defendingCommander.heroesCarried.Count > 0)
            {
                battleUIInformation.SetDefenderLeaderSkill(spawnManager.defendingCommander.heroesCarried[0]);
            }

            if(!isFromCreationScene)
            {
                BattlefieldSystemsManager.GetInstance.StartDay();
            }
            BattlefieldSpawnManager.GetInstance.SummonTeamHeroes();
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