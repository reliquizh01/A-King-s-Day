using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Managers;
using Characters;
using Kingdoms;
using GameResource;

public class BuildingInformationPanel : MonoBehaviour
{
    public InformationActionHandler myController;

    public BuildingType buildingType;

    public List<BuildingInformationPage> informationPageList;
    public BuildingInformationPage currentPage;

    public int cardIdx, selectedHeroIdx = 0, selectedMerchantIdx = 0;
    PlayerKingdomData playerData;
    public BuildingInformationData currentBuildingInformation;

    public void InitializeBuildingInformation(BuildingInformationData newInformation)
    {
        currentBuildingInformation = new BuildingInformationData();
        currentBuildingInformation = newInformation;

        SetupInformationPages();
    }

    public void SetupInformationPages()
    {
        if(informationPageList == null || informationPageList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < informationPageList.Count; i++)
        {
            informationPageList[i].InitializeInformationPanels(myController);
        }
    }
    public void UpdatePages(int actionIdx)
    {
        if(PlayerGameManager.GetInstance != null)
        {
            playerData = new PlayerKingdomData();
            playerData = PlayerGameManager.GetInstance.playerData;
        }

        if (currentPage != null)
        {
            myController.HideInfoBlocker();
            currentPage.gameObject.SetActive(false);
        }
        cardIdx = actionIdx;
        currentPage = informationPageList[cardIdx];
        if (currentPage != null)
        {
            currentPage.gameObject.SetActive(true);
        }
        switch (buildingType)
        {
            case BuildingType.Shop:
                UpdateShop();
                break;
            case BuildingType.Barracks:
                UpdateBarracks();
                break;
            case BuildingType.Tavern:
                UpdateTavern();
                break;
            case BuildingType.Smithery:

                break;
            case BuildingType.Houses:
                UpdateHouses();
                break;
            case BuildingType.Farm:
                UpdateFarm();
                break;
            case BuildingType.Market:
                UpdateMarket();
                break;
            default:
                break;
        }

    }


