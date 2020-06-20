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
    public class FarmInformationHandler : CardInformationHandler
    {
        public List<SubInformationHandler> subInformationHandler;
        public SubInformationHandler currentInformationHandler;
        public override void SetupCardInformation(Parameters p = null)
        {
            base.SetupCardInformation(p);

        }


        public override void HideCardInformation()
        {
            base.HideCardInformation();
            for (int i = 0; i < subInformationHandler.Count; i++)
            {
                subInformationHandler[i].gameObject.SetActive(false);
            }
        }
        public override void ChangeCardAction(int idx)
        {
            base.ChangeCardAction(idx);

            Debug.Log("Opening Action Card : " + idx);
            for (int i = 0; i < subInformationHandler.Count; i++)
            {
                if (i == idx)
                {
                    Debug.Log("Opening Index : " + i);
                    myController.HideInfoBlocker();
                    subInformationHandler[i].gameObject.SetActive(true);
                    currentInformationHandler = subInformationHandler[i];
                }
                else
                {
                    subInformationHandler[i].gameObject.SetActive(false);
                }
            }
            InitializeCurrentPanel();
        }

        public override void InitializeCurrentPanel()
        {
            base.InitializeCurrentPanel();
            if(currentInformationHandler == subInformationHandler[0])
            {
                SetupFarmersPanel();
            }
            else if(currentInformationHandler == subInformationHandler[1])
            {
                SetupHerdsmanPanel();
            }
            else if(currentInformationHandler == subInformationHandler[2])
            {
                SetupStorageKeeperPanel();
            }
        }

        public void SetupFarmersPanel()
        {

        }
        public void SetupHerdsmanPanel()
        {

        }
        public void SetupStorageKeeperPanel()
        {

        }
    }
}