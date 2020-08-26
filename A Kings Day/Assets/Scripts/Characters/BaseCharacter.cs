﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using KingEvents;
using Managers;
using System;

namespace Characters
{
    [RequireComponent(typeof(Animator), typeof(CharacterMovement))]
    public class BaseCharacter : MonoBehaviour
    {
        public CharacterMovement myMovements;
        public CharacterRange myRange;
        public CharacterAnimationControl myAnimation;
        public CharacterAudioControl mySfx;

        public bool isKing = false;
        public bool isMainCharacter = false;
        public bool isGuest = false;
        public bool isLeaving = false;

        [Header("Unit Information")]
        public UnitInformationData unitInformation;
        public bool isFighting;
        public bool canReturnToCamp;
        public TeamType teamType;
        public bool canStagger;
        private Action reachedCallback;
        public bool isBanishing = false;
        public void Start()
        {
            if(isKing)
            {
                isMainCharacter = true;
            }
        }

        public void Update()
        {
            if(myMovements.isMoving)
            {
                UpdateMovementFacingDirection();
            }
            if(unitInformation.curhealth > 0)
            {
                if(unitInformation.buffList != null && unitInformation.buffList.Count > 0 
                    && unitInformation.buffList.Find(x => !x.permanentBuff) != null)
                {
                    for (int i = 0; i < unitInformation.buffList.Count; i++)
                    {
                        if(!unitInformation.buffList[i].permanentBuff)
                        {
                            unitInformation.buffList[i].duration -= Time.deltaTime;
                        }
                        if(unitInformation.buffList[i].tickingBuff)
                        {
                            unitInformation.buffList[i].tickerCounter += Time.deltaTime;
                            if(unitInformation.buffList[i].tickerCounter >= 1)
                            {
                                if (unitInformation.buffList[i].effectAmount < 0)
                                {
                                    ReceiveDamage(unitInformation.buffList[i].effectAmount, UnitAttackType.SPELL, unitInformation.buffList[i].targetStats);
                                }
                                else if(unitInformation.buffList[i].effectAmount > 0)
                                {
                                    ReceiveHealing(unitInformation.buffList[i].effectAmount, UnitAttackType.SPELL, unitInformation.buffList[i].targetStats);
                                }
                                unitInformation.buffList[i].tickerCounter = 0;
                            }
                        }
                    }

                    List<BaseBuffInformationData> removeThisBuffs = unitInformation.buffList.FindAll(x => x.duration <= 0 && !x.permanentBuff);
                    for (int i = 0; i < removeThisBuffs.Count; i++)
                    {
                        unitInformation.RemoveBuff(removeThisBuffs[i]);
                    }
                }
            }
        }
        // Use For Guests and Unique Characters

      /*  public void TargetReached()
        {
            if(!isLeaving)
            {
                SpawnManager.GetInstance.PreStartCourt();
            }
            else
            {
                SpawnManager.GetInstance.CheckCourtUse();
                DestroyImmediate(this.gameObject);
            }
        } */

        public void SetupCharacter(UnitInformationData newUnitInformation)
        {
            unitInformation = new UnitInformationData();

            int randomCode = UnityEngine.Random.Range(0, 100);
            unitInformation.unitGenericName = newUnitInformation.unitName;
            unitInformation.unitName = newUnitInformation.unitName + randomCode.ToString();
            unitInformation.attackType = newUnitInformation.attackType;

            unitInformation.curhealth = newUnitInformation.maxHealth;
            unitInformation.maxHealth = newUnitInformation.maxHealth;
            unitInformation.minDamage = newUnitInformation.minDamage;
            unitInformation.maxDamage = newUnitInformation.maxDamage;
            unitInformation.curSpeed = newUnitInformation.origSpeed;
            unitInformation.origSpeed = newUnitInformation.origSpeed;
            unitInformation.range = newUnitInformation.range;
            unitInformation.unitCooldown = newUnitInformation.unitCooldown;
            unitInformation.prefabDataPath = newUnitInformation.prefabDataPath;
            unitInformation.deathThreshold = newUnitInformation.deathThreshold;
            unitInformation.morale = newUnitInformation.morale;
            unitInformation.currentState = newUnitInformation.currentState;

            unitInformation.buffList = new List<BaseBuffInformationData>();
            if(newUnitInformation.buffList != null && newUnitInformation.buffList.Count > 0)
            {
                for (int i = 0; i < newUnitInformation.buffList.Count; i++)
                {
                    BaseBuffInformationData tmp = new BaseBuffInformationData();
                    unitInformation.buffList.Add(tmp);
                }
            }

            UpdateStats();
        }

        public void AddBuff(BaseBuffInformationData thisBuff)
        {
            unitInformation.AddBuff(thisBuff);
            UpdateStats();
        }

