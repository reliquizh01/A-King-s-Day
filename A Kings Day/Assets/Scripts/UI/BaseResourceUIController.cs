using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using UnityEngine.EventSystems;
using Kingdoms;

namespace ResourceUI
{
    public class BaseResourceUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool isHovered = false;
        public bool isClicked = false;
        public bool allowToolTip = false;
        public ResourceType resourceType;
        private Action hoverCallback;
        private Action exitCallback;
        public List<BaseResourceIconUI> foodIcons;

        public int resourceDataCount = 0;
        public int testCount = 0;

        public Image bgPanel;
        // Prioritize the furthest right Icon
        public int lastIconIdx = 0;
        public int amountToAdjust = 0;
        public bool isAdding = false;
        public bool isUpdating = false;
        // BG panel
        private float activeAlpha = 1;
        private float inactiveAlpha = 0.5f;
        private bool startTimer = false;
        public BasePanelBehavior myPanel;

        [Header("Warning System")]
        public ResourceWarningHandler myWarning;

        public void Start()
        {
            for (int i = 0; i < foodIcons.Count; i++)
            {
                foodIcons[i].myController = this;
            }
            bgPanel.color = new Color(1, 1, 1, inactiveAlpha);
        }
        #region Increase and Decrease Resources
        public void IncreaseResource(int addThis)
        {
            // This is Only Temporary fix until we implement the game Data system
            if (resourceDataCount >= 50)
            {
                return;
            }
            else
            {
                resourceDataCount += addThis;
                isAdding = true;
                UpdateIcons();
            }
        }

        public void EnableWarning(string mesg)
        {
            myWarning.ShowWarning(mesg);
        }

        public void DisableWarning()
        {
            myWarning.HideWarning();
        }
        public void DecreaseResource(int subThis)
        {
            // This is Only Temporary fix until we implement the game Data system
            if (resourceDataCount <= 0)
            {
                return;
            }
            else
            {
                resourceDataCount -= subThis;
                if (resourceDataCount <= 0)
                {
                    resourceDataCount = 0;
                }
                isAdding = false;
                UpdateIcons();
            }
        }
        #endregion

        #region Update Icons
        public void UpdateIcons()
        {
            isUpdating = true;
            UpdatePanel();
            /* STEP BY STEP LOGIC
             * 1.) Get Total amount on how much the icons have (ObtainIconCount())
             * 2.) Get the latest amount (resourceDataCount).
             * 3.) Get the difference.
             * THEN 
             * 
             * 
             */
            int curAmount = ObtainIconCount();

            //Debug.Log("Resource Count: " + resourceDataCount + " Cur Amount:" + curAmount);
            amountToAdjust = Math.Abs(resourceDataCount - curAmount);
            //Debug.Log("Result:" + amountToAdjust);
            FillCurrentIcon();
        }

        public void FillCurrentIcon()
        {
            if(foodIcons[lastIconIdx].currentCount >=  foodIcons[lastIconIdx].capacity)
            {
                // IMPROVE THIS TO ADD BUFF (SURPLUST OF CAPACITY)
                if(lastIconIdx > foodIcons.Count - 1)
                {
                    return;
                }
            }

            amountToAdjust = foodIcons[lastIconIdx].ReceiveAmount(amountToAdjust, isAdding);

            if(amountToAdjust > 0)
            {
                GoToNextIcon();
                if(lastIconIdx < 5)
                {
                    if(gameObject.activeInHierarchy)
                    {
                        StartCoroutine(DelayNextFrame());
                    }
                }
                else if(lastIconIdx == 0)
                {
                    amountToAdjust = 0;
                }
            }
            else
            {
                isAdding = false;
                isUpdating = false;
            }
        }
        #endregion 

