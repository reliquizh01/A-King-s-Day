using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;

namespace Characters
{
    public enum CharacterStates
    {
        Idle,
        Walking,
        Damage_Received,
        Attack_State,
        Injured_State,
    }

    public enum FacingDirection
    {
        Up,
        Down,
        Right,
        Left,
    }
    public class CharacterAnimationControl : MonoBehaviour
    {
        public SpriteRenderer characterSprite;
        public Animator myAnimator;
        public int currentState; // 0 - Idle | 1 - Walking | 2 - Damage Received | 3 - Attack State |  4 - Injured State
        public float facingDirection = 0.0f; // 0 - Up | 1 - Down | 2 - Left | 3 - Right

        public bool paramCurrentStateAvailable;
        public bool paramFacingDirectionAvailable;

        public void Awake()
        {
            paramCurrentStateAvailable = HasParameter("Current State", myAnimator);
            paramFacingDirectionAvailable = HasParameter("Facing Direction", myAnimator);
        }
        public void Start()
        {

        }
        public void ChangeState(CharacterStates newState)
        {
            switch (newState)
            {
                case CharacterStates.Idle:
                    currentState = 0;
                    break;
                case CharacterStates.Walking:
                    currentState = 1;
                    break;
                case CharacterStates.Damage_Received:
                    currentState = 2;
                    break;
                case CharacterStates.Attack_State:
                    currentState = 3;
                    break;
                case CharacterStates.Injured_State:
                    currentState = 4;
                    break;
                default:
                    break;
            }

            UpdateStateAnimator(currentState);
        }
        public void ChangeFacingDirection(FacingDirection newDirection)
        {
            bool changeMade = false;

            if (facingDirection != (float)newDirection)
            {
                changeMade = true;
            }

            facingDirection = (float)newDirection;

            if(changeMade)
            {
                UpdateDirectionAnimator(facingDirection);
            }
        }

        public void UpdateDeath(bool newDeathState)
        {
             myAnimator.SetBool("Death", newDeathState);
        }
        public void UpdateStateAnimator(int state)
        {
            if(myAnimator == null)
            {
                return;
            }
            if(paramCurrentStateAvailable)
            {
                myAnimator.SetInteger("Current State", state);
            }
        }

        public void UpdateDirectionAnimator(float direction)
        {
            if (paramFacingDirectionAvailable)
            {
                myAnimator.SetFloat("Facing Direction", direction);
            }
        }

        public void UpdateWieldedWeapon(WieldedWeapon wieldedWeapon)
        {
            myAnimator.SetFloat("Weapon Wielded", (float)wieldedWeapon);
        }
        public void UpdateBanish(bool newBanishState)
        {
            myAnimator.SetBool("Banished", newBanishState);
        }
        public bool HasParameter(string paramName, Animator animator)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
    }
}
