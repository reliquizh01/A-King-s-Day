using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using KingEvents;
using Territory;
using Kingdoms;
using ResourceUI;
using SaveData;
using UnityEngine.UI;
using Dialogue;
using Drama;

namespace Managers
{
    /// <summary>
    /// Kingdom Manager handles when the events are called, increase in difficulty,
    /// people rebelling due to lack of loyalty and so forth.
    /// </summary>
    public class KingdomManager : BaseManager
    {
        #region Singleton
        private static KingdomManager instance;
        public static KingdomManager GetInstance
        {
            get
            {
                return instance;
            }
        }
        public void Awake()
        {
            if (KingdomManager.GetInstance != null && KingdomManager.GetInstance != this)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }
        #endregion

        public bool isPrologue = false;
        public CardsEventController cardEventController;
        // Create an Event Database so the manager can just obtain it there.
        public KingdomEventStorage eventStorage;
        public List<EventDecisionData> queuedEventsList = new List<EventDecisionData>();
        public EventDecisionData currentEvent;
        public StoryArcEventsData currentStory;
        public bool isInStory = false;


        private PlayerKingdomData playerData;
        // Number of Event to Spawn
        public int weeklyEvents = 0;
        public int eventFinished = 0;


        private bool savedDataLoaded = false;
        private bool allowStartEvent = false;
        public BasePanelBehavior eventBellBtn;