    // UPDATE BARRACKS NEEDS MORE DATA (HERO AND UNIT INFORMATION DATA)
    public void UpdateBarracks()
    {
        TroopResourceBehavior troopsBehavior = PlayerGameManager.GetInstance.troopBehavior;
        KingdomUnitStorage unitStorage = myController.unitStorage;
        if (cardIdx == 0) // TRAIN NEW SOLDIER
        {
            // Placeholders, need proper Unit Storage later on
            float hp = unitStorage.GetUnitInformation("Recruit").maxHealth;
            float dmg = unitStorage.GetUnitInformation("Recruit").maxDamage;
            float spd = unitStorage.GetUnitInformation("Recruit").origSpeed;
            spd *= 10;

            if(PlayerGameManager.GetInstance != null)
            {
                hp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                dmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
            }

            int recruitCount = 0;
            if(playerData.troopsList == null)
            {
                playerData.troopsList = new List<TroopsInformation>();
            }
            if(playerData.troopsList.Find(x => x.unitInformation.unitName == "Recruit") != null)
            {
                recruitCount = playerData.troopsList.Find(x => x.unitInformation.unitName == "Recruit").totalUnitCount;
            }
            List<float> tmp = new List<float>();tmp.Add(recruitCount);; tmp.Add(hp); tmp.Add(spd); tmp.Add(dmg);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Recruits"); // HP DAMAGE SPEED

            string secondDescription = "Max trained units the barracks can support [" + playerData.barracksCapacity + "]";
            currentPage.informationPanelList[1].SetSingleCounter(playerData.barracksCapacity,secondDescription, "Barracks Capacity"); // BARRACKS CAPACITY

            string thirdDescription = "Training cost to arm a person.";
            currentPage.informationPanelList[2].SetSingleCounter(troopsBehavior.GetRecruitCoins, thirdDescription, "Training Cost"); // TRAINING COST
        }
        else if(cardIdx == 2) // HEROES
        {
            if (PlayerGameManager.GetInstance != null)
            {
                if(PlayerGameManager.GetInstance.playerData.myHeroes != null && PlayerGameManager.GetInstance.playerData.myHeroes.Count > 0)
                {
                    BaseHeroInformationData curHero = PlayerGameManager.GetInstance.playerData.myHeroes[selectedHeroIdx];
                    currentPage.informationPanelList[0].SetGrowthCounter((int)curHero.unitInformation.maxHealth, curHero.healthGrowthRate, "HEALTH", curHero.unitInformation.unitName);
                    currentPage.informationPanelList[1].SetGrowthCounter((int)curHero.unitInformation.maxDamage, curHero.damageGrowthRate, "DAMAGE");
                    currentPage.informationPanelList[2].SetGrowthCounter((int)curHero.unitInformation.origSpeed, curHero.speedGrowthRate, "SPEED");
                }
                else
                {
                    myController.ShowInfoBlocker("Recruit Hero in Tavern");
                }
            }
        }
        else if(cardIdx == 1) // TRAIN SOLDIERS
        {
            // Placeholders, need proper Unit Storage later on
            float Sprhp = unitStorage.GetUnitInformation("Spearman").maxHealth, Sprdmg = unitStorage.GetUnitInformation("Spearman").maxDamage, Sprspd = unitStorage.GetUnitInformation("Spearman").origSpeed;
            float Swdhp = unitStorage.GetUnitInformation("Swordsman").maxHealth, Swddmg = unitStorage.GetUnitInformation("Swordsman").maxDamage, Swdspd = unitStorage.GetUnitInformation("Swordsman").origSpeed;
            float Archp = unitStorage.GetUnitInformation("Archer").maxHealth, Arcdmg = unitStorage.GetUnitInformation("Archer").maxDamage, Arcspd = unitStorage.GetUnitInformation("Archer").origSpeed;
            Sprspd *= 10; Arcspd *= 10; Swdspd *= 10;

            if (PlayerGameManager.GetInstance != null)
            {
                Sprhp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Swdhp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Archp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Sprdmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
                Swddmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
                Arcdmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
            }


            List<float> tmp = new List<float>();
            for (int i = 0; i < currentPage.informationPanelList.Count; i++)
            {
                if(i == 0) 
                {
                    int count = 0;
                    if (playerData.troopsList == null)
                    {
                        playerData.troopsList = new List<TroopsInformation>();
                    }
                    if (playerData.troopsList.Find(x => x.unitInformation.unitName == "Swordsman") != null)
                    {
                        count = playerData.troopsList.Find(x => x.unitInformation.unitName == "Swordsman").totalUnitCount;
                    }
                    tmp.Add(count);
                    tmp.Add(Swdhp); tmp.Add(Swdspd); tmp.Add(Swddmg);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Swordsman");
                    currentPage.informationPanelList[i].panelIcon.sprite = TransitionManager.GetInstance.unitStorage.GetUnitIcon("Swordsman");
                }
                else if(i == 1)
                {
                    int count = 0;
                    if (playerData.troopsList == null)
                    {
                        playerData.troopsList = new List<TroopsInformation>();
                    }
                    if (playerData.troopsList.Find(x => x.unitInformation.unitName == "Spearman") != null)
                    {
                        count = playerData.troopsList.Find(x => x.unitInformation.unitName == "Spearman").totalUnitCount;
                    }
                    tmp.Add(count);
                    tmp.Add(Sprhp); tmp.Add(Sprspd); tmp.Add(Sprdmg);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Spearman");
                    currentPage.informationPanelList[i].panelIcon.sprite = TransitionManager.GetInstance.unitStorage.GetUnitIcon("Spearman");
                }
                else if(i == 2)
                {
                    int count = 0;
                    if (playerData.troopsList == null)
                    {
                        playerData.troopsList = new List<TroopsInformation>();
                    }
                    if (playerData.troopsList.Find(x => x.unitInformation.unitName == "Archer") != null)
                    {
                        count = playerData.troopsList.Find(x => x.unitInformation.unitName == "Archer").totalUnitCount;
                    }
                    tmp.Add(count);
                    tmp.Add(Archp); tmp.Add(Arcspd); tmp.Add(Arcdmg);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Archer");
                    currentPage.informationPanelList[i].panelIcon.sprite = TransitionManager.GetInstance.unitStorage.GetUnitIcon("Archer");

                }
                tmp.Clear();
            }
        }
    }

