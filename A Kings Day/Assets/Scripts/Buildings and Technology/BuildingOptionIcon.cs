using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Buildings
{
    public enum BuildingOptionType
    {
        Upgrade,
        Tech,
        Use,
    }
    public class BuildingOptionIcon : BaseOptionIcon
    {
        
        public bool allowInteraction = true;
        public BuildingOptionHandler myController;
        public BuildingOptionType type;
        public Animation iconAnim;
        public string mesg;
        public override void OnMouseEnter()
        {
            if (!allowInteraction) return;

            base.OnMouseEnter();
            iconAnim.Play();

            if(myController != null && type == BuildingOptionType.Upgrade && myController.myBuilding.currentCondition == BuildingCondition.Ruins)
            {
                myController.ShowRepairAmountTooltip();
            }   
            else
            {
                if(myController.myBuilding.currentCondition == BuildingCondition.Functioning)
                {
                    myController.ShowToolTip("Upgrade");
                }
                if(!string.IsNullOrEmpty(mesg))
                {
                    myController.ShowToolTip(mesg);
                }
            }
        }

        public override void OnMouseUp()
        {
            if (!allowInteraction) return;
            base.OnMouseUp();
        }
        public override void OnMouseDown()
        {
            if (!allowInteraction) return;
            base.OnMouseDown();

            switch (type)
            {
                case BuildingOptionType.Upgrade:
                    myController.UpgradeClicked();
                    break;
                case BuildingOptionType.Tech:
                    myController.TechClicked();
                    break;
                case BuildingOptionType.Use:
                    myController.UseClicked();
                    break;
                default:
                    break;
            }
        }
        public override void OnMouseExit()
        {
            if (!allowInteraction) return;
            base.OnMouseExit();
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }

        public void SetInteraction(bool enable)
        {
            allowInteraction = enable;
            if(allowInteraction)
            {
                iconBackground.color = new Color(1,1,1,1f);
                optionIcon.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                iconBackground.color = new Color(1, 1, 1, 0.5f);
                optionIcon.color = new Color(1, 1, 1, 0.5f);
            }
        }

        public void ShowInsufficientMesg(string mesg = "")
        {
            if(string.IsNullOrEmpty(mesg))
            {
                myController.ShowToolTip("Not Enough Funds.");
            }
            else
            {
                myController.ShowToolTip(mesg);
            }
            iconBackground.color = Color.red;
        }
    }

}