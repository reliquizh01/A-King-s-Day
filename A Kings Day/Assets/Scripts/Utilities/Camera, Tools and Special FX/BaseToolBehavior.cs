using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utilities
{
    public enum ToolAndEffectsType
    {
        Camera,
        VisualEffects,
        SoundEffects,
    }

    public class BaseToolBehavior : MonoBehaviour
    {
        public ToolAndEffectsType toolType;
        [Header("Movement Mechanics")]
        public float moveSpeed = 0.75f;
        public Vector3 targetPos;
        public bool isMoving = false;

        public Action afterCallback;
        public void Update()
        {
            if(isMoving)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

                if(Vector3.Distance(transform.position, targetPos) <= 0.01f)
                {
                    isMoving = false;
                    if(afterCallback != null)
                    {
                        afterCallback();
                    }
                }
            }
        }
        public void OrderMovement(Vector3 thisPos, Action callBack = null)
        {
            targetPos = thisPos;
            isMoving = true;
            afterCallback = callBack;
        }

        public void OrderReaction(Action callback = null)
        {
            callback();
        }
    }
}
