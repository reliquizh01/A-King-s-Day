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
        public UnitAttackType attackType;
        public float curhealth, maxHealth, curSpeed, origSpeed, minDamage, maxDamage;

        public UnitState currentState;
        public float deathThreshold = -5.0f;

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

            if(checkAmount <= 0)
            {
                if(checkAmount <= deathThreshold)
                {
                    currentState = UnitState.Dead;
                }
                else
                {
                    currentState = UnitState.Injured;
                }
                curhealth = 0;
            }
            else
            {
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
