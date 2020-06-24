using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Characters;
using UnityEngine.SceneManagement;
using ResourceUI;

public enum GameViews
{
    OpeningView,
    KingdomCreationView,
    GameView,
    CourtroomView,
    EventView,
    ConversationView,
    BalconyView,
}

namespace Managers
{

    [Serializable]
    public class ViewManager
    {
        public GameViews gameView;
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
                Debug.Log(gameObject.name);
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
        public bool isNewGame = false;
        public ViewManager currentMgr;

        public BaseSceneManager currentSceneManager;
        public ResourceInformationController resourceInformationController;
        private SceneType preLoadThisScene;
        public SceneType previousScene;
        private bool isLoading = false;
        private bool isLoadingNewScene = false;
        public void Start()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.ENABLE_TAB_COVER, ShowTabCover);
            EventBroadcaster.Instance.AddObserver(EventNames.DISABLE_TAB_COVER, HideTabCover);
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

            SetAsCurrentManager(thisManager.gameView, true);
        }
        public void ShowTabCover(Parameters p = null)
        {
            tabCover.gameObject.SetActive(true);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }
        public void HideTabCover(Parameters p = null)
        {
            tabCover.gameObject.SetActive(false);
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
            StartCoroutine(loadingScreen.WaitAnimationForAction(loadingScreen.closeAnimationName, callback));

        }

        public void LoadScene(SceneType thisScene)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.SAVE_KINGDOM_DATA);
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
            previousScene = currentSceneManager.sceneType;
            preLoadThisScene = thisScene;

            ShowLoading(LoadScene);

            Debug.Log("----------Loading a Scene----------");
            isLoadingNewScene = true;
        }
        public void LoadScene()
        {
            SceneManager.LoadScene((int)preLoadThisScene);
        }

        IEnumerator DelayRemoveLoading()
        {
            yield return new WaitForSeconds(1);
            RemoveLoading(); // DELAY REMOVE
        }
        public void Update()
        {
            if(isLoadingNewScene)
            {
               if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex((int)preLoadThisScene))
                {
                    InitialTransitionOnNewScenes((int)preLoadThisScene);
                    // Call Balcony Manager Here!
                    isLoadingNewScene = false;
                    isLoading = false;
                    RemoveLoading();
                }
            }
        }

        public void InitialTransitionOnNewScenes(int sceneIdx)
        {
            switch(sceneIdx)
            {
                case 0: // Courtroom
                    TransitionToNextGameView(GameViews.CourtroomView);
                    break;
                case 1: // Balcony
                    TransitionToNextGameView(GameViews.BalconyView);
                    break;
            }
        }
        public void SetAsNewScene()
        {
            managerList.RemoveAll(x => x.thisManager == null);
            currentMgr = null;
        }

        public void AddManager(BaseManager thisManager)
        {
            ViewManager newMgr = new ViewManager();

            newMgr.thisManager = thisManager;
            newMgr.gameView = thisManager.gameView;

            managerList.Add(newMgr);
        }
        public void TransitionToNextGameView(GameViews thisView, bool isGameView = false)
        {
           // Debug.Log("Transitioning to This View: " + thisView);

            if(currentMgr != null)
            {
                if(currentMgr.thisManager != null)
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
                isLoading = false;
                RemoveLoading();
            }
        }
        public void SetAsCurrentManager(GameViews thisView,bool isGameView = false)
        {
           Debug.Log("Setting New Manager View : " + thisView);
           ViewManager thisMgr = managerList.Find(x => x.gameView == thisView);

            if(currentMgr != null)
            {
                if (currentMgr == thisMgr) return;
            }

            if (thisMgr != null)
            {
                currentMgr = thisMgr;
                Debug.Log("Current Mgr: " + currentMgr);
                if(currentMgr.gameView == GameViews.KingdomCreationView)
                {
                    isNewGame = true;
                }
                CheckSetupResourcePanel();
                StartCurrentManager();
            }


            if (isLoading && thisView != GameViews.KingdomCreationView)
            {
               // Debug.Log("Setting Current Manager!");
                RemoveLoading();
            }
            else if(thisView == GameViews.GameView)
            {
                KingdomManager.GetInstance.PreOpenManager();
            }
        }

        public void CheckSetupResourcePanel()
        {
            Debug.Log("SETTING UP RESOURCE PANEL!");
            if (currentMgr.gameView == GameViews.CourtroomView)
            {
                resourceInformationController.ShowResourcePanel(ResourcePanelType.overhead);
                resourceInformationController.ShowWeekendPanel();
            }
            else if (currentMgr.gameView == GameViews.BalconyView)
            {
                resourceInformationController.ShowResourcePanel(ResourcePanelType.overhead);
                resourceInformationController.ShowTravelPanel();
            }
        }
        public void StartCurrentManager()
        {
            if(currentMgr.thisManager != null)
            {
            //    Debug.Log("CURRENT VIEW : " + currentMgr.gameView);
                currentMgr.thisManager.PreOpenManager();
            }
        }
    }
}
