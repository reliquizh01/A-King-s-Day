using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Buildings;
using Utilities;

namespace Managers
{
    public enum SceneType
    {
        Opening,
        Creation,
        Courtroom,
        Balcony,
        Battlefield,
    }
    public class BaseSceneManager : BaseManager
    {
        public bool testRun;
        public SceneType sceneType;
        public BuildingOperationStorage buildingInformationStorage;

        public ScenePathfindingHandler scenePointHandler;
        public BaseCharacter player;
        public InGameInteractionHandler interactionHandler;
        public override void Start()
        {
            base.Start();
        }

        public override void StartManager()
        {
            base.StartManager();
        }
        public virtual void SetPositionFromTransition(SceneType prevScene, bool directToOffset = true)
        {

        }

        public ScenePointBehavior ObtainScenePoint(string pointName)
        {
            Debug.Log("Current Point Count:" + scenePointHandler.scenePoints.Count);
            return scenePointHandler.scenePoints.Find(x => x.gameObject.name == pointName);
        }
    }

}