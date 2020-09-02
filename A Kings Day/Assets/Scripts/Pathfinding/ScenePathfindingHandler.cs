using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class ScenePathfindingHandler : MonoBehaviour
{
    public List<ScenePointBehavior> scenePoints;

    public void SwitchScenePointsInteraction(bool switchTo)
    {
        if(scenePoints != null && scenePoints.Count > 0)
        {
            for (int i = 0; i < scenePoints.Count; i++)
            {
                scenePoints[i].isClickable = switchTo;
            }
        }
    }
}
