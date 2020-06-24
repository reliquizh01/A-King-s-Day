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
            if (KingdomManager.GetInstance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

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
        public void OnEnable()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.SAVE_KINGDOM_DATA, SaveData);
        }

        public void OnDisable()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SAVE_KINGDOM_DATA, SaveData);
        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
            Debug.Log("Starting Kingdom Events Manager");
            playerData = PlayerGameManager.GetInstance.playerData;

            // FOR TESTING ONLY
            // ADD ONBOARDING HERE ONCE BALANCING IS DONE
            if(PlayerGameManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                PlayerGameManager mgr = PlayerGameManager.GetInstance;
                Debug.Log("Player Queued Events : " + mgr.playerData.queuedDataEventsList.Count);
                LoadSavedData(mgr.playerData.queuedDataEventsList, mgr.playerData.curDataEvent, mgr.playerData.curDataStory);
                AllowStartEvent();
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

        public void LoadSavedData(List<EventDecisionData> prevQueuedData, EventDecisionData prevCurData, StoryArcEventsData prevStoryData)
        {
            queuedEventsList = new List<EventDecisionData>();
            Debug.Log(prevQueuedData.Count);
            queuedEventsList.AddRange(prevQueuedData);
            currentEvent = prevCurData;
            currentStory = prevStoryData;
            eventFinished = PlayerGameManager.GetInstance.playerData.eventFinished;
            savedDataLoaded = true;
        }

        public void SaveData(Parameters p = null)
        {
            PlayerGameManager.GetInstance.SaveCurDataEvent(currentEvent);
            PlayerGameManager.GetInstance.SaveQueuedData(queuedEventsList, eventFinished);
            PlayerGameManager.GetInstance.SaveCurStory(currentStory);

            SaveLoadManager.GetInstance.SaveCurrentData();
        }
        public void ProceedToNextWeek()
        {
            eventFinished = 0;
            //Debug.Log("Last Week Count: " + PlayerGameManager.GetInstance.playerData.weekCount);
            PlayerGameManager.GetInstance.playerData.weekCount += 1;
            //Debug.Log("Adding New Week, Current Count : " + PlayerGameManager.GetInstance.playerData.weekCount);

            PlayerGameManager.GetInstance.WeeklyResourceProductionUpdate();
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

            ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(0, weeklyEvents);

            if (TransitionManager.GetInstance.currentSceneManager != null)
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

            SaveData();
        }

        public void AllowStartEvent()
        {
            //Debug.Log("Allowing Event");
            allowStartEvent = true;
            GameUIManager.GetInstance.ShowBellButton();
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
                AllowStartEvent();
            }
            
            // RECEIVE EVENT AFTER GUEST REACHED POSITION
            //cardEventController.ReceiveEvent(currentEvent);
        }
        public void StartCards()
        {
            cardEventController.ReceiveEvent(currentEvent);
        }
        public void EndCards()
        {
            SpawnManager.GetInstance.PreLeaveCourt();
            eventFinished += 1;
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
                        CourtroomSceneManager.GetInstance.MakeGuardLeave(() => ResourceInformationController.GetInstance.currentPanel.weekController.UpdateEndButton(eventFinished, weeklyEvents));
                    }
                }
            }
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
                    AllowStartEvent();
                }
            }
            else
            {
                currentEvent = null;
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
