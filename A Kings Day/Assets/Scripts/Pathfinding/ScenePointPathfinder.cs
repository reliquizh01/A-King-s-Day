using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Utilities;


public class PathfinderScout
{
    public ScenePointBehavior currentScoutPoint;
    public List<ScenePointBehavior> pathTaken;
    public bool pathFinished = false;
}
public class ScenePointPathfinder : MonoBehaviour
{
    #region Singleton
    private static ScenePointPathfinder instance;
    public static ScenePointPathfinder GetInstance
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        instance = this;
    }
    #endregion

    public List<ScenePointBehavior> currentScenePoints = new List<ScenePointBehavior>();
    public int currentScene = 0;
    public List<PathfinderScout> latestScouts;
    public PathfinderScout finalScout;


    public void Start()
    {

    }
    public void AddCurrentScenePoints(ScenePointBehavior thisPoint)
    {
        if(currentScene != thisPoint.sceneIndex)
        {
            currentScenePoints.Clear();
            currentScene = thisPoint.sceneIndex;
        }

        currentScenePoints.Add(thisPoint);
    }

    public List<ScenePointBehavior> ObtainScenePath(ScenePointBehavior startingPoint, ScenePointBehavior endPoint)
    {
        List<ScenePointBehavior> finalPath = new List<ScenePointBehavior>();
        List<PathfinderScout> scoutsReleased = new List<PathfinderScout>();

        // Send All Scout to Starting Point Neighbors.
        for (int i = 0; i < startingPoint.neighborPoints.Count; i++)
        {
            PathfinderScout tmp = new PathfinderScout();
            tmp.pathTaken = new List<ScenePointBehavior>();
            if(!tmp.pathTaken.Contains(startingPoint.neighborPoints[i]))
            {
              //  Debug.Log("[INITIAL SCOUT " + i + "] is Released To : " + startingPoint.neighborPoints[i].gameObject.name);
                // Add Neighbor Point - To Set it as 0
                if(startingPoint.neighborPoints[i] != null)
                {
                    tmp.pathTaken.Add(startingPoint.neighborPoints[i]);
                }
                tmp.currentScoutPoint = tmp.pathTaken[tmp.pathTaken.Count-1];
            }
            if(tmp.pathTaken != null && tmp.pathTaken.Count > 0)
            {
                scoutsReleased.Add(tmp);
            }
        }

        if(scoutsReleased.Count > 0)
        {
            StartCoroutine(UpdateScouts(scoutsReleased, endPoint));

            if(finalScout != null && finalScout.pathTaken != null)
            {
                finalPath = finalScout.pathTaken;
            }
           // Debug.Log("---------Final Path---------");
           // for (int i = 0; i < finalPath.Count; i++)
           // {
           //     Debug.Log("[Path " + i + "] " + finalPath[i].gameObject.name);
           // }
            
        }

        return finalPath;
    }
    IEnumerator UpdateScouts(List<PathfinderScout> scouts, ScenePointBehavior targetPoint)
    {
        List<PathfinderScout> newScoutsReleased = new List<PathfinderScout>();

        finalScout = scouts.Find(x => x.currentScoutPoint == targetPoint);
        if (finalScout == null)
        {
            // Check Every Scout
            for (int i = 0; i < scouts.Count; i++)
            {
                //Debug.Log("[CHECKING CURRENT SCOUT " + i + " NEIGHBORS | NEIGHBOR COUNT : " + scouts[i].currentScoutPoint.neighborPoints.Count);
                // Check if Current Scout Has Neighbors 
                if (scouts[i].currentScoutPoint.neighborPoints != null && scouts[i].currentScoutPoint.neighborPoints.Count > 0)
                {
                    //Debug.Log("----------- CHECKING PER NEIGHBORS -----------");
                    // Then for ever neighbors release a scout
                    for (int x = 0; x < scouts[i].currentScoutPoint.neighborPoints.Count; x++)
                    {
                        PathfinderScout temp = new PathfinderScout();
                        temp.pathTaken = new List<ScenePointBehavior>();
                        ////Debug.Log("[SCOUT " + x + "] is Released To : "
                            ////+ scouts[i].currentScoutPoint.neighborPoints[x].gameObject.name
                            ////+ " COPIED FROM : " + i + " path taken Count: " + scouts[i].pathTaken.Count);
                        // Copy Previous Path, so it would look like a step-by-step pathing
                        temp.pathTaken.AddRange(scouts[i].pathTaken);
                        // Then Add the neighbor
                        //Debug.Log("WHO : " + scouts[i].currentScoutPoint.neighborPoints[x].gameObject.name);
                        if(!temp.pathTaken.Contains(scouts[i].currentScoutPoint.neighborPoints[x]))
                        {
                            ScenePointBehavior addNew = scouts[i].currentScoutPoint.neighborPoints[x];
                            temp.pathTaken.Add(addNew);
                        }
                        // Then Set the last pathTaken Index as its current Neighbor point
                        temp.currentScoutPoint = temp.pathTaken[temp.pathTaken.Count - 1];

                        if(temp.currentScoutPoint == targetPoint)
                        {
                            //Debug.Log("Final Scout is Number ["+x+"] Path Count is :" + temp.pathTaken.Count);
                            //Debug.Log("---------------------------- END ONE LOOP ------------------------");
                            finalScout = temp;
                            break;
                        }
                        else
                        {
                            //Debug.Log("ADDING SOMEONE!");
                            newScoutsReleased.Add(temp);
                        }
                    }
                }
                if(finalScout != null)
                {
                    break;
                }
            }

            if (newScoutsReleased.Count > 0 && finalScout == null)
            {
                //Debug.Log("[NEW SCOUTS RELEASED | COUNT : " + newScoutsReleased.Count);
                //Debug.Log("Releasing New Sets of Scouts!");
                yield return StartCoroutine(UpdateScouts(newScoutsReleased, targetPoint));
            }
            else
            {
                //Debug.Log("Final Scout Found!");
                yield return null;
            }
        }
        yield return null;
    }

    public ScenePointBehavior ObtainNearestScenePoint(Vector2 thisPoint)
    {
        ScenePointBehavior tmp = new ScenePointBehavior();
        float distToBeat = 0;
        int pointIdx = -1;

        for (int i = 0; i < currentScenePoints.Count; i++)
        {
            float curDist = Vector2.Distance(thisPoint, currentScenePoints[i].transform.position);
            if(i == 0)
            {
                distToBeat = curDist;
                pointIdx = i;
            }
            else if (curDist < distToBeat)
            {
                distToBeat = curDist;
                pointIdx = i;
            }
        }
        tmp = currentScenePoints[pointIdx];
        return tmp;
    }

    public ScenePointBehavior ObtainNearestScenePoint(string pointName)
    {
        Debug.Log("Current Point Count:" + currentScenePoints.Count);
        return currentScenePoints.Find(x => x.gameObject.name == pointName);
    }
}
