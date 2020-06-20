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
    public class InformativeTroopsPanel : CardPanelHandler
    {
        public Image troopIcon;
        public TextMeshProUGUI troopNameText, healthText, damageText, speedText;
        public TextMeshProUGUI troopCountText;


        private string currentCount = "";
        public override void InitializePanel(Parameters p = null)
        {
            base.InitializePanel(p);

            if(p.HasParameter("Count"))
            {
                currentCount = p.GetWithKeyParameterValue("Count", "");
                if(!string.IsNullOrEmpty(currentCount))
                {
                    troopCountText.text = currentCount;
                }
            }
        }
    }
}