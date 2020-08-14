using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using System;

namespace Characters
{

    public class CharacterMovement : MonoBehaviour
    {
        public float distForReach = 0.0025f;
        public bool isTest = false;
        public bool isMoving = false;
        public float speed = 1.25f;
        public ScenePointBehavior spawnPoint;
        public ScenePointBehavior currentPoint;
        public ScenePointBehavior currentTargetPoint;
        public List<ScenePointBehavior> pathToTargetPoint = new List<ScenePointBehavior>();
        [SerializeField] private int pathIdx = 0;
        [SerializeField] public Vector2 targetPos;
        private BaseCharacter myCharacter;
        public List<ScenePointBehavior> queuePoints;

        List<Action> reachLastTargetCallback = null;
        public void Start()
        {
            if (GetComponent<BaseCharacter>() != null)
            {
                myCharacter = GetComponent<BaseCharacter>();
                speed = myCharacter.unitInformation.curSpeed;
            }

            if (pathToTargetPoint == null && currentTargetPoint != null)
            {
                targetPos = currentTargetPoint.transform.position;
            }
        }
        public void Update()
        {
            if(isTest)
            {
                Debug.Log(CheckDistance(currentTargetPoint.transform.position));
            }

            if(isMoving)
            {
                if (myCharacter != null)
                {
                    MovementWithMyCharacter();
                }
                else
                {
                    MovementWithoutMyCharacter();
                }
            }
        }

        public void MovementWithMyCharacter()
        {
            if (myCharacter.unitInformation.curhealth > 0)
            {
                // CHECK IF FINAL POINT IS ALREADY TARGET POINT - TO IDENTIFY OFFSET
                if (pathToTargetPoint != null && pathToTargetPoint.Count > 0)
                {
                    if(pathIdx < pathToTargetPoint.Count)
                    {
                        if (pathToTargetPoint[pathIdx] == currentTargetPoint)
                        {
                            if (pathToTargetPoint[pathIdx].straightToOffset)
                            {
                                targetPos = currentTargetPoint.sceneOffset.transform.position;
                            }
                        }
                    }
                }
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                float dist = Vector2.Distance(transform.position, targetPos);

                if (dist <= distForReach)
                {
                    if (pathToTargetPoint != null && pathToTargetPoint.Count > 0)
                    {
                        if (pathIdx < pathToTargetPoint.Count)
                        {
                            if (pathToTargetPoint[pathIdx].sceneOffset != null)
                            {
                                if (pathToTargetPoint[pathIdx] == currentTargetPoint)
                                {
                                    FinishPathMovement();
                                }
                            }
                        }
                        if (pathToTargetPoint == null || pathToTargetPoint.Count <= 0)
                        {
                            FinishPathMovement();
                            return;
                        }
                        if(pathIdx < pathToTargetPoint.Count)
                        {
                            currentPoint = pathToTargetPoint[pathIdx];
                        }
                        if (currentPoint == currentTargetPoint)
                        {
                            if (currentTargetPoint.sceneOffset != null)
                            {
                                targetPos = currentTargetPoint.sceneOffset.transform.position;
                                FinishPathMovement();
                            }
                            else
                            {
                                FinishPathMovement();
                            }
                        }
                        else
                        {
                            pathIdx += 1;
                            if (pathIdx < pathToTargetPoint.Count)
                            {
                                if(pathToTargetPoint[pathIdx] != null)
                                {
                                    targetPos = pathToTargetPoint[pathIdx].transform.position;

                                    myCharacter.UpdateMovementFacingDirection();
                                }
                                else
                                {
                                    FinishPathMovement();
                                }
                            }
                            else
                            {
                                FinishPathMovement();
                            }
                        }
                    }
                    else
                    {
                        if (currentPoint == currentTargetPoint)
                        {
                            FinishPathMovement();
                        }
                    }
                }
            }
        }
        public void MovementWithoutMyCharacter()
        {
            if (isMoving)
            {
                // CHECK IF FINAL POINT IS ALREADY TARGET POINT - TO IDENTIFY OFFSET
                if (pathToTargetPoint != null && pathToTargetPoint.Count > 0)
                {
                    if (pathToTargetPoint[pathIdx] == currentTargetPoint)
                    {
                        if (pathToTargetPoint[pathIdx].straightToOffset)
                        {
                            targetPos = currentTargetPoint.sceneOffset.transform.position;
                        }
                    }
                }
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                float dist = Vector2.Distance(transform.position, targetPos);

                if (dist < distForReach)
                {
                    if (pathToTargetPoint != null && pathToTargetPoint.Count > 0)
                    {
                        if (pathToTargetPoint[pathIdx].sceneOffset != null)
                        {
                            if (pathToTargetPoint[pathIdx] == currentTargetPoint)
                            {
                                FinishPathMovement();
                            }
                        }
                        if (pathToTargetPoint == null || pathToTargetPoint.Count <= 0)
                        {
                            if (currentTargetPoint != pathToTargetPoint[pathIdx])
                            {
                                FinishPathMovement();
                                return;
                            }
                        }
                        currentPoint = pathToTargetPoint[pathIdx];
                        if (currentPoint == currentTargetPoint)
                        {
                            if (currentTargetPoint.sceneOffset != null)
                            {
                                targetPos = currentTargetPoint.sceneOffset.transform.position;
                                FinishPathMovement();
                            }
                            else
                            {
                                FinishPathMovement();
                            }
                        }
                        else
                        {
                            pathIdx += 1;
                            if (pathToTargetPoint.Count > pathIdx && pathToTargetPoint[pathIdx] != null)
                            {
                                targetPos = pathToTargetPoint[pathIdx].transform.position;
                            }
                            else
                            {
                                FinishPathMovement();
                            }
                        }
                    }
                    else
                    {
                        if (currentPoint == currentTargetPoint)
                        {
                            FinishPathMovement();
                        }
                    }
                }
            }
        }

