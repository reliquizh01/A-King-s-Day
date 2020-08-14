using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Technology;
using Utilities;
using Managers;
using ResourceUI;
using KingEvents;

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
                if(myBuilding.buildingInformation == null)
                {
                    continue;
                }

                if (myBuilding.buildingInformation.buildingCondition == BuildingCondition.Ruins)
                {
                    if (buildingOptions[i].type != BuildingOptionType.Upgrade)
                    {
                        buildingOptions[i].SetInteraction(false);
                    }
                }
                else
                {
                    if (buildingOptions[i].type == BuildingOptionType.Upgrade)
                    {
                        buildingOptions[i].SetInteraction(false);
                    }
                    else
                    {
                        buildingOptions[i].SetInteraction(true);
                    }
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
            if(myBuilding.buildingType == BuildingType.Smithery|| myBuilding.buildingType == BuildingType.Tavern)
            {
                buildingOptions.Find(x => x.type == BuildingOptionType.Tech).SetInteraction(false);
            }
            if(myBuilding.buildingType == BuildingType.Farm)
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
            string mesg = "Repair Cost " + myBuilding.buildingInformation.repairPrice.ToString() + ".";
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
            switch(myBuilding.buildingInformation.buildingCondition)
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
            myBuilding.UpgradeBuilding();
        }

        public void UpgradeHover()
        {

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