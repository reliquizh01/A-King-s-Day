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
    public class OperationCardDecision : MonoBehaviour
    {
        public InformationActionHandler myController;
        
        public Button myBtn;
        public CardActionType curCardActionType;
        public bool openSubOption;

        public Image iconOnly;
        public TextMeshProUGUI messageOnly;

        public GameObject iconMesgGroup;
        public Image icon;
        public TextMeshProUGUI iconMessage;

        public GameObject coinCostPanel;
        public TextMeshProUGUI coinCostMesg;

        public int actionIdx;

        public void SetActionType(CardActionType thisType, string Mesg = "")
        {

        }

        public void OnMouseClick()
        {
            myController.ImplementDecisionChanges(actionIdx);
            myController.UpdateCurrentPanel();
        }
        public void OnMouseHover()
        {
            if(!openSubOption)
            {
                myController.ShowOperationDecisionChanges(actionIdx);
            }
        }

        public void OnMouseLeave()
        {
            myController.HideOperationDecisionChanges();
        }
    }
}