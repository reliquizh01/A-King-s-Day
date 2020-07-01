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

    public void UpdatePages(int actionIdx)
    {
        if(PlayerGameManager.GetInstance != null)
        {
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
        if(cardIdx == 0) // TRAIN NEW SOLDIER
        {
            // Placeholders, need proper Unit Storage later on
            int hp = 4, dmg = 1, spd = 1;
            List<int> tmp = new List<int>();tmp.Add(playerData.recruits);; tmp.Add(hp); tmp.Add(dmg); tmp.Add(spd);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Recruits"); // HP DAMAGE SPEED

            string secondDescription = "Maximum units the barracks can support [" + playerData.recruits +"," 
                + playerData.swordsmenCount +"," + playerData.spearmenCount + "," + playerData.archerCount + "]";
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
            int Sprhp = 5, Sprdmg = 3, Sprspd = 1;
            int Swdhp = 5, Swddmg = 3, Swdspd = 1;
            int Archp = 5, Arcdmg = 3, Arcspd = 1;
            List<int> tmp = new List<int>();
            for (int i = 0; i < currentPage.informationPanelList.Count; i++)
            {
                if(i == 0) 
                {
                    if (PlayerGameManager.GetInstance != null)
                    {
                        tmp.Add(PlayerGameManager.GetInstance.playerData.swordsmenCount);
                    }
                    tmp.Add(Swdhp); tmp.Add(Swddmg); tmp.Add(Swdspd);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Swordsman");
                }
                else if(i == 1)
                {
                    if (PlayerGameManager.GetInstance != null)
                    {
                        tmp.Add(PlayerGameManager.GetInstance.playerData.spearmenCount);
                    }
                    tmp.Add(Sprhp); tmp.Add(Sprdmg); tmp.Add(Sprspd);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Spearmen");

                }
                else if(i == 2)
                {
                    if (PlayerGameManager.GetInstance != null)
                    {
                        tmp.Add(PlayerGameManager.GetInstance.playerData.archerCount);
                    }
                    tmp.Add(Archp); tmp.Add(Arcdmg); tmp.Add(Arcspd);

                    currentPage.informationPanelList[i].SetMultiCounter(tmp, "Archer");

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
            currentPage.informationPanelList[1].SetSingleCounter(playerData.potentialRefugee, "New Refugee from the utlands", "Seeking Refuge");
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
        else if(cardIdx == 1)
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
        else if(cardIdx == 2)
        {
            string firstDescription = "<color=green>+2</color> max safe food storage for every keeper.";
            currentPage.informationPanelList[0].SetSingleCounter(playerData.herdsmanCount, firstDescription, "Storage Keepers");


            string secondDescription = "Current amount of food safe from raids.";
            int birthChance = (int)foodBehavior.GetMaxFoodStorage;
            currentPage.informationPanelList[1].SetSingleCounter(birthChance, secondDescription, "Current Safe Food");


            string thirdDescription = "Gain <color=green>+2</color> safe cows for every expansion.";
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
        else if(cardIdx == 1)
        {
            List<int> tmp = new List<int>();
            tmp.Add(playerData.potentialCommonHero); tmp.Add(playerData.potentialRareHero); tmp.Add(playerData.potentialLegHero);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Total Hero Chances");

            List<int> tmp1 = new List<int>();
            tmp1.Add(playerData.potentialGoodsMerchant); tmp1.Add(playerData.potentialEquipsMerchant); tmp1.Add(playerData.potentialExoticMerchant);
            currentPage.informationPanelList[1].SetMultiCounter(tmp1, "Total Merchant Chances");

            List<int> tmp2 = new List<int>();
            tmp2.Add(playerData.potentialCommonHero); tmp2.Add(playerData.potentialRareHero); tmp2.Add(playerData.potentialLegHero);
            currentPage.informationPanelList[2].SetMultiCounter(tmp, "Total Mercenary Chances");
        }
        else if(cardIdx == 2)
        {
            List<int> tmp = new List<int>();
            tmp.Add(playerData.swordsmenMercAvail); tmp.Add(10); tmp.Add(4); tmp.Add(2);
            currentPage.informationPanelList[0].SetMultiCounter(tmp, "Swordsman Available");

            List<int> tmp1 = new List<int>();
            tmp1.Add(playerData.spearmenMercAvail); tmp1.Add(6); tmp1.Add(2); tmp1.Add(3);
            currentPage.informationPanelList[1].SetMultiCounter(tmp1, "Spearman Available");

            List<int> tmp2 = new List<int>();
            tmp2.Add(playerData.archerMercAvail); tmp2.Add(4); tmp2.Add(1); tmp2.Add(3);
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
