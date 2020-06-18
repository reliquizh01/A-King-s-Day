using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Territory;
using Kingdoms;

namespace KingEvents
{
    [System.Serializable]
    public class KingdomEventStorage : MonoBehaviour
    {
        #region Singleton
        private static KingdomEventStorage instance;
        public static KingdomEventStorage GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            instance = this;
        }
        #endregion


        public List<EventDecisionData> kingdomEvents;
        public List<StoryArcEventsData> storyArcEvents;

        private List<string> choices = new List<string>();
        private int curChoice = 0;
        private PlayerKingdomData curPlayerData;


        [Header("[Difficulty Dice Roll")]
        public float threateningChance = 0.03f;
        public float currentThreateningChance = 0.03f;
        public float troublesomeChance = 0.05f;
        public float currentTroublesomeChance = 0.05f;

        [Header("[Story Dice Roll]")]
        public float storyChance = 0.07f;
        public float currentStoryChance = 0.07f;

        // DEBUGGING PURPOSES
        [Header("[Events Debugger]")]
        public bool EnableDebugging = false;
        [Header("Difficulty")]
        public DifficultyType Difficultyforced;
        public bool forceDifficulty = false;

        [Header("Story Arcs")]
        public bool forceArc = false;
        public bool initialEventOnly = false;
        public bool allStoryArcEvents = false;
        [ShowOnly]public string arcForced;

        #region ADDING  AND OBTAINING AN EVENT
        public void AddKingdomEvent(EventDecisionData thisEvent)
        {
            if(kingdomEvents == null)
            {
                return;
            }

            if(kingdomEvents.Find(x => x.title == thisEvent.title) != null)
            {
                return;
            }
            if(kingdomEvents == null)
            {
                kingdomEvents = new List<EventDecisionData>();
            }

            kingdomEvents.Add(thisEvent);
        }

