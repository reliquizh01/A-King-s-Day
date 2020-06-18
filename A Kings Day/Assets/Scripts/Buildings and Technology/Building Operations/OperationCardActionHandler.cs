using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using KingEvents;
using Utilities;

namespace Buildings
{
    public class OperationCardActionHandler : MonoBehaviour
    {
        public List<OperationCardAction> actionList;

        public void SetupActionList(List<CardActiondata> actionDataList)
        {
            for (int i = 0; i < actionDataList.Count; i++)
            {
                switch (actionDataList[i].actionType)
                {
                    case CardActionType.MessageOnly:
                        actionList[i].iconMesgGroup.SetActive(false);
                        actionList[i].coinCostPanel.SetActive(false);
                        actionList[i].iconOnly.gameObject.SetActive(false);
                        actionList[i].messageOnly.gameObject.SetActive(true);


                        actionList[i].messageOnly.text = actionDataList[i].message;
                        break;


                    case CardActionType.LogoWithMessage:
                        actionList[i].iconMesgGroup.SetActive(true);
                        actionList[i].coinCostPanel.SetActive(false);
                        actionList[i].iconOnly.gameObject.SetActive(false);
                        actionList[i].messageOnly.gameObject.SetActive(false);

                        actionList[i].iconMessage.text = actionDataList[i].message;
                        actionList[i].icon.sprite = actionDataList[i].logoIcon;
                        break;
                    case CardActionType.CostMessageOnly:
                        actionList[i].iconMesgGroup.SetActive(false);
                        actionList[i].coinCostPanel.SetActive(true);
                        actionList[i].iconOnly.gameObject.SetActive(false);
                        actionList[i].messageOnly.gameObject.SetActive(false);


                        break;
                    case CardActionType.LogoOnly:
                        actionList[i].iconMesgGroup.SetActive(false);
                        actionList[i].coinCostPanel.SetActive(false);
                        actionList[i].iconOnly.gameObject.SetActive(true);
                        actionList[i].messageOnly.gameObject.SetActive(false);

                        actionList[i].iconOnly.sprite = actionDataList[i].logoIcon;
                        break;
                    default:
                        break;
                }
            }
        }

        public void ResetActionList()
        {
            for (int i = 0; i < actionList.Count; i++)
            {
                actionList[i].iconMesgGroup.SetActive(false);
                actionList[i].coinCostPanel.SetActive(false);
                actionList[i].iconOnly.gameObject.SetActive(false);
                actionList[i].messageOnly.gameObject.SetActive(false);
            }
        }
    }
}
