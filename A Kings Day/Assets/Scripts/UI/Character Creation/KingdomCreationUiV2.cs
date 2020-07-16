using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using Managers;
using ResourceUI;
using Utilities;

public class KingdomCreationUiV2 : MonoBehaviour
{
    public PlayerKingdomData temporaryKingdom;

    [Header("Resource Information")]
    public int distributeAmount = 20;
    public int initialFood, initialTroops, initialPopulation, initialCoins;
    public List<ResourcePage> resourcePagesList;
    public CountingEffectUI distributeCounter;

    [Header("Family Origin Information")]
    public List<KingdomDescriptionDropdown> kingdomDescriptionList;

    public void Start()
    {
        temporaryKingdom = new PlayerKingdomData();
        for (int i = 0; i < resourcePagesList.Count; i++)
        {
            resourcePagesList[i].myController = this;
        }

        for (int i = 0; i < kingdomDescriptionList.Count; i++)
        {
            kingdomDescriptionList[i].myController = this;
        }
        SetupInitialData();
        distributeCounter.curCount = distributeAmount;
    }

    public void SetupInitialData()
    {
        temporaryKingdom.foods = initialFood;
        temporaryKingdom.population = initialPopulation;
        temporaryKingdom.recruits = initialTroops;
        temporaryKingdom.coins = initialCoins;

        for (int i = 0; i < resourcePagesList.Count; i++)
        {
            resourcePagesList[i].UpdateResourceCount();
        }
    }

    #region RESOURCE INFORMATIOn
    public void IncreaseResource(ResourceType thisResource)
    {
        int amountToAdd = 1;

        if(distributeAmount <= 0)
        {
            return;
        }

        if(UtilitiesCommandObserver.GetInstance != null)
        {
            if (UtilitiesCommandObserver.GetInstance.isKeyToggled(KeyCode.LeftShift))
            {
                if (distributeAmount >= 5)
                {
                    amountToAdd = 5;
                }
                else
                {
                    amountToAdd = distributeAmount;
                }
            }
        }

        switch (thisResource)
        {
            case ResourceType.Food:
                temporaryKingdom.foods += amountToAdd;
                break;
            case ResourceType.Troops:
                temporaryKingdom.recruits += amountToAdd;
                break;
            case ResourceType.Population:
                temporaryKingdom.population += amountToAdd;
                break;
            case ResourceType.Coin:
                temporaryKingdom.coins += amountToAdd;
                break;
            case ResourceType.Cows:
                temporaryKingdom.cows += amountToAdd;
                break;
            case ResourceType.Archer:
                temporaryKingdom.archerCount += amountToAdd;
                break;
            case ResourceType.Swordsmen:
                temporaryKingdom.swordsmenCount += amountToAdd;
                break;
            case ResourceType.Spearmen:
                temporaryKingdom.spearmenCount += amountToAdd;
                break;
            case ResourceType.farmer:
                temporaryKingdom.farmerCount += amountToAdd;
                break;
            case ResourceType.herdsmen:
                temporaryKingdom.herdsmanCount += amountToAdd;
                break;
            case ResourceType.storageKeeper:
                temporaryKingdom.storageKeeperCount += amountToAdd;
                break;
            case ResourceType.ArcherMerc:
                temporaryKingdom.archerMercCount += amountToAdd;
                break;
            case ResourceType.SwordsmenMerc:
                temporaryKingdom.swordsmenMercCount += amountToAdd;
                break;
            case ResourceType.SpearmenMerc:
                temporaryKingdom.spearmenMercCount += amountToAdd;
                break;
            default:
                break;
        }

        distributeAmount -= amountToAdd;
        UpdateDistributeCounter();
    }

