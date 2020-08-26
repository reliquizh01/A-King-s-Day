using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Characters;
using UnityEngine.SceneManagement;
using ResourceUI;
using Kingdoms;
using Maps;

namespace Managers
{

    [Serializable]
    public class ViewManager
    {
        public SceneType gameView;
        public BaseManager thisManager;
    }

    public class TransitionManager : MonoBehaviour
    {
        #region Singleton
        private static TransitionManager instance;
        public static TransitionManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (TransitionManager.GetInstance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
        #endregion
        public BasePanelBehavior loadingScreen;
        public GameObject tabCover;

        public List<ViewManager> managerList;
        public bool inTransition = false;

        // USE THIS TO REMOVE ONBOARDING
        public bool isNewGame = false;
        public ViewManager currentMgr;
        public OptionsController optionController;

        public BaseSceneManager currentSceneManager;
        public KingdomUnitStorage unitStorage;
        public KingdomMapDataStorage kingdomMapStorage;

        [Header("Loading Mechanics")]
        public SceneType previousScene;
        public bool isLoading = false;
        public bool isLoadingNewScene = false;
        private SceneType preLoadThisScene;

        [Header("Traveller Battle Mechanics")]
        public BaseTravellerData attackedTravellerData;
        public bool isEngagedWithTraveller;
        [Header("Map Point Battle Mechanics")]
        public MapPointInformationData attackedPointInformationData;
        public bool isEngagedWithMapPoint;
        [Header("Did Player Attacked?")]
        public bool isPlayerAttacker;
        
        public void Start()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.ENABLE_TAB_COVER, ShowTabCover);
            EventBroadcaster.Instance.AddObserver(EventNames.DISABLE_TAB_COVER, HideTabCover);
        }
        public void Update()
        {
            if (isLoadingNewScene)
            {
                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex((int)preLoadThisScene))
                {
                    InitialTransitionOnNewScenes((int)preLoadThisScene);
                    // Call Balcony Manager Here!
                    isLoading = false;
                    if (currentSceneManager.Loaded)
                    {
                        RemoveLoading();
                        isLoadingNewScene = false;
                        EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TOOLTIP_MESG);
                        if(!isNewGame)
                        {
                            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_IN_GAME_INTERACTION);
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ENABLE_TAB_COVER, ShowTabCover);
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ENABLE_TAB_COVER, ShowTabCover);
        }

        public void SetAsNewSceneManager(BaseSceneManager thisManager)
        {
            if(currentSceneManager == thisManager)
            {
                return;
            }

            currentSceneManager = thisManager;
            currentSceneManager.PreOpenManager();

            SetAsCurrentManager(thisManager.gameView, true);

            if(thisManager.gameView == SceneType.Creation)
            {
                isNewGame = true;
            }
        }
        public void ShowTabCover(Parameters p = null)
        {
            tabCover.gameObject.SetActive(true);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }
        public void HideTabCover(Parameters p = null)
        {
            tabCover.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
        public void ShowLoading(Action callback = null)
        {
            if(!loadingScreen.gameObject.activeInHierarchy)
            {
                loadingScreen.gameObject.SetActive(true);
                StartCoroutine(loadingScreen.WaitAnimationForAction(loadingScreen.openAnimationName, callback));
                isLoading = true;
            }
            else
            {
                callback();
            }
        }
        public void RemoveLoading(Action callback = null)
        {
            if(callback != null)
            {
                StartCoroutine(loadingScreen.WaitAnimationForAction(loadingScreen.closeAnimationName, callback));
            }
            else
            {
                loadingScreen.PlayCloseAnimation();
            }
            isLoading = false;
        }

        public void LoadScene(SceneType thisScene)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TOOLTIP_MESG);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
            EventBroadcaster.Instance.PostEvent(EventNames.BEFORE_LOAD_SCENE);

            previousScene = currentSceneManager.sceneType;
            preLoadThisScene = thisScene;

            ShowLoading(LoadScene);
            if(previousScene != SceneType.Opening)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.SAVE_KINGDOM_DATA);
            }

            Debug.Log("----------Loading a Scene----------");
            isLoadingNewScene = true;
        }
        public void LoadScene()
        {
            SceneManager.LoadScene((int)preLoadThisScene);
        }

        public void ShowOptions(bool directOption)
        {
            ShowTabCover();
            if(directOption)
            {
                optionController.OpenOptionPanel();
            }
            else
            {
                optionController.OpenInGameOptions();
            }
        }


        public void InitialTransitionOnNewScenes(int sceneIdx)
        {
            TransitionToNextGameView((SceneType)sceneIdx);
        }
        public void SetAsNewScene()
        {
            managerList.RemoveAll(x => x.thisManager == null);
            currentMgr = null;
            Debug.Log("Nullifying Current Manager 2");
        }

        public void AddManager(BaseManager thisManager)
        {
            ViewManager newMgr = new ViewManager();

            newMgr.thisManager = thisManager;
            newMgr.gameView = thisManager.gameView;

            managerList.Add(newMgr);
        }
        public void TransitionToNextGameView(SceneType thisView, bool isGameView = false)
        {
           // Debug.Log("Transitioning to This View: " + thisView);

            if(currentMgr != null)
            {
                if(currentMgr.thisManager != null && currentMgr.gameView != thisView)
                {
                    currentMgr.thisManager.PreCloseManager();
                    currentMgr = null;
                }
            }

            if(!isLoading && !isLoadingNewScene)
            {
                // Debug.Log("View is not Loading and new Scene!");
                ShowLoading(() => SetAsCurrentManager(thisView, isGameView));
            }
            else
            {
                //Debug.Log("View is Loading and new Scene!");
                if(currentSceneManager != null)
                {
                    if(currentSceneManager.Loaded)
                    {
                        isLoading = false;
                        RemoveLoading();
                    }
                }
            }
        }
        public void SetAsCurrentManager(SceneType thisView,bool isGameView = false)
        {
           //Debug.Log("Setting New Manager View : " + thisView);
           ViewManager thisMgr = managerList.Find(x => x.gameView == thisView && x.thisManager.isPlayManager == true);

            if(currentMgr != null && currentMgr.thisManager != null)
            {
                if (currentMgr == thisMgr) return;
                else
                {
                    currentMgr.thisManager.PreCloseManager();
                }
            }

            if (thisMgr != null && currentMgr != thisMgr)
            {
                currentMgr = thisMgr;
                currentMgr.gameView = thisMgr.gameView;
                Debug.Log("Current Mgr: " + currentMgr.thisManager.gameObject.name);
                if(currentMgr.gameView == SceneType.Creation)
                {
                    isNewGame = true;
                }
                else
                {
                    isNewGame = false;
                }

                CheckSetupResourcePanel();
                StartCurrentManager();
            }


            if (isLoading && thisView != SceneType.Creation)
            {
                // Debug.Log("Setting Current Manager!");
                isLoading = false;
            }
        }

        public void CheckSetupResourcePanel()
        {
            if (currentMgr.gameView == SceneType.Courtroom)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
                ResourceInformationController.GetInstance.ShowWeekendPanel();
            }
            else if (currentMgr.gameView == SceneType.Balcony)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
                ResourceInformationController.GetInstance.ShowTravelPanel();
            }
        }
        public void StartCurrentManager()
        {
            if(currentMgr.thisManager != null)
            {
                if(!currentMgr.thisManager.Loaded)
                {
                    currentMgr.thisManager.PreOpenManager();
                }
            }
        }

        public void FaceMapPoint(MapPointInformationData mapPointInformation, bool isAttacker = false)
        {
            attackedPointInformationData = new MapPointInformationData();
            attackedPointInformationData = mapPointInformation;

            isEngagedWithMapPoint = true;
            isPlayerAttacker = isAttacker;
            
            LoadScene(SceneType.Battlefield);
            HideTabCover();
        }
        public void FaceTravellerInBattle(BaseTravellerData thisTraveller, bool isAttacker = false)
        {
            attackedTravellerData = new BaseTravellerData();
            attackedTravellerData = thisTraveller;

            isEngagedWithTraveller = true;
            isPlayerAttacker = isAttacker;

            LoadScene(SceneType.Battlefield);
            HideTabCover();
        }
        public void TransitionToCustomBattleView()
        {
            TransitionToNextGameView(SceneType.Battlefield, true);
        }

        public void TransitionToSplashScreen()
        {
            TransitionToNextGameView(SceneType.Opening, true);
        }
    }
}
