﻿using System;
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
    public enum BestTroops
    {
        recruits,
        swordsman,
        spearman,
        archer,
    }

    public enum FamousFor
    {
        FaithfulMerchant,
        CropsCultivator,
        SeasonedAdventurer,
        PeoplesChampion,
    }

    public enum WieldedWeapon
    {
        Sword,
        Spear,
        Bow,
        Books,
    }

    public enum FamilySecret
    {
        FamilyOfHalfDemons,
        RelicsHoarder,
        TreasuredGalleon,
        InstigatedRebellion,
    }

    public enum ResourceType
    {
        Food,
        Troops,
        Population,
        Coin,
        Cows,
        farmer,
        herdsmen,
        storageKeeper,
        Mercenary,
        foodStorage,
        cowStorage,
        housing,
        festivalDuration,
        PotentialChances,
    }
    public enum PotentialTravellers
    {
        refugee,
        newborn,
        mercenaries,
        randomMerchant,
        goodsMerchant,
        exoticMerchant,
        itemMerchant,
        RandomHero,
        CommonHero,
        RareHero,
        LegendaryHero,
        Plague,
    }
    /// <summary>
    /// Player Kingdom represents all information the player has
    /// </summary>
    [Serializable]
    public class PlayerKingdomData
    {
        public string _fileName;
        public bool fileData = false;

        [Header("Kingdom Age")]
        public int weekCount = 1;
        [Header("Kingdom Origin")]
        public string kingdomsName;
        public string dynastyName;
        public TerritoryLevel level;
        public BestTroops bestTroops;
        public FamousFor myFame;
        public WieldedWeapon wieldedWeapon;
        public FamilySecret familySecret;

        [Header("Events and Stories")]
        public List<StoryArcEventsData> finishedStories;
        public List<EventDecisionData> queuedDataEventsList;
        public StoryArcEventsData curDataStory;
        public EventDecisionData curDataEvent;
        public int eventFinished;

        [Header("Troops")]
        public int barracksCapacity;
        public int troopsLoyalty;
        public List<TroopsInformation> troopsList;
        public List<TroopsInformation> troopsMercList;

        public int GetTotalTroops
        {
            get {
                int tmp = 0;
                if(troopsList != null && troopsList.Count > 0)
                {
                    for (int i = 0; i < troopsList.Count; i++)
                    {
                        tmp += troopsList[i].totalUnitCount;
                    }
                }
                return tmp; }
        }
        public string RemoveScoutTroops()
        {
            string troopRemoved = "1 " + troopsList[0].unitInformation.unitName;
            troopsList[0].totalUnitCount -= 1;

            if(troopsList[0].totalUnitCount <= 0)
            {
                troopsList.RemoveAt(0);
            }

            return troopRemoved;
        }

        [Header("Population")]
        public int population;
        public int safePopulation = 50;
        public int pouplationLoyalty;
        public int curTaxWeeksCounter;
        public bool canReceiveTax;
        public int farmerCount, herdsmanCount, storageKeeperCount;
        public int deathByPlague;

        [Header("Game Chances")]
        /// This is a Festival Event
        /// within the span of the festival
        /// all chances will trigger
        /// until the festival ends.
        public int festivalWeeksDuration = 0;
        public int populationBurst = 0;
        public int potentialRefugee = 0;
        public int potentialMerchantArrival = 0;

        [Header("Building Information")]
        public bool balconyBuildingsAdded;
        public List<BuildingSavedData> buildingInformationData;


        [Header("Tavern Stuff")]
        /// When the map points summons the units
        /// this chances will affect if they will go to 
        /// your kingdom.
        public int potentialMerchant = 0;
        public int potentialGoodsMerchant = 0;
        public int potentialEquipsMerchant = 0;
        public int potentialExoticMerchant = 0;

        public int potentialRandomHero = 0;
        public int potentialCommonHero = 20;
        public int potentialRareHero = 10;
        public int potentialLegHero = 5;

        public int potentialMercenary = 0;

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
        public int curGrainWeeksCounter;
        public bool canReceiveGrainProduce;

        [Header("Cows")]
        public int cows;
        public int safeCows;
        public int barnExpansion;
        public int curCowBirthCounter;
        public bool canReceiveNewCows;


        [Header("Coins")]
        // Coins
        public int coins;
        public int coinsCapacity;
        public int curMonthTaxCounter;
        public int maxMonthTaxCount = 4;
        public bool canReceiveMonthlyTax;
        // Technology
        public List<BaseTechnologyData> currentTechnologies;

        [Header("Kingdom Roll")]
        public float storyRoll = 0.007f;
        public float currentRoll = 0.007f;

        public bool IsStoryArcFinished(string storyTitle)
        {
            bool result = false;

            if(finishedStories != null)
            {
                result = finishedStories.Find(x => x.storyTitle == storyTitle) != null;
            }

            return result;
        }

        public int ObtainTotalTroops()
        {
            int tmp = 0;

            if(this.troopsList != null && this.troopsList.Count > 0)
            {
                for (int i = 0; i < this.troopsList.Count; i++)
                {
                    tmp += this.troopsList[i].totalUnitCount;
                }
            }

            return tmp;
        }

        public TroopsInformation ObtainTroopInformation(string unitName)
        {
            if(troopsList == null)
            {
                return null;
            }

            return troopsList.Find(x => x.unitInformation.unitName == unitName);
        }

        public TroopsInformation ObtainMercenaryInformation(string unitName)
        {
            if (troopsMercList == null || troopsMercList.Count <= 0)
            {
                TroopsInformation tmp = new TroopsInformation();
                tmp.totalUnitCount = 0;
                return tmp;
            }

            return troopsMercList.Find(x => x.unitInformation.unitName == unitName);
        }

        public int ObtainResourceAmount(ResourceType type)
        {
            int amount = 0;
            switch (type)
            {
                case ResourceType.Food:
                    amount = foods;
                    break;
                case ResourceType.Troops:
                    amount = GetTotalTroops;
                    break;
                case ResourceType.Population:
                    amount = population;
                    break;
                case ResourceType.Coin:
                    amount = coins;
                    break;
                case ResourceType.Cows:
                    amount = cows;
                    break;
                case ResourceType.farmer:
                    amount = farmerCount;
                    break;
                case ResourceType.herdsmen:
                    amount = herdsmanCount;
                    break;
                case ResourceType.storageKeeper:
                    amount = storageKeeperCount;
                    break;
                case ResourceType.Mercenary:
                    if(troopsMercList == null || troopsMercList.Count <= 0)
                    {
                        amount = 0;
                    }
                    else
                    {
                        for (int i = 0; i < troopsMercList.Count; i++)
                        {
                            amount += troopsMercList[i].totalUnitCount;
                        }
                    }
                    break;
                case ResourceType.foodStorage:
                    amount = safeFood;
                    break;
                case ResourceType.cowStorage:
                    amount = safeCows;
                    break;
                case ResourceType.housing:
                    amount = safePopulation;
                    break;
                case ResourceType.festivalDuration:
                    amount = festivalWeeksDuration;
                    break;
                default:
                    break;
            }

            return amount;
        }

        public int ObtainChances(PotentialTravellers potentialTravellers)
        {
            int amount = 0;
            switch (potentialTravellers)
            {
                case PotentialTravellers.refugee:
                    amount = potentialRefugee;
                    break;
                case PotentialTravellers.newborn:
                    amount = populationBurst;
                    break;
                case PotentialTravellers.mercenaries:
                    amount = potentialMercenary;
                    break;
                case PotentialTravellers.randomMerchant:
                    amount = potentialMerchant;
                    break;
                case PotentialTravellers.goodsMerchant:
                    amount = potentialGoodsMerchant;
                    break;
                case PotentialTravellers.exoticMerchant:
                    amount = potentialExoticMerchant;
                    break;
                case PotentialTravellers.itemMerchant:
                    amount = potentialEquipsMerchant;
                    break;
                case PotentialTravellers.RandomHero:
                    amount = potentialRandomHero;
                    break;
                case PotentialTravellers.CommonHero:
                    amount = potentialCommonHero;
                    break;
                case PotentialTravellers.RareHero:
                    amount = potentialRareHero;
                    break;
                case PotentialTravellers.LegendaryHero:
                    amount = potentialLegHero;
                    break;
                case PotentialTravellers.Plague:
                    amount = deathByPlague;
                    break;
                default:
                    break;
            }
            return amount;
        }
        public void UpdateFoodStorage()
        {
            safeFood = 50 + (10 * storageKeeperCount);
        }

        public void UpdateCowStorage()
        {
            safeCows = barnExpansion + (2 * herdsmanCount);
        }
    }

}