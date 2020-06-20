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

        Action onEnterCallback, onExitCallback;
        public void SetActionType(CardActionType thisType, string Mesg = "")
        {

        }
        public void SetExitCallback(Action newCallback)
        {
            onExitCallback = newCallback;
        }
        public void SetEnterCallback(Action newCallback)
        {
            onEnterCallback = newCallback;
        }

        public void OnMouseHover()
        {
            onEnterCallback();
        }

        public void OnMouseLeave()
        {
            onExitCallback();
        }
    }
}