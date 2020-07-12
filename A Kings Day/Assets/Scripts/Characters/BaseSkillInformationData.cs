using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;

namespace Characters
{
    public enum SkillType
    {
        Offensive,
        Defensive,
        OffensiveBuff,
        DefensiveBuff,
        PassiveBuff,
    }

    public enum TriggeredBy
    {
        CommanderActivated,
        ReceivingDamage,
    }
    public enum TargetType
    {
        Unit,
        Tiles,
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
    }

    [System.Serializable]
    public class BaseSkillInformationData
    {
        public string skillName;
        public SkillType skillType;

        public TargetType targetType;

        public int skillLevel = 1;
        public bool statenhanced;

        public AreaAffected affectedArea;
        public int maxRange;

        // Target Inflicted means its a direct hit.
        public TargetStats targetStats;
        public float targetInflictedCount;

        // Buff effect are those effects with duration
        public List<BaseBuffInformationData> buffList;
    }

}