using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;

namespace Buildings
{
    public class OperationCardsHandler : MonoBehaviour
    {
        public List<OperationCard> operationCards;

        public void SetupOperationCards(BuildingInformationData thisData)
        {
            List<BuildingCardData> cardData = thisData.buildingCard;
            for (int i = 0; i < cardData.Count; i++)
            {
                operationCards[i].cardIcon.sprite = cardData[i].cardIcon;
            }
        }
    }
}