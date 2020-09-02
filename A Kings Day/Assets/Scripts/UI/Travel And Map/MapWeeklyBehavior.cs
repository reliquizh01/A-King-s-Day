using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;
using Maps;

public class MapWeeklyBehavior : MonoBehaviour
{
    public TravellerGenerator travellerGenerator;
    [Header("Character Information")]
    public PlayerCampaignData playerCampaignData;
    public PlayerKingdomData playerKingdomData;
    [Header("Traveller Chances")]
    public float invaderFlatChance = 20;
    public float merchantFlatChance = 10;
    public float heroFlatChance = 10;
    public float mercenaryFlatChance = 10;
    public float creaturesFlatChance = 0;
    [Header("Test Mode")]
    public bool TestMode = false;
    public string ForceVisitTravellersOnPoint;

    public void SetupMapBehavior()
    {
        if (PlayerGameManager.GetInstance != null)
        {
            playerCampaignData = PlayerGameManager.GetInstance.campaignData;
            playerKingdomData = PlayerGameManager.GetInstance.playerData;
        }
    }

    public void UpdateWeeklyProgress()
    {
        if(playerCampaignData.mapPointList == null && playerCampaignData.mapPointList.Count <= 0)
        {
            return;
        }

        if(TransitionManager.GetInstance.isNewGame)
        {
            return;
        }

        // Make The Travellers move from one place to another
        MakeAllTravellersMove();

        // Delay Generation of Travellers
        StartCoroutine(DelayTravellerGeneration(0));

        UpdateConqueredTaxFees();
    }

    public void UpdateConqueredTaxFees()
    {
        int totalTaxes = playerCampaignData.totalWeeklyTax;

        if(totalTaxes == 0)
        {
            return;
        }

        PlayerGameManager.GetInstance.ReceiveResource(totalTaxes, ResourceType.Coin);
        ProductionManager.GetInstance.ShowCoinNotif(totalTaxes, "Conquered Tax");
        ResourceUI.ResourceInformationController.GetInstance.UpdateCurrentPanel();
    }
    IEnumerator DelayTravellerGeneration(int idx)
    {
        yield return 0;

        if (!playerCampaignData.mapPointList[idx].isKingdomPoint)
        {
            for (int x = 0; x < playerCampaignData.mapPointList[idx].spawnableTravellers.Count; x++)
            {
                bool spawnNewTraveller = false;
                BaseTravellerData newTraveller = new BaseTravellerData();
                if (playerCampaignData.mapPointList[idx].travellersOnPoint == null)
                {
                    playerCampaignData.mapPointList[idx].travellersOnPoint = new List<BaseTravellerData>();
                }
                switch (playerCampaignData.mapPointList[idx].spawnableTravellers[x])
                {
                    case TravellerType.Invader:
                        spawnNewTraveller = ShouldSpawnThisUnit(playerKingdomData.ObtainChances(PotentialTravellers.mercenaries) + invaderFlatChance);
                        if (spawnNewTraveller)
                        {
                            newTraveller = travellerGenerator.GenerateRandomWarbandTraveller(20, -100);
                        }
                        break;
                    case TravellerType.Merchant:
                        spawnNewTraveller = ShouldSpawnThisUnit(playerKingdomData.ObtainChances(PotentialTravellers.randomMerchant) + merchantFlatChance);
                        if (spawnNewTraveller)
                        {
                            newTraveller = travellerGenerator.GenerateRandomMerchantTraveller(20, 0);
                        }
                        break;
                    case TravellerType.Hero:
                        spawnNewTraveller = ShouldSpawnThisUnit(playerKingdomData.ObtainChances(PotentialTravellers.RandomHero) + heroFlatChance);
                        // FIX HERO UNITS FIRST
                        break;
                    case TravellerType.Warband:
                        spawnNewTraveller = ShouldSpawnThisUnit(playerKingdomData.ObtainChances(PotentialTravellers.mercenaries) + mercenaryFlatChance);
                        if (spawnNewTraveller)
                        {
                            newTraveller = travellerGenerator.GenerateRandomWarbandTraveller(20, 0);
                        }
                        break;
                    case TravellerType.Creatures:
                        spawnNewTraveller = ShouldSpawnThisUnit(creaturesFlatChance);
                        break;
                    default:
                        break;
                }

                if (spawnNewTraveller)
                {
                    newTraveller.affiliatedTeam = playerCampaignData.mapPointList[idx].ownedBy;
                    newTraveller.originalSpawnPoint = playerCampaignData.mapPointList[idx].pointName;
                    playerCampaignData.mapPointList[idx].travellersOnPoint.Add(newTraveller);
                    Debug.Log("Spawned New Traveller in Area : " + playerCampaignData.mapPointList[idx].pointName + " Spawned this Kind : " + playerCampaignData.mapPointList[idx].spawnableTravellers[x]);
                }
            }
        }

        if(idx < playerCampaignData.mapPointList.Count-1)
        {
            idx += 1;
            StartCoroutine(DelayTravellerGeneration(idx));
        }
        else
        {
            SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
        }
    }

    public void MakeAllTravellersMove()
    {
        for (int i = 0; i < playerCampaignData.mapPointList.Count; i++)
        {
            if(playerCampaignData.mapPointList[i].travellersOnPoint != null && 
                playerCampaignData.mapPointList[i].travellersOnPoint.Count > 0)
            {
                for (int x = 0; x < playerCampaignData.mapPointList[i].travellersOnPoint.Count; x++)
                {
                    if (!playerCampaignData.mapPointList[i].travellersOnPoint[x].playerSentTraveller
                        && !playerCampaignData.mapPointList[i].travellersOnPoint[x].isGoingToMoveNextWeek)
                    {
                        float rand = UnityEngine.Random.Range(0, 10);
                        if(rand < 5)
                        {
                            playerCampaignData.mapPointList[i].travellersOnPoint[x].isGoingToMoveNextWeek = true;
                            playerCampaignData.mapPointList[i].travellersOnPoint[x].numberOfMovesNextWeek += 1;
                        }
                    }
                }
            }
        }
    }


    public bool ShouldSpawnThisUnit(float chanceToSpawn)
    {
        float spawnChance = UnityEngine.Random.Range(0, 100);

        if(spawnChance <= chanceToSpawn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
