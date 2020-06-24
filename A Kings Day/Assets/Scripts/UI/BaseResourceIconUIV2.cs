using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;

namespace ResourceUI
{
    public class BaseResourceIconUIV2 : MonoBehaviour
    {
        public BaseResourceUIControllerV2 myController;

        public Image potentialFill;
        public Image resourceFill;
        public Image reduceFill;

        public bool isEmpty = true;
        public bool isFilling = false;
        public bool fastFill = false;
        public bool fastDrop = false;


        public int currentCount = 0;
        public int capacity = 10;

        public float fillSpeed = 3f;
        public float fastFillSpd = 2.5f;
        public float latestFill = 0.0f;

        public void HidePotentials()
        {
            potentialFill.fillAmount = 0;
            reduceFill.fillAmount = 0;
        }
        public void Update()
        {
            if (isFilling)
            {
                if (!fastFill)
                {
                    if (latestFill > resourceFill.fillAmount)
                    {
                        resourceFill.fillAmount += fillSpeed * Time.deltaTime;
                        if (resourceFill.fillAmount >= latestFill)
                        {
                            resourceFill.fillAmount = latestFill;
                            isFilling = false;
                        }
                    }
                    else if (latestFill < resourceFill.fillAmount)
                    {
                        resourceFill.fillAmount -= fastFillSpd * Time.deltaTime;
                        if (resourceFill.fillAmount <= latestFill)
                        {
                            resourceFill.fillAmount = latestFill;
                            isFilling = false;
                        }
                    }
                }
                else
                {
                    if (latestFill > resourceFill.fillAmount)
                    {
                        resourceFill.fillAmount += fastFillSpd * Time.deltaTime;
                        if (resourceFill.fillAmount >= latestFill)
                        {
                            resourceFill.fillAmount = latestFill;
                            isFilling = false;
                            fastFill = false;
                        }
                    }
                    else if (latestFill < resourceFill.fillAmount)
                    {
                        resourceFill.fillAmount -= fastFillSpd * Time.deltaTime;
                        if (resourceFill.fillAmount <= latestFill)
                        {
                            resourceFill.fillAmount = latestFill;
                            isFilling = false;
                            fastFill = false;
                        }
                    }
                }
            }
        }

        public int ReceiveAmount(int amountToAdjust, bool isAdding = true)
        {
            //Debug.Log("AMOUNT TO ADJUST : " + amountToAdjust + " CURRENT COUNT:" + currentCount);
            if (isAdding)
            {
                int tmp = 0;
                // Space left
                tmp = capacity - currentCount;
                if (tmp >= amountToAdjust)
                {
                    currentCount += amountToAdjust;
                    amountToAdjust = 0;
                }
                else if (tmp < amountToAdjust)
                {
                    currentCount += tmp;
                    amountToAdjust -= tmp;
                }
            }
            else
            {
                //Debug.Log("Subtracting");
                if (amountToAdjust > currentCount)
                {
                    // Debug.Log("Subtracting Part A");
                    amountToAdjust -= currentCount;
                    currentCount -= currentCount;
                    // Debug.Log("amount left A:" + amountToAdjust);
                }
                else if (amountToAdjust < currentCount)
                {
                    //  Debug.Log("Subtracting Part B");
                    currentCount = currentCount - amountToAdjust;
                    amountToAdjust = 0;
                    //   Debug.Log("amount left C:" + amountToAdjust);
                }
                else if (amountToAdjust == currentCount)
                {
                    amountToAdjust = 0;
                    currentCount = 0;
                    //  Debug.Log("amount left D:" + amountToAdjust);
                }
                else
                {
                    amountToAdjust = currentCount;
                    currentCount -= amountToAdjust;
                    //   Debug.Log("amount left E:" + amountToAdjust);
                }
            }

            // Debug.Log("POST CURRENT COUNT: " +  currentCount + " GAMEOBJECT: "+ this.gameObject.name);
            ObtainLatestFillAmount();
            return amountToAdjust;
        }
        public void ObtainLatestFillAmount()
        {
            latestFill = (float)currentCount / (float)capacity;
            isFilling = true;

            if (latestFill > 0)
            {
                isEmpty = false;
            }
            else
            {
                isEmpty = true;
            }
            //Debug.Log("Latest FIll:"+ latestFill+"Fill Amount: " + latestFill +"["+ currentCount + "/"+ capacity + "]");
        }
    }
}
