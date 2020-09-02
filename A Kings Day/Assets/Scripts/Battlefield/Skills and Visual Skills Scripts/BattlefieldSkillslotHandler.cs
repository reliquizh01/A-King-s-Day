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
        public BattlefieldSkillsHandler myController;
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

        public void SetAsSkill(BaseSkillInformationData newSkill)
        {
            currentSkill = new BaseSkillInformationData();
            currentSkill = newSkill;
            
            if(currentSkill.isOnCooldown)
            {
                SetOnCooldown();
            }

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
            
            if(myController.myController.controlType == PlayerControlType.Computer)
            {
                myController.myController.ComputerPlayerSkillControl();
            }
        }
    }
}