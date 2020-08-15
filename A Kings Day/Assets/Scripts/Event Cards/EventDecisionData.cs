using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using UnityEngine.UI;
using Territory;

namespace KingEvents
{
    public enum EventTriggers
    {
        
    }
    public enum DifficultyType
    {
        common,
        troublesome,
        threatening,
    }

    public enum StoryRepetitionType
    {
        Once,
        Repeatable,
    }
    [System.Serializable]
    public class ResourceReward
    {
        public string resourceTitle;
        public ResourceType resourceType;
        public string unitName;
        public int rewardAmount;
        public PotentialTravellers potentialTraveller;
        public bool multiplied = false;
    }
    [System.Serializable]
    public class EventDecision
    {
        [TextArea(2, 5)]
        public string optionDescription;
        // Here you can place additional rewards, like enabling game
        public List<ResourceReward> rewards;

        [Header("Story Arc Information")]
        [ShowOnly] public int nextArcIdx = 0;
    }


    [System.Serializable]
    public class EventDecisionData
    {
        [ShowOnly] public string title;
        [ShowOnly] public CardType eventType;
        [ShowOnly] public ReporterType reporterType;
        [ShowOnly] public TerritoryLevel levelRequired;
        [ShowOnly] public DifficultyType difficultyType;
        [ShowOnly] public string description;
        [Space(15)]
        [ShowOnly] public bool isStoryArc;
        [ShowOnly] public string storyArc;
        [ShowOnly] public bool arcEnd = false;
        public List<EventDecision> eventDecision;

        public void SetMyStoryArc(StoryArcEventsData thisArc)
        {
            if(storyArc == thisArc.storyTitle)
            {
                return;
            }
            storyArc = thisArc.storyTitle;
        }
    }
    [System.Serializable]
    public class StoryArcEventsData
    {
        [ShowOnly] public int startingWeek;
        [ShowOnly] public StoryRepetitionType repetitionType;
        [ShowOnly] public string storyTitle;
        [ShowOnly] public string description;
        [Space(20)]

        // INDEX BASED INFORMATIONS
        [ShowOnly] public bool isLinear = false;
        [ShowOnly] public int curEventIdx;
        [ShowOnly] public List<string> storyEventTitlesList;
        [HideInInspector] public List<EventDecisionData> storyEvents;
         public List<int> eventIntervals = new List<int>();
         public List<float> eventBoosts = new List<float>();
        [ShowOnly] public int nextEventWeek;
        public void AddEventDecision(EventDecisionData data)
        {

            if (eventIntervals == null)
            {
                eventIntervals = new List<int>();
            }
            if(eventBoosts == null)
            {
                eventBoosts = new List<float>();
            }
            if (storyEvents == null)
            {
                storyEvents = new List<EventDecisionData>();
            }
            if(storyEvents.Find(x => x.title == data.title) != null)
            {
                Debug.Log("We detected story inside!");
                Debug.Log(storyEvents.Count);
                return;
            }
            Debug.Log("Entered!");
            storyEvents.Add(data);
            eventIntervals.Add(0);
            eventBoosts.Add(0.5f);
            if (storyEventTitlesList == null)
            {
                storyEventTitlesList = new List<string>();
            }
            Debug.Log("Adding New Event:" + data.title);
            string tmp = data.title;
            storyEventTitlesList.Add(tmp);
        }

        public void RemoveEventDecision(EventDecisionData data)
        {
            Debug.Log("Entering Remove Phase!");
            if(storyEvents == null)
            {
                storyEvents = new List<EventDecisionData>();
            }

            Debug.Log("Attempting Remove!");
            if(storyEvents.Find(x => x.title == data.title) != null)
            {
                Debug.Log("Meow");
                int idx = storyEvents.FindIndex(x => x.title == data.title);
                Debug.Log("Removing At:" + idx);
                storyEvents.RemoveAt(idx);
                eventBoosts.RemoveAt(idx);

                if(eventIntervals == null)
                {
                    eventIntervals = new List<int>();
                }

                eventIntervals.RemoveAt(idx);
                if (storyEventTitlesList == null)
                {
                    storyEventTitlesList = new List<string>();
                }
                storyEventTitlesList.RemoveAt(idx);
            }
        }

        public void AdjustTitleList()
        {
            storyEventTitlesList.Clear();

            for (int i = 0; i < storyEvents.Count; i++)
            {
                storyEventTitlesList.Add(storyEvents[i].title);
            }
        }

        public void AdjustNextEventWeek(int currentWeek)
        {
            nextEventWeek = currentWeek + eventIntervals[curEventIdx];
        }
    }

}
