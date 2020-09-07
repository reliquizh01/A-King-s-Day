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
        public int skillIdx;
        public TextMeshProUGUI nextSkill, prevSkill, activateSkill;
        [Header("Skill Counter")]
        public TimerUI cdCounter;
        public bool startCounting;

        public void SetupController(PlayerControlType playerControlType)
        {
            switch (playerControlType)
            {
                case PlayerControlType.PlayerOne:
                    nextSkill.transform.parent.gameObject.SetActive(true);
                    prevSkill.transform.parent.gameObject.SetActive(true);

                    nextSkill.text = "E";
                    prevSkill.text = "Q";
                    activateSkill.text = "F";
                    break;

                case PlayerControlType.PlayerTwo:
                    nextSkill.transform.parent.gameObject.SetActive(true);
                    prevSkill.transform.parent.gameObject.SetActive(true);

                    nextSkill.text = "7";
                    prevSkill.text = "8";
                    activateSkill.text = "9";
                    break;

                case PlayerControlType.Computer:
                    nextSkill.transform.parent.gameObject.SetActive(false);
                    prevSkill.transform.parent.gameObject.SetActive(false);
                    activateSkill.text = "-";
                    break;
                default:
                    break;
            }
        }
        public void SetAsSkill()
        {
            currentSkill = myController.currentHero.skillsList[skillIdx];
            
            if(currentSkill.isOnCooldown)
            {
                SetOnCooldown();
            }

            if(BattlefieldSpawnManager.GetInstance != null)
            {
                skillIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetSkillIcon(currentSkill.skillName);
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
            startCounting = true;

            cdCounter.gameObject.SetActive(true);
            cdCounter.StartTimer(0,(int)currentSkill.cooldown, EnableSkill);
        }

        public void EnableSkill()
        {
            isClickable = true;
            startCounting = false;
            cdCounter.gameObject.SetActive(false);

            if (myController.myController.controlType == PlayerControlType.PlayerOne)
            {
                Debug.Log("Skill is now Enabled and CurrentSkill On CD:" + currentSkill.isOnCooldown);
            }

            if(myController.myController.controlType == PlayerControlType.Computer)
            {
                myController.myController.ComputerPlayerSkillControl();
            }
        }
    }
}