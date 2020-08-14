using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;

namespace Maps
{
    public class MapInformationBehavior : MonoBehaviour
    {
        public Button myBtn;
        public TravelMapBehavior myController;
        public MapType mapType;
        public CurrentMapBehavior myMap;

        [Header("Map Point Mechanics")]
        public GameObject mapPoint;
        public TextMeshProUGUI pointName;
        public TextMeshProUGUI TroopCount;
        public TextMeshProUGUI CoinTax;
        public List<TextMeshProUGUI> knownForList;
        [Header("Map Point Actions")]
        public Button attackButton;
        public Button scoutButton;
        public Button sendRecruit, sendSwordsman, sendSpearman, sendArcher;

        [Header("Map Controlled Mechanics")]
        public GameObject mapControlledAction;
        public GameObject mapUncontrolledActions;
        [Header("Overall Information")]
        public TextMeshProUGUI potentialCoins;
        public TextMeshProUGUI visiblePlaces;
        public TextMeshProUGUI territoryCount;

        public void Start()
        {
            if(PlayerGameManager.GetInstance != null)
            {
                List<MapPointInformationData> mapData = PlayerGameManager.GetInstance.campaignData.mapPointList.FindAll(x => x.mapType == myMap.mapType);
                Debug.Log("Map Count:" + mapData.Count);
                for (int i = 0; i < mapData.Count; i++)
                {
                    if(myMap.myMapPoints.Find(x => x.myPointInformation.pointName == mapData[i].pointName) != null)
                    {
                        myMap.myMapPoints.Find(x => x.myPointInformation.pointName == mapData[i].pointName).LoadMapPointInformation(mapData[i]);
                    }
                }
            }
            SetupOverallInformation();
        }

        public void OnEnable()
        {
            SetupOverallInformation();
            mapPoint.SetActive(false);
        }

        public void OnDisable()
        {
            mapPoint.SetActive(false);
        }
        public void OnPointSelected()
        {
            mapPoint.SetActive(true);
            UpdateShownPointInformation(myMap.selectedMapPoint);

            if (myMap.selectedMapPoint.myPointInformation.ownedBy == TerritoryOwners.Player)
            {
                mapUncontrolledActions.SetActive(false);
                mapControlledAction.SetActive(true);
            }
            else
            {
                mapUncontrolledActions.SetActive(true);
                mapControlledAction.SetActive(false);
            }
        }
        public void SetupOverallInformation()
        {
            UpdateEarnedCoins();
            UpdateTerritoryCount();
            UpdateVisiblePlaces();
        }

        public void UpdateVisiblePlaces()
        {
            int totalVisiblePlaces = 0;
            for (int i = 0; i < myMap.myMapPoints.Count; i++)
            {
                if(myMap.myMapPoints[i].myPointInformation.visibleToPlayer)
                {
                    totalVisiblePlaces += 1;
                }
            }

            visiblePlaces.text = totalVisiblePlaces.ToString();
        }

        public void UpdateTerritoryCount()
        {
            int totalCount = myMap.myMapPoints.Count;
            int controlledCount = myMap.myMapPoints.FindAll(x => x.myPointInformation.ownedBy == TerritoryOwners.Player).Count;

            territoryCount.text = controlledCount + "/" + totalCount;
        }
        public void UpdateEarnedCoins()
        {
            int totalCoinTax = 0;
            for (int i = 0; i < myMap.myMapPoints.Count; i++)
            {
                if(myMap.myMapPoints[i].myPointInformation.ownedBy == TerritoryOwners.Player)
                {
                    totalCoinTax += myMap.myMapPoints[i].myPointInformation.coinTax;
                }
            }

            potentialCoins.text = totalCoinTax.ToString();
        }
        public void SelectThisMap()
        {
            myController.MapSelected(this);
        }

        public void UpdateShownPointInformation(MapPointBehavior thisPoint)
        {
            pointName.text = thisPoint.myPointInformation.pointName;

            if(thisPoint.myPointInformation.visibleToPlayer)
            {
                scoutButton.interactable = false;
                TroopCount.text = "Troops: "+ thisPoint.myPointInformation.ObtainTotalUnitCount().ToString();
            }
            else
            {
                scoutButton.interactable = true;
                TroopCount.text = "Troops: " + thisPoint.myPointInformation.ObtainVagueUnitCount(true).ToString() + " - "
                    + thisPoint.myPointInformation.ObtainVagueUnitCount(false).ToString();
            }

            CoinTax.text = "Coin Tax: " + thisPoint.myPointInformation.coinTax.ToString();

            for (int i = 0; i < thisPoint.myPointInformation.spawnableTravellers.Count; i++)
            {
                knownForList[i].text = thisPoint.myPointInformation.spawnableTravellers[i].ToString();
            }

            if(thisPoint.myPointInformation.ownedBy == TerritoryOwners.Player)
            {
                ShowControlledActions();
            }
            else
            {
                ShowUnControlledAction();
            }
        }

        public void ShowControlledActions()
        {
            mapControlledAction.SetActive(true);
            mapUncontrolledActions.SetActive(false);
        }

        public void ShowUnControlledAction()
        {
            mapUncontrolledActions.SetActive(true);
            mapControlledAction.SetActive(false);
        }

        public void SendScoutOnMapPoint()
        {
            if (myMap.selectedMapPoint.myPointInformation.visibleToPlayer)
                return;

            float successChance = 50;
            bool success = (UnityEngine.Random.Range(0, 100) > successChance) ? true : false;

            myMap.selectedMapPoint.myPointInformation.visibleToPlayer = success;

            if(!success)
            {
                if(PlayerGameManager.GetInstance != null)
                {
                    if(PlayerGameManager.GetInstance.playerData.GetTotalTroops > 0)
                    {
                        PlayerGameManager.GetInstance.playerData.RemoveScoutTroops();
                    }
                }
            }

            UpdateShownPointInformation(myMap.selectedMapPoint);
        }
        public void PlayerAttackMapPoint()
        {
            if(TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.FaceMapPoint(myMap.selectedMapPoint.myPointInformation, true);
                myController.myWindow.CloseWindow();
            }
        }
    }
}