        public void UpdateStats()
        {
            myMovements.speed = unitInformation.RealSpeed;
        }
        public void OrderMovement(ScenePointBehavior thisLocation, Action callBack = null)
        {
            myMovements.isMoving = false;
            myMovements.SetTarget(thisLocation, callBack);

            UpdateMovementFacingDirection();
            UpdateCharacterState(CharacterStates.Walking);
        }

        public void OrderMovement(Vector2 newPosition, Action callback = null)
        {

        }

        public void OrderToFace(FacingDirection newDirection)
        {
            myAnimation.ChangeFacingDireciton(newDirection);
        }

        public void SetAsBanish()
        {
            isBanishing = true;
            myAnimation.characterSprite.color = new Color(1, 1, 1, 0.0f);
        }
        public void OrderToBanish()
        {
            isBanishing = true;
            myAnimation.UpdateBanish(isBanishing);
        }
        public void OrderToReveal()
        {
            isBanishing = false;
            myAnimation.UpdateBanish(isBanishing);
        }
        public void UpdateMovementFacingDirection()
        {
            if((Vector2)transform.position != myMovements.targetPos)
            {
                // SAME X POSITION ( MOVE CHARACTER BASED )
                if(transform.position.x == myMovements.targetPos.x)
                {
                    if(transform.position.y > myMovements.targetPos.y)
                    {
                        myAnimation.ChangeFacingDireciton(FacingDirection.Down);
                    }
                    else
                    {
                        myAnimation.ChangeFacingDireciton(FacingDirection.Up);
                    }
                }
                else
                {
                    if (transform.position.x > myMovements.targetPos.x)
                    {
                        myAnimation.ChangeFacingDireciton(FacingDirection.Right);
                    }
                    else
                    {
                        myAnimation.ChangeFacingDireciton(FacingDirection.Left);
                    }
                }
            }

        }

