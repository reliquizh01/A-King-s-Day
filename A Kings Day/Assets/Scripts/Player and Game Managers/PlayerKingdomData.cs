using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Territory;
using Technology;
using Characters;
using KingEvents;
using GameItems;
using Buildings;

namespace Kingdoms
{
    public enum ResourceType
    {
        Food,
        Troops,
        Population,
        Coin,
        Cows,
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
        public bool fileData = false;
        public string kingdomsName;
        public string dynastyName;
        public TerritoryLevel level;
        public int weekCount = 1;
        public float storyRoll = 0.007f;
        public float currentRoll = 0.007f;

        [Header("Events and Stories")]
        public List<StoryArcEventsData> finishedStories;
        public List<EventDecisionData> queuedDataEventsList;
        public StoryArcEventsData curDataStory;
        public EventDecisionData curDataEvent;
        public int eventFinished;

        [Header("Troops")]
        public int recruits;
        public int barracksCapacity;
        public int troopsLoyalty;
        public int swordsmenCount, spearmenCount, archerCount;
        public int swordsmenMercCount, spearmenMercCount, archerMercCount;

        public int GetTotalTroops
        {
            get { return recruits + swordsmenCount + spearmenCount + archerCount; }
        }

        [Header("Population")]
        public int population;
        public int safePopulation = 50;
        public int pouplationLoyalty;
        public int curTaxWeeksCounter;
        public bool canReceiveTax;
        public int farmerCount, herdsmanCount, storageKeeperCount;

        [Header("Game Chances")]
        public int populationBurst = 0;
        public int potentialRefugee = 0;
        public int potentialMerchantArrival = 0;


        [Header("Building Information")]
        public bool balconyBuildingsAdded;
        public List<BuildingSavedData> buildingInformationData;


        [Header("Tavern Stuff")]
        public int potentialGoodsMerchant = 0;
        public int potentialEquipsMerchant = 0;
        public int potentialExoticMerchant = 0;
        public int potentialCommonHero = 20;
        public int potentialRareHero = 10;
        public int potentialLegHero = 5;
        public int potentialMercSwords = 5;
        public int potentialMercSpear = 5;
        public int potentialMercArcher = 5;
        public int swordsmenMercAvail, spearmenMercAvail, archerMercAvail;

        [Header("Heroes")]
        public List<BaseHeroInformationData> myHeroes;
        public List<BaseHeroInformationData> tavernHeroes;

        [Header("Item Inventory")]
        public List<ItemInformationData> myItems;

        [Header("Merchants")]
        public List<BaseMerchantInformationData> currentShopMerchants;
        public List<BaseMerchantInformationData> currentFestivalMerchants;

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
        public int safeFood;
        public int cows;
        public int safeCows;

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
        public List<BaseTechnology> currentTechnologies;

        public bool IsStoryArcFinished(string storyTitle)
        {
            bool result = false;

            result = finishedStories.Find(x => x.storyTitle == storyTitle) != null;

            return result;
        }
    }

}