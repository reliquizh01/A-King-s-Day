using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kingdoms;
using Battlefield;
using Characters;

namespace Maps
{
    public enum TerritoryOwners
    {
        Neutral,
        HolySee,
        FurKhan,
        Gates,
        Player,
    }
    [Serializable]
    public class MapPointInformationData
    {
        public string pointName;
        public int latestWeekUpdated;
        [Header("Map Information")]
        public List<MapPointInformationData> mapNeighborsList;
        public int population;
        public int coinTax;
        public TerritoryOwners ownedBy;
        public TerritoryOwners previousOwner;
        public MapType mapType;
        public bool visibleToPlayer;
        public bool isBeingAttacked;
        public bool isKingdomPoint;

        [Header("Travellers Temporarily In the Place")]
        public List<BaseTravellerData> travellersOnPoint;

        [Header("Troops Staying In Base")]
        public ChooseUnitMindset aiMindset;
        public BaseHeroInformationData leaderUnit;
        public List<TroopsInformation> troopsStationed;
        public List<TravellerType> spawnableTravellers;
        public int latestWeekUpdate;


        public void ReceiveReinforcementUnits(List<TroopsInformation> reinforcements)
        {
            if(troopsStationed != null && troopsStationed.Count > 0)
            {
                for (int i = 0; i < reinforcements.Count; i++)
                {
                    TroopsInformation tmp = null;
                    tmp = troopsStationed.Find(x => x.unitInformation.unitName == reinforcements[i].unitInformation.unitName);
                    if (tmp != null)
                    {
                        tmp.totalUnitCount += reinforcements[i].totalUnitCount;
                    }
                    else
                    {
                        tmp = new TroopsInformation();
                        tmp = reinforcements[i];
                        troopsStationed.Add(tmp);
                    }
                }
            }
            else
            {
                troopsStationed = new List<TroopsInformation>();
                troopsStationed.AddRange(reinforcements);
            }
        }
        public int ObtainTotalUnitCount()
        {
            int total = 0;
            if (troopsStationed != null && troopsStationed.Count > 0)
            {
                for (int i = 0; i < troopsStationed.Count; i++)
                {
                    total += troopsStationed[i].totalUnitCount;
                }
            }
            return total;
        }

        public int ObtainVagueUnitCount(bool minimum)
        {
            int totalCount = 0;
            totalCount = ObtainTotalUnitCount();
            if (totalCount > 5)
            {
                if (minimum)
                {
                    if (totalCount < 10)
                    {
                        totalCount -= UnityEngine.Random.Range(1, 2);
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
                    else if (totalCount > 90)
                    {
                        totalCount += UnityEngine.Random.Range(40, 70);
                    }
                }
            }
            return totalCount;
        }
    }
}