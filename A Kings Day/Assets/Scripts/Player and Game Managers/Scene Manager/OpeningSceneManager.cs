using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Managers
{

    public class OpeningSceneManager : BaseSceneManager
    {
        #region Singleton
        private static OpeningSceneManager instance;
        public static OpeningSceneManager GetInstance
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

        public override void Start()
        {
            base.Start();

            if (TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsNewSceneManager(this);

                if (TransitionManager.GetInstance.previousScene != sceneType)
                {
                    SetPositionFromTransition(TransitionManager.GetInstance.previousScene);
                }
            }
        }
    }
}