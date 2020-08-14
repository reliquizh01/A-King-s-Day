using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Battlefield;
using Managers;
using Kingdoms;

public class BaseTravellerBehavior : MonoBehaviour
{
    public TravellerClickDetector clickDetector;
    [Header("Traveller Data")]
    public BaseTravellerData myTravellerData;
    [Header("Visual Mechanics")]
    public SpriteRenderer spriteRend;
    public Sprite unknownTravelling;
    public Sprite unknownCamping;
    [Header("Movement Mechanics")]
    public ScenePointBehavior spawnPoint;
    public ScenePointBehavior targetPoint;
    [Header("Army Mechanics")]
    public BaseHeroInformationData leaderUnit;
    [Header("Army Information")]
    public float armySpeed;

    public ChooseUnitMindset chooseUnitMindset;
    public List<TroopsInformation> carriedTroopsList;


    [Header("Movement Mechanics")]
    public int pathIdx;
    public CharacterMovement armyMovement;
    public List<ScenePointBehavior> scenePaths;

    public void InitializeSpawnInvasion(BaseTravellerData newTravellerData)
    {
        myTravellerData = new BaseTravellerData();
        myTravellerData = newTravellerData;
        myTravellerData.currentScenePoint = newTravellerData.currentScenePoint;
        myTravellerData.currentSceneTargetPoint = newTravellerData.currentSceneTargetPoint;
        myTravellerData.originalSpawnPoint = newTravellerData.originalSpawnPoint;

        carriedTroopsList = myTravellerData.troopsCarried;
        armySpeed = myTravellerData.travellerSpeed;
        leaderUnit = myTravellerData.leaderUnit;

        if(!string.IsNullOrEmpty(myTravellerData.currentScenePoint))
        {
            spawnPoint = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(newTravellerData.currentScenePoint);
            armyMovement.SetPosition(spawnPoint, true);
        }

        if(!string.IsNullOrEmpty(myTravellerData.currentSceneTargetPoint))
        {
            targetPoint = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(newTravellerData.currentSceneTargetPoint);
            TravelMovement(targetPoint);
        }

        myTravellerData.originalSpawnPoint = spawnPoint.gameObject.name;
        myTravellerData.currentScenePoint = spawnPoint.gameObject.name;

        if (PlayerGameManager.GetInstance != null)
        {
            myTravellerData.weekSpawned = PlayerGameManager.GetInstance.playerData.weekCount;
            PlayerGameManager.GetInstance.SaveTraveller(myTravellerData);
        }
    }

    public void TravelMovement(ScenePointBehavior newTargetPoint)
    {
        spawnPoint = armyMovement.currentPoint;
        targetPoint = newTargetPoint; // Final Position
        myTravellerData.currentSceneTargetPoint = newTargetPoint.gameObject.name;

        armyMovement.SetTarget(targetPoint, SetAsCamping);
        SetAsTravelling();

        if (armyMovement.pathToTargetPoint != null && armyMovement.pathToTargetPoint.Count > 0)
        {
            scenePaths = new List<ScenePointBehavior>();
            for (int i = 0; i < armyMovement.pathToTargetPoint.Count; i++)
            {
                ScenePointBehavior tmp = new ScenePointBehavior();
                tmp = armyMovement.pathToTargetPoint[i];
                scenePaths.Add(tmp);
            }
        }

        if(scenePaths != null && scenePaths.Count > 0)
        {
            Debug.Log("Scene Paths are not empty, moving traveller to: " + scenePaths[0].gameObject.name);
            armyMovement.SetTarget(scenePaths[0], SaveCurrentPosition);
        }

        spriteRend.sortingOrder = spawnPoint.visualLayoutOrder;
    }

    public void SaveCurrentPosition()
    {
        Debug.Log("Saving Position For : " + myTravellerData.travellersName);
        myTravellerData.currentScenePoint = armyMovement.currentPoint.gameObject.name;

        if (SaveData.SaveLoadManager.GetInstance != null)
        {
            Debug.Log("Saving Traveller");
            SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
        }
    }
    public void SetAsCamping()
    {
        // Later on upon having better sprites, make unique camps for different travellers.
        // Check if the traveller is scouted successfully first.
        if(pathIdx > 1)
        {
            spriteRend.sprite = unknownCamping;
        }
    }

    public void SetAsTravelling()
    {
        spriteRend.sprite = unknownTravelling;
    }


    public void UpdateVisuals()
    {
        if(scenePaths[pathIdx].hasInteractionOptions)
        {
            spriteRend.sortingOrder = scenePaths[pathIdx].visualLayoutOrder;
        }
    }

    public void OnClick()
    {
        BalconyManager.GetInstance.OpenTravellersReporterTab(this);
    }
}
