using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;
using Characters;
using Managers;

namespace Kingdoms
{

    public enum TravelLocation
    {
        ThreeGatesPilgrim,
        ForestOfRetsnom,
        MountainOfAli,
    }
    public enum TravellerType
    {
        Invader,
        Merchant,
        Hero,
        Warband,
        Creatures,
    }
    public class TravellingSystem : MonoBehaviour
    {
        [Header("Unit Storage")]
        public KingdomUnitStorage unitStorage;
        public TravellerGenerator travellerGenerator;
        [Header("Traveller Mechanics")]
        public GameObject basicInvaderPrefab;
        public GameObject basicMerchantPrefab;
        public ScenePointBehavior pilgrimSpawn, mountainSpawn, forestSpawn;
        public ScenePointBehavior frontGate;
        [Header("Scout Mechanics")]
        public TravellersReportController travellersReport;

        public List<BaseTravellerBehavior> spawnedUnits;
        public void Awake()
        {
          
        }
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                SummonRandomTraveller(TravelLocation.ForestOfRetsnom, TravellerType.Invader, 4);
            }
        }

        public void SummonSpecificTraveller(BaseTravellerData travellerData, bool newSummon = true)
        {
            ScenePointBehavior spawnPoint = TransitionManager.GetInstance.currentSceneManager.ObtainScenePoint(travellerData.currentScenePoint);
            if(spawnPoint == null)
            {
                Debug.Log("WEIRD THERE'S NO " + travellerData.currentScenePoint);
            }
            GameObject tmp = null;
            tmp = GameObject.Instantiate(basicInvaderPrefab, spawnPoint.transform.position, Quaternion.identity, null);

            BaseTravellerBehavior temp = tmp.GetComponent<BaseTravellerBehavior>();
            temp.myTravellerData = new BaseTravellerData();
            temp.myTravellerData = travellerData;

            temp.armyMovement.SetPosition(spawnPoint, true);

            //ScenePointBehavior targetPoint = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(travellerData.TargetPosition());
            //temp.TravelMovement(targetPoint);

            spawnedUnits.Add(temp);
            CheckRelationship(travellerData.relationship, true);

            if (newSummon)
            {
                if (TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
                {
                    if (PlayerGameManager.GetInstance != null)
                    {
                        PlayerGameManager.GetInstance.SaveTraveller(travellerData);
                        SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
                    }
                }
            }
        }
        public void SummonRandomTraveller(TravelLocation startingPoint, TravellerType travellerType, int unitCount)
        {
            ScenePointBehavior spawnPoint = ObtainSpawnPoint(startingPoint);
            BaseTravellerData travellerData = new BaseTravellerData();
            GameObject tmp = null;
            tmp = GameObject.Instantiate(basicInvaderPrefab, spawnPoint.transform.position, Quaternion.identity, null);


            int rand = UnityEngine.Random.Range(0, 100);
            switch (travellerType)
            {
                case TravellerType.Invader:
                    travellerData = travellerGenerator.GenerateRandomWarbandTraveller(unitCount, -100);
                    travellerData.travellersName = "Sweet Bandits" + rand.ToString(); // Create a name generator
                    break;
                case TravellerType.Merchant:
                    travellerData = travellerGenerator.GenerateRandomMerchantTraveller(unitCount, 0);
                    travellerData.travellersName = "Merchant of " + rand.ToString(); // Create a name generator
                    break;
                case TravellerType.Hero:
                    break;
                case TravellerType.Warband:
                    travellerData = travellerGenerator.GenerateRandomWarbandTraveller(unitCount, 0);
                    travellerData.travellersName = "Mercenary " + rand.ToString();
                    break;
                default:
                    break;
            }

            // Positions and Target locations
            travellerData.originalSpawnPoint = spawnPoint.gameObject.name;
            travellerData.currentScenePoint = spawnPoint.gameObject.name;
            travellerData.currentSceneTargetPoint = frontGate.gameObject.name;

            BaseTravellerBehavior inv = tmp.GetComponent<BaseTravellerBehavior>();

            inv.InitializeSpawnInvasion(travellerData);
            inv.armyMovement.SetPosition(spawnPoint, true);
            inv.TravelMovement(frontGate);
            
            if (spawnedUnits == null)
            {
                spawnedUnits = new List<BaseTravellerBehavior>();
            }

            spawnedUnits.Add(inv);
            CheckRelationship(travellerData.relationship);

            if(TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                if(PlayerGameManager.GetInstance != null)
                {
                    PlayerGameManager.GetInstance.SaveTraveller(travellerData);
                    SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
                }
            }
        }

        public void CheckRelationship(float thisRelationship, bool savedTraveller = false)
        {
            if (BalconySceneManager.GetInstance == null)
                return;

            if(savedTraveller)
            {
                if (thisRelationship < 0)
                {
                    if (!BalconySceneManager.GetInstance.wallVisualController.defenderSpawned)
                    {
                        BalconySceneManager.GetInstance.wallVisualController.PlaceWallDefenders();
                    }
                }
            }
            else
            {
                if (thisRelationship < 0)
                {
                    if (!BalconySceneManager.GetInstance.wallVisualController.defenderSpawned)
                    {
                        BalconySceneManager.GetInstance.wallVisualController.SpawnWallDefenders();
                    }
                }
            }


            if (spawnedUnits.Find(x => x.myTravellerData.relationship < 0) == null)
            {
                if (BalconySceneManager.GetInstance.wallVisualController.defenderSpawned)
                {
                    BalconySceneManager.GetInstance.wallVisualController.RetreatWallDefenders();
                }
            }
        }

        public ScenePointBehavior ObtainSpawnPoint(TravelLocation thisPoint)
        {
            ScenePointBehavior thisSpawn = null;
            switch (thisPoint)
            {
                case TravelLocation.ThreeGatesPilgrim:
                    thisSpawn = pilgrimSpawn;
                    break;
                case TravelLocation.ForestOfRetsnom:
                    thisSpawn = forestSpawn;
                    break;
                case TravelLocation.MountainOfAli:
                    thisSpawn = mountainSpawn;
                    break;
                default:
                    break;
            }

            return thisSpawn;
        }

    }
}