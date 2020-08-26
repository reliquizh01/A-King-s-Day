using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceUI;
using Characters;
using Managers;
using Kingdoms;
using Utilities;

namespace Maps
{
    public enum MapType
    {
        ForestOfRetsnom,
        Gates,
        MountAli,
    }
    public class TravelMapBehavior : MonoBehaviour
    {
        public BasePanelWindow myWindow;
        public KingdomUnitStorage unitStorage;
        [Header("Map Selection Mechanics")]
        public BasePanelBehavior selectionAnim;
        public List<MapInformationBehavior> mapSelectionList;
        public MapInformationBehavior currentMap;
        [Header("Current Map Mechanics")]
        public bool selectedMapShowing = false;
        public SelectedVisualMapBehavior visualSelectedMap;
        public List<MapPointInformationData> pointsConquered;
        public void Start()
        {
            myWindow.parentCloseCallback = CloseWindow;
            if(PlayerGameManager.GetInstance != null)
            {
                if (PlayerGameManager.GetInstance.campaignData.mapPointList == null)
                {
                    PlayerGameManager.GetInstance.campaignData.mapPointList = new List<MapPointInformationData>();
                }

                MapPointInformationData checkIfTheresEmptyLand = PlayerGameManager.GetInstance.campaignData.mapPointList.Find(x => x.ownedBy == TerritoryOwners.Neutral);
                if (PlayerGameManager.GetInstance.campaignData.mapPointList.Count <= 0
                  || checkIfTheresEmptyLand != null && (checkIfTheresEmptyLand.troopsStationed == null || checkIfTheresEmptyLand.troopsStationed.Count <= 0))
                {
                    for (int i = 0; i < mapSelectionList.Count; i++)
                    {
                        currentMap = mapSelectionList[i];
                        if(currentMap.myMap.myMapPoints.Find(x => x.myPointInformation.ownedBy == TerritoryOwners.Neutral && 
                        (x.myPointInformation.troopsStationed == null || x.myPointInformation.troopsStationed.Count <= 0)))
                        {
                            GenerateMapUnits();
                        }
                    }
                    SaveMapUnits();
                }
                else
                {
                    InitializeMapUnits();
                }
            }
        }
        public void OnEnable()
        {
            selectionAnim.gameObject.SetActive(true);
            selectionAnim.PlayOpenAnimation();

            for (int i = 0; i < mapSelectionList.Count; i++)
            {
                mapSelectionList[i].gameObject.SetActive(true);
                mapSelectionList[i].myBtn.interactable = true;
            }

            if (TransitionManager.GetInstance.isEngagedWithMapPoint)
            {
                MapSelected(TransitionManager.GetInstance.attackedPointInformationData.mapType);
            }
        }
        public void OnDisable()
        {
            selectedMapShowing = false;
            visualSelectedMap.gameObject.SetActive(false);
            currentMap = null;
        }

        public void InitializeMapUnits()
        {
            if (PlayerGameManager.GetInstance == null)
            {
                return;
            }



            // OBTAIN ALL MAP POINTS
            List<MapPointBehavior> mapPoints = new List<MapPointBehavior>();
            for (int i = 0; i < mapSelectionList.Count; i++)
            {
                mapPoints.AddRange(mapSelectionList[i].myMap.myMapPoints);
            }

            // INITIALIZE
            PlayerCampaignData campaignData = PlayerGameManager.GetInstance.campaignData;
            for (int i = 0; i < campaignData.mapPointList.Count; i++)
            {
                MapPointBehavior tmp = mapPoints.Find(x => x.myPointInformation.pointName == campaignData.mapPointList[i].pointName);
                if (tmp != null)
                {
                    tmp.myPointInformation = new MapPointInformationData();
                    tmp.myPointInformation = campaignData.mapPointList[i];
                    tmp.myPointInformation.mapNeighborsList = new List<MapPointInformationData>();
                }
            }

            if(TransitionManager.GetInstance != null)
            {
                if(TransitionManager.GetInstance.isEngagedWithMapPoint)
                {
                    pointsConquered.Add(TransitionManager.GetInstance.attackedPointInformationData);
                    TransitionManager.GetInstance.isEngagedWithMapPoint = false;
                }
            }
        }
        public void SaveMapUnits()
        {
            if (PlayerGameManager.GetInstance == null)
            {
                return;
            }
            if (PlayerGameManager.GetInstance.campaignData.mapPointList == null)
            {
                PlayerGameManager.GetInstance.campaignData.mapPointList = new List<MapPointInformationData>();
            }

            for (int i = 0; i < mapSelectionList.Count; i++)
            {
                for (int x = 0; x < mapSelectionList[i].myMap.myMapPoints.Count; x++)
                {
                    PlayerGameManager.GetInstance.campaignData.mapPointList.Add(mapSelectionList[i].myMap.myMapPoints[x].myPointInformation);
                }
            }

            if(SaveData.SaveLoadManager.GetInstance != null)
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
            }
        }

