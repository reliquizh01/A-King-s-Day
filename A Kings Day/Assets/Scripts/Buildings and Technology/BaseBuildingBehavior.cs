using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Kingdoms;
using Utilities;
using Managers;
using UnityEngine.EventSystems;
namespace Buildings
{
    /// <summary>
    /// Handles all basic behavior of buildings inside the balcony scene
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BaseBuildingBehavior : BaseInteractableBehavior
    {
        [Header("Building Condition")]
        public BuildingCondition currentCondition;
        public BuildingOptionHandler optionHandler;
        public GameObject ruinEmpty, fixFilled;


        [Header("Building Information")]
        public string informationName;
        public BuildingInformationData buildingInformation;
        public ResourceType resTechType;
        public int repairPrice = 20;
        private BoxCollider2D myCol;
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
            buildingInformation = TransitionManager.GetInstance.currentSceneManager.buildingInformationStorage.ObtainBuildingOperation(informationName);
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

        public void UpgradeBuiilding()
        {
            if (!PlayerGameManager.GetInstance.CheckResourceEnough(repairPrice, ResourceType.Coin))
            {
                optionHandler.ShowOptionInsufficient(OptionType.Upgrade);
                return;
            }

            PlayerGameManager.GetInstance.RemoveResource(repairPrice, ResourceType.Coin);
            if(currentCondition == BuildingCondition.Ruins)
            {
                currentCondition = BuildingCondition.Functioning;
                optionHandler.UpdateRuinState();
                ruinEmpty.gameObject.SetActive(false);
                fixFilled.gameObject.SetActive(true);
            }

            optionHandler.CloseIcons();
        }

        public void TechBuilding()
        {
            if (BalconyManager.GetInstance != null)
            {
                BalconyManager.GetInstance.OpenTechTab(resTechType);
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