        public void UpdateCharacterState(CharacterStates newState)
        {
            CharacterStates curState = (CharacterStates)myAnimation.currentState;

            // Disable Updates in Character state if unit is dead.
            if (curState == CharacterStates.Injured_State)
            {
                return;
            }
            if(isBanishing)
            {
                return;
            }

            switch (newState)
            {
                case CharacterStates.Idle:
                    if(myMovements.isMoving)
                    {
                        break;
                    }
                    else if(curState == CharacterStates.Damage_Received)
                    {
                        break;
                    }

                    myAnimation.ChangeState(newState);
                    break;
                case CharacterStates.Walking:
                    if(myRange != null && myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
                    {
                        myRange.enemiesInRange.Clear();
                    }

                    myAnimation.ChangeState(newState);
                    break;

                case CharacterStates.Damage_Received:
                    if(curState == CharacterStates.Attack_State)
                    {
                        break;
                    }
                    if(!canStagger)
                    {
                        return;
                    }

                    myMovements.isMoving = false;
                    myAnimation.ChangeState(newState);
                    break;
                case CharacterStates.Attack_State:
                    myAnimation.ChangeState(newState);

                    break;
                case CharacterStates.Injured_State:
                    myAnimation.UpdateDeath(true);
                    myAnimation.ChangeState(newState);
                    break;
                default:
                    break;
            }
        }
        public void ContinueActionAfterReceiveDamage()
        {
            if(myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
            {
                UpdateCharacterState(CharacterStates.Attack_State);
            }
            else if(myMovements.pathToTargetPoint != null && myMovements.pathToTargetPoint.Count > 0)
            {
                myMovements.isMoving = true;
                UpdateCharacterState(CharacterStates.Walking);
            }
        }
        public void EnemyInRange()
        {
            myMovements.isMoving = false;
            UpdateCharacterState(CharacterStates.Attack_State);
        }

        public void SendDamage()
        {
            if(unitInformation.currentState != UnitState.Healthy)
            {
                return;
            }

            if(mySfx != null)
            {
                mySfx.PlaySendDamageAudio();
            }

            if(myRange.enemiesInRange == null 
                || myRange.enemiesInRange.Count <= 0
                || myRange.enemiesInRange[0] == null) 
            {
                UpdateToPotentialNextState();
                return;
            }

            if(unitInformation.attackType == UnitAttackType.MELEE)
            {
                if (myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
                {
                    myRange.enemiesInRange[0].ReceiveDamage(unitInformation.RealDamage, unitInformation.attackType, TargetStats.health);

                    if (myRange.enemiesInRange[0].unitInformation.curhealth <= 0)
                    {
                        myRange.enemiesInRange.RemoveAt(0);
                        UpdateToPotentialNextState();
                    }
                }
                else
                {
                    UpdateToPotentialNextState();
                }
            }
            else if(unitInformation.attackType == UnitAttackType.RANGE)
            {
                if (myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
                {
                    myRange.enemiesInRange[0].ReceiveDamage(unitInformation.RealDamage, unitInformation.attackType, TargetStats.health);

                    if (myRange.enemiesInRange[0].unitInformation.curhealth <= 0)
                    {
                        myRange.enemiesInRange.RemoveAt(0);
                        UpdateToPotentialNextState();
                    }
                }
                else
                {
                    UpdateToPotentialNextState();
                }
            }
            else if(unitInformation.attackType == UnitAttackType.SPELL)
            {

            }
        }

        public void ReceiveHealing(float amount, UnitAttackType attackType, TargetStats targetStats)
        {
            unitInformation.ReceiveHealing(amount, targetStats);
        }
        public void ReceiveDamage(float amount, UnitAttackType attackType, TargetStats targetStats)
        {
            UpdateCharacterState(CharacterStates.Damage_Received);

            bool blockAttempt = false;

            if(unitInformation.buffList != null && unitInformation.buffList.Count > 0)
            {
                List<BaseBuffInformationData> activateThisBuffs = unitInformation.buffList.FindAll(x => x.buffTrigger == TriggeredBy.ReceivingDamage);

                for (int i = 0; i < activateThisBuffs.Count; i++)
                {
                    switch (activateThisBuffs[i].targetStats)
                    {
                        case TargetStats.health:

                            break;
                        case TargetStats.damage: // decrease damage increae damage
                            amount += activateThisBuffs[i].effectAmount;
                            break;
                        case TargetStats.speed: 

                            break;
                        case TargetStats.range:

                            break;
                        case TargetStats.blockMelee:
                            if (!blockAttempt && attackType == UnitAttackType.MELEE)
                            {
                                blockAttempt = IsBlockAttemptSuccess(activateThisBuffs[i].effectAmount);
                            }
                            break;
                        case TargetStats.blockProjectile:
                            if(!blockAttempt && attackType == UnitAttackType.RANGE)
                            {
                                blockAttempt = IsBlockAttemptSuccess(activateThisBuffs[i].effectAmount);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if(blockAttempt)
            {
                switch (attackType)
                {
                    case UnitAttackType.MELEE:
                        mySfx.PlayBlockMelee();
                        break;
                    case UnitAttackType.RANGE:
                        mySfx.PlayBlockProjectile();
                        break;
                    case UnitAttackType.SPELL:
                        break;
                    default:
                        break;
                }

                return;
            }
            switch (attackType)
            {
                case UnitAttackType.MELEE:
                    mySfx.PlayReceiveMelee();
                    break;
                case UnitAttackType.RANGE:
                    mySfx.PlayReceiveProjectile();
                    break;
                case UnitAttackType.SPELL:
                    break;
                default:
                    break;
            }
            unitInformation.ReceiveDamage(amount, targetStats);

            //Debug.Log("[ DAMAGE RECEIVED : " + amount + " RECEIVED BY:"+ unitInformation.unitName + " ]");
            if(unitInformation.currentState == UnitState.Dead || unitInformation.currentState == UnitState.Injured)
            {
                UpdateCharacterState(CharacterStates.Injured_State);
                mySfx.PlayInjuredOrDead();
            }

            if(unitInformation.currentState != UnitState.Healthy)
            {
                if(BattlefieldSpawnManager.GetInstance != null)
                {
                    BattlefieldSpawnManager.GetInstance.RemoveThisUnit(this, unitInformation.currentState);
                }
            }
        }

        public bool IsBlockAttemptSuccess(float blockChance)
        {
            float rand = UnityEngine.Random.Range(0, 100);

            if(rand <= blockChance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ReturnThisUnit()
        {
            if(canReturnToCamp)
            {
                BattlefieldSpawnManager.GetInstance.RemoveThisUnit(this, unitInformation.currentState);
            }

            if(BattlefieldSceneManager.GetInstance != null)
            {
                BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();
            }
        }
        public void UpdateToPotentialNextState()
        {
            // Check if there's still enemy in range
            if (myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
            {
                UpdateCharacterState(CharacterStates.Attack_State);
            }
            // Check if Movement is still not done
            else if (myMovements.currentTargetPoint != null)
            {
                if (myMovements.currentPoint != myMovements.currentTargetPoint)
                {
                    myMovements.isMoving = true;
                    UpdateCharacterState(CharacterStates.Walking);
                }
            }
        }

        public void SpawnInThisPosition(ScenePointBehavior thisLocation, bool onOffset = true)
        {
            if(onOffset)
            {
                myMovements.SetPosition(thisLocation, true);
            }
            else
            {
                myMovements.SetPosition(thisLocation, true);
                myMovements.MoveTowards(thisLocation, false);
            }
        }

    }
}
