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
    public enum PanelDescriptionType
    {
        GoodsDescription,
        StatsDescription,
    }
    public class BasePanelDescription : MonoBehaviour
    {
        public PanelDescriptionType descriptionType;


        public virtual void SetupItemInformation(string itemInformation)
        {

        }

        public virtual void SetupItemInformation(ItemInformationData newItemInfo)
        {
            
        }
    }
}
