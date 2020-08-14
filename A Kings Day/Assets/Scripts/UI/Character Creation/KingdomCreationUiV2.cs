using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using Managers;
using ResourceUI;
using Utilities;
using SaveData;

public class KingdomCreationUiV2 : MonoBehaviour
{
    public BasePanelBehavior myPanel;
    public PlayerKingdomData temporaryKingdom;
    public int recruitIdx;
    [Header("Resource Information")]
    public int distributeAmount = 20;
    public int initialFood, initialTroops, initialPopulation, initialCoins;
    public List<ResourcePage> resourcePagesList;
    public CountingEffectUI distributeCounter;

    [Header("Family Origin Information")]
    public List<KingdomDescriptionDropdown> kingdomDescriptionList;

    [Header("Warning Tabs")]
    public BasePanelWindow warningDistributionTab;
    public BasePanelWindow playPrologueTab;
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
        temporaryKingdom.safeFood = 50;
        temporaryKingdom.population = initialPopulation;
        temporaryKingdom.safePopulation = 20;

        TroopsInformation tmp = new TroopsInformation();
        tmp.unitInformation = new Characters.UnitInformationData();
        tmp.unitInformation = TransitionManager.GetInstance.unitStorage.GetUnitInformation("Recruit");
        tmp.totalUnitCount = initialTroops;
        temporaryKingdom.troopsList = new List<TroopsInformation>();
        temporaryKingdom.troopsList.Add(tmp);
        recruitIdx = temporaryKingdom.troopsList.Count - 1;

        temporaryKingdom.barracksCapacity = 20;
        temporaryKingdom.coins = initialCoins;
        temporaryKingdom.coinsCapacity = 30;


        for (int i = 0; i < resourcePagesList.Count; i++)
        {
            resourcePagesList[i].UpdateResourceCount();
        }
    }

    #region RESOURCE INFORMATION
    public void IncreaseResource(ResourceType thisResource)
    {
        int amountToAdd = 1;

        if(distributeAmount <= 0)
        {
            return;
        }

        if(UtilitiesCommandObserver.GetInstance != null)
        {
            if (UtilitiesCommandObserver.GetInstance.isKeyToggled(UtilitiesCommandObserver.GetInstance.GetKey("INCREASE_COUNT_INCREMENT")))
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
                temporaryKingdom.troopsList[recruitIdx].totalUnitCount += amountToAdd;
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
            case ResourceType.farmer:
                temporaryKingdom.farmerCount += amountToAdd;
                break;
            case ResourceType.herdsmen:
                temporaryKingdom.herdsmanCount += amountToAdd;
                break;
            case ResourceType.storageKeeper:
                temporaryKingdom.storageKeeperCount += amountToAdd;
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
                thisCount = temporaryKingdom.troopsList[recruitIdx].totalUnitCount;
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
            case ResourceType.farmer:
                thisCount = temporaryKingdom.farmerCount;
                break;
            case ResourceType.herdsmen:
                thisCount = temporaryKingdom.herdsmanCount;
                break;
            case ResourceType.storageKeeper:
                thisCount = temporaryKingdom.storageKeeperCount;
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
                if (temporaryKingdom.troopsList[recruitIdx].totalUnitCount <= 0)
                {
                    return;
                }
                if (decreaseIncrement)
                {
                    amountToAdd = (temporaryKingdom.troopsList[recruitIdx].totalUnitCount >= 10) ? 10 : temporaryKingdom.troopsList[recruitIdx].totalUnitCount;
                }
                temporaryKingdom.troopsList[recruitIdx].totalUnitCount -= amountToAdd;
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

    public void CheckEstablishClick()
    {
        if(distributeAmount > 0)
        {
            OpenWarning();
        }
        else
        {
            OpenPrologueAccept();
        }
    }

    public void OpenWarning()
    {
        warningDistributionTab.gameObject.SetActive(true);
    }

    public void OpenPrologueAccept()
    {
        warningDistributionTab.CloseWindow();
        playPrologueTab.gameObject.SetActive(true);
    }


    public void PlayPrologue()
    {
        myPanel.PlayCloseAnimation();
        if(CreationSceneManager.GetInstance != null)
        {
            CreationSceneManager.GetInstance.StartPrologue();
        }
        playPrologueTab.CloseWindow();

        SaveLoadManager.GetInstance.inheritanceData = new PlayerKingdomData();
        SaveLoadManager.GetInstance.inheritanceData = temporaryKingdom;
    }


    public void StraightToCourtroom()
    {
        if(CreationSceneManager.GetInstance != null)
        {
            CreationSceneManager.GetInstance.EndPrologueScene(temporaryKingdom);
        }
        playPrologueTab.CloseWindow();
    }
}
