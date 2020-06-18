using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Utilities;
using Managers;
using System;

namespace KingEvents
{
    public class CardsEventController : MonoBehaviour
    {
        [SerializeField] DecisionHandler decisionHandler = null;
        [SerializeField] CardHandler cardHandler = null;

        public EventDecisionData currentEvent;
        public List<EventDecisionData> queuedEvents;

        [HideInInspector] public int idxClicked = 0;
        public void Start()
        {
            cardHandler.myController = this;
        }
        public void ReceiveEvent(EventDecisionData thisEvent)
        {
            if(!string.IsNullOrEmpty(currentEvent.title))
            {
                queuedEvents.Add(thisEvent);
            }
            else
            {
                currentEvent = thisEvent;
                ShowCurrentEvent();
            }
        }

        public void ShowCurrentEvent()
        {
            // Initialize Card
            cardHandler.ShowNewEvent(currentEvent);
            EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
            StartCoroutine(cardHandler.currentCard.myPanel.WaitAnimationForAction(cardHandler.currentCard.myPanel.openAnimationName,ShowDecisions));
        }
        public void ShowDecisions()
        {
            decisionHandler.SetupDecisions(currentEvent);
        }
        public void HideCurrentEvent(int thisIdx)
        {
            Action tmp = () => KingdomManager.GetInstance.RewardEvent(currentEvent, thisIdx);

            idxClicked = thisIdx;
            // Play all Exit Animation Here
            cardHandler.HideCurrentCard(tmp);
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);
            decisionHandler.HideCurrentDecisions();
        }

    }
}
