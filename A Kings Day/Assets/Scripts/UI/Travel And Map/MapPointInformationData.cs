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
        public int population;
        public int coinTax;
        public TerritoryOwners ownedBy;
        public ChooseUnitMindset aiMindset;
        public MapType mapType;
        public BaseHeroInformationData leaderUnit;
        public bool visibleToPlayer;
        public List<BaseTravellerData> travellersOnPoint;
        public List<TroopsInformation> troopsCarried;
        public List<TravellerType> spawnableTravellers;
        public int latestWeekUpdate;

        public int ObtainTotalUnitCount()
        {
            int total = 0;
            if (troopsCarried != null && troopsCarried.Count > 0)
            {
                for (int i = 0; i < troopsCarried.Count; i++)
                {
                    total += troopsCarried[i].totalUnitCount;
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