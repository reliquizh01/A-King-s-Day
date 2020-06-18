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
        public CharacterData charData;
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
            if(Input.GetKeyDown(KeyCode.Q))
            {
                if(reachedCallback != null)
                {
                    Debug.Log("There's 1");
                }
                else
                {
                    Debug.Log("Callback Empty!");
                }
            }
        }
        // Use For Guests and Unique Characters
        public void SetGuestInformation(ReporterType reporter, bool isUnique = false)
        {
            if(isUnique)
            {
                // Obtain Data From Name Storage.
            }
            else
            {
                charData.characterName = reporter.ToString();
            }
            isGuest = true;
        }

        public void GuestReached()
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
        }

        public void OrderMovement(ScenePointBehavior thisLocation, Action callBack = null)
        {
            myMovements.isMoving = false;
            myMovements.SetTarget(thisLocation);
            
            if(callBack != null)
            {
                reachedCallback = callBack;
            }
        }
        public void OrderMovementCallback()
        {
            if(reachedCallback != null)
            {
                reachedCallback();
                reachedCallback = null;
            }
        }
    }
}
