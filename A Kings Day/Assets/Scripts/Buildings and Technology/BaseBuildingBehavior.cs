using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Kingdoms;
using Utilities;
using Managers;
using UnityEngine.EventSystems;
using ResourceUI;
namespace Buildings
{
    public enum BuildingType
    {
        Shop,
        Barracks,
        Tavern,
        Smithery,
        Houses,
        Farm,
        Market,
    }
    /// <summary>
    /// Handles all basic behavior of buildings inside the balcony scene
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BaseBuildingBehavior : BaseInteractableBehavior
    {
        [Header("Building Condition")]
        public BuildingOptionHandler optionHandler;
        public GameObject ruinEmpty, fixFilled;


        [Header("Building Information")]
        public string informationName;
        public BuildingInformationData buildingInformation;
        public BuildingType buildingType;
        public int repairPrice = 20;


        [Header("Test And Debugging Only")]
        [SerializeField] private bool testMode = false;
        public override void Start()
        {
            base.Start();
            if(optionHandler != null)
            {
                optionHandler.myBuilding = this;
            }
            myCol = GetComponent<BoxCollider2D>();
            
        }

        public override void SetupInteractableInformation()
        {
            base.SetupInteractableInformation();
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;

            if(PlayerGameManager.GetInstance != null && playerData.buildingInformationData != null 
                && playerData.buildingInformationData.Count > 0 && 
                playerData.buildingInformationData.Find( x=> x.buildingName == this.buildingInformation.BuildingName) != null)
            {
                BuildingSavedData tmp = playerData.buildingInformationData.Find(x => x.buildingName == buildingInformation.BuildingName);

                buildingInformation = TransitionManager.GetInstance.currentSceneManager.buildingInformationStorage.ObtainBuildingOperation(informationName);

                buildingInformation.buildingCondition = tmp.buildingCondition;
                buildingInformation.buildingLevel = tmp.buildingLevel;
                buildingInformation.buildingType = tmp.buildingType;
            }
            else if(TransitionManager.GetInstance != null)
            {
                buildingInformation = TransitionManager.GetInstance.currentSceneManager.buildingInformationStorage.ObtainBuildingOperation(informationName);
            }
            else if(testMode)
            {
                buildingInformation = BalconySceneManager.GetInstance.buildingInformationStorage.ObtainBuildingOperation(buildingType);
            }

            UpdateBuildingState();

        }

        public void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (!string.IsNullOrEmpty(mesg) && !isInteractingWith)
            {
                Parameters p = new Parameters();
                p.AddParameter<string>("Mesg", mesg);
                EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
            }
        }

        public void OnMouseExit()
        {
            if (!string.IsNullOrEmpty(mesg))
            {
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
            }
        }
        public void OnMouseDown()
        {
            if (!isClickable) return;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (!optionHandler.isOpen && isClickable)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
                EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_IN_GAME_INTERACTION);
                optionHandler.OpenIcons();
                isInteractingWith = true;
                isClickable = false;
            }
        }

        public void UpdateBuildingState()
        {
            optionHandler.UpdateRuinState();
            switch (buildingInformation.buildingCondition)
            {
                case BuildingCondition.Ruins:
                    ruinEmpty.gameObject.SetActive(true);
                    fixFilled.gameObject.SetActive(false);
                    break;
                case BuildingCondition.Functioning:
                ruinEmpty.gameObject.SetActive(false);
                fixFilled.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        public void UpgradeBuilding()
        {
            if (!PlayerGameManager.GetInstance.CheckResourceEnough(buildingInformation.repairPrice, ResourceType.Coin))
            {
                optionHandler.ShowOptionInsufficient(OptionType.Upgrade);
                return;
            }


            if (buildingInformation.buildingCondition == BuildingCondition.Ruins)
            {
                PlayerGameManager.GetInstance.RemoveResource(buildingInformation.repairPrice, ResourceType.Coin);
                ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(ResourceType.Coin, buildingInformation.repairPrice, false);
                PlayerGameManager.GetInstance.playerData.buildingInformationData.Find(x => x.buildingName == this.buildingInformation.BuildingName).buildingCondition = BuildingCondition.Functioning;
                buildingInformation.buildingCondition = BuildingCondition.Functioning;

                UpdateBuildingState();
            }

            optionHandler.CloseIcons();

            SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
        }

        public void TechBuilding()
        {
            if (BalconyManager.GetInstance != null)
            {
                BalconyManager.GetInstance.OpenTechTab(buildingType);
            }
        }
        public void UseBuilding()
        {
            if(BalconyManager.GetInstance != null)
            {
                BalconyManager.GetInstance.OpenBuildingOperationTab(this);
            }
        }
    }
}
