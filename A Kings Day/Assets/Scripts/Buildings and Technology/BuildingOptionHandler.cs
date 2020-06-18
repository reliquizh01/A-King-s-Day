using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Technology;
using Utilities;
using Managers;

namespace Buildings
{

/// <summary>
/// Upgrade Handler handles the Upgrade list of all buildings.
/// </summary>
    public class BuildingOptionHandler : MonoBehaviour
    {
        public BaseBuildingBehavior myBuilding;
        public List<BuildingOptionIcon> buildingOptions;
        public BasePanelBehavior iconPanels;
        public CloseOption closeIcon;
        public bool isOpen = false;

        public void Start()
        {
            for (int i = 0; i < buildingOptions.Count; i++)
            {
                buildingOptions[i].myController = this;
            }

            //UpdateRuinState();
        }


        public void UpdateRuinState()
        {
            for (int i = 0; i < buildingOptions.Count; i++)
            {
                if (myBuilding.currentCondition == BuildingCondition.Ruins)
                {
                    if (buildingOptions[i].type != BuildingOptionType.Upgrade)
                    {
                        buildingOptions[i].allowInteraction = false;
                    }
                }
                else
                {
                    buildingOptions[i].allowInteraction = true;
                }
            }
        }

        public void OpenIcons()
        {
            closeIcon.isClickable = false;
            StartCoroutine(iconPanels.WaitAnimationForAction(iconPanels.openAnimationName, () => SwitchClickableMode(true)));
            isOpen = true;
        }
        public void FilterIcons()
        {
            if(myBuilding.resTechType == Kingdoms.ResourceType.Shop || myBuilding.resTechType == Kingdoms.ResourceType.Tavern)
            {
                buildingOptions.Find(x => x.type == BuildingOptionType.Tech).SetInteraction(false);
            }
            if(myBuilding.resTechType == Kingdoms.ResourceType.Food)
            {
                buildingOptions.Find(x => x.type == BuildingOptionType.Upgrade).SetInteraction(false);
            }
        }
        public void CloseIcons()
        {
            myBuilding.isClickable = false;
            myBuilding.isInteractingWith = false;
            isOpen = false;
            StartCoroutine(iconPanels.WaitAnimationForAction(iconPanels.closeAnimationName, ()=> SwitchClickableMode(true)));
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_IN_GAME_INTERACTION);
        }

        public void SwitchClickableMode(bool newMode)
        {
            closeIcon.isClickable = newMode;
            FilterIcons();
        }

        public void ShowRepairAmountTooltip()
        {
            string mesg = "Repair Cost " + myBuilding.repairPrice.ToString() + ".";
            Parameters p = new Parameters();
            p.AddParameter<string>("Mesg", mesg);

            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
        }
        public void ShowToolTip(string mesg)
        {
            Parameters p = new Parameters();
            p.AddParameter<string>("Mesg", mesg);

            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
        }
        public void SetupUpgrades()
        {
            switch(myBuilding.currentCondition)
            {
                case BuildingCondition.Ruins:

                    break;
                case BuildingCondition.Functioning:

                    break;
            }
        }

        public void UpgradeClicked()
        {
            if (PlayerGameManager.GetInstance == null)
            {
                return;
            }
            // Show Upgrade Tab
            myBuilding.UpgradeBuiilding();
        }

        public void TechClicked()
        {
            // Show Tech Tab
            if (BalconyManager.GetInstance == null)
            {
                return;
            }

            myBuilding.TechBuilding();
            CloseIcons();
        }

        public void UseClicked()
        {
            // Show Building Tab
            myBuilding.UseBuilding();
            CloseIcons();
        }

        public void ShowOptionInsufficient(OptionType type)
        {
            switch (type)
            {
                case OptionType.Upgrade:
                    buildingOptions.Find(x => x.type == BuildingOptionType.Upgrade).ShowInsufficientMesg();
                    break;
                case OptionType.Technology:
                    buildingOptions.Find(x => x.type == BuildingOptionType.Tech).ShowInsufficientMesg("No Tech Available.");
                    break;
                case OptionType.Use:
                    buildingOptions.Find(x => x.type == BuildingOptionType.Use).ShowInsufficientMesg("Under Maintenance");
                    break;
                default:
                    break;
            }
        }
    }

}