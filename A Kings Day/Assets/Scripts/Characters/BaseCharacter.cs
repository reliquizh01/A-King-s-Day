using System.Collections;
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

        public bool isKing = false;
        public bool isMainCharacter = false;
        public bool isGuest = false;
        public bool isLeaving = false;

        [Header("Unit Information")]
        public UnitInformationData unitInformation;
        public bool isFighting;
        public bool canReturnToCamp;
        public TeamType teamType;
        
        private Action reachedCallback;

        public void Start()
        {
            if(isKing)
            {
                isMainCharacter = true;
            }
        }

        public void Update()
        {

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
            unitInformation.buffList = newUnitInformation.buffList;
            unitInformation.currentState = newUnitInformation.currentState;

            myMovements.speed = unitInformation.RealSpeed / 10.0f;
        }


        public void OrderMovement(ScenePointBehavior thisLocation, Action callBack = null)
        {
            myMovements.isMoving = false;
            myMovements.SetTarget(thisLocation, callBack);

            UpdateMovementFacingDirection();
            UpdateCharacterState(CharacterStates.Walking);
        }

        public void OrderToFace(FacingDirection newDirection)
        {
            myAnimation.ChangeFacingDireciton(newDirection);
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
                    if(curState == CharacterStates.Damage_Received)
                    {
                        break;
                    }

                    myAnimation.ChangeState(newState);
                    break;

                case CharacterStates.Damage_Received:
                    if(curState == CharacterStates.Attack_State)
                    {
                        break;
                    }

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

            if(unitInformation.attackType == UnitAttackType.MELEE)
            {
                if (myRange.enemiesInRange != null && myRange.enemiesInRange.Count > 0)
                {
                    myRange.enemiesInRange[0].ReceiveDamage(unitInformation.RealDamage);

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

            }
            else if(unitInformation.attackType == UnitAttackType.SPELL)
            {

            }
        }

        public void ReceiveDamage(float amount)
        {
            unitInformation.ReceiveDamage(amount);

            Debug.Log("[ DAMAGE RECEIVED : " + amount + " RECEIVED BY:"+ unitInformation.unitName + " ]");
            if(unitInformation.currentState == UnitState.Dead || unitInformation.currentState == UnitState.Injured)
            {
                UpdateCharacterState(CharacterStates.Injured_State);
            }

            if(unitInformation.currentState != UnitState.Healthy)
            {
                if(BattlefieldSpawnManager.GetInstance != null)
                {
                    BattlefieldSpawnManager.GetInstance.RemoveThisUnit(this, unitInformation.currentState);
                }
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

        public void SpawnInThisPosition(ScenePointBehavior thisLocation)
        {
            myMovements.SetPosition(thisLocation, true);
        }

    }
}
