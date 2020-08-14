using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Utilities;
using Buildings;
using Maps;
using ResourceUI;

namespace Managers
{
    public class BalconyManager : BaseManager
    {
        #region Singleton
        private static BalconyManager instance;
        public static BalconyManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            instance = this;

            if (TransitionManager.GetInstance != null)
            {
                if (TransitionManager.GetInstance.managerList.Find(x => x.gameView == gameView) == null)
                {
                    Debug.Log("Adding and Setting Balcony Manager to Transition Manager");
                    TransitionManager.GetInstance.AddManager(this);
                    TransitionManager.GetInstance.SetAsCurrentManager(this.gameView);
                }
            }
            else
            {
                Debug.Log("CANT SEE TRANSITION MANAGER");
            }
        }
        #endregion

        [Header("Scene Tabs")]
        public TechnologyTabHandler techTab;
        public BaseOperationBehavior buildingTab;
        public TravellersReportController travellersTab;
        public TravelMapBehavior travelTab;
        public GameObject cover;
        public override void Start()
        {
            base.Start();

            EventBroadcaster.Instance.AddObserver(EventNames.OPEN_MAP_TAB, OpenTravelTab);
        }
        public void OnDisable()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.OPEN_MAP_TAB, OpenTravelTab);
        }
        public void ShowCover(Parameters p = null)
        {
            cover.gameObject.SetActive(true);
        }
        public void HideCover(Parameters p = null)
        {
            cover.gameObject.SetActive(false);
        }
        public void Update()
        {

        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
        }

        public override void StartManager()
        {
            base.StartManager();

            if(TransitionManager.GetInstance != null)
            {
                if(TransitionManager.GetInstance.previousScene == SceneType.Battlefield)
                {
                    if(TransitionManager.GetInstance.isEngagedWithMapPoint)
                    {
                        OpenTravelTab();
                    }
                }
            }
            ResourceInformationController.GetInstance.ShowResourcePanel(ResourceUI.ResourcePanelType.overhead);
        }

        public void OpenTechTab(BuildingType type)
        {
            if(type == BuildingType.Shop || type == BuildingType.Smithery)
            {
                return;
            }
            techTab.gameObject.SetActive(true);
            techTab.OpenTechnologyTab(type);
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
        }

        public void OpenBuildingOperationTab(BaseBuildingBehavior buildingInfomation)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
            buildingTab.gameObject.SetActive(true);
            buildingTab.OpenOperationTab(buildingInfomation);
        }

        public void OpenTravellersReporterTab(BaseTravellerBehavior thisTraveller)
        {
            if (travellersTab.isShowing)
            {
                return;
            }

            travellersTab.gameObject.SetActive(true);
            travellersTab.ShowTravellerReport(thisTraveller);
        }

        public void OpenTravelTab(Parameters p = null)
        {
            travelTab.gameObject.SetActive(true);
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
        }
    }
}