        public void FinishPathMovement()
        {
            if(!isMoving)
            {
                return;
            }

            isMoving = false;
            pathToTargetPoint.Clear();
            if (reachLastTargetCallback != null && reachLastTargetCallback.Count > 0)
            {
                for (int i = 0; i < reachLastTargetCallback.Count; i++)
                {
                    reachLastTargetCallback[i]();
                }
                reachLastTargetCallback.Clear();
            }
            pathIdx = 0;

            if (myCharacter != null)
            {
                myCharacter.UpdateCharacterState(CharacterStates.Idle);
                if (myCharacter.isFighting)
                {
                    myCharacter.ReturnThisUnit();
                }
            }
        }

        public Vector2 CheckDirection()
        {
            if(currentTargetPoint == null)
            {
                return Vector2.left;
            }
            else
            {
                if(currentTargetPoint.transform.position.x > transform.position.x)
                {
                    return Vector2.right;
                }
                else
                {
                    return Vector2.left;
                }
            }
        }

        /// <summary>
        /// Used to move to the next point in the current Path
        /// </summary>
        /// <param name="thisPoint">next Point in the current path</param>
        public void MoveTowards(ScenePointBehavior thisPoint, bool onOffset = true)
        {
            float dist = 0;

            if (!onOffset)
            {
                dist = Vector2.Distance(transform.position, thisPoint.transform.position);
                targetPos = thisPoint.gameObject.transform.position;
            }
            else
            {
                if(thisPoint.sceneOffset == null)
                {
                    dist = Vector2.Distance(transform.position, thisPoint.transform.position);
                    targetPos = thisPoint.gameObject.transform.position;
                }
                else
                {
                    dist = Vector2.Distance(transform.position, thisPoint.sceneOffset.transform.position);
                    targetPos = thisPoint.sceneOffset.transform.position;
                }
            }

            if (dist < 0.015f)
            {
                return;
            }

            isMoving = true;
        }

        public void SetPosition(ScenePointBehavior thisPoint, bool teleport = false)
        {
            currentPoint = thisPoint;
            //Debug.Log("Setting Position!");
            if (teleport)
            {
                spawnPoint = currentPoint;
                if(spawnPoint.sceneOffset == null)
                {
                    transform.position = spawnPoint.transform.position;
                }
                else
                {
                    transform.position = spawnPoint.sceneOffset.transform.position;
                }


                SetNewTargetPoint(thisPoint);
            }
        }

        public void SetNewTargetPoint(ScenePointBehavior newPoint)
        {
            //Debug.Log("Moving towards: " + newPoint.gameObject.name);
            if (currentTargetPoint != null)
            {
                currentTargetPoint.isInteractingWith = false;
            }
            currentTargetPoint = newPoint;

            if(currentTargetPoint.hasInteractionOptions)
            {
                currentTargetPoint.isInteractingWith = true;
            }
        }

        /// <summary>
        /// Creates a Path towards the said location
        /// </summary>
        /// <param name="thisPoint"> the target of the path</param>
        public void SetTarget(ScenePointBehavior thisPoint, Action targetCallBack = null)
        {
            if (reachLastTargetCallback == null)
            {
                reachLastTargetCallback = new List<Action>();
            }

            if(targetCallBack != null)
            {
                reachLastTargetCallback.Add(targetCallBack);
            }

            float targetDist = 0;

            if (thisPoint.straightToOffset)
            {
                targetDist = Vector2.Distance(transform.position, thisPoint.sceneOffset.transform.position);
            }
            else
            {
                targetDist = Vector2.Distance(transform.position, thisPoint.transform.position);
            }
            if (targetDist <= distForReach)
            {
                FinishPathMovement();
                return;
            }


            SetNewTargetPoint(thisPoint);
            pathIdx = 0;
            pathToTargetPoint.Clear();
            if (currentTargetPoint == currentPoint)
            {
                MoveTowards(thisPoint);
            }
            else
            {
                pathToTargetPoint = ScenePointPathfinder.GetInstance.ObtainScenePath(currentPoint, currentTargetPoint);
                targetPos = pathToTargetPoint[0].transform.position;
                //Debug.Log("Path:" + pathToTargetPoint[0].transform.position);
                isMoving = true;
            }

        }
        public void SetTarget(GameObject thisGameObject)
        {
            if (thisGameObject.GetComponent<ScenePointBehavior>() == null)
            {
                return;
            }
            else
            {
                currentTargetPoint = thisGameObject.GetComponent<ScenePointBehavior>();
            }
        }

        public float CheckDistance(Vector2 FromThisPosition)
        {
            float dist = Vector2.Distance(transform.position, FromThisPosition);

            return dist;
        }
    }
}