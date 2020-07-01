using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using KingEvents;
using Managers;
using System;

namespace Characters
{
    public enum AnimationState
    {
        Standing_Idle,
        Sitting_Idle,
        Stand_up,
        Walk_up,
        Walk_down,
        Walk_Left,
        Walk_right,
    }

    [RequireComponent(typeof(Animator), typeof(CharacterMovement))]
    public class BaseCharacter : MonoBehaviour
    {
        public Animator myAnimController;
        public CharacterMovement myMovements;
        public bool isKing = false;
        public bool isMainCharacter = false;
        public bool isGuest = false;
        public bool isLeaving = false;


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

        public void OrderMovement(ScenePointBehavior thisLocation, Action callBack = null)
        {
            myMovements.isMoving = false;
            myMovements.SetTarget(thisLocation, callBack);
        }

        public void SpawnInThisPosition(ScenePointBehavior thisLocation)
        {
            myMovements.SetPosition(thisLocation, true);
        }

    }
}
