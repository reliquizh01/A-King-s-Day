using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kingdoms;
using TMPro;
using Utilities;
using System;

namespace KingEvents
{
    public class CardHandler : MonoBehaviour
    {
        public CardsEventController myController;
        public List<CardOption> cardsList;
        public CardOption currentCard;
        
        public void ShowNewEvent(EventDecisionData thisEvent)
        {
            CardOption thisCard = cardsList.Find(x => x.thisCardType == thisEvent.eventType);


            if (thisCard != null)
            {
                thisCard.gameObject.SetActive(true);
                thisCard.InitializeText(thisEvent.title, thisEvent.description);
                currentCard = thisCard;
                currentCard.ShowCardAnim();
            }
        }

        public void HideCurrentCard(Action callback = null)
        {
            StartCoroutine(HideAnimCard(currentCard, callback));
        }

        IEnumerator HideAnimCard(CardOption thisCard, Action callback = null)
        {
            thisCard.myPanel.PlayCloseAnimation();
            yield return new WaitForSeconds(thisCard.myPanel.myAnim.GetClip(thisCard.myPanel.closeAnimationName).length);
            currentCard.gameObject.SetActive(false);
            currentCard = null;

            if(callback != null)
            {
                callback();
            }

            RemoveCurrentEvent();
        }

        public void RemoveCurrentEvent()
        {
            myController.currentEvent = new EventDecisionData();
        }

    }
}
