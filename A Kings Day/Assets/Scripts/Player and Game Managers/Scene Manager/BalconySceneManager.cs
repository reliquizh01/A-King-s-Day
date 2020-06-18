using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Utilities;

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
            if(TransitionManager.GetInstance != null)
            {
                Debug.Log("---------- SETTING NEW MANAGER!---------------");
                TransitionManager.GetInstance.SetAsNewScene();
                TransitionManager.GetInstance.currentSceneManager = this;
            }
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
