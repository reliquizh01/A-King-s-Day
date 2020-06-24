using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Managers;
using Kingdoms;

namespace KingEvents
{
    public class DecisionOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool isClickable = false;
        public BasePanelBehavior myPanel;
        public Button myBtn;

        [SerializeField] private DecisionHandler decisionHandler = null;
        [SerializeField] private RectTransform rect;
        [SerializeField] private int myIdx = 0;

        private EventDecision curDecision;

        private float inactivePosL = -42;
        private float inactivePosR = 42;
        private float activePos = 0;
        public TextMeshProUGUI optionDecription;
        private bool isClicked = false;
        private bool playerResourceLacking = false;
        public void Start()
        {
            if(rect == null)
            {
                rect = this.gameObject.GetComponent<RectTransform>();
            }
            if(myBtn == null)
            {
                myBtn = this.GetComponent<Button>();
            }
        }
        public void AddDescription(EventDecision description)
        {
            playerResourceLacking = false;
            isClicked = false;
            curDecision = description;
            optionDecription.text = curDecision.optionDescription;
            UpdateKingdomResources();
        }
        public void OnPointerEnter(PointerEventData evenData)
        {

            if(!isClickable)
            {
                return;
            }
            Active();
            AudioManager.GetInstance.PlayDecisionHover();
            decisionHandler.RespondToHover(myIdx);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isClickable)
            {
                return;
            }
            Inactive();
            decisionHandler.RespondToExit(myIdx);
        }
        public void UpdateKingdomResources()
        {
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;

            int tmpAmount = 0;

            for (int i = 0; i < curDecision.rewards.Count; i++)
            {
                switch (curDecision.rewards[i].resourceType)
                {
                    case ResourceType.Coin:
                        if (curDecision.rewards[i].rewardAmount < 0)
                        {
                            tmpAmount = Mathf.Abs(curDecision.rewards[i].rewardAmount);
                            tmpAmount = playerData.coins - tmpAmount;

                            if (tmpAmount < 0)
                            {
                                playerResourceLacking = true;
                            }
                        }
                        continue;
                    case ResourceType.Troops:
                        if (curDecision.rewards[i].rewardAmount < 0)
                        {
                            tmpAmount = Mathf.Abs(curDecision.rewards[i].rewardAmount);
                            tmpAmount = playerData.recruits - tmpAmount;

                            if (tmpAmount < 0)
                            {
                                playerResourceLacking = true;
                            }
                        }
                        continue;
                    case ResourceType.Food:
                        if (curDecision.rewards[i].rewardAmount < 0)
                        {
                            tmpAmount = Mathf.Abs(curDecision.rewards[i].rewardAmount);
                            tmpAmount = playerData.foods - tmpAmount;

                            if (tmpAmount < 0)
                            {
                                playerResourceLacking = true;
                            }
                        }
                        continue;
                    case ResourceType.Population:
                        if (curDecision.rewards[i].rewardAmount < 0)
                        {
                            tmpAmount = Mathf.Abs(curDecision.rewards[i].rewardAmount);
                            tmpAmount = playerData.population- tmpAmount;

                            if (tmpAmount < 0)
                            {
                                playerResourceLacking = true;
                            }
                        }
                        continue;
                }
                if(playerResourceLacking)
                {
                    break;
                }
            }

            if(playerResourceLacking)
            {
                isClickable = false;
                if (myBtn == null)
                {
                    myBtn = this.GetComponent<Button>();
                }
                myBtn.interactable = isClickable;
            }
            else
            {
                isClickable = true;
                if (myBtn == null)
                {
                    myBtn = this.GetComponent<Button>();
                }
                myBtn.interactable = isClickable;
            }
        }

        public void OnEnable()
        {
            Inactive();

        }

        public void OnActiveEnd()
        {
           
        }
        public void OnClick()
        {
            if(!isClickable)
            {
                return;
            }
            if(isClicked)
            {
                return;
            }

            isClicked = true;
            isClickable = false;
            myBtn.interactable = isClickable;

            decisionHandler.RespondToClick(myIdx);
            Inactive();
        }

        private void Active()
        {
            rect.SetLeft(activePos);
            rect.SetRight(activePos);
        }
        private void Inactive()
        {
            rect.SetLeft(inactivePosL);
            rect.SetRight(inactivePosR);
        }
    }

}