    public void UpdateMarket()
    {
        if(cardIdx == 0) // Prepare Festival
        {
            currentPage.informationPanelList[0].SetSingleCounter(playerData.populationBurst, "Chance of Instant Population", "Population Burst");
            currentPage.informationPanelList[1].SetSingleCounter(playerData.potentialRefugee, "New Refugee from the outlands", "Seeking Refuge");
            currentPage.informationPanelList[2].SetSingleCounter(playerData.potentialMerchantArrival, "Merchants arriving on Festival", "Merchant Arrival");
        }
        else if(cardIdx == 1) //Check Items Sold
        {
            if(playerData.currentFestivalMerchants != null && playerData.currentFestivalMerchants.Count > 0)
            {

                currentPage.informationPanelList[0].SetFlexibleCounter(playerData.currentFestivalMerchants[selectedMerchantIdx].itemsSold[0], 
                    playerData.currentFestivalMerchants[selectedMerchantIdx].merchantName);
                currentPage.informationPanelList[1].SetFlexibleCounter(playerData.currentFestivalMerchants[selectedMerchantIdx].itemsSold[1],
                    playerData.currentFestivalMerchants[selectedMerchantIdx].merchantName);
                currentPage.informationPanelList[2].SetFlexibleCounter(playerData.currentFestivalMerchants[selectedMerchantIdx].itemsSold[2],
                    playerData.currentFestivalMerchants[selectedMerchantIdx].merchantName);
            }
            else
            {
                myController.ShowInfoBlocker("No Merchants has arrived.");
            }
        }
        else if(cardIdx == 2) // Post Demand
        {

            string goodsDescription = "<color=green>+10%</color> Chance of Goods Merchant arrives.";
            currentPage.informationPanelList[0].SetSingleCounter(playerData.potentialGoodsMerchant, goodsDescription, "Goods Merchant");
            string exoticDescription = "<color=green>+2%</color> Chance of Exotic Merchant arrives.";
            currentPage.informationPanelList[1].SetSingleCounter(playerData.potentialGoodsMerchant, exoticDescription, "Exotic Merchant");
            string heroesDescription = "<color=green>+5%</color> Chance of Equipment Merchant arrives.";
            currentPage.informationPanelList[2].SetSingleCounter(playerData.potentialGoodsMerchant, heroesDescription, "Equipment Merchant");
        }
    }

    public void UpdateFarm()
    {
        FoodResourceBehavior foodBehavior = PlayerGameManager.GetInstance.foodBehavior;
        if (cardIdx == 0) // ADD Farmer
        {
            string firstDescription = "Number of Farmers";
            currentPage.informationPanelList[0].SetSingleCounter(playerData.farmerCount, firstDescription, "FARMERS");


            int foodPerFarmer = foodBehavior.techHarvestProduce + 1;
            string secondDescription = "<color=green>+"+foodPerFarmer+"</color> Food every farmer.";
            int foodProduce = (PlayerGameManager.GetInstance != null) ? foodBehavior.GetGrainProduction() : 0;
            currentPage.informationPanelList[1].SetSingleCounter(foodProduce, secondDescription, "Food Produce");

            int harvestTime = foodBehavior.GetHarvestTime;
            string thirdDescription = "Harvest every <color=green>"+harvestTime +"</color> weeks";
            currentPage.informationPanelList[2].SetSingleCounter(playerData.curGrainWeeksCounter, thirdDescription, "Harvest Time");
        }
        else if(cardIdx == 1) // ADD HERDSMAN
        {
            string firstDescription = "<color=green>+1</color> Food produce for every herdsman";
            currentPage.informationPanelList[0].SetSingleCounter(playerData.herdsmanCount, firstDescription, "Herdsman");


            string secondDescription = "<color=green>+1%</color> chance for every herdsman.";
            int birthChance = (int)(foodBehavior.GetCowBirth * 100.0f);
            currentPage.informationPanelList[1].SetSingleCounter(birthChance, secondDescription, "Cow Birthrate");

            int cowMeat = foodBehavior.GetCowMeat(1);
            string thirdDescription = "Gain <color=green>" + cowMeat + "</color> food per cow.";
            currentPage.informationPanelList[2].SetSingleCounter(playerData.cows, thirdDescription, "Number of Cows");

        }
        else if(cardIdx == 2) // STORAGE KEEPER
        {
            string firstDescription = "<color=green>+2</color> max safe food storage and an item for every keeper.";
            currentPage.informationPanelList[0].SetSingleCounter(playerData.storageKeeperCount, firstDescription, "Storage Keepers");


            string secondDescription = "Current amount of food safe from raids.";
            int birthChance = (int)foodBehavior.GetMaxFoodStorage;
            currentPage.informationPanelList[1].SetSingleCounter(birthChance, secondDescription, "Current Safe Food");


            string thirdDescription = "";
            if(currentBuildingInformation != null && !string.IsNullOrEmpty(currentBuildingInformation.BuildingName))
            {
                thirdDescription = "Gain <color=green>+"+ currentBuildingInformation.ObtainCardDataReward(2,2, ResourceType.cowStorage) + "</color> safe cows for every expansion.";
            }
            else
            {
                thirdDescription = "Gain <color=green>+2</color> safe cows for every expansion.";
            }

            currentPage.informationPanelList[2].SetSingleCounter(playerData.safeCows, thirdDescription, "Safe Cow Barn");
        }
    }