        public void SetHoverExitCallBack(Action hovCallback, Action extCallback = null)
        {
            hoverCallback = hovCallback;
            exitCallback = extCallback;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            bgPanel.color = new Color(1, 1, 1, activeAlpha);
            ShowToolTip();
            
        }
        public void ShowToolTip()
        {
            if (!allowToolTip) return;
            string mesg = resourceDataCount.ToString();
            Parameters p = new Parameters();
            p.AddParameter<string>("Mesg", mesg);
            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
        }
        public void HideToolTip()
        {
            if (!allowToolTip) return;

            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(myPanel != null)
            {
                StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, UpdatePanel));
            }
            isClicked = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            if(!isUpdating)
            {
                bgPanel.color = new Color(1, 1, 1, inactiveAlpha);
            }
            HideToolTip();
        }

        public void UpdatePanel()
        {
            if(isUpdating)
            {
                startTimer = false;
                bgPanel.color = new Color(1, 1, 1,activeAlpha);
            }
            else
            {
                StartCoroutine(DeactivateBackground());
            }
        }

        public void ShowIncrease(int toBeAdded)
        {
            // Get Last Index, let it be the start.
            int startIdx = lastIconIdx;
            if(foodIcons[startIdx].currentCount >= foodIcons[startIdx].capacity)
            {
                startIdx += 1;
            }
            int emptyIcons = foodIcons.Count - lastIconIdx;

            //Debug.Log("To Be Added:" + toBeAdded + " Initial Index : " + startIdx);
            for (int i = 0; i < emptyIcons-1; i++)
            {
                toBeAdded = ComputeIncrease(toBeAdded, startIdx);
                //Debug.Log("-----[To Be Added:" + toBeAdded + " Post Index : " + startIdx + "]-----");
                if (toBeAdded > 0)
                {
                //    Debug.Log("**Incrementing Index**");
                    startIdx += 1;
                }
                else
                {
                    break;
                }
            }

        }

        public int ComputeIncrease(int toBeAdded, int lastIdx)
        {
            int tmp = toBeAdded;
            // Get the last Icon's value, so it'll be the base value.
            int initValue = foodIcons[lastIdx].currentCount;
            int capacity = foodIcons[lastIdx].capacity;
            // Now Obtain the space that can be filled.
            int difference = Mathf.Abs(initValue - capacity);

            if (tmp > difference)
            {
                tmp -= difference;
            }
            else
            {
                difference = tmp;
                tmp = 0;
            }
            // Set LastIcon's Potential Fill meter;
            foodIcons[lastIdx].potentialFill.fillAmount = (float)initValue + (float)difference / (float)capacity;

            return tmp;
        }
        public void ShowReduction(int toReduce)
        {
            // Get the Last index Filled.
            int startIdx = lastIconIdx;
            // Check if next icon has amount;
            if (lastIconIdx < foodIcons.Count && foodIcons[lastIconIdx + 1].currentCount > 0)
            {
                startIdx += 1;
            }

            int filledIcons = lastIconIdx + 1;

            if(filledIcons > 1)
            {
                for (int i = 0; i < filledIcons-1; i++)
                {
                   // Debug.Log("To Be Taken: " + toReduce +" Initial Value: "+ foodIcons[startIdx].currentCount +" Initial Index : " + startIdx + " GameObject:" + foodIcons[startIdx].gameObject.name);

                    toReduce = ComputeReduction(toReduce, startIdx);
                    if(toReduce > 0)
                    {
                        startIdx -= 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                toReduce = ComputeReduction(toReduce, 0);
            }

        }
        public int ComputeReduction(int toReduce, int lastIdx)
        {
            int tmp = Mathf.Abs(toReduce);
            // Obtain value of last Icon
            int initValue = foodIcons[lastIdx].currentCount;
            int capacity = foodIcons[lastIdx].capacity;
            // Obtain Empty Amount
            int emptyValue = capacity - initValue;

            if (initValue > tmp)
            {
                foodIcons[lastIdx].reduceFill.fillAmount = (float)emptyValue + (float)tmp / (float)capacity;
                tmp = 0;
            }
            else if (initValue <= tmp)
            {
                tmp -= initValue;
                foodIcons[lastIdx].reduceFill.fillAmount = (float)capacity / (float)capacity;
            }

            return tmp;
        }
        public void HidePotentials()
        {
            for (int i = 0; i < foodIcons.Count; i++)
            {
                foodIcons[i].HidePotentials();
            }
        }
        IEnumerator DelayNextFrame()
        {
            yield return new WaitForEndOfFrame();
            if(this.gameObject.activeInHierarchy)
            {
                FillCurrentIcon();
            }
        }
        IEnumerator DeactivateBackground()
        {
            yield return new WaitForSeconds(0.5f);
            if (!this.gameObject.activeInHierarchy)
            {
                bgPanel.color = new Color(1, 1, 1, inactiveAlpha);
            }
            else
            {
                if (!isUpdating)
                {
                    startTimer = true;
                }
                yield return new WaitForSeconds(0.5f);
                if(!isUpdating && startTimer && !isHovered && !isClicked)
                {
                    bgPanel.color = new Color(1, 1, 1, inactiveAlpha);
                }
            }

        }
        // Check if Data == Visual Count
        public bool IsUpdating()
        {
            return (ObtainIconCount() == resourceDataCount);
        }

        // Check Visual Total Count
        public int ObtainIconCount()
        {
            int amount = 0;
            for (int i = 0; i < foodIcons.Count; i++)
            {
                amount += foodIcons[i].currentCount;
            }

            //Debug.Log("Icon Amounts : " + amount);
            return amount;
        }

        // Shift in Current Icon
        public void GoToNextIcon()
        {
            if(isAdding && lastIconIdx < foodIcons.Count-1)
            {
                lastIconIdx += 1;
            }
            else if(!isAdding)
            {
                if(lastIconIdx > 0)
                {
                    lastIconIdx -= 1;
                }
                else
                {
                    lastIconIdx = 0;
                }
            }
        }
    }
}
