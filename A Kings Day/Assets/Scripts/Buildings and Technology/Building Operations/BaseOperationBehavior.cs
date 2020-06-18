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
    public class BaseOperationBehavior : MonoBehaviour
    {
        public BasePanelBehavior myPanel;

        public BaseBuildingBehavior currentBuildingClicked;

        public OperationCardsHandler cardHandler;
        public OperationCardActionHandler cardActionHandler;

        public List<OperationCard> operationCardsList;
        public OperationCard currentCard;


        [Header("Visual Informations")]
        public TextMeshProUGUI buildingNameText;
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI flavorText;
        public Image focusedCardIcon;
        private int selectedCardIdx;
        public void Start()
        {
            flavorText.text = "";
            cardNameText.text = "";
        }
        public void OpenOperationTab(BaseBuildingBehavior buildingInformation)
        {
            currentBuildingClicked = buildingInformation;
            SetupCardInformation();
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, StartIntroduction));
            if(ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.side);
            }
        }
        public void CloseOperationTab()
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, ResetInformation));
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);

            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
            }
        }
        public void ResetInformation()
        {
            focusedCardIcon.gameObject.SetActive(false);
            flavorText.text = "";
            cardNameText.text = "";
            cardActionHandler.ResetActionList();
        }
        public void StartIntroduction()
        {
            int rand = Random.Range(0, currentBuildingClicked.buildingInformation.introductionMessages.Count - 1);
            flavorText.text = currentBuildingClicked.buildingInformation.introductionMessages[rand];
        }
        public void SetupCardInformation()
        {
            Debug.Log("Setting Up Operations Cards Sprites!");
            // Setup Titles
            buildingNameText.text = currentBuildingClicked.buildingInformation.BuildingName;
            // Setup Cards
            for (int i = 0; i < operationCardsList.Count; i++)
            {
                operationCardsList[i].cardIcon.sprite = currentBuildingClicked.buildingInformation.buildingCard[i].cardIcon;
                operationCardsList[i].SetAsUnselected();
            }
        }
        public void SetAsCurrentCard(OperationCard thisCard)
        {
            if(thisCard != currentCard)
            {
                currentCard = thisCard;
                if(!focusedCardIcon.gameObject.activeInHierarchy)
                {
                    focusedCardIcon.gameObject.SetActive(true);
                }
                focusedCardIcon.sprite = currentCard.cardIcon.sprite;
                CardClickFlavorText(currentCard);
            }

            // SETTING VISUAL FOR ACTION CARDS
            for (int i = 0; i < operationCardsList.Count; i++)
            {
                if(operationCardsList[i] != thisCard)
                {
                    operationCardsList[i].SetAsUnselected();
                }
                else
                {
                    operationCardsList[i].SetAsSelected();
                    selectedCardIdx = i;
                    cardNameText.text = currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].cardName;
                }
            }

            cardActionHandler.SetupActionList(currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes);
        }
        public virtual void ImplementThisAction(int actionIdx)
        {

        }

        public void CardClickFlavorText(OperationCard thisCard)
        {
            int cardClickedIdx = operationCardsList.FindIndex(x => x == thisCard);

            int flavorRandIdx = 0;
    
            // Positive Feedback (Successful Click)
            if(thisCard == currentCard)
            {
                flavorRandIdx = Random.Range(0, currentBuildingClicked.buildingInformation.buildingCard[cardClickedIdx].cardPosMesg.Count);
                 flavorText.text = currentBuildingClicked.buildingInformation.buildingCard[cardClickedIdx].cardPosMesg[flavorRandIdx];
            }
            // Negative Feedback
            else
            {
                flavorRandIdx = Random.Range(0, currentBuildingClicked.buildingInformation.buildingCard[cardClickedIdx].cardNegMesg.Count);

                flavorText.text = currentBuildingClicked.buildingInformation.buildingCard[cardClickedIdx].cardNegMesg[flavorRandIdx];
            }
        }
    }

}