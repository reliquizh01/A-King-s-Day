using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Utilities;
using Kingdoms;
using System;
using Kingdoms;
using Managers;

namespace GameResource
{
    [Serializable]
    public class WarningMessageClass
    {
        public bool showWarning;
        public ResourceType dependent;
        public string message;
        [Header("Warning Mechanics")]
        public bool showOnZero;
        public bool showOnSurpassing;
    }

    public class BaseResourceBehavior : MonoBehaviour
    {
        public PlayerKingdomData curPlayer;

        [Header("Dependent List")]
        public List<WarningMessageClass> warningDependentList;

        [Header("Resource Mechanics")]
        public ResourceType resourceType;
        
        public virtual void SetupResourceBehavior()
        {

        }
        public virtual void UpdateWeeklyProgress()
        {
            UpdateWarningMechanics();

        }
        public virtual void ImplementTechnology()
        {

        }

        public virtual void UpdateWarningMechanics()
        {
            for (int i = 0; i < warningDependentList.Count; i++)
            {
                int warningAmount = 0;

                // if its shown only if surpassing
                if (warningDependentList[i].showOnSurpassing)
                {
                    int curAmount = curPlayer.ObtainResourceAmount(resourceType);
                    warningAmount = curPlayer.ObtainResourceAmount(warningDependentList[i].dependent);
                    warningDependentList[i].showWarning = (curAmount > warningAmount);
                }
                else if (warningDependentList[i].showOnZero)
                {
                    warningAmount = PlayerGameManager.GetInstance.playerData.ObtainResourceAmount(warningDependentList[i].dependent);
                    warningDependentList[i].showWarning = (warningAmount <= 0);
                }
            }
        }
    }
}
