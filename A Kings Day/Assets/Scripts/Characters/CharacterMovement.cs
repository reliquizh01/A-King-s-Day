using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
public class CharacterMovement : MonoBehaviour
{
    public bool isMoving = false;
    public float speed = 1.25f;
    public ScenePointBehavior currentPoint;
    public ScenePointBehavior currentTargetPoint;
    public List<ScenePointBehavior> pathToTargetPoint = new List<ScenePointBehavior>();
    [SerializeField] private int pathIdx = 0;
    [SerializeField] private Vector2 targetPos;
    private BaseCharacter myCharacter;
    public List<ScenePointBehavior> queuePoints;
    public void Start()
    {
        if(GetComponent<BaseCharacter>() != null)
        {
            myCharacter = GetComponent<BaseCharacter>();
        }

        if(currentTargetPoint != null)
        {
            targetPos = currentTargetPoint.transform.position;
        }
    }
    public void Update()
    {
        if(isMoving)
        {
            if(pathToTargetPoint != null && pathToTargetPoint.Count > 0)
            {
                if(pathToTargetPoint[pathIdx] == currentTargetPoint)
                {
                    if(pathToTargetPoint[pathIdx].straightToOffset)
                    {
                        targetPos = currentTargetPoint.sceneOffset.transform.position;
                    }
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            float dist = Vector2.Distance(transform.position, targetPos);

            if (dist < 0.0015f)
            {
                if(pathToTargetPoint != null && pathToTargetPoint.Count > 0)
                {
                    if(pathToTargetPoint[pathIdx].sceneOffset != null)
                    {
                        if(pathToTargetPoint[pathIdx] == currentTargetPoint)
                        {
                            isMoving = false;
                        }
                    }
                    if (pathToTargetPoint == null || pathToTargetPoint.Count <= 0)
                    {
                        if (currentTargetPoint != pathToTargetPoint[pathIdx])
                        {
                            isMoving = false;
                            PointReachedCallback();
                            pathIdx = 0;
                            return;
                        }
                    }
                    currentPoint = pathToTargetPoint[pathIdx];
                    if(currentPoint == currentTargetPoint)
                    {
                        if(currentTargetPoint.sceneOffset != null)
                        {
                            targetPos = currentTargetPoint.sceneOffset.transform.position;
                            PointReachedCallback();
                            isMoving = false;
                            pathIdx = 0;
                        }
                        else
                        {
                            isMoving = false;
                            PointReachedCallback();
                            pathIdx = 0;
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

                            isMoving = false;
                            PointReachedCallback();
                            pathIdx = 0;
                        }
                    }
                }
                else
                {
                    if(currentPoint == currentTargetPoint)
                    {
                        isMoving = false;
                        pathToTargetPoint.Clear();
                        PointReachedCallback();
                        pathIdx = 0;
                    }
                }
            }
        }
    }

    public void PointReachedCallback()
    {
        if(myCharacter.isGuest)
        {
            myCharacter.GuestReached();
        }
        else
        {
            myCharacter.OrderMovementCallback();
        }
    }
    public void MoveTowards(ScenePointBehavior thisPoint)
    {
        float dist = 0;
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

        if(dist < 0.015f)
        {
            return;
        }

        isMoving = true;
    }

    public void SetPosition(ScenePointBehavior thisPoint, bool teleport = false)
    {
        currentPoint = thisPoint;
        Debug.Log("Setting Position!");
        if(teleport)
        {
            transform.position = thisPoint.transform.position;
            SetNewTargetPoint(thisPoint);
        }
    }

    public void SetNewTargetPoint(ScenePointBehavior newPoint)
    {
        Debug.Log("Moving towards: " + newPoint.gameObject.name);
        if (currentTargetPoint != null)
        {
            currentTargetPoint.isInteractingWith = false;
        }
        currentTargetPoint = newPoint;
        currentTargetPoint.isInteractingWith = true;
    }

    public void SetTarget(ScenePointBehavior thisPoint)
    {
        SetNewTargetPoint(thisPoint);

        if (currentTargetPoint == currentPoint)
        {
            if(currentTargetPoint.sceneLoader && myCharacter.isKing)
            {

                MoveTowards(thisPoint);
            }
        }
        else
        {
            pathToTargetPoint = ScenePointPathfinder.GetInstance.ObtainScenePath(currentPoint, currentTargetPoint);
            targetPos = pathToTargetPoint[0].transform.position;
            isMoving = true;
        }

    }
    public void SetTarget(GameObject thisGameObject)
    {
        if(thisGameObject.GetComponent<ScenePointBehavior>() == null)
        {
            return;
        }
        else
        {
            currentTargetPoint = thisGameObject.GetComponent<ScenePointBehavior>();
        }
    }
}
