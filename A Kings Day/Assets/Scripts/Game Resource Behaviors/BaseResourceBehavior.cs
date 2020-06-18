using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Utilities;
using Kingdoms;

namespace GameResource
{
    public class BaseResourceBehavior : MonoBehaviour
    {
        public ResourceType resourceType;
        public PlayerKingdomData curPlayer;


        public virtual void SetupResourceBehavior()
        {

        }
        public virtual void UpdateWeeklyProgress()
        {

        }
        public virtual void ImplementTechnology()
        {

        }

    }
}
