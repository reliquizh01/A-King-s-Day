using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using ResourceUI;

namespace KingEvents
{
    /// <summary>
    /// Call forth any additional animation.
    /// </summary>

    public class DecisionHandler : MonoBehaviour
    {
        public CardsEventController myController;
        public List<DecisionOption> decisionOptionsList;

        public bool enableDelay = true;
        int activeDecisionCount = 0;
        public void SetupDecisions(EventDecisionData thisEvent)
        {
            activeDecisionCount = thisEvent.eventDecision.Count;
            for (int i = 0; i < decisionOptionsList.Count; i++)
            {
                if(i < thisEvent.eventDecision.Count)
                {
                    decisionOptionsList[i].AddDescription(thisEvent.eventDecision[i]);

                    if (enableDelay)
                    {
                            StartCoroutine(DelayActivationDecision(i));

                    }
                    else
                    {
                        decisionOptionsList[i].gameObject.SetActive(true);
                        decisionOptionsList[i].isClickable = true;
                    }

                }
                else
                {
                    decisionOptionsList[i].gameObject.SetActive(false);
                }
            }
        }

        public IEnumerator DelayActivationDecision(int idx)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.015f + (idx * 0.02f));
            decisionOptionsList[idx].gameObject.SetActive(true);
            StartCoroutine(decisionOptionsList[idx].myPanel.WaitAnimationForAction(decisionOptionsList[idx].myPanel.openAnimationName, CheckAllAnimationFinished));
        }
        public void CheckAllAnimationFinished()
        {
            bool activateClikables = true;

            for (int i = 0; i < activeDecisionCount; i++)
            {
                if(decisionOptionsList[i].myPanel.isPlaying)
                {
                    activateClikables = false;
                    break;
                }
            }

            if(activateClikables)
            {
                for (int i = 0; i < activeDecisionCount; i++)
                {
                    decisionOptionsList[i].isClickable = true;
                }
            }
        }
        public void RespondToHover(int idx)
        {
            ResourceInformationController.GetInstance.currentPanel.ShowPotentialResourceChanges(myController.currentEvent.eventDecision[idx].rewards);
        }
        public void RespondToExit(int idx)
        {
            ResourceInformationController.GetInstance.HideCurrentPanelPotentialResourceChanges();
        }
        public void RespondToClick(int idx)
        {
            // Call Animation here
            ResourceInformationController.GetInstance.HideCurrentPanelPotentialResourceChanges();
            // Call Controllers Reward Option 
            Debug.Log("--- Responding to Click ---");
            myController.HideCurrentEvent(idx);
           
        }
        
        public void HideCurrentDecisions()
        {
            for (int i = 0; i < decisionOptionsList.Count; i++)
            {
                decisionOptionsList[i].gameObject.SetActive(false);
            }
        }
    }
}
