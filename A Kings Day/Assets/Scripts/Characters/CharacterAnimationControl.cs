using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Down,
        Up,
        Left,
        Right,
    }
    public class CharacterAnimationControl : MonoBehaviour
    {
        public Animator myAnimator;
        public int currentState; // 0 - Idle | 1 - Walking | 2 - Damage Received | 3 - Attack State |  4 - Injured State
        public float facingDirection = 0.0f; // 0 - Down | 1 - Up | 2 - Left | 3 - Right


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

            UpdateAnimator(currentState, facingDirection);
        }
        public void ChangeFacingDireciton(FacingDirection newDirection)
        {
            switch (newDirection)
            {
                case FacingDirection.Down:
                    facingDirection = 0;
                    break;
                case FacingDirection.Up:
                    facingDirection = 1;
                    break;
                case FacingDirection.Left:
                    facingDirection = 2;
                    break;
                case FacingDirection.Right:
                    facingDirection = 3;
                    break;

                default:
                    break;
            }

            UpdateAnimator(currentState, facingDirection);
        }

        public void UpdateDeath(bool newDeathState)
        {
            myAnimator.SetBool("Death", newDeathState);
        }
        public void UpdateAnimator(int state, float direction)
        {
            myAnimator.SetInteger("Current State", state);
            myAnimator.SetFloat("Facing Direction", direction);
        }
    }
}
