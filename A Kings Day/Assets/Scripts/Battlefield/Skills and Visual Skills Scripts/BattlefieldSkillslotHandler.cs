using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;
using TMPro;
using Managers;

namespace Battlefield
{
    public class BattlefieldSkillslotHandler : MonoBehaviour
    {
        [Header("Current Skill")]
        public BaseSkillInformationData currentSkill;

        [Header("Skillslot Mechanic")]
        public bool isClickable;
        public GameObject selectedIconArrow;
        public Image skillIcon;
        public Sprite EmptyIcon;

        [Header("Skill Counter")]
        public TimerUI cdCounter;
        public bool startCounting;
        public float currentCount;
        public float maxCount;

        public void SetAsSkill(BaseSkillInformationData newSkill)
        {
            currentSkill = new BaseSkillInformationData();
            currentSkill = newSkill;

            currentCount = 0;
            maxCount = newSkill.cooldown;

            startCounting = true;

            if(BattlefieldSpawnManager.GetInstance != null)
            {
                skillIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetSkillIcon(newSkill.skillName);
            }
        }

        public void SetAsCurrentSkillSlot()
        {
            selectedIconArrow.gameObject.SetActive(true);
        }
        public void SetAsUnselectedSkillSlot()
        {
            selectedIconArrow.gameObject.SetActive(false);
        }
        public void SetAsEmpty()
        {
            skillIcon.sprite = EmptyIcon;
        }

        public void SetOnCooldown()
        {
            currentSkill.isOnCooldown = true;
            startCounting = true;

            cdCounter.gameObject.SetActive(true);
            cdCounter.StartTimer(0,(int)currentSkill.cooldown, EnableSkill);
        }

        public void EnableSkill()
        {
            currentSkill.isOnCooldown = false;
            cdCounter.gameObject.SetActive(false);
        }
    }
}