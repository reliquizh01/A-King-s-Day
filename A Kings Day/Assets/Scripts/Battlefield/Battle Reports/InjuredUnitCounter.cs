using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Battlefield
{
    public class InjuredUnitCounter : MonoBehaviour
    {
        public Image iconBg, titleBg, countBg, characterIcon;
        public TextMeshProUGUI titleText;
        public DailyReportPanel myController;
        public Image unitIcon;
        public CountingEffectUI injuredCount;
        public GameObject selectedPanel;
        public bool allowClick = false;
        public bool panelDisabled = false;

        public Color disableColor = new Color(1, 1, 1, 0.025f);
        public Color enableColor = new Color(1, 1, 1, 1f);
        public void OnClickSelect()
        {
            if(!allowClick && panelDisabled)
            {
                return;
            }

            myController.SetAsSelectedPanel(this);
        }
        public void SetAsSelected()
        {
            selectedPanel.gameObject.SetActive(true);
        }

        public void UnSelect()
        {
            selectedPanel.gameObject.SetActive(false);
        }

        public void EnablePanel()
        {
            iconBg.color = enableColor;
            titleBg.color = enableColor;
            countBg.color = enableColor;
            characterIcon.color = enableColor;

            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, enableColor.a);
            injuredCount.countText.color = new Color(injuredCount.countText.color.r, injuredCount.countText.color.g, injuredCount.countText.color.b, enableColor.a);

            panelDisabled = false;
        }

        public void DisablePanel()
        { 
            iconBg.color = new Color(disableColor.r, disableColor.g, disableColor.b, disableColor.a);
            Debug.Log("Icon BG: " + iconBg.color + " COLOR : " + disableColor);
            titleBg.color = disableColor;
            countBg.color = disableColor;
            characterIcon.color = disableColor;

            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, disableColor.a);
            injuredCount.countText.color = new Color(injuredCount.countText.color.r, injuredCount.countText.color.g, injuredCount.countText.color.b, disableColor.a);

            panelDisabled = true;
        }
    }

}