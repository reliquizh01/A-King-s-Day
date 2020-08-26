using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kingdoms;
using KingEvents;
using ResourceUI;
using System;

namespace Buildings
{
    public class SubOptionPanel : MonoBehaviour
    {
        public SubOptionPage myController;
        public Slider panelSlider;
        public TextMeshProUGUI nameTitleText;
        public TextMeshProUGUI currentCountText;
        public TextMeshProUGUI amountToSellText;

        public int totalCount;
        public int currentCount;
        public int amountToSell;

        public int pricePerItem;

        public Action changeCallback;
        public void Start()
        {
            panelSlider.maxValue = currentCount;
        }
        public void SetupOptionPanel(int currentAmount,int itemPrice, string titleText, Action slideChangeCallback = null)
        {
            nameTitleText.text = titleText;

            panelSlider.maxValue = currentAmount;
            panelSlider.value = 0;
            totalCount = currentAmount;
            currentCount = currentAmount;
            amountToSell = 0;
            pricePerItem = itemPrice;

            if(slideChangeCallback != null)
            {
                panelSlider.onValueChanged.AddListener(VolumeCallbackAdjusted);
            }
            AdjustAmountToSell();
        }

        public void VolumeCallbackAdjusted(float value)
        {
            if(changeCallback != null)
            {
                changeCallback();
            }
            AdjustAmountToSell();
        }
        public void AdjustAmountToSell()
        {
            amountToSell = (int)panelSlider.value;
            amountToSellText.text = amountToSell.ToString();

            currentCount = totalCount - amountToSell;
            currentCountText.text = currentCount.ToString();

            if(myController != null)
            {
                myController.ShowPotentialChanges();
            }
        }
    }

}