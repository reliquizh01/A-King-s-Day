using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Utilities;
using Kingdoms;
using Drama;
using Dialogue;
using Balcony;

namespace Managers
{
    public class BalconySceneManager : BaseSceneManager
    {
        #region Singleton
        private static BalconySceneManager instance;
        public static BalconySceneManager GetInstance
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

        public BalconyTutorialController balconyTutorial;
        public TravellingSystem travelSystem;
        public WallVisualController wallVisualController;

        [Header("Player Entry Mechanics")]
        public ScenePointBehavior balconyPoint;
        public ScenePointBehavior entryPoint;
        public override void Start()
        {
            base.Start();

           
            if(TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsNewSceneManager(this);

                if (TransitionManager.GetInstance.previousScene != sceneType)
                {
                    SetPositionFromTransition(TransitionManager.GetInstance.previousScene);
                }
            }
        }

        public void Update()
        {

        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
        }
        public override void StartManager()
        {
            if (testRun)
                return;
            base.StartManager();




            if (TransitionManager.GetInstance != null)
            {
                if(!TransitionManager.GetInstance.isNewGame)
                {
                    // Player Data Check
                    InitializeBalconyPlayerData();
                    // Player Campaign Data
                    InitializeBalconyCampaignData();
                    TransitionManager.GetInstance.SetAsCurrentManager(SceneType.Balcony);
                }

                if (TransitionManager.GetInstance.isNewGame)
                {
                    interactionHandler.SwitchInteractableClickables(false);
                    MakeAllBuildingsFixed();
                    StartCoroutine(DelayPrologueDrama());
                }

                interactionHandler.SetupInteractablesInformation();

                if (PlayerGameManager.GetInstance != null)
                {
                    if (PlayerGameManager.GetInstance.playerData.currentTechnologies == null || PlayerGameManager.GetInstance.playerData.currentTechnologies.Count <= 0)
                    {
                        PlayerGameManager.GetInstance.playerData.currentTechnologies = new List<Technology.BaseTechnologyData>();
                        PlayerGameManager.GetInstance.playerData.currentTechnologies.AddRange(TechnologyManager.GetInstance.InitializePlayerTech());
                    }
                }
            }
        }

        IEnumerator DelayPrologueDrama()
        {
            yield return new WaitForSeconds(2);

            PrologueDramaEvents();
        }

        public void InitializeBalconyCampaignData()
        {
            Debug.Log("-------------INITIALIZING CAMPAIGN DATA---------");
            PlayerCampaignData campaignData = new PlayerCampaignData();
            campaignData = PlayerGameManager.GetInstance.campaignData;

            if(campaignData.travellerList != null && campaignData.travellerList.Count > 0)
            {
                for (int i = 0; i < campaignData.travellerList.Count; i++)
                {
                    travelSystem.SummonSpecificTraveller(campaignData.travellerList[i], false);
                }
            }
            /*if (campaignData.travellerList != null && campaignData.travellerList.Count > 0)
            {
                for (int i = 0; i < campaignData.travellerList.Count; i++)
                {
                    travelSystem.SummonSpecificTraveller(campaignData.travellerList[i], false);
                }
            }*/
        }

        public void InitializeBalconyPlayerData()
        {
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;
            if (!playerData.balconyBuildingsAdded)
            {
                if (playerData.buildingInformationData == null)
                {
                    playerData.buildingInformationData = new List<BuildingSavedData>();
                }
                for (int i = 0; i < buildingInformationStorage.buildingOperationList.Count; i++)
                {
                    BuildingSavedData tmp = new BuildingSavedData();
                    tmp.buildingName = buildingInformationStorage.buildingOperationList[i].BuildingName;
                    tmp.buildingType = buildingInformationStorage.buildingOperationList[i].buildingType;
                    tmp.buildingLevel = buildingInformationStorage.buildingOperationList[i].buildingLevel;

                    if (TransitionManager.GetInstance != null && TransitionManager.GetInstance.isNewGame)
                    {
                        tmp.buildingCondition = BuildingCondition.Functioning;
                    }
                    else
                    {
                        tmp.buildingCondition = buildingInformationStorage.buildingOperationList[i].buildingCondition;
                    }

                    playerData.buildingInformationData.Add(tmp);
                }
                playerData.balconyBuildingsAdded = true;
            }

        }
        public void PrologueDramaEvents()
        {
            if(DramaticActManager.GetInstance != null)
            {
                DramaticActManager.GetInstance.PlayScene("[Part 4] Prologue - The kingdom's View", () => balconyTutorial.StartBalconyTutorial(true));
            }
        }

        public override void SetPositionFromTransition(SceneType prevScene, bool directToOffset = true)
        {
            if(TransitionManager.GetInstance != null && TransitionManager.GetInstance.isNewGame)
            {

            }
            else
            {
                player.SpawnInThisPosition(entryPoint);
                player.OrderMovement(balconyPoint);
            }
        }
        // PROLOGUE STUFF
        public void MakeAllBuildingsFixed()
        {
            List<BaseBuildingBehavior> buildings = new List<BaseBuildingBehavior>();

            for (int i = 0; i < interactionHandler.interactableList.Count; i++)
            {
                if(interactionHandler.interactableList[i].GetComponent<BaseBuildingBehavior>() != null)
                {
                    buildings.Add(interactionHandler.interactableList[i].GetComponent<BaseBuildingBehavior>());
                }
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].buildingInformation.buildingCondition = BuildingCondition.Functioning;
                buildings[i].UpdateBuildingState();
            }
        }
    }
}
