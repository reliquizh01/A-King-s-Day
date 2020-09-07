using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;
using Managers;

namespace Battlefield
{
    public class CustomHeroPanel : CustomUnitPanel
    {
        public TextMeshProUGUI heroName;
        public Image skillIcon;
        public TextMeshProUGUI skillName;
        public TextMeshProUGUI skillDesc;

        public Sprite emptySkillIcon;

        public void SetupHero(BaseHeroInformationData baseHeroInformationData)
        {
            heroName.text = baseHeroInformationData.unitInformation.attackType.ToString();
            if(baseHeroInformationData.skillsList != null && baseHeroInformationData.skillsList.Count > 0)
            {
                SetupSkillSet(baseHeroInformationData.skillsList[0]);
            }
            else
            {
                SetupSkillSet(null);
            }

            List<float> tmpStats = new List<float>();
            UnitInformationData tmp = baseHeroInformationData.unitInformation;
            tmpStats.Add(tmp.maxHealth);
            tmpStats.Add(tmp.maxDamage);

            float visualSpd = tmp.RealSpeed * 10;
            tmpStats.Add(visualSpd);
            tmpStats.Add(tmp.range);

            SetupFillIcons(tmpStats);

        }
        public void SetupSkillSet(BaseSkillInformationData baseSkillInformationData)
        {
            if (baseSkillInformationData != null)
            {
                skillName.text = baseSkillInformationData.skillName;
                skillDesc.text = baseSkillInformationData.skillDescription;
                skillIcon.sprite = TransitionManager.GetInstance.unitStorage.GetSkillIcon(baseSkillInformationData.skillName);
            }
            else
            {
                skillName.text = "No Skill";
                skillDesc.text = "Empty";
                skillIcon.sprite = emptySkillIcon;
            }
        }
    }

}