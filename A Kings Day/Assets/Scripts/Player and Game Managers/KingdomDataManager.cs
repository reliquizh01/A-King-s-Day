using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using KingEvents;
using Territory;
using Kingdoms;


namespace Managers
{
    public class KingdomDataManager : BaseManager
    {
        #region Singleton
        private static KingdomDataManager instance;
        public static KingdomDataManager GetInstance
        {
            get
            {
                return instance;
            }
        }
        public void Awake()
        {
            if (KingdomDataManager.GetInstance == null)
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


        public List<EventDecisionData> queuedDataEventsList = new List<EventDecisionData>();
        public EventDecisionData curDataEvent;
        public StoryArcEventsData curDataStory;
        public int eventFinished = 0;

        // CALL THIS WHEN PLAYER WANTS TO CONTINUE THE GAME
        public void LoadGameData(PlayerKingdomData thisData)
        {

        }

        public void SaveQueuedData(List<EventDecisionData> queuedDataList, int finishCount)
        {
            Debug.Log("SAVING SHIT!");
            queuedDataEventsList = queuedDataList;
            eventFinished = finishCount;
        }

        public void SaveCurDataEvent(EventDecisionData eventData)
        {
            curDataEvent = eventData;
        }

        public void SaveCurStory(StoryArcEventsData thisStory)
        {
            curDataStory = thisStory;
        }

        public void ClearSavedData()
        {
            Debug.Log("CLEARING DATA!");
            queuedDataEventsList.Clear();
            curDataEvent = null;
            curDataStory = null;
        }
    }
}
