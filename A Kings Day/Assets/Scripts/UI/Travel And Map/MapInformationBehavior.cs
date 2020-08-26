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
        public MapPointBehavior currentMapPoint;
        public GameObject mapPoint;
        public TextMeshProUGUI pointName;
        public TextMeshProUGUI TroopCount;
        public TextMeshProUGUI CoinTax;
        public List<TextMeshProUGUI> knownForList;
        public GameObject closeEyes, openEyes;
        [Header("Map Point Actions")]
        public ChooseLeadHeroHandler leaderHeroHandler;
        public SendTroopsSubOptionPage sendTroopsPage;
        public Button attackButton;
        public Button scoutButton;

        [Header("Map Controlled Mechanics")]
        public GameObject mapControlledAction;
        [Header("Overall Information")]
        public BasePanelBehavior overallPanel;
        public TextMeshProUGUI potentialCoins;
        public TextMeshProUGUI visiblePlaces;
        public TextMeshProUGUI territoryCount;
        public bool overallInfoShown = false;

        public void Start()
        {
            if(PlayerGameManager.GetInstance != null)
            {
                List<MapPointInformationData> mapData = PlayerGameManager.GetInstance.campaignData.mapPointList.FindAll(x => x.mapType == myMap.mapType);
                Debug.Log("Map Count:" + mapData.Count);
                if(mapData != null && mapData.Count > 0)
                {
                    for (int i = 0; i < mapData.Count; i++)
                    {
                        if(myMap.myMapPoints.Find(x => x.myPointInformation.pointName == mapData[i].pointName) != null)
                        {
                            myMap.myMapPoints.Find(x => x.myPointInformation.pointName == mapData[i].pointName).LoadMapPointInformation(mapData[i]);
                        }
                    }
                }
            }
            if(PlayerGameManager.GetInstance != null)
            {
                leaderHeroHandler.SetupAvailableHeroes(PlayerGameManager.GetInstance.playerData.myHeroes);
            }

            SetupOverallInformation();
        }

        public void OnEnable()
        {
            SetupOverallInformation();
            mapPoint.SetActive(false);
            overallPanel.PlayOpenAnimation();
            overallInfoShown = true;
            
        }

        public void OnDisable()
        {
            mapPoint.SetActive(false);
            if (myMap.selectedMapPoint != null)
            {
                myMap.selectedMapPoint.RemoveClicked();
                myMap.selectedMapPoint = null;
            }

        }
        public void OnPointSelected()
        {
            mapPoint.SetActive(true);
            UpdateShownPointInformation(myMap.selectedMapPoint);
            if(overallInfoShown)
            {
                overallPanel.PlayCloseAnimation();
                overallInfoShown = false;
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

            territoryCount.text = controlledCount + "/" + (totalCount-1);
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
            currentMapPoint = thisPoint;

            if (thisPoint.myPointInformation.visibleToPlayer || thisPoint.myPointInformation.ownedBy == TerritoryOwners.Player)
            {
                scoutButton.interactable = false;
                SwitchScoutEyes(true);
                thisPoint.myPointInformation.visibleToPlayer = true;
                TroopCount.text = "Troops: "+ thisPoint.myPointInformation.ObtainTotalUnitCount().ToString();
            }
            else
            {
                SwitchScoutEyes(false);
                scoutButton.interactable = true;
                TroopCount.text = "Troops: " + thisPoint.myPointInformation.ObtainVagueUnitCount(true).ToString() + " - "
                    + thisPoint.myPointInformation.ObtainVagueUnitCount(false).ToString();
            }

            CoinTax.text = "Coin Tax: " + thisPoint.myPointInformation.coinTax.ToString();

            for (int i = 0; i < thisPoint.myPointInformation.spawnableTravellers.Count; i++)
            {
                knownForList[i].text = thisPoint.myPointInformation.spawnableTravellers[i].ToString();
            }
        }

        public void SwitchScoutEyes(bool switchTo)
        {
            if (switchTo)
            {
                openEyes.SetActive(true);
                closeEyes.SetActive(false);
            }
            else
            {
                closeEyes.SetActive(true);
                openEyes.SetActive(false);
            }
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
            else
            {
                UpdateShownPointInformation(myMap.selectedMapPoint);
            }

            if (SaveData.SaveLoadManager.GetInstance != null)
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
            }
        }

        public void ShowSendTroopsWindow()
        {
            sendTroopsPage.gameObject.SetActive(true);
            StartCoroutine(sendTroopsPage.myPanel.WaitAnimationForAction(sendTroopsPage.myPanel.openAnimationName, () => sendTroopsPage.SetUnitList(PlayerGameManager.GetInstance.playerData.troopsList)));
        }
        public void HideTroopsWindow()
        {
            if(!this.gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine(sendTroopsPage.myPanel.WaitAnimationForAction(sendTroopsPage.myPanel.closeAnimationName, () => sendTroopsPage.gameObject.SetActive(false)));            
        }
        public void SendTroopsToBattle()
        {
            if(myMap.selectedMapPoint.myPointInformation.ownedBy == TerritoryOwners.Player)
            {
                // REINFORCE LOCATION
                if (!myMap.selectedMapPoint.myPointInformation.isBeingAttacked)
                {
                    List<TroopsInformation> tmp = new List<TroopsInformation>();
                    tmp.AddRange(myMap.selectedMapPoint.myPointInformation.troopsStationed);

                    if (PlayerGameManager.GetInstance != null)
                    {
                        List<TroopsInformation> tmpUnitsSent = PlayerGameManager.GetInstance.unitsToSend.troopsCarried;
                        for (int i = 0; i < tmpUnitsSent.Count; i++)
                        {
                            TroopsInformation tempTroop = tmp.Find(x => x.unitInformation.unitName == tmpUnitsSent[i].unitInformation.unitName);
                            if (tempTroop != null)
                            {
                                tempTroop.totalUnitCount += tmpUnitsSent[i].totalUnitCount;
                            }
                            // New Type of Troop Sent
                            else
                            {
                                tempTroop = new TroopsInformation();
                                tempTroop = tmpUnitsSent[i];

                                tmp.Add(tempTroop);
                            }
                        }
                    }

                    myMap.selectedMapPoint.myPointInformation.troopsStationed = new List<TroopsInformation>();
                    myMap.selectedMapPoint.myPointInformation.troopsStationed = tmp;
                }
                else
                {
                    // SEND TROOPS TO BATTLE WITH THE SAID UNITS TO BE ADDED TO THE MAPS CURRENT STATIONED UNITS.
                    if (TransitionManager.GetInstance != null)
                    {
                        TransitionManager.GetInstance.FaceMapPoint(myMap.selectedMapPoint.myPointInformation, false);
                        myController.myWindow.CloseWindow();
                    }
                }
            }
            // JUST ATTACK WITH THE SAID UNITS THE ONLY UNITS ON PLAYER MANAGER
            else if(myMap.selectedMapPoint.myPointInformation.ownedBy != TerritoryOwners.Player)
            {
                if (TransitionManager.GetInstance != null)
                {
                    TransitionManager.GetInstance.FaceMapPoint(myMap.selectedMapPoint.myPointInformation, true);
                    myController.myWindow.CloseWindow();
                }
            }
        }
    }
}