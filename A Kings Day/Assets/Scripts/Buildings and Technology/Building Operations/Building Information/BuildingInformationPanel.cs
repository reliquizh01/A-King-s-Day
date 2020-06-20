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
                break;
            case BuildingType.Barracks:
                UpdateBarracks();
                break;
            case BuildingType.Tavern:
                break;
            case BuildingType.Smithery:
                break;
            case BuildingType.Houses:
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

        if(cardIdx == 0) // TRAIN NEW SOLDIER
        {
            // Placeholders, need proper Unit Storage later on
            int hp = 4, dmg = 1, spd = 1;

            currentPage.informationPanelList[0].SetSingleCounter(hp, "HEALTH", "SOLDIER");
            currentPage.informationPanelList[1].SetSingleCounter(dmg, "DAMAGE");
            currentPage.informationPanelList[2].SetSingleCounter(spd, "SPEED");
        }
        else if(cardIdx == 2) // HEROES
        {
            if (PlayerGameManager.GetInstance != null)
            {
                if(PlayerGameManager.GetInstance.playerData.myHeroes != null && PlayerGameManager.GetInstance.playerData.myHeroes.Count > 0)
                {
                    Debug.Log("PUSSH");
                    BaseHeroInformationData curHero = PlayerGameManager.GetInstance.playerData.myHeroes[selectedHeroIdx];
                    currentPage.informationPanelList[0].SetGrowthCounter((int)curHero.unitInformation.maxHealth, curHero.healthGrowthRate, "HEALTH", curHero.heroName);
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
            currentPage.informationPanelList[2].SetSingleCounter(playerData.cowCapacity, thirdDescription, "Safe Cow Barn");
        }
    }
}
