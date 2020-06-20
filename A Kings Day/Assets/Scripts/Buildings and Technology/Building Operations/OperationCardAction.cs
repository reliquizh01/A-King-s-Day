using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
namespace Buildings
{
    public enum CardActionType
    {
        MessageOnly,
        LogoWithMessage,
        CostMessageOnly,
        LogoOnly,
    }
    public class OperationCardAction : MonoBehaviour
    {
        public Button myBtn;
        public CardActionType curCardActionType;


        public Image iconOnly;
        public TextMeshProUGUI messageOnly;

        public GameObject iconMesgGroup;
        public Image icon;
        public TextMeshProUGUI iconMessage;

        public GameObject coinCostPanel;
        public TextMeshProUGUI coinCostMesg;

        public void SetActionType(CardActionType thisType, string Mesg = "")
        {

        }
        public void OnMouseHover()
        {

        }

        public void OnMouseLeave()
        {

        }
    }
}