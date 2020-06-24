using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;


/// <summary>
/// Handles all Updates to the current panel shown.
/// Handles switching of panels.
/// </summary>
public class InformationActionHandler : MonoBehaviour
{
    public List<OperationCardAction> actionList;
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
    public void ClosePanelList()
    {
        currentBuildingPanel.gameObject.SetActive(false);
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
