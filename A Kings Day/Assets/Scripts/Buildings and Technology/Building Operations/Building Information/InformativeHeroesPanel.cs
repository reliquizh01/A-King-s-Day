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
    public class InformativeHeroesPanel : CardPanelHandler
    {
        public Image heroIcon;
        public TextMeshProUGUI heroNameText, healthText, damageText, speedText;
        public Image skill1, skill2;
        public TextMeshProUGUI attackType;
    }
}