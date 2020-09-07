using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utilities;
using TMPro;

namespace Battlefield
{
    public class CustomUnitPanel : MonoBehaviour
    {
        public MultiCountInformationPanel multiCounter;
        public List<Image> fillIcons;
        public List<TextMeshProUGUI> fillCountTexts;

        public void SetupFillIcons(List<float> thisStats)
        {
            for (int i = 0; i < fillIcons.Count; i++)
            {
                fillIcons[i].fillAmount = thisStats[i] / 20.0f;
                fillCountTexts[i].text = thisStats[i].ToString();
            }
        }
    }

}