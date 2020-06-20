using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
using ResourceUI;

namespace Buildings
{
    public class CardInformationHandler : MonoBehaviour
    {
        public BuildingInformationHandler myController;
        public BuildingType buildingType;
        public int currentCardIdx;

        public virtual void SetupCardInformation(Parameters p = null)
        {

        }

        public virtual void ChangeCardAction(int idx)
        {

        }
        public virtual void HideCardInformation()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void InitializeCurrentPanel()
        {

        }

        /// <summary>
        /// This Functions should show potential changes that should occure on a sub panel
        /// </summary>
       
        public virtual void ShowActionPotential(int idx)
        {

        }

        public virtual void HideAllPotential()
        {

        }
        public virtual void UpdateCurrentPanel()
        {

        }

    }
}