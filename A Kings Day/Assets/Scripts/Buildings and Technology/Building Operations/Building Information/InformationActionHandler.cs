using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;
using ResourceUI;
using KingEvents;
using Managers;

/// <summary>
/// Handles all Updates to the current panel shown.
/// Handles switching of panels.
/// </summary>
public class InformationActionHandler : MonoBehaviour
{
    public BaseOperationBehavior myController;
    public List<OperationCardDecision> operationDecisionList;
    public List<BuildingInformationPanel> buildingPanels;
    public BuildingInformationPanel currentBuildingPanel;


    // Index of the Card clicked from the bottom
    public int selectedCardIdx;
    public void OpenPanel(BuildingType buildingType)
    {
        if(currentBuildingPanel != null)
        {
            currentBuildingPanel.transform.parent.gameObject.SetActive(false);
            currentBuildingPanel.gameObject.SetActive(false);
        }

        currentBuildingPanel = buildingPanels.Find(x => x.buildingType == buildingType);

        if(currentBuildingPanel != null)
        {
            currentBuildingPanel.gameObject.SetActive(true);
            currentBuildingPanel.transform.parent.gameObject.SetActive(true);
            currentBuildingPanel.UpdatePages(selectedCardIdx);
        }
    }

    public GameObject infoBlocker;
    public TextMeshProUGUI blockMesgText;

    public void UpdateCurrentPanel()
    {
        currentBuildingPanel.UpdatePages(selectedCardIdx);
    }
    public void SetupActionList(List<CardActiondata> actionDataList)
    {
        for (int i = 0; i < actionDataList.Count; i++)
        {
            switch (actionDataList[i].actionType)
            {
                case CardActionType.MessageOnly:
                    operationDecisionList[i].iconMesgGroup.SetActive(false);
                    operationDecisionList[i].coinCostPanel.SetActive(false);
                    operationDecisionList[i].iconOnly.gameObject.SetActive(false);
                    operationDecisionList[i].messageOnly.gameObject.SetActive(true);


                    operationDecisionList[i].messageOnly.text = actionDataList[i].message;
                    break;


                case CardActionType.LogoWithMessage:
                    operationDecisionList[i].iconMesgGroup.SetActive(true);
                    operationDecisionList[i].coinCostPanel.SetActive(false);
                    operationDecisionList[i].iconOnly.gameObject.SetActive(false);
                    operationDecisionList[i].messageOnly.gameObject.SetActive(false);

                    operationDecisionList[i].iconMessage.text = actionDataList[i].message;
                    operationDecisionList[i].icon.sprite = actionDataList[i].logoIcon;
                    break;
                case CardActionType.CostMessageOnly:
                    operationDecisionList[i].iconMesgGroup.SetActive(false);
                    operationDecisionList[i].coinCostPanel.SetActive(true);
                    operationDecisionList[i].iconOnly.gameObject.SetActive(false);
                    operationDecisionList[i].messageOnly.gameObject.SetActive(false);


                    break;
                case CardActionType.LogoOnly:
                    operationDecisionList[i].iconMesgGroup.SetActive(false);
                    operationDecisionList[i].coinCostPanel.SetActive(false);
                    operationDecisionList[i].iconOnly.gameObject.SetActive(true);
                    operationDecisionList[i].messageOnly.gameObject.SetActive(false);

                    operationDecisionList[i].iconOnly.sprite = actionDataList[i].logoIcon;
                    break;
                default:
                    break;
            }
            operationDecisionList[i].actionIdx = i;
        }
    }

    public void ShowOperationDecisionChanges(int idx)
    {
        List<ResourceReward> tmp = myController.currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[idx].rewardList;
        ResourceInformationController.GetInstance.currentPanel.ShowPotentialResourceChanges(tmp);
    }
    public void ImplementDecisionChanges(int idx)
    {
        if(PlayerGameManager.GetInstance != null)
        {
            // CHECK IF PLAYER RESOURCES IS ENOUGH

            List<ResourceReward> tmp = myController.currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[idx].rewardList;
            bool isResourcesEnough = true;

            for (int i = 0; i < tmp.Count; i++)
            {
                if(tmp[i].rewardAmount < 0)
                {
                    isResourcesEnough = PlayerGameManager.GetInstance.CheckResourceEnough(tmp[i].rewardAmount, tmp[i].resourceType);
                }
                if(!isResourcesEnough)
                {
                    break;
                }
            }

            if(isResourcesEnough)
            {
                for (int i = 0; i < tmp.Count; i++)
                {
                    PlayerGameManager.GetInstance.ReceiveResource(tmp[i].rewardAmount, tmp[i].resourceType);

                    if(tmp[i].rewardAmount < 0)
                    {
                        ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(tmp[i].resourceType, tmp[i].rewardAmount, false);
                    }
                    else
                    {
                        ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(tmp[i].resourceType, tmp[i].rewardAmount);
                    }
                }
                myController.CardDecisionFlavorText(idx, true);
                HideOperationDecisionChanges();
                ResourceInformationController.GetInstance.currentPanel.ShowPotentialResourceChanges(tmp);
            }
            else
            {
                myController.CardDecisionFlavorText(idx, false);
            }
        }
    }
    public void HideOperationDecisionChanges()
    {
        ResourceInformationController.GetInstance.currentPanel.HidePotentialResourceChanges();
    }
    public void ClosePanelList()
    {
        currentBuildingPanel.gameObject.SetActive(false);
    }
    public void ResetActionList()
    {
        for (int i = 0; i < operationDecisionList.Count; i++)
        {
            operationDecisionList[i].iconMesgGroup.SetActive(false);
            operationDecisionList[i].coinCostPanel.SetActive(false);
            operationDecisionList[i].iconOnly.gameObject.SetActive(false);
            operationDecisionList[i].messageOnly.gameObject.SetActive(false);
        }
    }

    public void ShowInfoBlocker(string newMesg)
    {
        infoBlocker.gameObject.SetActive(true);
        blockMesgText.text = newMesg;
    }
    public void HideInfoBlocker()
    {
        infoBlocker.gameObject.SetActive(false);
    }
}