        public List<EventDecisionData> ObtainWeeklyEvents(TerritoryLevel thisLevel, int eventCount, PlayerKingdomData playerData, bool inStory = false)
        {
            List<EventDecisionData> result = new List<EventDecisionData>();
            curPlayerData = playerData;
            List<EventDecisionData> forcedEvent = new List<EventDecisionData>();

            #region DEV DEBUGGING
            if (EnableDebugging)
            {
                if(!KingdomManager.GetInstance.isInStory && !playerData.IsStoryArcFinished(arcForced))
                {
                    // ADD INITIAL EVENT FIRST
                    if(initialEventOnly)
                    {
                        forcedEvent.Add(ForcedInitialStoryEventDataToReturn());
                        result.Add(forcedEvent[forcedEvent.Count-1]);
                        eventCount -= forcedEvent.Count;
                    }
                    // GET ALL STORY ARC EVENT
                    else if(allStoryArcEvents)
                    {
                        forcedEvent.AddRange(ForcedStoryEventsDataToReturn());
                        result.AddRange(forcedEvent);
                        eventCount -= forcedEvent.Count;
                    }

                }
            }
            #endregion

            if(result.Count >= eventCount)
            {
               return result;
            }
            else
            {
                // GET TERRITORY 
                result.AddRange(GetTerritoryFilteredEvents(thisLevel));

                // Filter by Difficulty 
                if(EnableDebugging && forceDifficulty)
                {
                    result = ForceDifficultyFilteredEvent(Difficultyforced, result);
                }
                else
                {
                    result = GetDifficultyRolledEvents(curPlayerData.level, result);

                }

                if(!KingdomManager.GetInstance.isInStory)
                {
                    //Remove other Arcs
                    if(EnableDebugging && forceArc)
                    {
                        result = FilterToInitialStoryArcEvent(result, arcForced);
                    }
                    // FILTER BY STORY
                    if(!initialEventOnly && !inStory)
                    {
                        // CHECK IF RESULT HAS STORIES, REMOVE ALL BUT 1.
                        result = GetTerritoryDifficultyFilteredEvents(result);

                    }
                }
                else
                {
                    List<EventDecisionData> nonStory = new List<EventDecisionData>();
                    nonStory.AddRange(result.FindAll(x => !x.isStoryArc));
                    result = result.FindAll(x => x.isStoryArc && x.storyArc == KingdomManager.GetInstance.currentStory.storyTitle);
                    result.AddRange(nonStory);
                }

                // GET THE TOP 3
                List<EventDecisionData> finalResult = new List<EventDecisionData>();

                List<int> randomEvents = new List<int>();
                // ALWAYS HAVE THE FORCED EVENTS INSIDE THE RESULT.
                if (EnableDebugging)
                {
                    if(forcedEvent.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if(forcedEvent.Find(x => x.title == result[i].title) != null)
                            {
                                randomEvents.Add(i);
                            }
                        }
                    }
                }
               // Debug.Log("RESULT COUNT: " + result.Count);
                for (int i = 0; i < eventCount; i++)
                {
                    int tmp = UnityEngine.Random.Range(0, result.Count-1);
                    if(randomEvents.Contains(tmp))
                    {
                        i--;
                    }
                    else
                    {
                        randomEvents.Add(tmp);
                    }
                }

                for (int i = 0; i < randomEvents.Count; i++)
                {
                    finalResult.Add(result[randomEvents[i]]);
                }

               // Debug.Log("Final Result Count: " + finalResult.Count);
                return finalResult;
            }
        }

        public List<EventDecisionData> FilterToInitialStoryArcEvent(List<EventDecisionData> prevResult, string thisArc)
        {
            List<EventDecisionData> result = prevResult;

            result.RemoveAll(x => x.isStoryArc);

            if(!curPlayerData.IsStoryArcFinished(thisArc))
            {
                result.Add(storyArcEvents.Find(x => x.storyTitle == thisArc).storyEvents[0]);
            }

            return result;
        }
        public List<EventDecisionData> GetTerritoryFilteredEvents(TerritoryLevel thisLevel, List<EventDecisionData> prevResult = null)
        {
            List<EventDecisionData> result = prevResult;

            if(prevResult != null)
            {
                result.AddRange(kingdomEvents.FindAll(x => x.levelRequired == thisLevel && !result.Contains(x)));
            }
            else
            {
                result = kingdomEvents.FindAll(x => x.levelRequired == thisLevel);
            }
            return result;
        }

        // FILTER BY STORY
        public List<EventDecisionData> GetTerritoryDifficultyFilteredEvents(List<EventDecisionData> difficultyTerritoryFiltered)
        {

            /* ================================ COMPUTATION NOTES ================================ 
             * 
             * PLAYER ROLL BASED FOR STORIES
             * COMPUTATION[Base % +(# of Weeks of NO EVENT * 0.007%) = TOTAL STORY % REVEAL]
             * 
             * ===================================================================================
             */
            List<EventDecisionData> result = difficultyTerritoryFiltered;
           // Debug.Log("FILTERED COUNT : " + result.Count);
            int chosenStory = result.FindIndex(x => x.isStoryArc);

            bool allowStory = ComputeStoryArcChance(curPlayerData.weekCount);
            EventDecisionData item = result[chosenStory];

                // ROLL DICE
            if (allowStory && !curPlayerData.IsStoryArcFinished(item.storyArc))
            {
                currentStoryChance = storyChance;
            }
            else
            {
                currentStoryChance += storyChance;
            }

            for (int i = 0; i < result.Count; i++)
            {
                if(i != chosenStory)
                {
                    if(result[i].isStoryArc)
                    {
                        result[i] = null;
                    }
                }
                else
                {
                    // REPLACE CHOSEN STORY FOR INITIAL STORY.
                    if(i == chosenStory && allowStory && !curPlayerData.IsStoryArcFinished(item.storyArc))
                    {
                        EventDecisionData newStory = storyArcEvents.Find(x => x.storyTitle == result[i].storyArc).storyEvents[0];
                        result[i] = newStory;
                    }
                    else
                    {
                        result[i] = null;
                    }
                }
            }

            List<EventDecisionData> filteredResult = result.FindAll(x => x != null);

            return filteredResult;
        }

        #endregion

        #region DIFFICULTY COMPUTATIONS
        public bool ComputeStoryArcChance(int weekCount, DifficultyType difficulty = DifficultyType.common)
        {
            float difficultyAdjustment = 0;
            switch (difficulty)
            {
                case DifficultyType.troublesome:
                    difficultyAdjustment = currentTroublesomeChance;
                    break;
                case DifficultyType.threatening:
                    difficultyAdjustment = currentThreateningChance;
                    break;
                case DifficultyType.common:
                    difficultyAdjustment = 25f;
                    break;
            }

            float checkStoryChance = storyChance + (weekCount * storyChance);
            checkStoryChance += difficultyAdjustment;

            float rollDice = UnityEngine.Random.Range(0.0f, 100.0f);
            //Debug.Log("----------|| Dice Roll for Story: " + rollDice + " over 100" + " Chance Rate:" + checkStoryChance);
            return (rollDice <= checkStoryChance);
        }

        public List<EventDecisionData> GetDifficultyRolledEvents(TerritoryLevel curLevel,List<EventDecisionData> territoryFiltered)
        {
            List<EventDecisionData> result = territoryFiltered;

            bool allowTroublesome = false;
            bool allowThreatening = false;


            /* ================================ COMPUTATION NOTES ================================ 

             *     PLAYER GAME DIFFICULTY
             *     COMPUTATION [currentChanceBase + (Level * baseChance)] = TOTAL DIFFICULTY
             * 
             * ====================================================================================
             *  RULES:
             *  1.)  INCREMENT ALL DIFFICULTIES
            */

            float rollDice = 0;
            // THREATENING
            float checkThreateningChance = currentThreateningChance + ((int)curLevel * threateningChance) + threateningChance;
            rollDice = UnityEngine.Random.Range(0.0f, 100.0f);
            if(rollDice <= checkThreateningChance)
            {
                allowThreatening = true;
                currentThreateningChance = threateningChance;
            }
            else
            {
                currentThreateningChance += threateningChance;
            }
            //Debug.Log("----------|| Dice Roll for Threatening: " + rollDice + " over 100" + " Chance Rate:" + currentThreateningChance);
            // TROUBLESOME
            float checkTroublesomeChance = currentTroublesomeChance + ((int)curLevel * threateningChance) + threateningChance;
            rollDice = UnityEngine.Random.Range(0.0f, 100.0f);
            //Debug.Log("----------|| Dice Roll for Troublesome: " + rollDice + " over 100" + " Chance Rate:" + currentTroublesomeChance);
            if (rollDice <= checkTroublesomeChance)
            {
                allowTroublesome = true;
                currentTroublesomeChance = troublesomeChance;
            }
            else
            {
                currentTroublesomeChance += troublesomeChance;
            }


            if (!allowThreatening)
            {
                result.RemoveAll(x => x.difficultyType == DifficultyType.threatening);
            }
            if(!allowTroublesome)
            {
                result.RemoveAll(x => x.difficultyType == DifficultyType.troublesome);
            }



            return result;
        }
        public bool DifficultyRollComputation(int weekCount, float increment)
        {
            float tmp = weekCount * increment;

            bool allowDifficultyEvent = false;

            float roll = UnityEngine.Random.Range(0, 100);

            allowDifficultyEvent = (roll <= tmp);

            return allowDifficultyEvent;
        }

        public List<EventDecisionData> FilterEvents(List<EventDecisionData> thisData, int weekCount, bool removeStory = false)
        {
            StoryArcEventsData storyBearer = null;
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;


            List<EventDecisionData> tmp = thisData;

         
            return tmp;
        }
        #endregion

        #region CONFIRMING CONDITIONS
        public bool IsStoryArcAllowed(int currentWeek, StoryArcEventsData thisArc)
        {
            return (currentWeek >= thisArc.startingWeek);
        }

        public bool IsStoryArcAllowed(int currentWeek, string thisStoryTitle)
        {
            StoryArcEventsData thisArc = storyArcEvents.Find(x => x.storyTitle == thisStoryTitle);
            return (currentWeek >= thisArc.startingWeek);
        }
        #endregion

        #region DEBUGGING FUNCTIONS

        public EventDecisionData ForcedInitialStoryEventDataToReturn(string thisArc = "")
        {
            EventDecisionData thisData = null;
            List<EventDecisionData> filteredEventList = new List<EventDecisionData>();
            filteredEventList = kingdomEvents.FindAll(x => x.storyArc == arcForced);
            StoryArcEventsData arc = new StoryArcEventsData();

            if(string.IsNullOrEmpty(thisArc))
            {
                arc = storyArcEvents.Find(x => x.storyTitle == arcForced);
            }
            else
            {
                arc = storyArcEvents.Find(x => x.storyTitle == thisArc);
            }

            thisData = arc.storyEvents[0];
            Debug.Log("FORCED EVENT NAME: " + arc.storyEvents[0].title);
            return thisData;
        }
        public List<EventDecisionData> ForceDifficultyFilteredEvent(DifficultyType thisDifficulty, List<EventDecisionData> territoryFiltered)
        {
            List<EventDecisionData> result = territoryFiltered.FindAll(x => x.difficultyType == thisDifficulty);

            return result;
        }

        // A WHOLE LIST OF STORY ARC EVENTS
        public List<EventDecisionData> ForcedStoryEventsDataToReturn()
        {
            List<EventDecisionData> filteredEventList = new List<EventDecisionData>();
            filteredEventList = kingdomEvents.FindAll(x => x.storyArc == arcForced);
            StoryArcEventsData arc = storyArcEvents.Find(x => x.storyTitle == arcForced);

            return arc.storyEvents;
        }
        #endregion
    }

}