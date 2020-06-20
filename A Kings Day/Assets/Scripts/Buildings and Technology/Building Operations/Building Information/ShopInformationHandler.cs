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
    public class ShopInformationHandler : CardInformationHandler
    {
        public List<InformativeItemPanel> shopPanelList;

        public override void SetupCardInformation(Parameters p = null)
        {
            base.SetupCardInformation(p);
            List<ItemInformationData> newItemList;

            if (p.HasParameter("Items Sold"))
            {
                newItemList = p.GetWithKeyParameterValue<List<ItemInformationData>>("Items Sold", new List<ItemInformationData>());
                if(newItemList.Count > 0)
                {
                    for (int i = 0; i < shopPanelList.Count; i++)
                    {
                        shopPanelList[i].SetupShopItem(newItemList[i]);
                    }
                }
            }
        }
    }
}
