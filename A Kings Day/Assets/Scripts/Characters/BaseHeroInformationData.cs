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

    public enum UnitState
    {
        Healthy,
        Injured,
        Dead,
    }

    public enum UnitType
    {
        Recruit,
        Swordsman,
        Spearman,
        Archer,
        Hero,
    }
    [Serializable]
    public class UnitInformationData
    {
        public string unitName = "";
        public string unitGenericName;
        public UnitAttackType attackType;
        public float curhealth, maxHealth, curSpeed, origSpeed, minDamage, maxDamage;
        public int range;
        public int unitCooldown = 1;

        public string prefabDataPath;
        public UnitState currentState;
        public float deathThreshold = -5.0f;

        public int morale;

        public List<BaseBuffInformationData> buffList;

        public float RealSpeed
        {
            get {

                float currentRealSpeed = origSpeed;
                if(buffList != null && buffList.Count > 0)
                {
                    for (int i = 0; i < buffList.Count; i++)
                    {
                        if(buffList[i].targetStats == TargetStats.speed)
                        {
                            currentRealSpeed += buffList[i].effectAmount;
                        }
                    }
                }
                return currentRealSpeed;
            }
        }
        public float RealDamage
        {
            get
            {
                float randDamage = UnityEngine.Random.Range(minDamage, maxDamage);
                float bonusDmg = 0;
                if (buffList != null && buffList.Count > 0)
                {
                    for (int i = 0; i < buffList.Count; i++)
                    {
                        if (buffList[i].targetStats == TargetStats.speed)
                        {
                            bonusDmg += buffList[i].effectAmount;
                        }
                    }
                }
                randDamage += bonusDmg;

                return randDamage;
            }
        }

        public void AddBuff(BaseBuffInformationData thisBuff)
        {
            buffList.Add(thisBuff);
        }

        public void ReceiveDamage(float damageAmount)
        {
            float checkAmount = curhealth - damageAmount;
            Debug.Log("[Current Health: "+curhealth+"][Potential Health: " + checkAmount + "] [Damage Amount:" + damageAmount+"]" + " RECEIVED BY:" + unitName + " ]");

            if (checkAmount <= 0)
            {
                curhealth -= damageAmount;
                if (curhealth <= deathThreshold)
                {
                    currentState = UnitState.Dead;
                }
                else
                {
                    currentState = UnitState.Injured;
                }
            }
            else
            {
                Debug.Log("Normal Dmg");
                curhealth -= damageAmount;
            }
        }

        public void ReceiveHealing(float healAmount)
        {
            float checkAmount = curhealth + healAmount;

            if (checkAmount > maxHealth)
            {
                curhealth = maxHealth;
            }
            else
            {
                curhealth += healAmount;
            }
        }


    }

    [Serializable]
    public class BaseHeroInformationData
    {
        public HeroRarity heroRarity;
        public bool isRandomGenerated;
        public int heroLevel = 1;
        public UnitInformationData unitInformation;
        public int healthGrowthRate, speedGrowthRate, damageGrowthRate;

        public int baseHeroCoinPrice;
        public List<BaseSkillInformationData> skillsList;
        public List<ItemInformationData> equipments;
        public int GetEquipmentTotalPrice
        {
            get
            {
                if(equipments != null && equipments.Count > 0)
                {
                    int totalAmount = 0;

                    for (int i = 0; i < equipments.Count; i++)
                    {
                        totalAmount += equipments[i].itemPrice;
                    }

                    return totalAmount;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int GetHeroPrice
        {
            get { return baseHeroCoinPrice + GetEquipmentTotalPrice; }
        }

    }

}
