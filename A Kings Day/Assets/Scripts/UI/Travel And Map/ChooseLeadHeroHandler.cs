using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Characters;
using TMPro;
using UnityEngine.UI;

namespace Maps
{
    public class ChooseLeadHeroHandler : MonoBehaviour
    {
        public MapInformationBehavior myController;
        public Image heroVisualChoice;
        public Sprite noHeroIcon;

        public List<Sprite> currentHeroSpriteList;
        public int idx = 0;

        [Header("Current Hero Information")]
        public TextMeshProUGUI heroName;
        public TextMeshProUGUI healthCount, dmgCount, speedCount;
        [Header("Available Heroes")]
        public List<BaseHeroInformationData> availableHeroes;
        public void SetupAvailableHeroes(List<BaseHeroInformationData> thisHeroes)
        {
            currentHeroSpriteList = new List<Sprite>();

            idx = 0;
            currentHeroSpriteList.Add(noHeroIcon);
            BaseHeroInformationData temp = new BaseHeroInformationData();
            availableHeroes = new List<BaseHeroInformationData>();

            availableHeroes.Add(temp);
            availableHeroes.AddRange(thisHeroes);

            for (int i = 0; i < thisHeroes.Count; i++)
            {
                Sprite tmp = myController.myController.unitStorage.GetUnitIcon(thisHeroes[i].unitInformation.unitName);
                currentHeroSpriteList.Add(tmp);
            }

            UpdateHeroVisualChoice();
        }

        public void UpdateHeroVisualChoice()
        {
            heroVisualChoice.sprite = currentHeroSpriteList[idx];

            if(idx != 0)
            {
                heroName.text = availableHeroes[idx].unitInformation.unitName;
                healthCount.text = availableHeroes[idx].unitInformation.maxHealth.ToString();
                dmgCount.text = availableHeroes[idx].unitInformation.minDamage.ToString() + "-" + availableHeroes[idx].unitInformation.maxDamage.ToString();

                float speed = availableHeroes[idx].unitInformation.origSpeed * 10;
                speedCount.text = speed.ToString();
            }
            else
            {
                heroName.text = "No Leader";
                healthCount.text = "0";
                dmgCount.text = "0";
                speedCount.text = "0";
            }
        }

        public bool IsHeroChosen()
        {
            if (idx == 0) return false;
            else
            {
                return true;
            }
        }
        public void NextHero()
        {
            if (idx < (currentHeroSpriteList.Count-1))
            {
                idx += 1;
            }
            else
            {
                idx = 0;
            }

            UpdateHeroVisualChoice();
        }

        public void PreviousHero()
        {
            if (idx > 0)
            {
                idx -= 1;
            }
            else
            {
                idx = (currentHeroSpriteList.Count - 1);
            }
            UpdateHeroVisualChoice();
        }
    }
}