using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;

public class PanelWindowManager : MonoBehaviour
{
    #region Singleton
    private static PanelWindowManager instance;
    public static PanelWindowManager GetInstance
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        if (PanelWindowManager.GetInstance == null)
        {
            if(transform.parent == null)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }
    #endregion


    public List<BasePanelWindow> openedWindowsList;
    public Transform warningCanvasParent; 
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(openedWindowsList != null && openedWindowsList.Count > 0)
            {
                if(openedWindowsList.Count > 0)
                {
                    if(openedWindowsList[openedWindowsList.Count - 1] == null)
                    {
                        openedWindowsList.RemoveAt(openedWindowsList.Count - 1);
                    }
                    openedWindowsList[openedWindowsList.Count - 1].CloseWindow();
                    if(openedWindowsList.Count <= 0)
                    {
                        TransitionManager.GetInstance.HideTabCover();
                    }
                }
            }
            else if (TransitionManager.GetInstance.currentSceneManager.sceneType != SceneType.Opening)
            {
                TransitionManager.GetInstance.ShowOptions(false);
            }
        }
    }

    public void AddWindow(BasePanelWindow thisWindow)
    {
        Debug.Log("Adding A Window Cuz Why Not : " + thisWindow.gameObject.name);

        if(openedWindowsList == null)
        {
            openedWindowsList = new List<BasePanelWindow>();
        }
        if(thisWindow.transferEnabled)
        {
            thisWindow.transform.parent = warningCanvasParent;
        }
        openedWindowsList.Add(thisWindow);
        EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_IN_GAME_INTERACTION);

        if (TransitionManager.GetInstance != null)
        {
            if(TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Battlefield
                && TransitionManager.GetInstance.previousScene != SceneType.Battlefield)
            {
                Debug.Log("------------------------------------ STOPPING TIME -----------------------");
                Time.timeScale = 0.0f;
            }
            TransitionManager.GetInstance.ShowTabCover();
        }
    }

    public void CloseWindow(BasePanelWindow thisWindow)
    {
        if(openedWindowsList != null && openedWindowsList.Count > 0)
        {
            int idx = -1;
            idx = openedWindowsList.FindIndex(x => x == thisWindow);
            if(idx != -1)
            {
                openedWindowsList.RemoveAt(idx);
            }

            if (thisWindow.transferEnabled)
            {
                thisWindow.transform.parent = thisWindow.origParent;
            }
            if (openedWindowsList.Count <= 0)
            {
                if(TransitionManager.GetInstance != null)
                {
                    TransitionManager.GetInstance.HideTabCover();
                }
            }
        }
        EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_IN_GAME_INTERACTION);
    }
    public void Start()
    {
        openedWindowsList = new List<BasePanelWindow>();

    }
}
