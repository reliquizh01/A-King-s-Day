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
        public MapWeeklyBehavior mapBehavior;

        public PlayerKingdomData playerData = new PlayerKingdomData();
        public PlayerCampaignData campaignData = new PlayerCampaignData();
        public void Start()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.WEEKLY_UPDATE, WeeklyResourceProductionUpdate);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.WEEKLY_UPDATE, WeeklyResourceProductionUpdate);
        }
        public void SetupResourceProductionUpdate()
        {
            foodBehavior.SetupResourceBehavior();

            troopBehavior.SetupResourceBehavior();

            populationBehavior.SetupResourceBehavior();

            coinBehavior.SetupResourceBehavior();
        }
        public void WeeklyResourceProductionUpdate(Parameters p = null)
        {
            Debug.Log("Updating ResourceProduction ----------------");
            foodBehavior.UpdateWeeklyProgress();

            troopBehavior.UpdateWeeklyProgress();

            populationBehavior.UpdateWeeklyProgress();

            coinBehavior.UpdateWeeklyProgress();

            ResourceUI.ResourceInformationController.GetInstance.UpdateCurrentPanel();
        }
        public void ReceiveCampaignData(PlayerCampaignData newData)
        {
            Debug.Log("Receiving Data From : " + newData._fileName + " Count:" + newData.travellerList.Count);
            campaignData = new PlayerCampaignData();
            campaignData = newData;


            List<BaseTravellerData> newTemp = new List<BaseTravellerData>();
            Debug.Log("Traveller Count:" + campaignData.travellerList.Count);

            Debug.Log("Loading Campaign Finish");
        }
        public void ReceiveData(PlayerKingdomData newData)
        {
            playerData._fileName = newData._fileName;
            playerData.fileData = newData.fileData;

            playerData.kingdomsName = newData.kingdomsName;
            playerData.dynastyName = newData.dynastyName;
            playerData.level = newData.level;
            playerData.weekCount = newData.weekCount;

            playerData.finishedStories = newData.finishedStories;

            if(newData.queuedDataEventsList != null && newData.queuedDataEventsList.Count > 0)
            {
                if(playerData.queuedDataEventsList ==null)
                {
                    playerData.queuedDataEventsList = new List<EventDecisionData>();
                }
                playerData.queuedDataEventsList.AddRange(newData.queuedDataEventsList);
            }
            playerData.curDataEvent = newData.curDataEvent;
            playerData.curDataStory = newData.curDataStory;
            playerData.eventFinished = newData.eventFinished;

            Debug.Log("-------[Events Finished: " + playerData.eventFinished + " NewData Finish : " + newData.eventFinished + "]-------");
            playerData.barracksCapacity = newData.barracksCapacity;
            playerData.troopsLoyalty = newData.troopsLoyalty;

            playerData.troopsList = new List<TroopsInformation>();
            if(newData.troopsList != null && newData.troopsList.Count > 0)
            {
                for (int i = 0; i < newData.troopsList.Count; i++)
                {
                    playerData.troopsList.Add(newData.troopsList[i]);
                }
            }

            playerData.troopsMercList = new List<TroopsInformation>();
            if(newData.troopsMercList != null && newData.troopsMercList.Count > 0)
            {
                for (int i = 0; i < newData.troopsMercList.Count; i++)
                {
                    playerData.troopsMercList.Add(newData.troopsMercList[i]);
                }
            }

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

            playerData.potentialMercenary = newData.potentialMercenary;

            playerData.myHeroes = new List<BaseHeroInformationData>();
            if(newData.myHeroes != null)
            {
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
            }

            playerData.tavernHeroes = new List<BaseHeroInformationData>();
            if(newData.tavernHeroes != null)
            {
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
            }

            playerData.myItems = new List<ItemInformationData>();
            if(newData.myItems != null)
            {
                for (int i = 0; i < newData.myItems.Count; i++)
                {
                    GameItems.ItemInformationData tmp = newData.myItems[i];
                    playerData.myItems.Add(tmp);
                }
            }

            playerData.currentShopMerchants = new List<BaseMerchantInformationData>();
            if(newData.currentShopMerchants != null)
            {
                for (int i = 0; i < newData.currentShopMerchants.Count; i++)
                {
                    BaseMerchantInformationData tmp = new BaseMerchantInformationData();
                    tmp.merchantName = newData.currentShopMerchants[i].merchantName;
                    tmp.itemsSold = newData.currentShopMerchants[i].itemsSold;

                    playerData.currentShopMerchants.Add(tmp);
                }
            }

            Debug.Log("Food Count Before: " + newData.foods);
            playerData.foods = newData.foods;
            Debug.Log("Food Count After: " + newData.foods);
            playerData.safeFood = newData.safeFood;
            playerData.cows = newData.cows;
            playerData.safeCows = newData.safeCows;
            playerData.barnExpansion = newData.barnExpansion;

            playerData.curGrainWeeksCounter = newData.curGrainWeeksCounter;
            playerData.canReceiveGrainProduce = newData.canReceiveGrainProduce;

            playerData.curCowBirthCounter = newData.curCowBirthCounter;
            playerData.canReceiveNewCows = newData.canReceiveNewCows;

            playerData.coins = newData.coins;
            playerData.coinsCapacity = newData.coinsCapacity;
            playerData.curMonthTaxCounter = newData.curMonthTaxCounter;
            playerData.canReceiveMonthlyTax = newData.canReceiveMonthlyTax;

            playerData.currentTechnologies = new List<BaseTechnologyData>();
            if (newData.currentTechnologies != null)
            {
                for (int i = 0; i < newData.currentTechnologies.Count; i++)
                {
                    BaseTechnologyData tmp = new BaseTechnologyData();
                    tmp.technologyName = newData.currentTechnologies[i].technologyName;
                    tmp.improvedType =   newData.currentTechnologies[i].improvedType;
                    tmp.goldLevelRequirements = newData.currentTechnologies[i].goldLevelRequirements;

                    tmp.currentLevel = newData.currentTechnologies[i].currentLevel;
                    tmp.goldRequirement = newData.currentTechnologies[i].goldRequirement;
                    tmp.curGold = newData.currentTechnologies[i].curGold;
                    tmp.bonusIncrement = newData.currentTechnologies[i].bonusIncrement;

                    tmp.troopTechType = newData.currentTechnologies[i].troopTechType;
                    tmp.popTechType = newData.currentTechnologies[i].popTechType;
                    tmp.foodTechType = newData.currentTechnologies[i].foodTechType;
                    tmp.coinTechType = newData.currentTechnologies[i].coinTechType;

                    tmp.effectMesg = newData.currentTechnologies[i].effectMesg;
                    tmp.wittyMesg = newData.currentTechnologies[i].wittyMesg;

                    playerData.currentTechnologies.Add(tmp);
                }
            }

        }
        public void ReceiveTroops(int amount, string unitName)
        {
            int idx = playerData.troopsList.FindIndex(x => x.unitInformation.unitName == unitName);
            Debug.Log(idx + " Receiving Unit : " + unitName);
            if (idx != -1)
            {
                playerData.troopsList[idx].totalUnitCount += amount;
            }
            else
            {
                if(TransitionManager.GetInstance != null)
                {
                    TroopsInformation tmp = new TroopsInformation();
                    tmp.unitInformation = new UnitInformationData();
                    tmp.unitInformation = TransitionManager.GetInstance.unitStorage.GetUnitInformation(unitName);
                    tmp.totalUnitCount += amount;

                    if(tmp.unitInformation != null && !string.IsNullOrEmpty(tmp.unitInformation.unitName))
                    {
                        playerData.troopsList.Add(tmp);
                    }
                }
            }
        }

        public void ReceiveMercenary(int amount, string unitName)
        {
            int idx = playerData.troopsMercList.FindIndex(x => x.unitInformation.unitName == unitName);

            if (idx != -1)
            {
                playerData.troopsMercList[idx].totalUnitCount += amount;
            }
            else
            {
                if (TransitionManager.GetInstance != null)
                {
                    TroopsInformation tmp = new TroopsInformation();
                    tmp.unitInformation = new UnitInformationData();
                    tmp.unitInformation = TransitionManager.GetInstance.unitStorage.GetUnitInformation(unitName);
                    tmp.totalUnitCount += amount;

                    if (tmp.unitInformation != null && !string.IsNullOrEmpty(tmp.unitInformation.unitName))
                    {
                        playerData.troopsMercList.Add(tmp);
                    }
                }
            }
        }

        public void ReceiveResource(int amount, ResourceType type, string troopName = "")
        {
            switch (type)
            {
                case ResourceType.Food:
                    playerData.foods += amount;
                    break;
                case ResourceType.Population:
                    playerData.SetPopulation(amount);
                    break;
                case ResourceType.Coin:
                    playerData.coins += amount;
                    break;
                case ResourceType.Cows:
                    playerData.cows += amount;
                    break;
                case ResourceType.farmer:
                    playerData.farmerCount += amount;
                    break;
                case ResourceType.herdsmen:
                    playerData.herdsmanCount += amount;
                    playerData.UpdateCowStorage();
                    break;
                case ResourceType.storageKeeper:
                    playerData.storageKeeperCount += amount;
                    playerData.UpdateFoodStorage();
                    break;
                case ResourceType.Troops:
                    ReceiveTroops(amount, troopName);
                    break;
                case ResourceType.Mercenary:
                    ReceiveMercenary(amount, troopName);
                    break;
                case ResourceType.cowStorage:
                    playerData.barnExpansion += amount;
                    playerData.UpdateCowStorage();
                    break;
                case ResourceType.foodStorage:
                    playerData.safeFood += amount;
                    break;

                default:
                    break;
            }

            if(TransitionManager.GetInstance != null)
            {
                if(!TransitionManager.GetInstance.isNewGame)
                {
                    SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
                }
            }
            else
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
            }


            foodBehavior.UpdateWarningMechanics();
            troopBehavior.UpdateWarningMechanics();
            populationBehavior.UpdateWarningMechanics();
            coinBehavior.UpdateWarningMechanics();
            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.UpdateCurrentPanelWarnings();
            }

        }
        public void RemoveResource(int amount, ResourceType type, string troopName = "")
        {
            amount = Mathf.Abs(amount);
            switch (type)
            {
                case ResourceType.Food:
                    playerData.foods -= amount;
                    break;
                case ResourceType.Population:
                    playerData.SetPopulation(amount);
                    break;
                case ResourceType.Coin:
                    playerData.coins -= amount;
                    break;
                case ResourceType.Cows:
                    playerData.cows -= amount;
                    break;
                case ResourceType.farmer:
                    playerData.farmerCount -= amount;
                    break;
                case ResourceType.herdsmen:
                    playerData.herdsmanCount -= amount;
                    playerData.UpdateCowStorage();
                    break;
                case ResourceType.storageKeeper:
                    playerData.storageKeeperCount -= amount;
                    playerData.UpdateFoodStorage();
                    break;
                case ResourceType.Troops:
                    RemoveTroops(amount, troopName);
                    break;
                case ResourceType.Mercenary:
                    RemoveMercenary(amount, troopName);
                    break;
                default:
                    break;
            }

            if(TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
            }


            foodBehavior.UpdateWarningMechanics();
            troopBehavior.UpdateWarningMechanics();
            populationBehavior.UpdateWarningMechanics();
            coinBehavior.UpdateWarningMechanics();
            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.UpdateCurrentPanelWarnings();
            }
        }
        public void RemoveTroops(int amount, string unitName)
        {
            if(string.IsNullOrEmpty(unitName))
            {
                unitName = playerData.troopsList[0].unitInformation.unitName;
                RemoveResource(amount, ResourceType.Troops, unitName);
            }
            else
            {
                int idx = playerData.troopsList.FindIndex(x => x.unitInformation.unitName == unitName);
                if (idx != -1)
                {
                    playerData.troopsList[idx].totalUnitCount -= amount;
                }

                if (playerData.troopsList[idx].totalUnitCount <= 0)
                {
                    playerData.troopsList.RemoveAt(idx);
                }
            }
        }
        public void RemoveMercenary(int amount, string unitName)
        {
            int idx = playerData.troopsMercList.FindIndex(x => x.unitInformation.unitName == unitName);
            if (idx != -1)
            {
                playerData.troopsMercList[idx].totalUnitCount -= amount;
            }

            if (playerData.troopsMercList[idx].totalUnitCount <= 0)
            {
                playerData.troopsMercList.RemoveAt(idx);
            }
        }
        public void SaveQueuedData(List<EventDecisionData> queuedDataList, int finishCount)
        {
            Debug.Log("Saving Data For Queued Events: " + finishCount);
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

            if(eventData != null && !string.IsNullOrEmpty(eventData.title))
            {
                playerData.curDataEvent.title = eventData.title;
                playerData.curDataEvent.description = eventData.description;
                playerData.curDataEvent.difficultyType = eventData.difficultyType;
                playerData.curDataEvent.arcEnd = eventData.arcEnd;
                playerData.curDataEvent.eventDecision = eventData.eventDecision;
                playerData.curDataEvent.eventType = eventData.eventType;
                playerData.curDataEvent.isStoryArc = eventData.isStoryArc;
                playerData.curDataEvent.storyArc = eventData.storyArc;
                playerData.curDataEvent.reporterType = eventData.reporterType;
            }
            else
            {
                playerData.curDataEvent = null;
            }
        }

        public bool CheckResourceEnough(int amount, ResourceType type, string troopName = "")
        {
            amount = Mathf.Abs(amount);
            
            switch (type)
            {
                case ResourceType.Food:
                    return (playerData.foods >= amount);
                case ResourceType.Troops:
                    int totalTroops = 0;
                    if(playerData.troopsList != null && playerData.troopsList.Count > 0)
                    {
                        for (int i = 0; i < playerData.troopsList.Count; i++)
                        {
                            totalTroops += playerData.troopsList[i].totalUnitCount;
                        }
                    }
                    return (totalTroops >= amount);
                case ResourceType.Population:
                    return (playerData.population >= amount);
                case ResourceType.Coin:
                    return (playerData.coins >= amount);
                case ResourceType.Cows:
                    return (playerData.cows >= amount);
                case ResourceType.Mercenary:
                    int totalMerc = 0;
                    if (playerData.troopsMercList != null && playerData.troopsMercList.Count > 0)
                    {
                        totalMerc = playerData.troopsMercList.Find(x => x.unitInformation.unitName == troopName).totalUnitCount;
                    }
                    return (totalMerc >= amount);
                case ResourceType.herdsmen:
                    return (playerData.herdsmanCount >= amount);
                default:
                    return false;
            }
        }

        public BaseTravellerData ObtainTraveller(BaseHeroInformationData thisTraveller)
        {
            BaseTravellerData tmp = campaignData.travellerList.Find(x => x.leaderUnit == thisTraveller);
            
            return tmp;
        }

        public void SaveTraveller(BaseTravellerData thisTraveller)
        {
            int idx = -1;
            if(campaignData.travellerList == null)
            {
                campaignData.travellerList = new List<BaseTravellerData>();
            }

            if(campaignData.travellerList != null && campaignData.travellerList.Count > 0)
            {
                idx = campaignData.travellerList.FindIndex(x => x.travellersName == thisTraveller.travellersName);
            }

            if (idx != -1)
            {
                campaignData.travellerList[idx] = thisTraveller;
            }
            else
            {
                campaignData.travellerList.Add(thisTraveller);
            }

           
        }
    }
}