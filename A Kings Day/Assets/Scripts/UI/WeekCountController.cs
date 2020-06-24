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

namespace ResourceUI
{
    public class WeekCountController : MonoBehaviour
    {
        [Header("Event Information")]
        public InteractiveText endWeekBtnText;
        public InteractiveImage endWeekBtnImage;
        public InteractiveText weekCountText;

        public void Start()
        {
            weekCountText.SetHoverCallback(OnWeekCountExit);
            endWeekBtnText.SetExitCallback(OnWeekCountHover);
            weekCountText.AddTransition(ShowWeeklyResult);
        }

        public void UpdateEndButton(int cur, int target)
        {

            if (cur >= target)
            {
                endWeekBtnText.text.text = "END WEEK";
            }
            else
            {
                endWeekBtnText.text.text = cur.ToString() + "/" + target.ToString();
            }
            float targetFill = ((float)cur / (float)target);
            //Debug.Log("CUR: " + cur + " TAR:" + target + " Fill: " + targetFill);
            endWeekBtnImage.StartFilling(targetFill);
        }
        public void ShowWeeklyResult()
        {
            if (!KingdomManager.GetInstance.IsWeekEventsFinished())
            {
                Debug.LogWarning("Attempted to Call Weekly Result but failed, check event count!");
                return;
            }

            // Then Proceed to Week 2
            KingdomManager.GetInstance.ProceedToNextWeek();

            UpdateWeekCountText();
        }

        public void UpdateWeekCountText()
        {
            weekCountText.text.text = "Week " + PlayerGameManager.GetInstance.playerData.weekCount.ToString();
        }
        public void OnWeekCountHover()
        {
            endWeekBtnText.text.gameObject.SetActive(true);
            weekCountText.text.gameObject.SetActive(false);
        }
        public void OnWeekCountExit()
        {
            endWeekBtnText.text.gameObject.SetActive(false);
            weekCountText.text.gameObject.SetActive(true);
        }
    }
}
