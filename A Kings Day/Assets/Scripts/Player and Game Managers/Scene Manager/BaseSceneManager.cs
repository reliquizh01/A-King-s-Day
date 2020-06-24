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
        Courtroom,
        Balcony,
    }
    public class BaseSceneManager : BaseManager
    {
        public SceneType sceneType;
        public BuildingOperationStorage buildingInformationStorage;

        public ScenePathfindingHandler scenePointHandler;
        public BaseCharacter king;
        public InGameInteractionHandler interactionHandler;
        public override void Start()
        {
            base.Start();
        }
        public virtual void OrderCharacterToMove(ScenePointBehavior toThisPoint)
        {
            if(toThisPoint.sceneLoader)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
            }
        }

        public virtual void SetPositionFromTransition(SceneType prevScene)
        {

        }
    }

}