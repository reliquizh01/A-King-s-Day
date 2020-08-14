using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utilities;
using Characters;
using Kingdoms;

namespace Battlefield
{
    public class UnitSelectPanel : MonoBehaviour
    {
        public BattlefieldUnitSelectionController myController;
        public Image selectedImage;

        [Header("Unit Panel")]
        public TextMeshProUGUI countText;
        public Image fill;
        public Image counterPanelBG;
        public Image unitImage;
        public Sprite noUnitAvailable;

        public bool panelDisable = false;

        [Header("Unit Cooldown")]
        public float currentCooldownCounter;
        public float currentMaxCooldown;
        public bool cooldownFinish = false;
        public bool startCounting = false;


        public void Update()
        {
            if(!cooldownFinish && startCounting)
            {
                currentCooldownCounter += Time.deltaTime;
                fill.fillAmount = currentCooldownCounter / currentMaxCooldown;

                if (currentCooldownCounter >= currentMaxCooldown)
                {
                    cooldownFinish = true;
                    if(myController.controlType == PlayerControlType.Computer)
                    {
                        if(BattlefieldSystemsManager.GetInstance.dayInProgress)
                        {
                            myController.ComputerPlayerControl();
                        }
                    }
                }
            }
        }
        public void SetAsSelected()
        {
            selectedImage.gameObject.SetActive(true);
        }

        public void UnSelect()
        {
            selectedImage.gameObject.SetActive(false);
        }

        public void ResetCooldown()
        {
            cooldownFinish = false;
            currentCooldownCounter = 0;
        }

        public void DisablePanel()
        {
            float alphaOff = 0.5f;
            countText.color = new Color(countText.color.r, countText.color.g, countText.color.b, alphaOff );
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, alphaOff );
            counterPanelBG.color = new Color(counterPanelBG.color.r, counterPanelBG.color.g, counterPanelBG.color.b, alphaOff );
        }
        public void EnablePanel()
        {
            float alphaOff = 1f;
            countText.color = new Color(countText.color.r, countText.color.g, countText.color.b, alphaOff );
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, alphaOff );
            counterPanelBG.color = new Color(counterPanelBG.color.r, counterPanelBG.color.g, counterPanelBG.color.b, alphaOff );
        }

        public void SetAsUnknown()
        {
            unitImage.sprite = noUnitAvailable;
            startCounting = false;
            DisablePanel();
        }
    }

}