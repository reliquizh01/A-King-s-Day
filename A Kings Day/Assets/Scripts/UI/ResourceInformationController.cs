using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using UnityEngine.EventSystems;
using Kingdoms;
using Managers;
using KingEvents;

namespace ResourceUI
{
    public enum ResourcePanelType
    {
        overhead,
        side,
    }

    public class ResourceInformationController : MonoBehaviour
    {
        #region Singleton
        private static ResourceInformationController instance;
        public static ResourceInformationController GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (ResourceInformationController.GetInstance == null)
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


        [Header("Resource Information Handler")]
        public ResourceInformationHandler overheadPanel;
        public ResourceInformationHandler sidePanel;


        public ResourceInformationHandler currentPanel;

        public void Start()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.SHOW_RESOURCES, ShowCurrentPanel);
            EventBroadcaster.Instance.AddObserver(EventNames.HIDE_RESOURCES, HideCurrentPanel);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SHOW_RESOURCES, ShowCurrentPanel);
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.HIDE_RESOURCES, HideCurrentPanel);
        }
        public void ShowCurrentPanel(Parameters p = null)
        {
            if(currentPanel != null)
            {
                currentPanel.myPanel.PlayOpenAnimation();
            }
        }

        public void ShowCurrentPanelPotentialResourceChanges(List<ResourceReward> rewardList)
        {
            currentPanel.ShowPotentialResourceChanges(rewardList);
        }
        public void HideCurrentPanel(Parameters p = null)
        {
            if (currentPanel != null)
            {
                currentPanel.myPanel.PlayCloseAnimation();
            }
        }
        public void ShowResourcePanel(ResourcePanelType panelType)
        {
            if(currentPanel != null)
            {
                currentPanel.myPanel.PlayCloseAnimation();
            }

            switch (panelType)
            {
                case ResourcePanelType.overhead:
                    overheadPanel.gameObject.SetActive(true);
                    overheadPanel.InitializeData();
                    overheadPanel.myPanel.PlayOpenAnimation();
                    currentPanel = overheadPanel;
                    break;
                case ResourcePanelType.side:
                    sidePanel.gameObject.SetActive(true);
                    sidePanel.InitializeData();
                    sidePanel.myPanel.PlayOpenAnimation();
                    currentPanel = sidePanel;
                    break;
                default:
                    break;
            }
        }
        public void ShowWeekendPanel()
        {
            if(currentPanel == overheadPanel)
            {
                overheadPanel.ShowWeekendPanel();
            }
        }
        public void ShowTravelPanel()
        {
            if(currentPanel == overheadPanel)
            {
                overheadPanel.ShowTravelPanel();
            }
        }

    }
}