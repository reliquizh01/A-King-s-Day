using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;

namespace Characters
{
    public enum SkillType
    {
        Offensive, // Skills that hurts the enemy (spawnable skills)
        Defensive, // Skills that helps the allies (spawnable skills)
        OffensiveBuff,  // Skills that hurts the enemies (passive addition)
        DefensiveBuff, // Skills that helps the allies (passive addition)
        SummonUnits, // Summon Units, Either from prefab or your current selected unit.
    }

    public enum TriggeredBy
    {
        CommanderActivated,
        ReceivingDamage,
        ActivateNow,
        EverySecond,
    }
    public enum TargetType
    {
        UnitOnly,
        UnitOnTiles,
        TilesOnly,
    }

    public enum TargetStats
    {
        health,
        damage,
        speed,
        range,
        blockProjectile,
        blockMelee,
    }

    public enum AreaAffected
    {
        Single,
        Row,
        Column,
        Nearby,
        All,
        Aimed,
    }
    [System.Serializable]
    public class BaseBuffInformationData
    {
        public string buffName;
        public float duration;
        public TargetStats targetStats;
        public float effectAmount;
        public bool permanentBuff;
        public TriggeredBy buffTrigger;

        public bool tickingBuff;
        public float tickerCounter;
    }

    [System.Serializable]
    public class BaseSkillInformationData
    {
        public string skillName;
        public string skillDescription;
        public SkillType skillType;
        public TargetType targetType;
        public int skillLevel = 1;
        public bool targetAlive = true;
        
        public bool statEnhanced;
        public bool spawnPrefab;
        public string prefabStringPath;

        public AreaAffected affectedArea;
        public int maxRange;

        public float skillDurationOnTile;

        public bool isOnCooldown;
        public float curCooldown;
        public float cooldown;
        public float cooldownLevelGrowth;

        // Target Inflicted means its a direct hit.
        public TargetStats targetStats;
        public float targetInflictedCount;
        public float effectgrowth;

        // Buff effect are those effects with duration
        public List<BaseBuffInformationData> buffList;
    }

}