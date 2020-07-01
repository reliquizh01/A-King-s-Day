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
        public Image selectedImage;
        public Image fill;
        public TextMeshProUGUI countText;

        [Header("Unit Information")]
        public UnitType unitType;

        [Header("Unit Cooldown")]
        public float currentCooldownCounter;
        public float currentMaxCooldown;
        public bool cooldownFinish = false;


        public void Update()
        {
            if(!cooldownFinish)
            {
                currentCooldownCounter += Time.deltaTime;
                fill.fillAmount = currentCooldownCounter / currentMaxCooldown;

                if (currentCooldownCounter >= currentMaxCooldown)
                {
                    cooldownFinish = true;
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
    }

}