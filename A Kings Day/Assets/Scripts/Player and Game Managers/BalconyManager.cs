using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Utilities;
using Buildings;

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
        }
        #endregion

        [Header("Scene Tabs")]
        public TechnologyTabHandler techTab;
        public BaseOperationBehavior buildingTab;
        public GameObject cover;
        public override void Start()
        {
            base.Start();

            if (TransitionManager.GetInstance != null)
            {

                if (TransitionManager.GetInstance.managerList.Find(x => x.gameView == gameView) == null)
                {
                    TransitionManager.GetInstance.AddManager(this);
                }
            }
            else
            {
                Debug.Log("CANT SEE TRANSITION MANAGER");
            }

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
            // Testing Purposes Only!
            if(Input.GetKeyDown(KeyCode.Q))
            {
                PreOpenManager();
            }
        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();

            BalconySceneManager.GetInstance.PreOpenManager();
        }


        public void OpenTechTab(Kingdoms.ResourceType type)
        {
            if(type == Kingdoms.ResourceType.Shop || type == Kingdoms.ResourceType.Blacksmith)
            {
                return;
            }
            techTab.OpenTechnologyTab(type);
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
        }

        public void OpenBuildingOperationTab(BaseBuildingBehavior buildingInfomation)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
            buildingTab.OpenOperationTab(buildingInfomation);
        }
    }
}
