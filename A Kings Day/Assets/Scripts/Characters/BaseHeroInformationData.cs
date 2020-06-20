using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public enum HeroRarity
    {
        Common,
        Rare,
        Legendary,
    }
    [Serializable]
    public class UnitInformationData
    {
        public float curhealth, maxHealth, curSpeed, origSpeed, minDamage, maxDamage;

    }

    [Serializable]
    public class BaseHeroInformationData
    {
        public HeroRarity heroRarity;
        public string heroName;
        public UnitInformationData unitInformation;

        public int healthGrowthRate, speedGrowthRate, damageGrowthRate; 
    }

}
