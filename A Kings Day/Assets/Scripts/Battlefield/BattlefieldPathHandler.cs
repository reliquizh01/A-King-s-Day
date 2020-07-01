using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldPathHandler : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<ScenePointBehavior> scenePoints;
    public ScenePointBehavior attackerSpawnPoint;
    public ScenePointBehavior defenderSpawnPoint;


    public GameObject attackerSpawnArrow;
    public void Start()
    {
        SetupScenePoints();
    }

    public void SetupScenePoints()
    {
        attackerSpawnPoint.neighborPoints.Add(scenePoints[0]);
        defenderSpawnPoint.neighborPoints.Add(scenePoints[scenePoints.Count - 1]);

        for (int i = 0; i < scenePoints.Count; i++)
        {
            if((i-1) >= 0)
            {
                scenePoints[i].neighborPoints.Add(scenePoints[i - 1]);
            }
            if((i + 1) < scenePoints.Count)
            {
                scenePoints[i].neighborPoints.Add(scenePoints[i + 1]);
            }
            scenePoints[i].battleTile.myController = this;
        }
    }

    public void ShowSpawnArrow()
    {
        attackerSpawnArrow.SetActive(true);
    }

    public void HideSpawnArrow()
    {
        attackerSpawnArrow.SetActive(false);
    }
}
