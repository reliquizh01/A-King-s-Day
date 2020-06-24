using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Utilities;
using Kingdoms;


namespace Managers
{
    public class BalconySceneManager : BaseSceneManager
    {
        #region Singleton
        private static BalconySceneManager instance;
        public static BalconySceneManager GetInstance
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

        public ScenePointBehavior balconyPoint;

        public override void Start()
        {
            base.Start();

           
            TransitionManager.GetInstance.SetAsNewSceneManager(this);
        }

        public void Update()
        {

        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
            king.OrderMovement(balconyPoint, StartManager);
        }
        public override void StartManager()
        {
            base.StartManager();

            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;
            if(!playerData.balconyBuildingsAdded)
            {
                if(playerData.buildingInformationData == null)
                {
                    playerData.buildingInformationData = new List<BuildingSavedData>();
                }
                for (int i = 0; i < buildingInformationStorage.buildingOperationList.Count; i++)
                {
                    BuildingSavedData tmp = new BuildingSavedData();
                    tmp.buildingName = buildingInformationStorage.buildingOperationList[i].BuildingName;
                    tmp.buildingType = buildingInformationStorage.buildingOperationList[i].buildingType;
                    tmp.buildingLevel = buildingInformationStorage.buildingOperationList[i].buildingLevel;
                    tmp.buildingCondition = buildingInformationStorage.buildingOperationList[i].buildingCondition;

                    playerData.buildingInformationData.Add(tmp);
                }
                playerData.balconyBuildingsAdded = true;
            }

            interactionHandler.SetupInteractablesInformation();
        }
        public override void OrderCharacterToMove(ScenePointBehavior toThisPoint)
        {
            base.OrderCharacterToMove(toThisPoint);
            if(toThisPoint.sceneLoader)
            {
                king.OrderMovement(toThisPoint, () => TransitionManager.GetInstance.LoadScene(toThisPoint.SceneToLoad));
            }
            else
            {
                king.OrderMovement(toThisPoint);
            }
        }
    }
}
