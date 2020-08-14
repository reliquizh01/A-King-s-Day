using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using KingEvents;
using Managers;
using ResourceUI;

namespace Buildings
{
    public class SubOptionPage : MonoBehaviour
    {
        public SubOptionHandler myController;

        [Header("Option Panel Prefab")]
        public List<SubOptionPanel> optionPanelList;
        public Transform optionParent;
        public GameObject optionPanelPrefab;

        public ResourceType currentResourceReturnType;
        public void OpenSellTroopsPanel()
        {
            ClearCurrentOptionPanelList();

            currentResourceReturnType = ResourceType.Coin;

            /// CHANGE THE PRICE BY HAVING A PRICE STORAGE
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;
            if(playerData.troopsList != null)
            {
                for (int i = 0; i < playerData.troopsList.Count; i++)
                {
                    GameObject tmp = (GameObject)Instantiate(optionPanelPrefab, optionParent);

                    optionPanelList.Add(tmp.GetComponent<SubOptionPanel>());
                    optionPanelList[optionPanelList.Count-1].myController = this;
                    optionPanelList[optionPanelList.Count - 1].SetupOptionPanel(playerData.troopsList[i].totalUnitCount, playerData.troopsList[i].unitInformation.unitPrice, playerData.troopsList[i].unitInformation.unitName);
                }                
            }
        }

        public void ClearCurrentOptionPanelList()
        {
            if(optionPanelList == null || optionPanelList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < optionPanelList.Count; i++)
            {
                DestroyImmediate(optionPanelList[i].gameObject);
            }
            optionPanelList.Clear();
        }
        public void ShowPotentialChanges()
        {
            if (ResourceInformationController.GetInstance == null) return;

            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;

            int tmp = 0;

            List<ResourceReward> rewardTmp = new List<ResourceReward>();
            ResourceReward reward = new ResourceReward();

            for (int i = 0; i < optionPanelList.Count; i++)
            {
                tmp += optionPanelList[i].amountToSell * optionPanelList[i].pricePerItem;

                ResourceReward tempReward = new ResourceReward();
                int sold = optionPanelList[i].amountToSell;
                tempReward.resourceType = ResourceType.Troops;
                tempReward.unitName = optionPanelList[i].nameTitleText.text;
                tempReward.rewardAmount = -sold;
                rewardTmp.Add(tempReward);
            }

            reward.resourceType = currentResourceReturnType;
            reward.rewardAmount = tmp;

            rewardTmp.Add(reward);

            ResourceInformationController.GetInstance.ShowCurrentPanelPotentialResourceChanges(rewardTmp);
        }
        public void SellTroops()
        {
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;
            int tmp = 0;
            for (int i = 0; i < optionPanelList.Count; i++)
            {
                tmp += optionPanelList[i].amountToSell * optionPanelList[i].pricePerItem;
                int soldTroopCount = optionPanelList[i].amountToSell;
                PlayerGameManager.GetInstance.RemoveResource(soldTroopCount, ResourceType.Troops, playerData.troopsList[i].unitInformation.unitName);
                ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(ResourceType.Troops, soldTroopCount, false);
            }
            PlayerGameManager.GetInstance.ReceiveResource(tmp, currentResourceReturnType);

            myController.myController.currentBuildingPanel.UpdateBarracks();
            ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(currentResourceReturnType, tmp);
            ResourceInformationController.GetInstance.HideCurrentPanelPotentialResourceChanges();

            ClearCurrentOptionPanelList();
            ClosePanel();

        }

        public void ClosePanel()
        {
            myController.CloseSubOption();
            this.gameObject.SetActive(false);
        }
    }
}