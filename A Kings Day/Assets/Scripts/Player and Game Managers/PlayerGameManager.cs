using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using Kingdoms;
using Technology;
using GameResource;
using ResourceUI;

namespace Managers
{
    public class PlayerGameManager : MonoBehaviour
    {
        #region Singleton
        private static PlayerGameManager instance;
        public static PlayerGameManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (PlayerGameManager.GetInstance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        public FoodResourceBehavior foodBehavior;
        public TroopResourceBehavior troopBehavior;
        public PopulationResourceBehavior populationBehavior;
        public CoinResourceBehavior coinBehavior;

        public PlayerKingdomData playerData = new PlayerKingdomData();
        public void Start()
        {

        }
        public void UpdatePlayerbasedVisuals()
        {
            if (GameUIManager.GetInstance == null)
            {
                return;
            }
            if (playerData.farmerCount <= 0)
            {
                ResourceInformationController.GetInstance.currentPanel.foodControl.EnableWarning("No Farmers!");
            }
            else if(playerData.foods <= 5 && playerData.foods >= 0)
            {
                ResourceInformationController.GetInstance.currentPanel.foodControl.EnableWarning("Low Food!");
            }
        }

        public void SetupResourceProductionUpdate()
        {
            foodBehavior.SetupResourceBehavior();
            troopBehavior.SetupResourceBehavior();
            populationBehavior.SetupResourceBehavior();
            coinBehavior.SetupResourceBehavior();
        }
        public void WeeklyResourceProductionUpdate()
        {
            foodBehavior.UpdateWeeklyProgress();
            troopBehavior.UpdateWeeklyProgress();   
            populationBehavior.UpdateWeeklyProgress();
            coinBehavior.UpdateWeeklyProgress();
        }
        public void ReceiveData(PlayerKingdomData newData)
        {
            playerData.kingdomsName = newData.kingdomsName;
            playerData.dynastyName = newData.dynastyName;
            playerData.coins = newData.coins;
            playerData.coinsCapacity = newData.coinsCapacity;
            playerData.foods = newData.foods;
            playerData.foodCapacity = newData.foodCapacity;
            playerData.population = newData.population;
            playerData.populationCapacity = newData.populationCapacity;
            playerData.pouplationLoyalty = newData.pouplationLoyalty;
            playerData.troops = newData.troops;
            playerData.troopsCapacity = newData.troopsCapacity;
            playerData.troopsLoyalty = newData.troopsLoyalty;
            playerData.weekCount = newData.weekCount;
            playerData.finishedStories = newData.finishedStories;

            SetupResourceProductionUpdate();
        }

        public void ReceiveResource(int amount, ResourceType type)
        {
            switch(type)
            {
                case ResourceType.Food:
                    playerData.foods += amount;
                    break;
                case ResourceType.Coin:
                    playerData.coins += amount;
                    break;
                case ResourceType.Population:
                    playerData.SetPopulation(amount);
                    break;
                case ResourceType.Troops:
                    playerData.troops += amount;
                    break;
            }
        }
        public void RemoveResource(int amount, ResourceType type)
        {
            amount = Mathf.Abs(amount);
            switch (type)
            {
                case ResourceType.Food:
                    playerData.foods -= amount;
                    break;
                case ResourceType.Coin:
                    playerData.coins -= amount;
                    break;
                case ResourceType.Population:
                    playerData.SetPopulation(-amount);
                    break;
                case ResourceType.Troops:
                    playerData.troops -= amount;
                    break;
            }
        }

        public bool CheckResourceEnough(int amount, ResourceType type)
        {
            amount = Mathf.Abs(amount);
            
            switch (type)
            {
                case ResourceType.Food:
                    return (playerData.foods >= amount);
                case ResourceType.Troops:
                    return (playerData.troops >= amount);
                case ResourceType.Population:
                    return (playerData.population >= amount);
                case ResourceType.Coin:
                    return (playerData.coins >= amount);
                case ResourceType.Cows:
                    return (playerData.cows >= amount);
                default:
                    return false;
            }
        }
    }
}