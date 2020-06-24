using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameItems;

namespace Characters
{
    public enum HeroRarity
    {
        Common,
        Rare,
        Legendary,
    }
    public enum UnitAttackType
    {
        MELEE,
        RANGE,
        SPELL,
    }

    [Serializable]
    public class UnitInformationData
    {
        public UnitAttackType attackType;
        public float curhealth, maxHealth, curSpeed, origSpeed, minDamage, maxDamage;
    }

    [Serializable]
    public class BaseHeroInformationData
    {
        public HeroRarity heroRarity;
        public string heroName;
        public UnitInformationData unitInformation;

        public int healthGrowthRate, speedGrowthRate, damageGrowthRate;

        public List<ItemInformationData> equipments;

    }

}