    public int ObtainResourceCount(ResourceType thisResource)
    {
        int thisCount = -1;

        switch (thisResource)
        {
            case ResourceType.Food:
                thisCount = temporaryKingdom.foods;
                break;
            case ResourceType.Troops:
                thisCount = temporaryKingdom.recruits;
                break;
            case ResourceType.Population:
                thisCount = temporaryKingdom.population;
                break;
            case ResourceType.Coin:
                thisCount = temporaryKingdom.coins;
                break;
            case ResourceType.Cows:
                thisCount = temporaryKingdom.cows;
                break;
            case ResourceType.Archer:
                thisCount = temporaryKingdom.archerCount;
                break;
            case ResourceType.Swordsmen:
                thisCount = temporaryKingdom.swordsmenCount;
                break;
            case ResourceType.Spearmen:
                thisCount = temporaryKingdom.spearmenCount;
                break;
            case ResourceType.farmer:
                thisCount = temporaryKingdom.farmerCount;
                break;
            case ResourceType.herdsmen:
                thisCount = temporaryKingdom.herdsmanCount;
                break;
            case ResourceType.storageKeeper:
                thisCount = temporaryKingdom.storageKeeperCount;
                break;
            case ResourceType.ArcherMerc:
                thisCount = temporaryKingdom.archerMercCount;
                break;
            case ResourceType.SwordsmenMerc:
                thisCount = temporaryKingdom.swordsmenMercCount;
                break;
            case ResourceType.SpearmenMerc:
                thisCount = temporaryKingdom.spearmenMercCount;
                break;
            default:
                break;
        }

        return thisCount;
    }
    public void DecreaseResource(ResourceType thisResource)
    {
        int amountToAdd = 1;
        bool decreaseIncrement = false;

        if (UtilitiesCommandObserver.GetInstance != null)
        {
            if (UtilitiesCommandObserver.GetInstance.isKeyToggled(KeyCode.LeftShift))
            {
                decreaseIncrement = true;
            }
        }
        switch (thisResource)
        {
            case ResourceType.Food:
                if(temporaryKingdom.foods <= 0)
                {
                    return;
                }
                if(decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.foods >= 10) ? 10 : temporaryKingdom.foods;
                }
                temporaryKingdom.foods -= amountToAdd;
                break;
            case ResourceType.Troops:
                if (temporaryKingdom.recruits <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.recruits >= 10) ? 10 : temporaryKingdom.recruits;
                }
                temporaryKingdom.recruits -= amountToAdd;
                break;
            case ResourceType.Population:
                if (temporaryKingdom.population <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.population >= 10) ? 10 : temporaryKingdom.population;
                }
                temporaryKingdom.population -= amountToAdd;
                break;
            case ResourceType.Coin:
                if (temporaryKingdom.coins <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.coins >= 10) ? 10 : temporaryKingdom.coins;
                }
                temporaryKingdom.coins -= amountToAdd;
                break;
            case ResourceType.Cows:
                if (temporaryKingdom.cows <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.cows >= 10) ? 10 : temporaryKingdom.cows;
                }
                temporaryKingdom.cows -= amountToAdd;
                break;
            case ResourceType.Archer:
                if (temporaryKingdom.archerCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.archerCount >= 10) ? 10 : temporaryKingdom.archerCount;
                }
                temporaryKingdom.archerCount -= amountToAdd;
                break;
            case ResourceType.Swordsmen:
                if (temporaryKingdom.swordsmenCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.swordsmenCount >= 10) ? 10 : temporaryKingdom.swordsmenCount;
                }
                temporaryKingdom.swordsmenCount -= amountToAdd;
                break;
            case ResourceType.Spearmen:
                if (temporaryKingdom.spearmenCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.spearmenCount >= 10) ? 10 : temporaryKingdom.spearmenCount;
                }
                temporaryKingdom.spearmenCount -= amountToAdd;
                break;
            case ResourceType.farmer:
                if (temporaryKingdom.farmerCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.farmerCount >= 10) ? 10 : temporaryKingdom.farmerCount;
                }
                temporaryKingdom.farmerCount -= amountToAdd;
                break;
            case ResourceType.herdsmen:
                if (temporaryKingdom.herdsmanCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.herdsmanCount >= 10) ? 10 : temporaryKingdom.herdsmanCount;
                }
                temporaryKingdom.herdsmanCount -= amountToAdd;
                break;
            case ResourceType.storageKeeper:
                if (temporaryKingdom.storageKeeperCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.storageKeeperCount >= 10) ? 10 : temporaryKingdom.storageKeeperCount;
                }
                temporaryKingdom.storageKeeperCount -= amountToAdd;
                break;
            case ResourceType.ArcherMerc:
                if (temporaryKingdom.archerMercCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.archerMercCount >= 10) ? 10 : temporaryKingdom.archerMercCount;
                }
                temporaryKingdom.archerMercCount -= amountToAdd;
                break;
            case ResourceType.SwordsmenMerc:
                if (temporaryKingdom.swordsmenMercCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.swordsmenMercCount >= 10) ? 10 : temporaryKingdom.swordsmenMercCount;
                }
                temporaryKingdom.swordsmenMercCount -= amountToAdd;
                break;
            case ResourceType.SpearmenMerc:
                if (temporaryKingdom.spearmenMercCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.spearmenMercCount >= 10) ? 10 : temporaryKingdom.spearmenMercCount;
                }
                temporaryKingdom.spearmenMercCount -= amountToAdd;
                break;
            default:
                break;
        }

        distributeAmount += amountToAdd;
        UpdateDistributeCounter();
    }

    public void UpdateDistributeCounter()
    {
        distributeCounter.SetTargetCount(distributeAmount);
    }

    #endregion

    #region FAMILY ORIGIN
    public void AdjustFamilyOrigin(KingdomDescriptionDropdown thisDropDown)
    {
        int dropDownIdx = kingdomDescriptionList.IndexOf(thisDropDown);

        switch (dropDownIdx)
        {
            case 0:
                temporaryKingdom.bestTroops = (BestTroops)kingdomDescriptionList[dropDownIdx].dropDown.value;
                break;
            case 1:
                temporaryKingdom.myFame = (FamousFor)kingdomDescriptionList[dropDownIdx].dropDown.value;
                break;
            case 2:
                temporaryKingdom.wieldedWeapon = (WieldedWeapon)kingdomDescriptionList[dropDownIdx].dropDown.value;
                break;
            case 3:
                temporaryKingdom.familySecret = (FamilySecret)kingdomDescriptionList[dropDownIdx].dropDown.value;
                break;
            default:
                break;
        }
    }
    #endregion
}
