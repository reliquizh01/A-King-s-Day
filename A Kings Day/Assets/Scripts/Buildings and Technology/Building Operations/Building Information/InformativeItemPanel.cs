using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
using ResourceUI;
using GameItems;

namespace Buildings
{
    public class InformativeItemPanel : CardPanelHandler
    {
        public Image itemIcon;
        public TextMeshProUGUI itemNameText;
        public List<BasePanelDescription> descriptionHandlerList;

        public BasePanelDescription currentDescription;
        public override void InitializePanel(Parameters p = null)
        {
            base.InitializePanel(p);
           
        }

        public void SetupShopItem(ItemInformationData itemData)
        {
            if(currentDescription != null)
            {
                currentDescription.gameObject.SetActive(false);
                currentDescription = null;
            }

            switch (itemData.ItemType)
            {
                case ItemType.Resources:
                    currentDescription = descriptionHandlerList.Find(x => x.descriptionType == PanelDescriptionType.GoodsDescription);
                    break;
                case ItemType.Equipment:
                    currentDescription = descriptionHandlerList.Find(x => x.descriptionType == PanelDescriptionType.StatsDescription);
                    break;
                default:
                    break;
            }

            if(currentDescription == null)
            {
                return;
            }

            currentDescription.gameObject.SetActive(true);
        }
    }
}
