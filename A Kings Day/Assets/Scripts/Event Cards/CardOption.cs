using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kingdoms;
using TMPro;
using Utilities;

namespace KingEvents
{
    public enum CardType
    {
        Security,
        Crime,
        Merchant,
    }
    public enum ReporterType
    {
        Villager,
        Soldier,
        Merchant,
        Count,
        Barbarian,
    }


    public class CardOption : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public CardType thisCardType;
        public BasePanelBehavior myPanel;
        public void Start()
        {
            
        }

        public void ShowCardAnim()
        {
            myPanel.PlayOpenAnimation();
        }
        public void InitializeText(string title, string description)
        {
            titleText.text = title;
            descriptionText.text = description;

        }
    }
}
