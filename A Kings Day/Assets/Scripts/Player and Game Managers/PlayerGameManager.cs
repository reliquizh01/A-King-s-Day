using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using Kingdoms;
using Technology;
using GameResource;
using ResourceUI;
using GameItems;
using Characters;
using KingEvents;
using Managers;

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
            playerData.fileData = newData.fileData;
            playerData.kingdomsName = newData.kingdomsName;
            playerData.dynastyName = newData.dynastyName;
            playerData.level = newData.level;
            playerData.weekCount = newData.weekCount;

            playerData.finishedStories = newData.finishedStories;

            playerData.queuedDataEventsList.AddRange(newData.queuedDataEventsList);
            playerData.curDataEvent = newData.curDataEvent;
            playerData.curDataStory = newData.curDataStory;
            playerData.eventFinished = newData.eventFinished;

            playerData.recruits = newData.recruits;
            playerData.barracksCapacity = newData.barracksCapacity;
            playerData.troopsLoyalty = newData.troopsLoyalty;
            playerData.swordsmenCount = newData.swordsmenCount;
            playerData.spearmenCount = newData.spearmenCount;
            playerData.archerCount= newData.archerCount;
            playerData.swordsmenMercCount = newData.swordsmenMercCount;
            playerData.spearmenMercCount = newData.spearmenMercCount;
            playerData.archerMercCount = newData.archerMercCount;

            playerData.population = newData.population;
            playerData.safePopulation = newData.safePopulation;
            playerData.pouplationLoyalty = newData.pouplationLoyalty;
            playerData.curTaxWeeksCounter = newData.curTaxWeeksCounter;
            playerData.canReceiveTax = newData.canReceiveTax;
            playerData.farmerCount = newData.farmerCount;
            playerData.herdsmanCount = newData.herdsmanCount;
            playerData.storageKeeperCount = newData.storageKeeperCount;

            playerData.populationBurst = newData.populationBurst;
            playerData.potentialRefugee = newData.potentialRefugee;
            playerData.potentialMerchantArrival = newData.potentialMerchantArrival;

            playerData.balconyBuildingsAdded = newData.balconyBuildingsAdded;
            playerData.buildingInformationData = newData.buildingInformationData;

            playerData.potentialGoodsMerchant = newData.potentialGoodsMerchant;
            playerData.potentialExoticMerchant = newData.potentialExoticMerchant;
            playerData.potentialEquipsMerchant = newData.potentialEquipsMerchant;

            playerData.potentialCommonHero = newData.potentialCommonHero;
            playerData.potentialRareHero = newData.potentialRareHero;
            playerData.potentialLegHero = newData.potentialLegHero;

            playerData.potentialMercSwords = newData.potentialMercSwords;
            playerData.potentialMercSpear = newData.potentialMercSpear;
            playerData.potentialMercArcher = newData.potentialMercArcher;

            playerData.swordsmenMercAvail = newData.swordsmenMercAvail;
            playerData.spearmenMercAvail = newData.spearmenMercAvail;
            playerData.archerMercAvail = newData.archerMercAvail;

            playerData.myHeroes = new List<BaseHeroInformationData>();
            for (int i = 0; i < newData.myHeroes.Count; i++)
            {
                BaseHeroInformationData tmp = new BaseHeroInformationData();

                tmp.damageGrowthRate = newData.myHeroes[i].damageGrowthRate;
                tmp.healthGrowthRate = newData.myHeroes[i].healthGrowthRate;
                tmp.speedGrowthRate = newData.myHeroes[i].speedGrowthRate;
                tmp.unitInformation = newData.myHeroes[i].unitInformation;
                tmp.equipments = newData.myHeroes[i].equipments;

                playerData.myHeroes.Add(tmp);
            }

            playerData.tavernHeroes = new List<BaseHeroInformationData>();
            for (int i = 0; i < newData.tavernHeroes.Count; i++)
            {
                BaseHeroInformationData tmp = new BaseHeroInformationData();

                tmp.damageGrowthRate = newData.tavernHeroes[i].damageGrowthRate;
                tmp.healthGrowthRate = newData.tavernHeroes[i].healthGrowthRate;
                tmp.speedGrowthRate = newData.tavernHeroes[i].speedGrowthRate;
                tmp.unitInformation = newData.tavernHeroes[i].unitInformation;
                tmp.equipments = newData.tavernHeroes[i].equipments;

                playerData.tavernHeroes.Add(tmp);
            }

            playerData.myItems = new List<ItemInformationData>();
            for (int i = 0; i < newData.myItems.Count; i++)
            {
                GameItems.ItemInformationData tmp = newData.myItems[i];
                playerData.myItems.Add(tmp);
            }

            playerData.currentShopMerchants = new List<BaseMerchantInformationData>();
            for (int i = 0; i < newData.currentShopMerchants.Count; i++)
            {
                BaseMerchantInformationData tmp = new BaseMerchantInformationData();
                tmp.merchantName = newData.currentShopMerchants[i].merchantName;
                tmp.itemsSold = newData.currentShopMerchants[i].itemsSold;

                playerData.currentShopMerchants.Add(tmp);
            }

            playerData.foods = newData.foods;
            playerData.safeFood = newData.safeFood;
            playerData.cows = newData.cows;
            playerData.safeCows = newData.safeCows;
            playerData.curGrainWeeksCounter = newData.curGrainWeeksCounter;
            playerData.canReceiveGrainProduce = newData.canReceiveGrainProduce;

            playerData.curCowBirthCounter = newData.curCowBirthCounter;
            playerData.canReceiveNewCows = newData.canReceiveNewCows;

            playerData.coins = newData.coins;
            playerData.coinsCapacity = newData.coinsCapacity;
            playerData.curMonthTaxCounter = newData.curMonthTaxCounter;
            playerData.canReceiveMonthlyTax = newData.canReceiveMonthlyTax;

            playerData.currentTechnologies = new List<BaseTechnology>();
            for (int i = 0; i < newData.currentTechnologies.Count; i++)
            {
                BaseTechnology tmp = new BaseTechnology();
                tmp.bonusIncrement = newData.currentTechnologies[i].bonusIncrement;
                tmp.coinTechType = newData.currentTechnologies[i].coinTechType;
                tmp.curGold = newData.currentTechnologies[i].curGold;
                tmp.currentLevel = newData.currentTechnologies[i].currentLevel;
                tmp.effectMesg = newData.currentTechnologies[i].effectMesg;
                tmp.foodTechType = newData.currentTechnologies[i].foodTechType;
                tmp.goldLevelRequirements = newData.currentTechnologies[i].goldLevelRequirements;
                tmp.goldRequirement = newData.currentTechnologies[i].goldRequirement;
                tmp.improvedType = newData.currentTechnologies[i].improvedType;
                tmp.popTechType = newData.currentTechnologies[i].popTechType;
                tmp.techIcon = newData.currentTechnologies[i].techIcon;
                tmp.technologyName = newData.currentTechnologies[i].technologyName;
                tmp.troopTechType = newData.currentTechnologies[i].troopTechType;
                tmp.wittyMesg = newData.currentTechnologies[i].wittyMesg;

                playerData.currentTechnologies.Add(tmp);
            }

            SetupResourceProductionUpdate();
        }

        public void ReceiveResource(int amount, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food:
                    playerData.foods += amount;
                    break;
                case ResourceType.Troops:
                    playerData.recruits += amount;
                    break;
                case ResourceType.Population:
                    playerData.SetPopulation(amount);
                    break;
                case ResourceType.Coin:
                    playerData.coins += amount;
                    break;
                case ResourceType.Cows:
                    break;
                case ResourceType.Archer:
                    break;
                case ResourceType.Swordsmen:
                    break;
                case ResourceType.Spearmen:
                    break;
                case ResourceType.farmer:
                    break;
                case ResourceType.herdsmen:
                    break;
                case ResourceType.storageKeeper:
                    break;
                default:
                    break;
            }

            SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
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
                    playerData.recruits -= amount;
                    break;
            }

            SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
        }
        public void SaveQueuedData(List<EventDecisionData> queuedDataList, int finishCount)
        {
            playerData.queuedDataEventsList = queuedDataList;
            playerData.eventFinished = finishCount;
        }
        public void SaveCurStory(StoryArcEventsData thisStory)
        {
            playerData.curDataStory = thisStory;
        }

        public void SaveCurDataEvent(EventDecisionData eventData)
        {
            playerData.curDataEvent = new EventDecisionData();

            if(eventData.title != null && !string.IsNullOrEmpty(eventData.title))
            {
                playerData.curDataEvent.title = eventData.title;
            }
            playerData.curDataEvent.description = eventData.description;
            playerData.curDataEvent.difficultyType = eventData.difficultyType;
            playerData.curDataEvent.arcEnd = eventData.arcEnd;
            playerData.curDataEvent.eventDecision = eventData.eventDecision;
            playerData.curDataEvent.eventType = eventData.eventType;
            playerData.curDataEvent.isStoryArc = eventData.isStoryArc;
            playerData.curDataEvent.storyArc = eventData.storyArc;
            playerData.curDataEvent.reporterType = eventData.reporterType;
        }

        public bool CheckResourceEnough(int amount, ResourceType type)
        {
            amount = Mathf.Abs(amount);
            
            switch (type)
            {
                case ResourceType.Food:
                    return (playerData.foods >= amount);
                case ResourceType.Troops:
                    return (playerData.recruits >= amount);
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