    public void UpdateHouses()
    {
        PopulationResourceBehavior populationBehavior = PlayerGameManager.GetInstance.populationBehavior;
        if (cardIdx == 0) // Birthrate Chance
        {
            string firstDescription = "Chance of people dying per week.";
            currentPage.informationPanelList[0].SetSingleCounter(populationBehavior.baseDeathChance, firstDescription, "Weekly Death Chance");
            string secondDescription = "Population supported by 1 food consumed per week.";
            currentPage.informationPanelList[1].SetSingleCounter(populationBehavior.GetPopPerFood, secondDescription, "Rotten Food");

            string thirdDescription = "Chance of plague events occurring";
            currentPage.informationPanelList[2].SetSingleCounter(populationBehavior.uncleanWeeks, thirdDescription, "Plague Events");
        }
        else if (cardIdx == 1) // Build New Houses
        {
            string firstDescription = "Chance of attracting new settlers.";
            currentPage.informationPanelList[0].SetSingleCounter(populationBehavior.GetSettlerChance, firstDescription, "Rising Land Value");

            string secondDescription = "Maximum population who can live in your kingdom";
            currentPage.informationPanelList[1].SetSingleCounter(populationBehavior.GetMaxPopulation, secondDescription, "Population Capacity");

            string thirdDescription = "Population we can save in case of raids.";
            currentPage.informationPanelList[2].SetSingleCounter(playerData.safePopulation, thirdDescription, "Stone House");
        }
        else if (cardIdx == 2) // Clean the area
        {
            string firstDescription = "Chance of people dying per week.";
            currentPage.informationPanelList[0].SetSingleCounter(populationBehavior.baseDeathChance, firstDescription, "Weekly Death Chance");

            string secondDescription = "Population supported by <color=green>1</color> food consumed per week.";
            currentPage.informationPanelList[1].SetSingleCounter(populationBehavior.GetPopPerFood, secondDescription, "Rotten Food");

            string thirdDescription = "Chance of plague events occurring";
            currentPage.informationPanelList[2].SetSingleCounter(populationBehavior.uncleanWeeks, thirdDescription, "Plague Events");

        }
    }

