using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
using ResourceUI;
using Characters;

namespace Buildings
{
    public class BaseOperationBehavior : MonoBehaviour
    {
        public BasePanelBehavior myPanel;

        public BaseBuildingBehavior currentBuildingClicked;

        public OperationCardsHandler cardHandler;

        [Header("Cards Information")]
        public List<OperationCard> operationCardsList;
        public OperationCard currentCard;
        [Header("Building Information")]
        public InformationActionHandler informationActionHandler;


        [Header("Visual Informations")]
        public TextMeshProUGUI buildingNameText;
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI flavorText;
        public Image focusedCardIcon;
        public int selectedCardIdx;

        public void Start()
        {
            flavorText.text = "";
            cardNameText.text = "";
        }
        public void OpenOperationTab(BaseBuildingBehavior thisBuilding)
        {
            currentBuildingClicked = thisBuilding;
            SetupCardInformation();
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, StartIntroduction));
            // Open Building [ NOT THE CARD ]
           if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.side);
            }
        }
        public void CloseOperationTab()
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, ResetInformation));

            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead, () => EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER));
            }

            informationActionHandler.HideInfoBlocker();
            informationActionHandler.ResetActionList();
            informationActionHandler.ClosePanelList();
        }
        public void ResetInformation()
        {
            focusedCardIcon.gameObject.SetActive(false);
            flavorText.text = "";
            cardNameText.text = "";
            informationActionHandler.ResetActionList();
        }
        public void StartIntroduction()
        {
            int rand = Random.Range(0, currentBuildingClicked.buildingInformation.introductionMessages.Count - 1);
            flavorText.text = currentBuildingClicked.buildingInformation.introductionMessages[rand];

            SetAsCurrentCard(operationCardsList[0]);
        }
        public void SetupCardInformation()
        {
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
                informationActionHandler.SwitchSubPanel(false);
                // BACKGROUND ENLARGED VERSION OF THE IMAGE BUT NOT VISIBLE
                if (!focusedCardIcon.gameObject.activeInHierarchy)
                {
                    focusedCardIcon.gameObject.SetActive(true);
                }
                focusedCardIcon.sprite = currentCard.cardIcon.sprite;
                // --------------------------- IF FOUND USELESS [RECOMMENDED TO REMOVE]

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
                    informationActionHandler.selectedCardIdx = i;
                    selectedCardIdx = i;
                    cardNameText.text = currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].cardName;
                }
            }

            // OPEN CARD INFORMATION [AFTER BUILDING]
            informationActionHandler.SetupActionList(currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes);
            informationActionHandler.OpenPanel(currentBuildingClicked.buildingType);

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

        public void CardDecisionFlavorText(int decisionIdx, bool isPositive)
        {
            int flavorRandIdx = 0;

            // Positive Feedback (Successful Click)
            if (isPositive)
            {
                flavorRandIdx = Random.Range(0, currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[decisionIdx].AcceptMesg.Count);
                flavorText.text = currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[decisionIdx].AcceptMesg[flavorRandIdx];
            }
            // Negative Feedback
            else
            {
                flavorRandIdx = Random.Range(0, currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[decisionIdx].DenyMesg.Count);
                flavorText.text = currentBuildingClicked.buildingInformation.buildingCard[selectedCardIdx].actionTypes[decisionIdx].DenyMesg[flavorRandIdx];
            }
        }
    }

}