﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Buildings;
using Utilities;

namespace Managers
{
    public class InGameInteractionHandler : MonoBehaviour
    {
        public List<BaseInteractableBehavior> interactableList;

        public BaseInteractableBehavior currentlyInteractingWith;


        public void Awake()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.ENABLE_IN_GAME_INTERACTION, EnableAllInteraction);
            EventBroadcaster.Instance.AddObserver(EventNames.DISABLE_IN_GAME_INTERACTION, DisableAllInteraction);
        }

        public void OnDestroy()
        {
            if (EventBroadcaster.Instance != null)
            {
                EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ENABLE_IN_GAME_INTERACTION, EnableAllInteraction);
                EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.DISABLE_IN_GAME_INTERACTION, DisableAllInteraction);
            }
        }

        public void SetupInteractablesInformation()
        {
            for (int i = 0; i < interactableList.Count; i++)
            {
                interactableList[i].SetupInteractableInformation();
            }
        }

        public void SwitchInteractableClickables(bool newMode)
        {
            for (int i = 0; i < interactableList.Count; i++)
            {
                interactableList[i].isClickable = newMode;
            }
        }
        public void EnableAllInteraction(Parameters p = null)
        {
            for (int i = 0; i < interactableList.Count; i++)
            {
                interactableList[i].isClickable = true;
                interactableList[i].myCol.enabled = true;
            }
        }

        public void DisableAllInteraction(Parameters p = null)
        {
            for (int i = 0; i < interactableList.Count; i++)
            {
                interactableList[i].isClickable = false;
                interactableList[i].myCol.enabled = false;
            }
        }
    }
}