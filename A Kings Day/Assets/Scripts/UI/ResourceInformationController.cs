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
using Dialogue;

namespace ResourceUI
{
    public enum ResourcePanelType
    {
        overhead,
        side,
        bottom,
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
                if (transform.parent == null)
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


        [Header("Resource Information Handler")]
        public ResourceInformationHandler overheadPanel;
        public ResourceInformationHandler sidePanel;
        public ResourceInformationHandler bottomPanel;

        public OverheadTutorialController overheadTutorialController;

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

        public void StartOverheadTutorial(bool fromCreationScene = false)
        {
            
            List<DialogueIndexReaction> callBacks = new List<DialogueIndexReaction>();

            DialogueIndexReaction temp1 = new DialogueIndexReaction();
            temp1.dialogueIndex = 0;
            temp1.potentialCallback = () => overheadTutorialController.ShowFoodTutorial();

            DialogueIndexReaction temp2 = new DialogueIndexReaction();
            temp2.dialogueIndex = 3;
            temp2.potentialCallback = () => overheadTutorialController.ShowTroopTutorial();

            DialogueIndexReaction temp3 = new DialogueIndexReaction();
            temp3.dialogueIndex = 4;
            temp3.potentialCallback = () => overheadTutorialController.ShowCoinTutorial();

            DialogueIndexReaction temp4 = new DialogueIndexReaction();
            temp4.dialogueIndex = 2;
            temp4.potentialCallback = () => overheadTutorialController.ShowPopulationTutorial();

            callBacks.Add(temp1);
            callBacks.Add(temp2);
            callBacks.Add(temp3);
            callBacks.Add(temp4);

            if (DialogueManager.GetInstance != null)
            {
                ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Introduce The Resources");
                
                if(fromCreationScene)
                {
                   
                }
                else
                {

                }
                ShowResourcePanel(ResourcePanelType.overhead, () => DialogueManager.GetInstance.StartConversation(tmp, PrologueScene, callBacks));
            }
        }

        public void PrologueScene()
        {
            overheadTutorialController.HideAllTutorial();
            if(KingdomManager.GetInstance != null)
            {
                KingdomManager.GetInstance.PrologueEvents();
            }
        }

        public void UpdateCurrentPanel()
        {
            currentPanel.InitializeData();
        }
        public void ShowCurrentPanel(Parameters p = null)
        {
            if(currentPanel != null && !currentPanel.isShowing)
            {
                currentPanel.gameObject.SetActive(true);
                currentPanel.myPanel.PlayOpenAnimation();
                currentPanel.isShowing = true;
            }
        }

        public void ShowCurrentPanelPotentialResourceChanges(List<ResourceReward> rewardList)
        {
            currentPanel.ShowPotentialResourceChanges(rewardList);
        }
        public void HideCurrentPanelPotentialResourceChanges()
        {
            currentPanel.HidePotentialResourceChanges();
        }
        public void HideCurrentPanel(Parameters p = null)
        {
            if (currentPanel != null && currentPanel.isShowing)
            {
                currentPanel.isShowing = false;
                currentPanel.myPanel.PlayCloseAnimation();
                currentPanel = null;
            }
        }
        public void ShowResourcePanel(ResourcePanelType panelType, Action extraCallBack = null)
        {
            if (currentPanel != null)
            {
                currentPanel.isShowing = false;
                if (currentPanel.resourcePanelType != panelType)
                {
                    StartCoroutine(currentPanel.myPanel.WaitAnimationForAction(currentPanel.myPanel.closeAnimationName, () => SwapDelay(panelType, extraCallBack)));
                }
                else
                {
                    switch (panelType)
                    {
                        case ResourcePanelType.overhead:
                            overheadPanel.gameObject.SetActive(true);
                            sidePanel.gameObject.SetActive(false);
                            overheadPanel.InitializeData();
                            currentPanel = overheadPanel;
                            break;
                        case ResourcePanelType.side:
                            sidePanel.gameObject.SetActive(true);
                            overheadPanel.gameObject.SetActive(false);
                            sidePanel.InitializeData();
                            currentPanel = sidePanel;
                            break;
                        case ResourcePanelType.bottom:
                            bottomPanel.gameObject.SetActive(true);
                            sidePanel.gameObject.SetActive(false);
                            overheadPanel.gameObject.SetActive(false);
                            bottomPanel.InitializeData();
                            currentPanel = bottomPanel;
                            break;
                        default:
                            break;
                    }
                    currentPanel.isShowing = true;
                    StartCoroutine(currentPanel.myPanel.WaitAnimationForAction(currentPanel.myPanel.openAnimationName, extraCallBack));
                }
            }
            else
            {
                switch (panelType)
                {
                    case ResourcePanelType.overhead:
                        overheadPanel.gameObject.SetActive(true);
                        overheadPanel.InitializeData();
                        currentPanel = overheadPanel;
                        currentPanel.weekController.UpdateWeekCountText();
                        break;
                    case ResourcePanelType.side:
                        sidePanel.gameObject.SetActive(true);
                        sidePanel.InitializeData();
                        currentPanel = sidePanel;
                        break;
                    case ResourcePanelType.bottom:
                        bottomPanel.gameObject.SetActive(true);
                        bottomPanel.InitializeData();
                        currentPanel = bottomPanel;
                        break;
                    default:
                        break;
                }
                currentPanel.isShowing = true;
                StartCoroutine(currentPanel.myPanel.WaitAnimationForAction(currentPanel.myPanel.openAnimationName, extraCallBack));
            }
        }

        public void SwapDelay(ResourcePanelType panelType, Action extraCallBack = null)
        {
            switch (panelType)
            {
                case ResourcePanelType.overhead:
                    overheadPanel.gameObject.SetActive(true);
                    sidePanel.gameObject.SetActive(false);
                    bottomPanel.gameObject.SetActive(false);
                    overheadPanel.InitializeData();
                    overheadPanel.myPanel.PlayOpenAnimation();
                    currentPanel = overheadPanel;
                    break;
                case ResourcePanelType.side:
                    sidePanel.gameObject.SetActive(true);
                    overheadPanel.gameObject.SetActive(false);
                    bottomPanel.gameObject.SetActive(false);
                    sidePanel.InitializeData();
                    sidePanel.myPanel.PlayOpenAnimation();
                    currentPanel = sidePanel;
                    break;
                case ResourcePanelType.bottom:
                    bottomPanel.gameObject.SetActive(true);
                    sidePanel.gameObject.SetActive(false);
                    overheadPanel.gameObject.SetActive(false);
                    bottomPanel.InitializeData();
                    bottomPanel.myPanel.PlayOpenAnimation();
                    currentPanel = bottomPanel;
                    break;
                default:
                    break;
            }

            currentPanel.isShowing = true;

            if (extraCallBack != null)
            {
                extraCallBack();
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