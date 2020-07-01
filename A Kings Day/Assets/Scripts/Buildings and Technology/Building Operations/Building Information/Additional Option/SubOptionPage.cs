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
        public List<SubOptionPanel> optionPanelList;

        public ResourceType currentResourceReturnType;
        public void OpenSellTroopsPanel()
        {
            currentResourceReturnType = ResourceType.Coin;

            /// CHANGE THE PRICE BY HAVING A PRICE STORAGE
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;
            // Recruit
            optionPanelList[0].SetupOptionPanel(playerData.recruits, 12, "Recruits");
            // Swordsman
            optionPanelList[1].SetupOptionPanel(playerData.swordsmenCount, 15,"Swordsman");
            // Spearman
            optionPanelList[2].SetupOptionPanel(playerData.spearmenCount, 15,"Spearman");
            // Archer
            optionPanelList[3].SetupOptionPanel(playerData.archerCount,15, "Archer");

        }

        public void ShowPotentialChanges()
        {
            if (ResourceInformationController.GetInstance == null) return;

            int tmp = 0;
            for (int i = 0; i < optionPanelList.Count; i++)
            {
                tmp += optionPanelList[i].amountToSell * optionPanelList[i].pricePerItem;
            }

            List<ResourceReward> rewardTmp = new List<ResourceReward>();
            ResourceReward reward = new ResourceReward();

            reward.resourceType = currentResourceReturnType;
            reward.rewardAmount = tmp;

            rewardTmp.Add(reward);


            ResourceReward troop = new ResourceReward();
            int recruitsSold = optionPanelList[0].amountToSell;
            troop.resourceType = ResourceType.Troops;
            troop.rewardAmount = -recruitsSold;
            rewardTmp.Add(troop);

            ResourceReward swordsman = new ResourceReward();
            int swrdManSold = optionPanelList[1].amountToSell;
            swordsman.resourceType = ResourceType.Swordsmen;
            swordsman.rewardAmount = -swrdManSold;
            rewardTmp.Add(swordsman);

            ResourceReward spearman = new ResourceReward();
            int sprmanSold = optionPanelList[2].amountToSell;
            spearman.resourceType = ResourceType.Spearmen;
            spearman.rewardAmount = -sprmanSold;
            rewardTmp.Add(spearman);

            ResourceReward archer = new ResourceReward();
            int archerSold = optionPanelList[3].amountToSell;
            archer.resourceType = ResourceType.Archer;
            archer.rewardAmount = -archerSold;
            rewardTmp.Add(archer);



            ResourceInformationController.GetInstance.ShowCurrentPanelPotentialResourceChanges(rewardTmp);
        }
        public void SellTroops()
        {

            int tmp = 0;
            for (int i = 0; i < optionPanelList.Count; i++)
            {
                tmp += optionPanelList[i].amountToSell * optionPanelList[i].pricePerItem;
            }

            int recruitsSold = optionPanelList[0].amountToSell;
            int swrdManSold = optionPanelList[1].amountToSell;
            int sprmanSold = optionPanelList[2].amountToSell;
            int archerSold = optionPanelList[3].amountToSell;

            PlayerGameManager.GetInstance.ReceiveResource(tmp, currentResourceReturnType);

            PlayerGameManager.GetInstance.RemoveResource(recruitsSold, ResourceType.Troops);
            PlayerGameManager.GetInstance.RemoveResource(swrdManSold, ResourceType.Swordsmen);
            PlayerGameManager.GetInstance.RemoveResource(sprmanSold, ResourceType.Spearmen);
            PlayerGameManager.GetInstance.RemoveResource(archerSold, ResourceType.Archer);

            ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(currentResourceReturnType, tmp);
            ResourceInformationController.GetInstance.HideCurrentPanelPotentialResourceChanges();

            ClosePanel();
        }

        public void ClosePanel()
        {
            myController.CloseSubOption();
            this.gameObject.SetActive(false);
        }
    }
}