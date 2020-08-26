using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameItems;
using Kingdoms;

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
        public WieldedWeapon wieldedWeapon;
        public float curhealth, maxHealth, curSpeed, origSpeed, minDamage, maxDamage;
        public float range;
        public int unitCooldown = 1;
        public int healcost = 5;
        public int unitPrice = 5;
        public float blockProjectile, blockMelee;

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

        public void RemoveBuff(BaseBuffInformationData thisBuff)
        {
            buffList.Remove(thisBuff);

            ReceiveDamage(thisBuff.effectAmount, thisBuff.targetStats);
        }
        public void ReceiveDamage(float damageAmount, TargetStats targetStats)
        {
            float checkAmount = 0;
            switch (targetStats)
            {
                case TargetStats.health:
                        checkAmount = curhealth - damageAmount;
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
                            curhealth -= damageAmount;
                        }
                    break;
                case TargetStats.damage:
                    checkAmount = minDamage - damageAmount;
                    if (checkAmount <= 0)
                    {
                        minDamage = 0.1f;
                    }
                    else
                    {
                        minDamage -= damageAmount;
                    }

                    checkAmount = maxDamage - damageAmount;
                    if(checkAmount <= 0)
                    {
                        maxDamage = 0.2f;
                    }
                    else
                    {
                        maxDamage -= damageAmount;
                    }
                    break;
                case TargetStats.speed:
                    checkAmount = origSpeed - damageAmount;
                    if (checkAmount <= 0)
                    {
                        origSpeed = 0.1f;
                    }
                    else
                    {
                        origSpeed -= damageAmount;
                    }
                    break;
                case TargetStats.range:
                    checkAmount = range - damageAmount;
                    if (checkAmount <= 0)
                    {
                        range = 0.1f;
                    }
                    else
                    {
                        range -= damageAmount;
                    }
                    break;
                case TargetStats.blockProjectile:
                    blockProjectile -= damageAmount;
                    break;
                case TargetStats.blockMelee:
                    blockMelee -= damageAmount;
                    break;
                default:
                    break;
            }
 
        }

        public void ReceiveHealing(float healAmount, TargetStats targetStats)
        {
            Debug.Log("We've Received a Healing for : " + targetStats + " Amount: " + healAmount);
            float checkAmount = 0;
            switch (targetStats)
            {
                case TargetStats.health:
                    checkAmount = curhealth + healAmount;
                    if (checkAmount >= maxHealth)
                    {
                        curhealth = maxHealth;
                    }
                    else
                    {
                        curhealth += healAmount;
                    }
                    break;
                case TargetStats.damage:
                    minDamage += healAmount;
                    maxDamage += healAmount;
                    break;
                case TargetStats.speed:
                    origSpeed += healAmount;
                    break;
                case TargetStats.range:
                    range += healAmount;
                    break;
                case TargetStats.blockProjectile:
                    blockProjectile += healAmount;
                    break;
                case TargetStats.blockMelee:
                    blockMelee += healAmount;
                    break;
                default:
                    break;
            }
        }


    }

    [Serializable]
    public class BaseHeroInformationData
    {
        public HeroRarity heroRarity;
        public bool isHeroBaseState;
        public bool isRandomGenerated;
        public int heroLevel = 1;
        public UnitInformationData unitInformation;
        public int healthGrowthRate, speedGrowthRate, damageGrowthRate;
        public int upgradesAdded;

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
