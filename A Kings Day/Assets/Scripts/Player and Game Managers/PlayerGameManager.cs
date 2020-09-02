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

        [Header("Temporary Data")]
        public BaseTravellerData unitsToSend;
        public void Start()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.WEEKLY_UPDATE, WeeklyResourceProductionUpdate);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.WEEKLY_UPDATE, WeeklyResourceProductionUpdate);
        }

        public void SendThisUnits(List<TroopsInformation> thisUnits, List<BaseHeroInformationData> heroesLeading, bool isAttack = true)
        {
            if(isAttack)
            {
                unitsToSend.troopsCarried = new List<TroopsInformation>();
                unitsToSend.troopsCarried.AddRange(thisUnits);
                unitsToSend.leaderUnit = new List<BaseHeroInformationData>();
                unitsToSend.leaderUnit.AddRange(heroesLeading);
            }
        }
        public void SetupResourceProductionUpdate()
        {
            foodBehavior.SetupResourceBehavior();

            troopBehavior.SetupResourceBehavior();

            populationBehavior.SetupResourceBehavior();

            coinBehavior.SetupResourceBehavior();

            mapBehavior.SetupMapBehavior();
        }
        public void WeeklyResourceProductionUpdate(Parameters p = null)
        {
            Debug.Log("Updating ResourceProduction ----------------");
            foodBehavior.UpdateWeeklyProgress();

            troopBehavior.UpdateWeeklyProgress();

            populationBehavior.UpdateWeeklyProgress();

            coinBehavior.UpdateWeeklyProgress();

            mapBehavior.UpdateWeeklyProgress();


            ResourceUI.ResourceInformationController.GetInstance.UpdateCurrentPanel();
        }
        public void ReceiveCampaignData(PlayerCampaignData newData)
        {
            Debug.Log("Receiving Data From : " + newData._fileName + " Count:" + newData.travellerList.Count);
            campaignData = new PlayerCampaignData();
            campaignData = newData;


            List<BaseTravellerData> newTemp = new List<BaseTravellerData>();

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
            if(newData.myHeroes != null && newData.myHeroes.Count > 0)
            {
                Debug.Log("Temporary Kingdom Heroes Count: " + newData.myHeroes.Count + " And Skill Count: " + newData.myHeroes[0].skillsList.Count);

                for (int i = 0; i < newData.myHeroes.Count; i++)
                {
                    BaseHeroInformationData tmp = new BaseHeroInformationData();

                    tmp.damageGrowthRate = newData.myHeroes[i].damageGrowthRate;
                    tmp.healthGrowthRate = newData.myHeroes[i].healthGrowthRate;
                    tmp.speedGrowthRate = newData.myHeroes[i].speedGrowthRate;
                    tmp.unitInformation = newData.myHeroes[i].unitInformation;
                    tmp.equipments = newData.myHeroes[i].equipments;

                    tmp.skillsList = new List<BaseSkillInformationData>();
                    tmp.skillsList.AddRange(newData.myHeroes[i].skillsList);

                    playerData.myHeroes.Add(tmp);
                }
            }

            playerData.tavernHeroes = new List<BaseHeroInformationData>();
            if(newData.tavernHeroes != null && newData.tavernHeroes.Count > 0)
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
            if(newData.myItems != null && newData.myItems.Count > 0)
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
            if (newData.currentTechnologies != null && newData.currentTechnologies.Count > 0)
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

            int totalCheck = playerData.ObtainResourceAmount(type) - amount;
            if(totalCheck <= 0)
            {
                amount = playerData.ObtainResourceAmount(type);
            }

            switch (type)
            {
                case ResourceType.Food:
                    playerData.foods -= amount;
                    if (playerData.foods < 0) playerData.foods = 0;
                    break;
                case ResourceType.Population:
                    playerData.population -= amount;
                    if (playerData.population < 0) playerData.population = 0;
                    break;
                case ResourceType.Coin:
                    playerData.coins -= amount;
                    if (playerData.coins < 0) playerData.coins = 0;
                    break;
                case ResourceType.Cows:
                    playerData.cows -= amount;
                    if (playerData.cows < 0) playerData.cows = 0;
                    break;
                case ResourceType.farmer:
                    playerData.farmerCount -= amount;
                    if (playerData.farmerCount < 0) playerData.farmerCount = 0;
                    break;
                case ResourceType.herdsmen:
                    playerData.herdsmanCount -= amount;
                    if (playerData.herdsmanCount < 0) playerData.herdsmanCount = 0;
                    playerData.UpdateCowStorage();
                    break;
                case ResourceType.storageKeeper:
                    playerData.storageKeeperCount -= amount;
                    if (playerData.storageKeeperCount < 0) playerData.storageKeeperCount = 0;
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

            if(SaveData.SaveLoadManager.GetInstance != null)
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
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
                case ResourceType.farmer:
                    return (playerData.farmerCount >= amount);
                default:
                    return false;
            }
        }

        public BaseTravellerData ConvertWholeGarrisonAsTraveller()
        {
            BaseTravellerData wholeKingdom = new BaseTravellerData();
            wholeKingdom.troopsCarried = new List<TroopsInformation>();

            if(playerData.troopsList != null && playerData.troopsList.Count > 0)
            {
                for (int i = 0; i < playerData.troopsList.Count; i++)
                {
                    wholeKingdom.troopsCarried.Add(playerData.troopsList[i]);
                }
            }

            wholeKingdom.leaderUnit = new List<BaseHeroInformationData>();
            wholeKingdom.leaderUnit.Add(playerData.myHeroes.Find(x => x.unitInformation.unitName == "Player"));

            return wholeKingdom;
        }
        public BaseTravellerData ObtainTraveller(BaseHeroInformationData thisTraveller)
        {
            BaseTravellerData tmp = campaignData.travellerList.Find(x => x.leaderUnit.Contains(thisTraveller));
            
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