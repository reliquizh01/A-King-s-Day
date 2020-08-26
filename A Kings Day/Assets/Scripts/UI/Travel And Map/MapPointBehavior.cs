using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kingdoms;
using Managers;

namespace Maps
{

    public class MapPointBehavior : MonoBehaviour
    {
        public GameObject arrowClickedIcon;
        public bool isClicked;
        public BasePanelBehavior myAnim;


        [Header("Point Mechanics")]
        public bool pointTowardsKingdom;
        public CurrentMapBehavior myController;
        public MapPointInformationData myPointInformation;

        [Header("Icon Mechancis")]
        public Image flagIcon;
        public Sprite playerSprite, khanSprite, holySeeSprite;
        public Sprite neutralSprite;
        [Header("Path System")]
        public List<MapPointBehavior> neighborPoints;

        public void Start()
        {

        }
        public void SetAsClicked()
        {
            if(myController.selectedMapPoint == this)
            return;

            arrowClickedIcon.SetActive(true);
            myController.SetAsSelectedPoint(this);
        }

        public void RemoveClicked()
        {
            arrowClickedIcon.SetActive(false);
        }

        public void OnDisable()
        {
            arrowClickedIcon.SetActive(false);
        }
        public void OnEnable()
        {
            myAnim.myAnim.Play("Show Map Animation");
        }
        public void LoadMapPointInformation(MapPointInformationData mapPointInformationData)
        {
            myPointInformation = new MapPointInformationData();
            myPointInformation = mapPointInformationData;
            myPointInformation.mapType = myController.mapType;
            UpdateFlagCrest();

            if(myPointInformation.travellersOnPoint != null && myPointInformation.travellersOnPoint.Count > 0 && !pointTowardsKingdom)
            {
                RandomizeTravellerMovement();
            }
            else if(pointTowardsKingdom && myPointInformation.travellersOnPoint != null && myPointInformation.travellersOnPoint.Count > 0)
            {
                if(myPointInformation.latestWeekUpdated != PlayerGameManager.GetInstance.playerData.weekCount)
                {
                    SpawnThisTravelerUnitToVisibleMap(myPointInformation.travellersOnPoint[0], myPointInformation.mapType);
                    myPointInformation.travellersOnPoint.RemoveAt(0);
                }
            }

            if(myPointInformation.travellersOnPoint.Find(x => x.numberOfMovesNextWeek > 0) != null)
            {
                StartCoroutine(DelayWeekTravel());
            }

            myPointInformation.latestWeekUpdated = PlayerGameManager.GetInstance.playerData.weekCount;
        }
        public void SpawnThisTravelerUnitToVisibleMap(BaseTravellerData thisTraveller, MapType startingLocation)
        {
            switch (startingLocation)
            {
                case MapType.ForestOfRetsnom:
                    thisTraveller.currentScenePoint = BalconySceneManager.GetInstance.travelSystem.forestSpawn.gameObject.name;
                    break;
                case MapType.Gates:
                    thisTraveller.currentScenePoint = BalconySceneManager.GetInstance.travelSystem.pilgrimSpawn.gameObject.name;
                    break;
                case MapType.MountAli:
                    thisTraveller.currentScenePoint = BalconySceneManager.GetInstance.travelSystem.mountainSpawn.gameObject.name;
                    break;
                default:
                    break;
            }

            BalconySceneManager.GetInstance.travelSystem.SummonSpecificTraveller(thisTraveller, true);
        }
        IEnumerator DelayWeekTravel()
        {
            yield return 0;

            RandomizeTravellerMovement();

            if (myPointInformation.travellersOnPoint.Find(x => x.numberOfMovesNextWeek > 0) != null)
            {
                StartCoroutine(DelayWeekTravel());
            }
        }
        public void RandomizeTravellerMovement()
        {
            if (myPointInformation.travellersOnPoint == null && myPointInformation.travellersOnPoint.Count <= 0)
            {
                return;
            }

            List<BaseTravellerData> removedTravellers = new List<BaseTravellerData>();
            for (int i = 0; i < myPointInformation.travellersOnPoint.Count; i++)
            {
                if(myPointInformation.travellersOnPoint[i].isGoingToMoveNextWeek)
                {
                    int moveChance = UnityEngine.Random.Range(0, 100);
                    if (moveChance > 50)
                    {
                        myPointInformation.travellersOnPoint[i].numberOfMovesNextWeek -= 1;
                        MoveThisTravellerToNeighbor(myPointInformation.travellersOnPoint[i]);
                        removedTravellers.Add(myPointInformation.travellersOnPoint[i]);
                    }
                }
            }

            if (myPointInformation.travellersOnPoint != null && removedTravellers.Count > 0)
            {
                 myPointInformation.travellersOnPoint.RemoveAll(x => removedTravellers.Contains(x));
            }
        }

        public void MoveThisTravellerToNeighbor(BaseTravellerData thisTraveller)
        {
            int moveToIdx = 0;
            if (neighborPoints != null && neighborPoints.Count > 0)
            {
                moveToIdx = UnityEngine.Random.Range(0, neighborPoints.Count - 1);
            }

            neighborPoints[moveToIdx].ReceiveTraveller(thisTraveller);
        }

        public void ReceiveTraveller(BaseTravellerData thisTraveller)
        {
            BaseTravellerData temp = new BaseTravellerData();
            temp = thisTraveller;

            myPointInformation.travellersOnPoint.Add(temp);
        }

        public void ConqueredBy(TerritoryOwners newOwner)
        {
            Debug.Log("New Owner of : " + myPointInformation.pointName + " is : " + newOwner.ToString());
            myPointInformation.ownedBy = newOwner;
            SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();

            StartCoroutine(myAnim.WaitAnimationForAction("Map Point Conquered Animation", UpdateFlagCrest));

        }

        public void UpdateFlagCrest()
        {
            switch (myPointInformation.ownedBy)
            {
                case TerritoryOwners.Neutral:
                    flagIcon.sprite = neutralSprite;
                    break;
                case TerritoryOwners.HolySee:
                    flagIcon.sprite = holySeeSprite;
                    break;
                case TerritoryOwners.FurKhan:
                    flagIcon.sprite = khanSprite;
                    break;
                case TerritoryOwners.Gates:

                    break;
                case TerritoryOwners.Player:
                    flagIcon.sprite = playerSprite;
                    break;
                default:
                    break;
            }
        }
    }
}