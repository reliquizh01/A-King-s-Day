using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;

namespace Territory
{
    public enum TerritoryLevel
    {
        Hamlet,
        Village,
        SmallTown,
        Town,
        RegionHub,
        Kingdom,
    }

    [Serializable]
    public class TerritoryRequirements
    {
        [ShowOnly]public TerritoryLevel thisLevel;
        [ShowOnly]public int foodReq, troopReq, coinReq, populationReq;
        public bool RequirementPassed(PlayerKingdomData thisKingdom)
        {
            if(foodReq > thisKingdom.foods)
            {
                return false;
            }
            if (populationReq > thisKingdom.population)
            {
                return false;
            }
            if (troopReq > thisKingdom.troops)
            {
                return false;
            }
            if (coinReq > thisKingdom.coins)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///  HANDLES THE FOLLOWING
    /// 1. Territory Upgrades
    /// 2. Warehouse Upgrades
    /// 3. Barracks Upgrades
    /// 4. Village Upgrades
    /// 5. Treasury Upgrades
    /// 
    /// /// </summary>
    public class TerritoryHandler : MonoBehaviour
    {
        public List<TerritoryRequirements> requirements;


    }

}