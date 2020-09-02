using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Territory;
using Technology;
using Characters;
using KingEvents;
using GameItems;
using Buildings;
using Battlefield;
using Maps;

namespace Kingdoms
{
    [Serializable]
    public class TravellerFlavourPhrase
    {
        public float relationshipGauge;
        public string flavourText;
        public bool textRevealed;
    }
    [Serializable]
    public class BaseTravellerData
    {
        public string travellersName;
        [Header("Basic Traveller Information")]
        public TravellerType travellerType;
        public TerritoryOwners affiliatedTeam;
        public bool playerSentTraveller;
        public string targetDestinationMapPoint;

        [Header("Automated Map Movement Mechanics")]
        public bool isGoingToMoveNextWeek;
        public int numberOfMovesNextWeek;

        [Header("Duration Travelled Mechanics")]
        public int weekSpawned;
        public int lastWeekTravelled;

        [Header("Near Kingdom Travel Mechanics")]
        public string originalSpawnPoint;
        public string currentScenePoint;
        public string currentSceneTargetPoint;

        [Header("Units Travelling Mechanics")]
        public List<TroopsInformation> troopsCarried;
        public float travellerSpeed;
        public List<BaseHeroInformationData> leaderUnit;
        public ChooseUnitMindset leaderMindset;

        [Header("Player Reaction and Relation Mechanics")]
        public float relationship = 0;
        public List<string> actionsDealt;
        public bool scoutSentSuccessfully;

        public List<TravellerFlavourPhrase> flavourTexts;
        public int ObtainTotalUnitCount()
        {
            int totalCount = 0;
            for (int i = 0; i < troopsCarried.Count; i++)
            {
                totalCount += troopsCarried[i].totalUnitCount;
            }

            return totalCount;
        }

        public void UpdateRelationship(float increment)
        {
            relationship += increment;

            if(travellerType == TravellerType.Warband || travellerType == TravellerType.Invader)
            {
                if (relationship < 0)
                {
                    travellerType = TravellerType.Invader;
                }
                else
                {
                    travellerType = TravellerType.Warband;
                }
            }
        }
        public List<TravellerFlavourPhrase> GetRelationshipFlavorText()
        {
            List<TravellerFlavourPhrase> currentTastes = new List<TravellerFlavourPhrase>();

            if(relationship >= 0) // Neutral Conversations
            {
                currentTastes = flavourTexts.FindAll(x => relationship >= x.relationshipGauge && !x.textRevealed);
            }
            else if(relationship < 0) // Hateful speeches
            {
                currentTastes = flavourTexts.FindAll(x => relationship <= x.relationshipGauge && !x.textRevealed);
            }

            return currentTastes;
        }
        public int ObtainVagueUnitCount(bool minimum)
        {
            int totalCount = 0;
            totalCount = ObtainTotalUnitCount();
            if(totalCount > 5)
            {
                if (minimum)
                {
                    if(totalCount < 10)
                    {
                        totalCount -= UnityEngine.Random.Range(1,2);
                    }
                    else if (totalCount < 20)
                    {
                        totalCount -= UnityEngine.Random.Range(3, 5);
                    }
                    else if (totalCount < 50)
                    {
                        totalCount -= UnityEngine.Random.Range(10, 15);
                    }
                    else if (totalCount < 70)
                    {
                        totalCount -= UnityEngine.Random.Range(17, 23);
                    }
                    else if (totalCount < 90)
                    {
                        totalCount -= UnityEngine.Random.Range(20, 27);
                    }
                    else if (totalCount > 90)
                    {
                        totalCount -= UnityEngine.Random.Range(40, 70);
                    }
                }
                else
                {
                    if (totalCount < 10)
                    {
                        totalCount += UnityEngine.Random.Range(1, 2);
                    }
                    else if (totalCount < 20)
                    {
                        totalCount += UnityEngine.Random.Range(3, 5);
                    }
                    else if (totalCount < 50)
                    {
                        totalCount += UnityEngine.Random.Range(10, 15);
                    }
                    else if (totalCount < 70)
                    {
                        totalCount += UnityEngine.Random.Range(17, 23);
                    }
                    else if (totalCount < 90)
                    {
                        totalCount += UnityEngine.Random.Range(20, 27);
                    }
                    else if(totalCount > 90)
                    {
                        totalCount += UnityEngine.Random.Range(40, 70);
                    }
                }
            }
            return totalCount;
        }
    }
    [Serializable]
    public class PlayerCampaignData
    {
        public string _fileName;
        public bool fileData = false;

        public List<BaseTravellerData> travellerList; // Travellers Visible from the kingdom
        public List<MapPointInformationData> mapPointList; // points seen in the map
        public int totalWeeklyTax;
    }
}
