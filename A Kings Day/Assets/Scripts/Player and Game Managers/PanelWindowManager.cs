using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

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



    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(openedWindowsList != null && openedWindowsList.Count > 0)
            {
                openedWindowsList[openedWindowsList.Count - 1].CloseWindow();
                if(openedWindowsList.Count <= 0)
                {
                    TransitionManager.GetInstance.HideTabCover();
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
        if(openedWindowsList == null)
        {
            openedWindowsList = new List<BasePanelWindow>();
        }

        openedWindowsList.Add(thisWindow);

        Time.timeScale = 0.0f;
    }

    public void CloseWindow(BasePanelWindow thisWindow)
    {
        if(openedWindowsList != null && openedWindowsList.Count > 0)
        {
            openedWindowsList.Remove(thisWindow);

            if(openedWindowsList.Count <= 0)
            {
                if(TransitionManager.GetInstance != null)
                {
                    TransitionManager.GetInstance.HideTabCover();
                }
            }
        }
    }
    public void Start()
    {
        openedWindowsList = new List<BasePanelWindow>();

    }
}
