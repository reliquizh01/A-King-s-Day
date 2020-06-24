using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using Managers;
using ResourceUI;
using Utilities;

public class KingdomCreationUI : MonoBehaviour
{
    public PlayerKingdomData temporaryKingdom;
    public BaseResourceUIController food, troops, population, coins;


    public TextMeshProUGUI troopCount, popCount, coinCount, foodCount, distributeCount;
    public int distributeAmount = 20;
    public GameObject troopGameObject, popGameObject, coinGameObject, foodGameObject;

    public void OnEnable()
    {
        temporaryKingdom.recruits += 10;
        temporaryKingdom.foods += 10;
        temporaryKingdom.coins += 10;
        temporaryKingdom.population += 10;
        food.IncreaseResource(10); foodCount.text = temporaryKingdom.foods.ToString();
        troops.IncreaseResource(10); troopCount.text = temporaryKingdom.GetTotalTroops.ToString();
        population.IncreaseResource(10); popCount.text = temporaryKingdom.population.ToString();
        coins.IncreaseResource(10); coinCount.text = temporaryKingdom.coins.ToString();
    }

    public void Update()
    {
        if (troops.isHovered)
        {
            troopGameObject.SetActive(true);
        }
        else
        {
            troopGameObject.SetActive(false);
        }
        if (population.isHovered)
        {
            popGameObject.SetActive(true);
        }
        else
        {
            popGameObject.SetActive(false);
        }
        if (food.isHovered)
        {
            foodGameObject.SetActive(true);
        }
        else
        {
            foodGameObject.SetActive(false);
        }
        if (coins.isHovered)
        {
            coinGameObject.SetActive(true);
        }
        else
        {
            coinGameObject.SetActive(false);
        }
    }
    public void AddInitialResource(int thisResource)
    {
        if(distributeAmount <= 0)
        {
            return;
        }

        int amountToAdd = 1;
        
        if(UtilitiesCommandObserver.GetInstance.isKeyToggled(KeyCode.LeftShift))
        {
            if(distributeAmount >= 5)
            {
                amountToAdd = 5;
            }
            else
            {
                amountToAdd = distributeAmount;
            }
        }
        switch (thisResource)
        {
            case 0:
                if (temporaryKingdom.foods >= 50) return;
                temporaryKingdom.foods += amountToAdd;
                foodCount.text = temporaryKingdom.foods.ToString();
                food.IncreaseResource(amountToAdd);
                break;

            case 1:
                if (temporaryKingdom.recruits >= 50) return;
                temporaryKingdom.recruits += amountToAdd;
                troopCount.text = temporaryKingdom.recruits.ToString();
                troops.IncreaseResource(amountToAdd);
                break;
            case 2:
                if (temporaryKingdom.population >= 50) return;
                temporaryKingdom.population += amountToAdd;
                popCount.text = temporaryKingdom.population.ToString();
                population.IncreaseResource(amountToAdd);
                break;

            case 3:
                if (temporaryKingdom.coins >= 50) return;
                temporaryKingdom.coins += amountToAdd;
                coinCount.text = temporaryKingdom.coins.ToString();
                coins.IncreaseResource(amountToAdd);
                break;
        }

        distributeAmount -= amountToAdd;
        distributeCount.text = distributeAmount.ToString();
    }
    public void RemoveInitialResource(int thisResource)
    {
        if(distributeAmount > 120)
        {
            return;
        }
        bool noChanges = false;

        int amountToRemove = 1;

        if (UtilitiesCommandObserver.GetInstance.isKeyToggled(KeyCode.LeftShift))
        {
             amountToRemove = 5;
        }

        switch (thisResource)
        {
            case 0:
                if (temporaryKingdom.foods <= 5)
                {
                    noChanges = true;
                    break;
                }
                amountToRemove = (temporaryKingdom.foods < amountToRemove) ? temporaryKingdom.foods : amountToRemove;
                temporaryKingdom.foods -= amountToRemove;
                foodCount.text = temporaryKingdom.foods.ToString();
                food.DecreaseResource(amountToRemove);
                break;

            case 1:
                if (temporaryKingdom.recruits <= 5)
                {
                    noChanges = true;
                    break;
                }
                amountToRemove = (temporaryKingdom.recruits < amountToRemove) ? temporaryKingdom.recruits : amountToRemove;
                temporaryKingdom.recruits -= amountToRemove;
                troopCount.text = temporaryKingdom.recruits.ToString();
                troops.DecreaseResource(amountToRemove);
                break;
            case 2:
                if (temporaryKingdom.population <= 5)
                {
                    noChanges = true;
                    break;
                }
                amountToRemove = (temporaryKingdom.population < amountToRemove) ? temporaryKingdom.population : amountToRemove;
                temporaryKingdom.population -= amountToRemove;
                popCount.text = temporaryKingdom.population.ToString();
                population.DecreaseResource(amountToRemove);
                break;

            case 3:
                if (temporaryKingdom.coins <= 5)
                {
                    noChanges = true;
                    break;
                }
                amountToRemove = (temporaryKingdom.coins < amountToRemove) ? temporaryKingdom.coins : amountToRemove;
                temporaryKingdom.coins -= amountToRemove;
                coinCount.text = temporaryKingdom.coins.ToString();
                coins.DecreaseResource(amountToRemove);
                break;
        }

        if(!noChanges)
        {
            distributeAmount += amountToRemove;
            distributeCount.text = distributeAmount.ToString();
        }
    }
    public void Establish()
    {
        temporaryKingdom.weekCount = 1;
        temporaryKingdom.coinsCapacity = 50;
        temporaryKingdom.safePopulation= 50;
        temporaryKingdom.barracksCapacity = 50;
        temporaryKingdom.safeFood = 50;
        temporaryKingdom.safeCows = 10;
        temporaryKingdom.fileData = true;
        temporaryKingdom.myItems = new List<GameItems.ItemInformationData>();

        PlayerGameManager.GetInstance.ReceiveData(temporaryKingdom);
        SaveData.SaveLoadManager.GetInstance.SetNewSaveData(PlayerGameManager.GetInstance.playerData);
        SaveData.SaveLoadManager.GetInstance.SaveData();
        temporaryKingdom = null;
        TransitionManager.GetInstance.LoadScene(SceneType.Courtroom);
        TransitionManager.GetInstance.TransitionToNextGameView(GameViews.CourtroomView);
        if(TechnologyManager.GetInstance != null)
        {
            TechnologyManager.GetInstance.InitializePlayerTech();
        }
    }
}
