using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kingdoms;

namespace Technology
{
    public enum FoodTechType
    {
        IncreaseStorage,
        lessWeekTotalInterval,
        IncreaseProduce,
        IncreaseCowBirth,
        IncreaseCowMeat,
    }

    public enum CoinTechType
    {
        IncreaseDiscount,
        IncreaseMerchantArrival,
        IncreaseSecurityIncome,
        IncreaseTaxMonthlyIncome,
        IncreaseMerchantTax,
    }

    public enum PopulationTechType
    {
        IncreaseBirthrate, // Housing
        IncreaseSettlerChance, // Festival
        IncreaseFoodSupport, // 
        IncreaseMaxVillagers,
        IncreaseTaxCollected,

    }

    public enum TroopTechType
    {
        IncreaseHealth,
        DecreaseCoinRecruit,
        IncreaseDamage,
        IncreaseMaxTroops,
        IncreaseMorale,
    }
    [Serializable]
    public class BaseTechnologyData
    {
        [Header("Editable")]
        public string technologyName;
        public ResourceType improvedType;
        public List<int> goldLevelRequirements;

        [Header("Player Configured")]
        public int currentLevel;
        public int goldRequirement;
        public int curGold;

        public int bonusIncrement;

        [Header("Troop Tech")]
        public TroopTechType troopTechType;

        [Header("Population Tech")]
        public PopulationTechType popTechType;

        [Header("Food Tech")]
        public FoodTechType foodTechType;

        [Header("Coin Tech")]
        public CoinTechType coinTechType;

        [Header("Editable Message")]
        public string effectMesg;
        public string wittyMesg;


        public BaseTechnologyData ConverToData(BaseTechnology thisTech)
        {
            technologyName = thisTech.technologyName;
            improvedType = thisTech.improvedType;
            goldLevelRequirements = thisTech.goldLevelRequirements;

            currentLevel = thisTech.currentLevel;
            goldRequirement = thisTech.goldRequirement;
            curGold = thisTech.curGold;
            bonusIncrement = thisTech.bonusIncrement;

            troopTechType = thisTech.troopTechType;
            popTechType = thisTech.popTechType;
            foodTechType = thisTech.foodTechType;
            coinTechType = thisTech.coinTechType;

            effectMesg = thisTech.effectMesg;
            wittyMesg = thisTech.wittyMesg;

            return this;
        }
    }

    [Serializable]
    public class BaseTechnology
    {
        // Editable
        [Header("Editable")]
        public string technologyName;
        public ResourceType improvedType;
        public Sprite techIcon;
        public List<int> goldLevelRequirements;

        [Header("Player Configured")]
        public int currentLevel;
        public int goldRequirement;
        public int curGold;

        public int bonusIncrement;

        [Header("Troop Tech")]
        public TroopTechType troopTechType;

        [Header("Population Tech")]
        public PopulationTechType popTechType;

        [Header("Food Tech")]
        public FoodTechType foodTechType;

        [Header("Coin Tech")]
        public CoinTechType coinTechType;

        [Header("Editable Message")]
        public string effectMesg;
        public string wittyMesg;

        public BaseTechnology ConverToData(BaseTechnologyData thisTech)
        {
            technologyName = thisTech.technologyName;
            improvedType = thisTech.improvedType;
            goldLevelRequirements = thisTech.goldLevelRequirements;

            currentLevel = thisTech.currentLevel;
            goldRequirement = thisTech.goldRequirement;
            curGold = thisTech.curGold;
            bonusIncrement = thisTech.bonusIncrement;

            troopTechType = thisTech.troopTechType;
            popTechType = thisTech.popTechType;
            foodTechType = thisTech.foodTechType;
            coinTechType = thisTech.coinTechType;

            effectMesg = thisTech.effectMesg;
            wittyMesg = thisTech.wittyMesg;

            return this;
        }
    }

}