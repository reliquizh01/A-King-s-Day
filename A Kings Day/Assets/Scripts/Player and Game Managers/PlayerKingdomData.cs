using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Territory;
using Technology;
using Characters;

namespace Kingdoms
{
    public enum ResourceType
    {
        Food,
        Troops,
        Population,
        Coin,
        Cows,
        Shop,
        Tavern,
        Blacksmith,
        Market,
        Cow,
        Archer,
        Swordsmen,
        Spearmen,
        farmer,
        herdsmen,
        storageKeeper,
    }
    /// <summary>
    /// Player Kingdom represents all information the player has
    /// </summary>
    [Serializable]
    public class PlayerKingdomData
    {
        public string kingdomsName;
        public string dynastyName;
        public TerritoryLevel level;
        public int weekCount = 1;
        public float storyRoll = 0.007f;
        public float currentRoll = 0.007f;
        public List<string> finishedStories;
        [Header("Troops")]
        // Troops
        public int troops;
        public int troopsCapacity;
        public int troopsLoyalty;
        public int swordsmenCount, spearmenCount, archerCount;
        public int swordsmenMercCount, spearmenMercCount, archerMercCount;

        public int GetTroopsCount
        {
            get { return troops + swordsmenCount + spearmenCount + archerCount; }
        }
        [Header("Population")]
        // Population
        public int population;
        public int populationCapacity;
        public int pouplationLoyalty;
        public int curTaxWeeksCounter;
        public bool canReceiveTax;
        public int farmerCount, herdsmanCount, storageKeeperCount;

        [Header("Game Chances")]
        [Header("Heroes")]
        public List<BaseHeroInformationData> myHeroes;

        [Header("Merchants")]
        public List<BaseMerchantInformationData> currentShopMerchants;
        
        public int GetPopulationCount
        {
            get { return farmerCount + herdsmanCount + storageKeeperCount + population; }
        }
        public void SetPopulation(int amount)
        {
            if(amount < 0)
            {
                amount = Mathf.Abs(amount);
                population -= amount;
            }
            else
            {
                population += amount;
            }
        }
        [Header("Food")]
        // Food
        public int foods;
        public int foodCapacity;
        public int cows;
        public int cowCapacity;

        public int curGrainWeeksCounter;
        public bool canReceiveGrainProduce;
        public int curCowBirthCounter;
        public bool canReceiveNewCows;
        [Header("Coins")]
        // Coins
        public int coins;
        public int coinsCapacity;
        public int curMonthTaxCounter;
        public bool canReceiveMonthlyTax;
        // Technology
        public List<BaseTechnology> currentTechanologies;

        public bool IsStoryArcFinished(string storyTitle)
        {
            bool result = false;

            result = finishedStories.Contains(storyTitle);

            return result;
        }
    }

}