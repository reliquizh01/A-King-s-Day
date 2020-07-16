using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CreationSceneManager : BaseSceneManager
    {

        public void Awake()
        {
            if (TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsNewSceneManager(this);
            }
        }
        public override void PreOpenManager()
        {
            base.PreOpenManager();
            Loaded = true;
        }
    }

}
