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
    public class BuildingInformationHandler : MonoBehaviour
    {


        public List<CardInformationHandler> cardInformationHandlerList;


        public CardInformationHandler currentCardInfoHandler;

        public GameObject infoBlocker;
        public TextMeshProUGUI blockerText;


        public void ShowInfoBlocker(string mesg)
        {
            infoBlocker.SetActive(true);
            blockerText.text = mesg;
        }

        public void HideInfoBlocker()
        {
            infoBlocker.gameObject.SetActive(false);
        }
        public void OpenBuildingInformation(BuildingType thisType)
        {
            if(currentCardInfoHandler != null)
            {
                currentCardInfoHandler.HideCardInformation();
                currentCardInfoHandler = null;
            }
            switch (thisType)
            {
                case BuildingType.Shop:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Shop);
                    break;
                case BuildingType.Barracks:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Barracks);
                    break;
                case BuildingType.Tavern:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Tavern);
                    break;
                case BuildingType.Smithery:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Smithery);
                    break;
                case BuildingType.Houses:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Houses);
                    break;
                case BuildingType.Farm:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Farm);
                    break;
                case BuildingType.Market:
                    currentCardInfoHandler = cardInformationHandlerList.Find(x => x.buildingType == BuildingType.Market);
                    break;
                default:
                    break;
            }
            if(currentCardInfoHandler != null)
            {
                currentCardInfoHandler.gameObject.SetActive(true);
                currentCardInfoHandler.SetupCardInformation();
            }
        }
        
        public void CloseBuildingInformation()
        {
            if(currentCardInfoHandler != null)
            {
                currentCardInfoHandler.HideCardInformation();
                currentCardInfoHandler.gameObject.SetActive(false);
                infoBlocker.gameObject.SetActive(false);
            }
        }
        public void SetCurrentCardAction(int idx)
        {
            currentCardInfoHandler.ChangeCardAction(idx);
        }

    }
}
