using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using KingEvents;

namespace Buildings
{
    public class SubOptionHandler : MonoBehaviour
    {
        public InformationActionHandler myController;

        public List<SubOptionPage> optionPageList;


        public void CloseSubOption()
        {
            this.gameObject.SetActive(false);
        }
        public void OpenSellTroops()
        {
            optionPageList[0].gameObject.SetActive(true);
            optionPageList[0].OpenSellTroopsPanel();
        }

        public void OpenSubOption(BuildingType thisBuildingType, int cardIdx, int actionIdx)
        {
            switch (thisBuildingType)
            {
                case BuildingType.Shop:

                    break;
                case BuildingType.Barracks:
                    OpenSellTroops();
                    break;
                case BuildingType.Tavern:

                    break;
                case BuildingType.Smithery:

                    break;
                case BuildingType.Houses:

                    break;
                case BuildingType.Farm:

                    break;
                case BuildingType.Market:

                    break;
                default:
                    break;
            }
        }
    }
}