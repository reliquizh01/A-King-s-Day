using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;

namespace Buildings
{
    public class OperationCard : MonoBehaviour
    {
        public BaseOperationBehavior operationController;
        public Image cardIcon;
        public GameObject selected, unSelected;
        public bool isClickable;

        public void Clicked()
        {
            if(operationController != null)
            {
                if(isClickable)
                {
                    operationController.SetAsCurrentCard(this);
                }
                else
                {
                    operationController.CardClickFlavorText(this);
                }
            }
        }

        public void SetAsSelected()
        {
            selected.gameObject.SetActive(true);
            unSelected.gameObject.SetActive(false);
        }

        public void SetAsUnselected()
        {
            selected.gameObject.SetActive(false);
            unSelected.gameObject.SetActive(true);
        }
    }
}