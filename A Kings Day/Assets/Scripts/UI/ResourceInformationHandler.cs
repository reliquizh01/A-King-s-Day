using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using UnityEngine.EventSystems;
using Kingdoms;
using Managers;
using KingEvents;

namespace ResourceUI
{
    public class ResourceInformationHandler : MonoBehaviour
    {
        public ResourcePanelType resourcePanelType;
        public BasePanelBehavior myPanel;
        public bool isShowing = false;
        [Header("Kingdom Information")]
        public BaseResourceUIControllerV2 foodControl;
        public BaseResourceUIControllerV2 troopControl;
        public BaseResourceUIControllerV2 villagerControl;
        public BaseResourceUIControllerV2 coinControl;

        [Header("Middle Information")]
        public WeekCountController weekController;
        public GameObject travelBtn;

        public void InitializeData()
        {
            PlayerKingdomData data = PlayerGameManager.GetInstance.playerData;


            foodControl.SetupStorageCapacity(data.safeFood);
            troopControl.SetupStorageCapacity(data.barracksCapacity);
            villagerControl.SetupStorageCapacity(data.safePopulation);
            coinControl.SetupStorageCapacity(data.coinsCapacity);
            foodControl.SetResource(data.foods);
            troopControl.SetResource(data.GetTotalTroops);
            villagerControl.SetResource(data.population);
            coinControl.SetResource(data.coins);
        }

        public void ShowWeekendPanel()
        {
            if(travelBtn.activeInHierarchy)
            {
                travelBtn.gameObject.SetActive(false);
            }
            weekController.gameObject.SetActive(true);
        }
        public void ShowTravelPanel()
        {
            if (weekController.gameObject.activeInHierarchy)
            {
                weekController.gameObject.SetActive(false);
            }
            travelBtn.SetActive(true);
        }

        public void OpenTravelMap()
        {
            EventBroadcaster.Instance.PostEvent(EventNames.OPEN_MAP_TAB);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
        }
        public void ShowPotentialResourceChanges(List<ResourceReward> rewardList)
        {
            HidePotentialResourceChanges();
            for (int i = 0; i < rewardList.Count; i++)
            {
                if (rewardList[i].rewardAmount > 0)
                {
                    switch (rewardList[i].resourceType)
                    {
                        case ResourceType.Food:
                            foodControl.ShowIncrease(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Coin:
                            coinControl.ShowIncrease(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Troops:
                            troopControl.ShowIncrease(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Population:
                            villagerControl.ShowIncrease(rewardList[i].rewardAmount);
                            break;
                    }
                }
                else if(rewardList[i].rewardAmount < 0)
                {
                    switch (rewardList[i].resourceType)
                    {
                        case ResourceType.Food:
                            foodControl.ShowReduction(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Coin:
                            coinControl.ShowReduction(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Troops:
                            troopControl.ShowReduction(rewardList[i].rewardAmount);
                            break;
                        case ResourceType.Population:
                            villagerControl.ShowReduction(rewardList[i].rewardAmount);
                            break;
                    }
                }
                else
                {
                    switch (rewardList[i].resourceType)
                    {
                        case ResourceType.Food:
                            foodControl.HidePotentials();
                            break;
                        case ResourceType.Coin:
                            coinControl.HidePotentials();
                            break;
                        case ResourceType.Troops:
                            troopControl.HidePotentials();
                            break;
                        case ResourceType.Population:
                            villagerControl.HidePotentials();
                            break;
                    }
                }
            }
        }
        public void HidePotentialResourceChanges()
        {
            foodControl.HidePotentials();
            coinControl.HidePotentials();
            troopControl.HidePotentials();
            villagerControl.HidePotentials();
        }
        public void UpdateResourceData(ResourceType type, int amount, bool isIncrease = true)
        {
            amount = Mathf.Abs(amount);
            switch (type)
            {
                case ResourceType.Food:
                    UpdateFood(isIncrease, amount);
                    break;
                case ResourceType.Coin:
                    UpdateCoins(isIncrease, amount);
                    break;
                case ResourceType.Population:
                    UpdatePopulation(isIncrease, amount);
                    break;
                case ResourceType.Troops:
                    UpdateTroops(isIncrease, amount);
                    break;
            }

            UpdateReliability();
        }
        private void UpdatePopulation(bool isIncrease = true, int amount = 0)
        {
            if (isIncrease)
            {
                villagerControl.IncreaseResource(amount);
            }
            else
            {
                villagerControl.DecreaseResource(amount);
            }
        }
        private void UpdateCoins(bool isIncrease = true, int amount = 0)
        {
            if (isIncrease)
            {
                coinControl.IncreaseResource(amount);
            }
            else
            {
                coinControl.DecreaseResource(amount);
            }
        }
        private void UpdateTroops(bool isIncrease = true, int amount = 0)
        {
            if (isIncrease)
            {
                troopControl.IncreaseResource(amount);
            }
            else
            {
                troopControl.DecreaseResource(amount);
            }
        }
        private void UpdateFood(bool isIncrease = true, int amount = 0)
        {
            if (isIncrease)
            {
                foodControl.IncreaseResource(amount);
            }
            else
            {
                foodControl.DecreaseResource(amount);
            }
        }

        private void UpdateReliability()
        {

        }
    }
}