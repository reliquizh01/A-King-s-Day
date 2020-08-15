using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;
using ResourceUI;
using KingEvents;
using Managers;
using Characters;
/// <summary>
/// Handles all Updates to the current panel shown.
/// Handles switching of panels.
/// </summary>
public class InformationActionHandler : MonoBehaviour
{
    public BaseOperationBehavior myController;
    public KingdomUnitStorage unitStorage;

    [Header("Building Panels Mechanics")]
    public List<BuildingInformationPanel> buildingPanels;
    public BuildingInformationPanel currentBuildingPanel;
    [Header("Sub Option Mechanics")]
    public SubOptionHandler subOptionHandler;
    public AdditionalInformationHandler addedInformationHandler;
    [Header("Operation Decision Mechanics")]
    public List<OperationCardDecision> operationDecisionList;
    public OperationCardDecision hoveredOperationDecision;

    [Header("Clicked Option Card")]
    // Index of the Card clicked from the bottom
    public int selectedCardIdx;
    public void OpenPanel(BuildingType buildingType)
    {

        if (currentBuildingPanel != null)
        {
            currentBuildingPanel.transform.parent.gameObject.SetActive(false);
            currentBuildingPanel.gameObject.SetActive(false);
        }

        BaseSceneManager tmp = TransitionManager.GetInstance.currentSceneManager;
        BuildingInformationData buildingInfo = tmp.buildingInformationStorage.ObtainBuildingOperation(buildingType);

        currentBuildingPanel = buildingPanels.Find(x => x.buildingType == buildingType);

        if(currentBuildingPanel != null)
        {
            currentBuildingPanel.gameObject.SetActive(true);
            currentBuildingPanel.transform.parent.gameObject.SetActive(true);

            currentBuildingPanel.InitializeBuildingInformation(buildingInfo);
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

            operationDecisionList[i].openSubOption = actionDataList[i].openSubOption;
            operationDecisionList[i].actionIdx = i;
        }
    }

    public void ShowOperationDecisionChanges(int idx)
    {
        List<ResourceReward> tmp = myController.currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[idx].rewardList;
        ResourceInformationController.GetInstance.currentPanel.ShowPotentialResourceChanges(tmp);
        hoveredOperationDecision = operationDecisionList[idx];
        ShowAddedInformation(idx);
    }
    public void ShowAddedInformation(int idx)
    {
        addedInformationHandler.gameObject.SetActive(true);
        addedInformationHandler.ShowOnAddedInfoAction(idx, myController.currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[idx]);

    }
    public void ImplementDecisionChanges(int idx)
    {
        if(operationDecisionList[idx].openSubOption)
        {
            SwitchSubPanel(true);
            subOptionHandler.OpenSubOption(myController.currentBuildingClicked.buildingType, selectedCardIdx, idx);
        }
        else
        {
            if(subOptionHandler.gameObject.activeSelf)
            {
                SwitchSubPanel(false);
            }

            if (PlayerGameManager.GetInstance != null)
            {
                List<ResourceReward> tmp = myController.currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[idx].rewardList;
                bool isResourcesEnough = true;

                // CHECK IF PLAYER RESOURCES IS ENOUGH
                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i].rewardAmount < 0)
                    {
                        if(tmp[i].resourceType == Kingdoms.ResourceType.Troops || tmp[i].resourceType == Kingdoms.ResourceType.Mercenary)
                        {
                            isResourcesEnough = PlayerGameManager.GetInstance.CheckResourceEnough(tmp[i].rewardAmount, tmp[i].resourceType, tmp[i].unitName);
                        }
                        else
                        {
                            isResourcesEnough = PlayerGameManager.GetInstance.CheckResourceEnough(tmp[i].rewardAmount, tmp[i].resourceType);
                        }
                    }
                    if (!isResourcesEnough)
                    {
                        break;
                    }
                }

                if (isResourcesEnough && !myController.informationActionHandler.currentBuildingPanel.currentPage.hasUniqueRequirements)
                {
                    ImplementResourceChanges(tmp, idx, 1);
                }
                else if(isResourcesEnough && myController.informationActionHandler.currentBuildingPanel.currentPage.hasUniqueRequirements)
                {
                    myController.informationActionHandler.currentBuildingPanel.currentPage.ImplementPageAction(idx);
                    ImplementResourceChanges(tmp, idx, myController.informationActionHandler.currentBuildingPanel.currentPage.ObtainRewardMultiplier());
                }
                else
                {
                    myController.CardDecisionFlavorText(idx, false);
                }
            }

        }
    }

    public void ImplementResourceChanges(List<ResourceReward> tmp, int idx, int multiplier)
    {
        for (int i = 0; i < tmp.Count; i++)
        {
            PlayerGameManager.GetInstance.ReceiveResource(tmp[i].rewardAmount, tmp[i].resourceType, tmp[i].unitName);

            if (tmp[i].rewardAmount < 0)
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
    public void HideAddedInformation()
    {
        addedInformationHandler.gameObject.SetActive(false);
        addedInformationHandler.HideOnAddedInfoAction();
    }
    public void HideOperationDecisionChanges()
    {
        HideAddedInformation();

        ResourceInformationController.GetInstance.currentPanel.HidePotentialResourceChanges();
    }
    public void ClosePanelList()
    {
        currentBuildingPanel.gameObject.SetActive(false);
    }
    public void SwitchSubPanel(bool switchTo)
    {
        subOptionHandler.gameObject.SetActive(switchTo);
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