        public void OnEnable()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.SAVE_KINGDOM_DATA, SaveData);
            EventBroadcaster.Instance.AddObserver(EventNames.HIDE_RESOURCES, HideBellButton);
        }

        public void OnDisable()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SAVE_KINGDOM_DATA, SaveData);
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.HIDE_RESOURCES, HideBellButton);
        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
            Debug.Log("Starting Kingdom Events Manager");

            playerData = new PlayerKingdomData();
            playerData = PlayerGameManager.GetInstance.playerData;

            // ADD ONBOARDING HERE ONCE BALANCING IS DONE
            if(PlayerGameManager.GetInstance != null)
            {
                if(TransitionManager.GetInstance.isNewGame && !DramaticActManager.GetInstance.currentlyPlayingDrama)
                {

                }
                else if(TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Courtroom)
                {
                    PlayerGameManager mgr = PlayerGameManager.GetInstance;
                    LoadSavedData(mgr.playerData.queuedDataEventsList, mgr.playerData.curDataEvent, mgr.playerData.curDataStory);
                    // CHECKS IF LOADED DATA STILL HAS QUEUED EVENTS

                    if (mgr.playerData.eventFinished < 3)
                    {
                        if (mgr.playerData.queuedDataEventsList.Count > 0 || mgr.playerData.curDataEvent != null && !string.IsNullOrEmpty(mgr.playerData.curDataEvent.title))
                        {
                            AllowStartEvent(); // PREOPEN
                        }
                        else
                        {
                            StartWeekEvents();
                        }
                    }
                }
            }
            else
            {
                StartWeekEvents();
            }
        }
        public override void StartManager()
        {
            base.StartManager();

        }

        public void PrologueEvents()
        {
            playerData = PlayerGameManager.GetInstance.playerData;

            EventDecisionData event1 = eventStorage.GetEventByTitle("Scout of the Wilds");
            EventDecisionData event2 = eventStorage.GetEventByTitle("Angered Fur Khan");
            EventDecisionData event3 = eventStorage.GetEventByTitle("Unexpected Crusade");

            queuedEventsList.Add(event1);
            queuedEventsList.Add(event2);
            queuedEventsList.Add(event3);

            eventFinished = PlayerGameManager.GetInstance.playerData.eventFinished;
            weeklyEvents = 3;
            isPrologue = true;

            AllowStartEvent(); // PROLOGUE EVENTS

            ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(0, weeklyEvents, ShowEndWeekPrologue);
        }

        public void ShowEndWeekPrologue()
        {
            ResourceInformationController.GetInstance.overheadTutorialController.ShowWeekTutorial();

            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Prologue - Week End Guide");
            DialogueManager.GetInstance.StartConversation(tmp, ResourceInformationController.GetInstance.overheadTutorialController.HideAllTutorial);
        }
        public void EndPrologueEvents(Action endDialogueProceeding = null)
        {
            if (DialogueManager.GetInstance == null)
                return;

            if (DramaticActManager.GetInstance == null)
                return;

            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Prologue - Before the Week has passed.");
            DramaticActManager.GetInstance.FadeToDark(true,() => DialogueManager.GetInstance.StartConversation(tmp, endDialogueProceeding));
        }
        public void LoadSavedData(List<EventDecisionData> prevQueuedData, EventDecisionData prevCurData, StoryArcEventsData prevStoryData)
        {
            queuedEventsList = new List<EventDecisionData>();
            queuedEventsList.AddRange(prevQueuedData);
            currentEvent = prevCurData;
            currentStory = prevStoryData;
            eventFinished = PlayerGameManager.GetInstance.playerData.eventFinished;
            savedDataLoaded = true;

            weeklyEvents = ((int)playerData.level + 1) * 3;

            ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(eventFinished, weeklyEvents);
        }

        public void SaveData(Parameters p = null)
        {
            PlayerGameManager.GetInstance.SaveCurDataEvent(currentEvent);
            PlayerGameManager.GetInstance.SaveQueuedData(queuedEventsList, eventFinished);
            PlayerGameManager.GetInstance.SaveCurStory(currentStory);
            if(TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                SaveLoadManager.GetInstance.SaveCurrentData();
            }
        }

        public void ProceedToNextWeek()
        {
            Debug.Log("Where are you coming from!");
            eventFinished = 0;
            //Debug.Log("Last Week Count: " + PlayerGameManager.GetInstance.playerData.weekCount);
            PlayerGameManager.GetInstance.playerData.weekCount += 1;
            EventBroadcaster.Instance.PostEvent(EventNames.WEEKLY_UPDATE);
            //Debug.Log("Adding New Week, Current Count : " + PlayerGameManager.GetInstance.playerData.weekCount);
            StartWeekEvents();
        }

        public bool IsWeekEventsFinished()
        {
            if(currentEvent == null)
            {
                if(queuedEventsList == null)
                {
                    queuedEventsList = new List<EventDecisionData>();
                }

                if(queuedEventsList.Count <= 0)
                {
                    //Debug.Log("Events Count is 0");
                    return true;
                }
                else
                {
                   // Debug.Log("Events Count is "+queuedEventsList.Count);
                    return false;
                }
            }
            else
            {
                if(queuedEventsList.Count <= 0 && string.IsNullOrEmpty(currentEvent.title))
                {
                    return true;
                }
               // Debug.Log("Current Event is not null");
                return false;
            }
        }
        public void StartWeekEvents()
        {
            if (DramaticActManager.GetInstance.currentDrama != null)
            { 
                return;
            }

            weeklyEvents = ((int)playerData.level + 1) * 3;
            int eventsToAdd = weeklyEvents;
            Debug.Log("-------------- STARTING WEEK EVENTS! --------------");
            List<EventDecisionData> temp = new List<EventDecisionData>();

            // IF THERE'S an EXISTING STORY -> GET THE NEXT INDEX FIRST
            if (isInStory)
            {
                // Check if in Story - 
                if (isInStory && eventStorage.EnableDebugging)
                {
                    // Check currentStory Interval.
                    if (currentStory.curEventIdx == 0)
                    {
                        // Check if we're past starting week 
                        if (currentStory.startingWeek <= playerData.weekCount)
                        {
                            currentEvent = currentStory.storyEvents[currentStory.curEventIdx];
                            eventsToAdd -= 1;
                        }
                    }
                    else
                    {
                        if (currentStory.nextEventWeek <= playerData.weekCount)
                        {
                            currentEvent = currentStory.storyEvents[currentStory.curEventIdx];
                            eventsToAdd -= 1;
                        }
                    }
                }
                else
                {
                    // CHECK WEEK REQUIREMENT
                    int curWeek = playerData.weekCount;
                    int nextEventIdx = currentStory.curEventIdx;
                    if (curWeek >= currentStory.nextEventWeek)
                    {
                        if(eventStorage.ComputeStoryArcChance(curWeek,currentStory.storyEvents[nextEventIdx].difficultyType))
                        {
                            //Debug.Log("------------- ADDING THRU START WEEK EVENT-------------------");
                            temp.Add(currentStory.storyEvents[nextEventIdx]);
                            eventsToAdd -= 1;
                        }
                    }
                }
            }

            if(queuedEventsList.Count > eventsToAdd)
            {
                return;
            }

            if (TransitionManager.GetInstance.previousScene != SceneType.Courtroom)
            {
                // Check if there's Saved Events.
                PlayerGameManager mgr = PlayerGameManager.GetInstance;
                if (mgr.playerData.queuedDataEventsList.Count <= 0 || mgr.playerData.curDataEvent == null)
                {
                    temp = eventStorage.ObtainWeeklyEvents(playerData.level, eventsToAdd, playerData, isInStory);
                }
                else
                {
                    // if there is
                    LoadSavedData(mgr.playerData.queuedDataEventsList, mgr.playerData.curDataEvent, mgr.playerData.curDataStory);
                    savedDataLoaded = false;
                }
            }
            else
            {
                temp = eventStorage.ObtainWeeklyEvents(playerData.level, eventsToAdd, playerData, isInStory);
            }

            // Set QueueEvents
            queuedEventsList.AddRange(temp);

            if(ResourceInformationController.GetInstance.currentPanel != null)
            {
                ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(0, weeklyEvents);
            }

            if (TransitionManager.GetInstance.currentSceneManager != null)
            {
                if(CourtroomSceneManager.GetInstance != null)
                {
                    if (TransitionManager.GetInstance.currentSceneManager == CourtroomSceneManager.GetInstance)
                    {
                        CourtroomSceneManager.GetInstance.MakeGuardShow(() => AllowStartEvent());
                    }
                    else
                    {
                        Debug.Log("Item Name: " + TransitionManager.GetInstance.currentSceneManager.gameObject.name + " Manager Name: " + CourtroomSceneManager.GetInstance.gameObject.name);
                    }
                }
            }

            SaveData();
        }

        public void AllowStartEvent()
        {
            //Debug.Log("Allowing Event");
            allowStartEvent = true;
            ShowBellButton();
        }

        public void ShowBellButton()
        {
            if(!TransitionManager.GetInstance.isNewGame && TransitionManager.GetInstance.currentSceneManager.sceneType != SceneType.Courtroom)
            {
                return;
            }

            if(PlayerGameManager.GetInstance == null)
            {
                return;
            }

            if(queuedEventsList == null && string.IsNullOrEmpty(currentStory.description) &&queuedEventsList.Count <= 0)
            {
                return;
            }

            eventBellBtn.gameObject.SetActive(true);
            eventBellBtn.GetComponent<Button>().interactable = true;
            StartCoroutine(DelayBellButton());
        }

        public void HideBellButton(Parameters p = null)
        {
            if(!eventBellBtn.gameObject.activeSelf)
            {
                return;
            }

            if (!TransitionManager.GetInstance.isNewGame && TransitionManager.GetInstance.currentSceneManager.sceneType != SceneType.Courtroom)
            {
                return;
            }

            if (PlayerGameManager.GetInstance == null)
            {
                return;
            }

            if (queuedEventsList == null && queuedEventsList.Count <= 0)
            {
                if (currentStory == null)
                {
                    return;
                }
            }

            eventBellBtn.PlayCloseAnimation();
        }
        public void ActivateNextEvent()
        {
            eventBellBtn.GetComponent<Button>().interactable = false;
            eventBellBtn.PlayCloseAnimation();
            StartCoroutine(DelayStartNextEvent());
        }
        IEnumerator DelayStartNextEvent()
        {
            yield return new WaitForSeconds(1);

            StartEvent();
        }

        public IEnumerator DelayBellButton()
        {
            yield return new WaitForSeconds(1);

            eventBellBtn.PlayOpenAnimation();
        }

        public void StartEvent()
        {
            if(currentEvent == null)
            {
                currentEvent = new EventDecisionData();
            }
            if(allowStartEvent)
            {
                Debug.Log("-------------- Start Event --------------");
                if(!string.IsNullOrEmpty(currentEvent.title))
                {
                    SummonGuest();
                }
                else
                {
                    CheckNextEvent();
                }
            }
            else if(queuedEventsList != null && queuedEventsList.Count > 0)
            {
                AllowStartEvent(); // START EVENT
            }
            
            // RECEIVE EVENT AFTER GUEST REACHED POSITION
            //cardEventController.ReceiveEvent(currentEvent);
        }
        public void StartCards()
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
            
            cardEventController.ReceiveEvent(currentEvent);
            SaveData();
            
        }
        public void EndCards()
        {

            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);

            SpawnManager.GetInstance.PreLeaveCourt();

            if(eventFinished < weeklyEvents)
            {
                ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(eventFinished, weeklyEvents);
            }
            else
            {
                if(TransitionManager.GetInstance.currentSceneManager != null)
                {
                    if(TransitionManager.GetInstance.currentSceneManager == CourtroomSceneManager.GetInstance)
                    {
                        CourtroomSceneManager.GetInstance.MakeGuardLeave(EndWeekCards);
                    }
                    else
                    {
                        ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(eventFinished, weeklyEvents);
                    }
                }
            }

        }
        public void EndWeekCards()
        {
            ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(eventFinished, weeklyEvents);
        }
        public void SummonGuest()
        {
            if(string.IsNullOrEmpty(currentEvent.title))
            {
                return;
            }
            SpawnManager.GetInstance.SpawnCourtGuest(currentEvent.reporterType);

        }

        public void RewardEvent(EventDecisionData thisEvent, int rewardIdx)
        {
            if(thisEvent.eventDecision == null || thisEvent.eventDecision[rewardIdx] == null)
            {
                return;
            }
            // This is to release every Reward
            for (int i = 0; i < thisEvent.eventDecision[rewardIdx].rewards.Count; i++)
            {
               // Debug.Log("[REWARD INDEX " + i + " AMOUNT]" + thisEvent.eventDecision[rewardIdx].rewards[i].rewardAmount);
                ResourceReward tmp = thisEvent.eventDecision[rewardIdx].rewards[i];
                if (tmp.rewardAmount > 0)
                {
                   // Debug.Log("Reward Amount: " + tmp.rewardAmount);
                    PlayerGameManager.GetInstance.ReceiveResource(tmp.rewardAmount, tmp.resourceType);
                    ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(tmp.resourceType, tmp.rewardAmount);
                }
                else
                {
                   // Debug.Log("Penalty Amount: " + tmp.rewardAmount);
                    PlayerGameManager.GetInstance.RemoveResource(tmp.rewardAmount, tmp.resourceType);
                    ResourceInformationController.GetInstance.currentPanel.UpdateResourceData(tmp.resourceType, tmp.rewardAmount, false);
                }
            }


            Debug.Log("Current Event is Arc :" + currentEvent.isStoryArc);
            if(currentEvent.isStoryArc)
            {
                if(currentStory == null || string.IsNullOrEmpty(currentStory.storyTitle))
                {
                    SetStoryArc();
                }
                else
                {
                    ProgressStoryArc();
                }
            }

            Debug.Log("Rewarding Event");
            currentEvent = null;
            allowStartEvent = false;
            eventFinished += 1;

            if (PlayerGameManager.GetInstance != null)
            {
                SaveData();
            }

            EndCards();
        }
        public void ProgressStoryArc()
        {
            // HERE YOU SHOULD PLACE THE EVENT PICTURE.
            if (currentStory == null) return;


            if(currentStory.storyEvents[currentStory.curEventIdx] != currentEvent)
            {
                return;
            }
            
            // LINEAR STORY LINE
            if(currentStory.isLinear)
            {
                Debug.Log("Linear Progression");
                if(currentStory.curEventIdx < currentStory.storyEvents.Count-1)
                {
                    currentStory.curEventIdx += 1;
                    currentStory.AdjustNextEventWeek(playerData.weekCount);
                }
                else
                {
                    EndStoryArc();
                }
            }
            // BRANCHING STORYLINE
            else
            {
                if(currentEvent.arcEnd)
                {
                    EndStoryArc();
                }
                else
                {
                    int nxtArc = cardEventController.idxClicked;
                    Debug.Log("Branching Progression :" + nxtArc);
                    currentStory.curEventIdx = currentEvent.eventDecision[nxtArc].nextArcIdx;
                    currentStory.AdjustNextEventWeek(playerData.weekCount);
                }
            }
        }
        public void CheckNextEvent()
        {
            if(queuedEventsList != null && queuedEventsList.Count > 0)
            {
                currentEvent = queuedEventsList[0];
                queuedEventsList.RemoveAt(0);

                if(allowStartEvent)
                {
                    SummonGuest();
                }
                else
                {
                    AllowStartEvent(); // CHECKING NEXT EVENT
                }
            }
            else
            {
                currentEvent = null;
            }

            if(TransitionManager.GetInstance != null)
            {
                if(!TransitionManager.GetInstance.isNewGame)
                {
                    SaveData();
                }

            }
            else
            {
                SaveData();
            }
        }

        public void SetStoryArc()
        {
            if (!currentEvent.isStoryArc && isInStory) return;

            currentStory = new StoryArcEventsData();
            currentStory = eventStorage.storyArcEvents.Find(x => x.storyTitle == currentEvent.storyArc);
            isInStory = true;

            ProgressStoryArc();
        }
        public void EndStoryArc()
        {
            if (isInStory)
            {
                playerData.currentRoll = 0.007f;
            }

            if(playerData.finishedStories == null)
            {
                playerData.finishedStories = new List<StoryArcEventsData>();
            }

            // SAVE ALL ONE TIME STORIES SO THEY WONT REPEAT AGAIN.
            if(currentStory.repetitionType == StoryRepetitionType.Once)
            {
                playerData.finishedStories.Add(currentStory);
            }

            currentStory = null;
            isInStory = false;
        }
    }
}