    public void UpdateTavern()
    {
        if(cardIdx == 0) // RECRUIT HEROES
        {
            if(playerData.tavernHeroes == null || playerData.tavernHeroes.Count <= 0)
            {
                myController.ShowInfoBlocker("No Heroes has Arrived");
            }
            else
            {
                for (int i = 0; i < currentPage.informationPanelList.Count; i++)
                {
                    if(playerData.tavernHeroes[i] == null)
                    {
                        break;
                    }
                    List<int> health = new List<int>();
                    health.Add((int)playerData.tavernHeroes[i].unitInformation.maxHealth);
                    health.Add((int)playerData.tavernHeroes[i].healthGrowthRate);
                    List<int> speed = new List<int>();
                    speed.Add((int)playerData.tavernHeroes[i].unitInformation.origSpeed);
                    speed.Add((int)playerData.tavernHeroes[i].speedGrowthRate);
                    List<int> damage = new List<int>();
                    damage.Add((int)playerData.tavernHeroes[i].unitInformation.minDamage);
                    damage.Add((int)playerData.tavernHeroes[i].unitInformation.maxDamage);
                    damage.Add((int)playerData.tavernHeroes[i].damageGrowthRate);

                    currentPage.informationPanelList[i].SetHeroCounter(health, damage, speed, playerData.tavernHeroes[i].unitInformation.attackType,
                        playerData.tavernHeroes[i].unitInformation.unitName);
                }
            }
        }
        else if(cardIdx == 1) // GIVE DRINKS
        {
            List<float> tmp = new List<float>();
            tmp.Add(playerData.potentialCommonHero); tmp.Add(playerData.potentialRareHero); tmp.Add(playerData.potentialLegHero);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Total Hero Chances");

            List<float> tmp1 = new List<float>();
            tmp1.Add(playerData.potentialGoodsMerchant); tmp1.Add(playerData.potentialEquipsMerchant); tmp1.Add(playerData.potentialExoticMerchant);
            currentPage.informationPanelList[1].SetMultiCounter(tmp1, "Total Merchant Chances");

            List<float> tmp2 = new List<float>();
            tmp2.Add(playerData.potentialCommonHero); tmp2.Add(playerData.potentialRareHero); tmp2.Add(playerData.potentialLegHero);
            currentPage.informationPanelList[2].SetMultiCounter(tmp, "Total Mercenary Chances");
        }
        else if(cardIdx == 2) // HIRE MERCENARIES
        {
            KingdomUnitStorage unitStorage = myController.unitStorage;

            float Sprhp = unitStorage.GetUnitInformation("Spearman").maxHealth, Sprdmg = unitStorage.GetUnitInformation("Spearman").maxDamage, Sprspd = unitStorage.GetUnitInformation("Spearman").origSpeed;
            float Swdhp = unitStorage.GetUnitInformation("Swordsman").maxHealth, Swddmg = unitStorage.GetUnitInformation("Swordsman").maxDamage, Swdspd = unitStorage.GetUnitInformation("Swordsman").origSpeed;
            float Archp = unitStorage.GetUnitInformation("Archer").maxHealth, Arcdmg = unitStorage.GetUnitInformation("Archer").maxDamage, Arcspd = unitStorage.GetUnitInformation("Archer").origSpeed;
            Sprspd *= 10; Arcspd *= 10; Swdspd *= 10;
            if (PlayerGameManager.GetInstance != null)
            {
                Sprhp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Swdhp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Archp += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                Sprdmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
                Swddmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
                Arcdmg += PlayerGameManager.GetInstance.troopBehavior.techDmg;
            }

            List<float> tmp = new List<float>();
            tmp.Add(playerData.ObtainMercenaryInformation("Swordsman").totalUnitCount); tmp.Add(Swdhp); tmp.Add(Swddmg); tmp.Add(Swdspd);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Swordsman Available");

            List<float> tmp1 = new List<float>();
            tmp1.Add(playerData.ObtainMercenaryInformation("Spearman").totalUnitCount); tmp1.Add(Sprhp); tmp1.Add(Sprdmg); tmp1.Add(Sprspd);
            currentPage.informationPanelList[1].SetMultiCounter(tmp1, "Spearman Available");

            List<float> tmp2 = new List<float>();
            tmp2.Add(playerData.ObtainMercenaryInformation("Archer").totalUnitCount); tmp2.Add(Archp); tmp2.Add(Arcdmg); tmp2.Add(Arcspd);
            currentPage.informationPanelList[2].SetMultiCounter(tmp2, "Archer Available");
        }
    }

    public void UpdateShop()
    {
        if (playerData.currentShopMerchants.Count <= 0)
        {
            Debug.Log("No merchant Available!");
            myController.ShowInfoBlocker("No Merchant in Stall.");
            currentPage.informationPanelList[0].ResetCounter();
            currentPage.informationPanelList[1].ResetCounter();
            currentPage.informationPanelList[2].ResetCounter();
        }
        else
        {
            Debug.Log("Merchant Available!");
            for (int i = 0; i < currentPage.informationPanelList.Count; i++)
            {
                currentPage.informationPanelList[i].SetFlexibleCounter(playerData.currentShopMerchants[cardIdx].itemsSold[i], playerData.currentShopMerchants[cardIdx].merchantName);
            }
        }
    
    }
}
