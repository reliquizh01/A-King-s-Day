using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using UnityEngine.EventSystems;
using Kingdoms;
using Managers;
using GameResource;

namespace ResourceUI
{

    public class BaseResourceUIControllerV2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public BaseResourceIconUIV2 storageFill;
        [Header("Counter")]
        public TextMeshProUGUI currentAmount;
        public Color normalColor;
        public Color increaseColor;
        public Color decreaseColor;

        [Header("Warning System")]
        public ResourceWarningHandler myWarning;

        [Header("Storage")]
        public int storageCapacity;
        public int currentCount;
        public void SetupStorageCapacity(int newCapacity)
        {
            storageCapacity = newCapacity;
            storageFill.capacity = newCapacity;
        }
        public void UpdatePanel(int amount)
        {
            int difference = Mathf.Abs(amount - currentCount);

            if (amount > currentCount)
            {
                IncreaseResource(difference);
            }
            else
            {
                DecreaseResource(difference);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {

        }
        public void OnPointerClick(PointerEventData eventData)
        {

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void SetResource(int setThis)
        {
            currentCount = setThis;
            storageFill.currentCount = setThis;
            storageFill.ObtainLatestFillAmount();
            currentAmount.text = currentCount.ToString();
        }
        public void IncreaseResource(int addThis)
        {
            currentCount += addThis;
            storageFill.ReceiveAmount(addThis);
            currentAmount.text = currentCount.ToString();
        }
        public void DecreaseResource(int removeThis)
        {
            currentCount -= removeThis;
            if(currentCount < 0)
            {
                currentCount = 0;
            }
            storageFill.ReceiveAmount(removeThis, false);
            currentAmount.text = currentCount.ToString();
        }

        public void ShowIncrease(int toBeAdded)
        {
            toBeAdded = Mathf.Abs(toBeAdded);
            int potentialTotal = toBeAdded + storageFill.currentCount;
            currentAmount.color = increaseColor;
            currentAmount.text = currentCount.ToString() + "[+" + toBeAdded.ToString() + "]";

            storageFill.potentialFill.fillAmount = (float)potentialTotal / (float)storageCapacity;
        }
        public void ShowReduction(int toBeReduced)
        {
            toBeReduced = Mathf.Abs(toBeReduced);
            int potentialTotal = storageFill.currentCount + toBeReduced;
            currentAmount.color = decreaseColor;

            int textTotal = currentCount - toBeReduced;
            currentAmount.text = currentCount.ToString() + "[-" + toBeReduced.ToString() + "]";

            storageFill.reduceFill.fillAmount = (float)potentialTotal / (float)storageCapacity;
        }

        public void HidePotentials()
        {
            storageFill.HidePotentials();
            currentAmount.color = normalColor;
            currentAmount.text = currentCount.ToString();
        }

    }
}