        public void GenerateMapUnits()
        {
            for (int i = 0; i < currentMap.myMap.myMapPoints.Count; i++)
            {
                if (currentMap.myMap.myMapPoints[i].myPointInformation.troopsStationed == null)
                {
                    currentMap.myMap.myMapPoints[i].myPointInformation.troopsStationed = new List<TroopsInformation>();
                }

                int unitCount = 10;
                if (currentMap.myMap.myMapPoints[i].myPointInformation.coinTax >= 30)
                {
                    unitCount = UnityEngine.Random.Range(25, 35);
                }
                else if(currentMap.myMap.myMapPoints[i].myPointInformation.coinTax >= 20)
                {
                    unitCount = UnityEngine.Random.Range(15, 25);
                }

                // Base this stuff depending on what the spawnable of the place is.
                currentMap.myMap.myMapPoints[i].myPointInformation.troopsStationed.AddRange(unitStorage.GenerateBasicWarband(unitCount));
            }
        }

        public void MapSelected(MapInformationBehavior thisMap)
        {

            if(!selectedMapShowing)
            {
                currentMap = thisMap;
                currentMap.myBtn.interactable = false;
                StartCoroutine(selectionAnim.WaitAnimationForAction("Map Selected", UpdateMapShown));


                if (ResourceInformationController.GetInstance != null)
                {
                    ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.bottom);
                }
            }
            else
            {
                int idx = mapSelectionList.FindIndex(x => x == currentMap);
                if(idx < mapSelectionList.Count-1)
                {
                    idx += 1;
                }
                else
                {
                    idx = 0;
                }

                currentMap = mapSelectionList[idx];
                UpdateMapShown();
            }
        }

        public void MapSelected(MapType type)
        {
            if (!selectedMapShowing)
            {
                currentMap = mapSelectionList.Find(x => x.mapType == type);
                currentMap.myBtn.interactable = false;
                StartCoroutine(selectionAnim.WaitAnimationForAction("Map Selected", UpdateMapShown));


                if (ResourceInformationController.GetInstance != null)
                {
                    ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.bottom);
                }
            }
            else
            {
                int idx = mapSelectionList.FindIndex(x => x == currentMap);
                if (idx < mapSelectionList.Count - 1)
                {
                    idx += 1;
                }
                else
                {
                    idx = 0;
                }

                currentMap = mapSelectionList[idx];
                UpdateMapShown();
            }
        }
        public void UpdateMapShown()
        {
            // Huge Square Map
            if(selectedMapShowing)
            {
                visualSelectedMap.ShowCurrentMap(currentMap.mapType, false);
            }
            else
            {
                visualSelectedMap.gameObject.SetActive(true);
                visualSelectedMap.ShowCurrentMap(currentMap.mapType, true);
                selectedMapShowing = true;
            }

            // Map Information
            for (int i = 0; i < mapSelectionList.Count; i++)
            {
                if(mapSelectionList[i] != currentMap)
                {
                    mapSelectionList[i].gameObject.SetActive(false);
                    mapSelectionList[i].myBtn.interactable = false;
                }
                else
                {
                    mapSelectionList[i].gameObject.SetActive(true);
                    if(pointsConquered != null && pointsConquered.Count > 0)
                    {
                        mapSelectionList[i].myMap.ConquerMapPoint(pointsConquered);
                    }
                    mapSelectionList[i].myBtn.interactable = true;
                }
            }
        }
        public void CloseWindow()
        {
            if(ResourceInformationController.GetInstance != null && !TransitionManager.GetInstance.isLoadingNewScene)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
            }
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);
        }
    }
}
