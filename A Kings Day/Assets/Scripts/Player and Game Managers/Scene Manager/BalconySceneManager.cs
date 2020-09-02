using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Utilities;
using Kingdoms;
using Drama;
using Dialogue;
using Balcony;
using ResourceUI;
using Characters;

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
                    scenePointHandler.SwitchScenePointsInteraction(false);
                    // Player Data Check
                    InitializeBalconyPlayerData();
                    // Player Campaign Data
                    InitializeBalconyCampaignData();
                    TransitionManager.GetInstance.SetAsCurrentManager(SceneType.Balcony);
                    interactionHandler.SetupInteractablesInformation();
                }
                else
                {
                    if (TransitionManager.GetInstance.shortCutTestDebug)
                    {
                        ShortcutTest();
                    }
                    interactionHandler.SwitchInteractableClickables(false);
                    MakeAllBuildingsFixed();
                    StartCoroutine(DelayPrologueDrama());
                }

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

        public void ShortcutTest()
        {
            PlayerGameManager.GetInstance.ReceiveTroops(25, "Recruit");
            PlayerGameManager.GetInstance.ReceiveTroops(15, "Archer");
            PlayerGameManager.GetInstance.ReceiveTroops(20, "Swordsman");
            PlayerGameManager.GetInstance.ReceiveTroops(20, "Spearman");

            ResourceInformationController.GetInstance.UpdateCurrentPanel();

            BaseHeroInformationData playerAsHero = new BaseHeroInformationData();
            playerAsHero = TransitionManager.GetInstance.unitStorage.ObtainHeroBaseInformation(WieldedWeapon.Spear);
            playerAsHero.unitInformation.unitName = "Player";
            playerAsHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";

            PlayerGameManager.GetInstance.playerData.myHeroes = new List<BaseHeroInformationData>();
            PlayerGameManager.GetInstance.playerData.tavernHeroes = new List<BaseHeroInformationData>();

            BaseHeroInformationData tmp = new BaseHeroInformationData();
            tmp.unitInformation = playerAsHero.unitInformation;
            tmp.heroLevel = playerAsHero.heroLevel;
            tmp.heroRarity = playerAsHero.heroRarity;

            tmp.healthGrowthRate = playerAsHero.healthGrowthRate;
            tmp.damageGrowthRate = playerAsHero.damageGrowthRate;
            tmp.speedGrowthRate = playerAsHero.speedGrowthRate;


            tmp.skillsList = new List<BaseSkillInformationData>();
            for (int i = 0; i < playerAsHero.skillsList.Count; i++)
            {
                BaseSkillInformationData tmpSkill = new BaseSkillInformationData();
                tmpSkill = playerAsHero.skillsList[i];
                tmp.skillsList.Add(tmpSkill);
            }

            PlayerGameManager.GetInstance.playerData.myHeroes.Add(tmp);
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
                player.OrderMovement(balconyPoint, ()=> player.OrderToFace(Characters.FacingDirection.Right));
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
