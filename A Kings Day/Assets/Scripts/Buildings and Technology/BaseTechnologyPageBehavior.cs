using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Utilities;
using Kingdoms;
using Managers;
using System;
namespace Technology
{
    public class BaseTechnologyPageBehavior : MonoBehaviour
    {
        public BasePanelBehavior myPanel;
        public List<BaseTechnologyOptionBehavior> techOptions;

        public TextMeshProUGUI titleText, wittyText;
        public TextMeshProUGUI curEffectText, levelText;
        public TextMeshProUGUI upgradePriceText;
        public Button upgradeBtn;

        [SerializeField]private List<BaseTechnology> currentTechs;
        private int selectedIdx;
        public ResourceType currentType;

        public void OpenPageTech(ResourceType type)
        {
            if(PlayerGameManager.GetInstance != null)
            {
                SetType(type);
                SelectThisIcon(0);
            }
            myPanel.PlayOpenAnimation();
        }

        public void ClosePageTech(Action callback = null)
        {
            if(callback != null)
            {
                StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, callback));
            }
        }

        public void SelectThisIcon(int idx)
        {
            if(techOptions[selectedIdx].isSelected)
            {
                techOptions[selectedIdx].DeSelect();
            }

            selectedIdx = idx;
            techOptions[selectedIdx].Select();

            SetTechPanelInformation();
        }
        public void SetType(ResourceType thisType)
        {
            PlayerKingdomData playerData = PlayerGameManager.GetInstance.playerData;

            currentTechs.Clear();

            currentType = thisType;

            for (int i = 0; i < playerData.currentTechnologies.Count; i++)
            {
                Debug.Log("Current Tech:" + playerData.currentTechnologies[i].improvedType + " Compared To : " + thisType);

                if (playerData.currentTechnologies[i].improvedType == thisType)
                {
                    Debug.Log("ADDING TECH INDEX : " + i + " TECH NAME : " + playerData.currentTechnologies[i].technologyName);

                    BaseTechnology tmp = new BaseTechnology().ConverToData(playerData.currentTechnologies[i]);
                    tmp.techIcon = TechnologyManager.GetInstance.techStorage.technologies.Find(x => x.technologyName == tmp.technologyName).techIcon;

                    currentTechs.Add(tmp);
                }
            }

            for (int i = 0; i < currentTechs.Count; i++)
            {
                techOptions[i].techIcon.sprite = currentTechs[i].techIcon;
                techOptions[i].bgTechFill.fillAmount = (float)currentTechs[i].currentLevel / (float)currentTechs[i].goldLevelRequirements.Count;
            }
        }

        public void SetTechPanelInformation()
        {
            SetType(currentType);

            titleText.text = currentTechs[selectedIdx].technologyName;
            wittyText.text = currentTechs[selectedIdx].wittyMesg;
            curEffectText.text = currentTechs[selectedIdx].effectMesg;

            levelText.text = "Level " + currentTechs[selectedIdx].currentLevel;

            if(TechnologyManager.GetInstance != null)
            {
                upgradePriceText.text = TechnologyManager.GetInstance.ObtainTechUpgradePrice(currentTechs[selectedIdx]).ToString();
            }
            else
            {
                upgradePriceText.text = "FREE";
            }
        }
        public void UpgradeTech()
        {
            int coinCost = currentTechs[selectedIdx].goldLevelRequirements[currentTechs[selectedIdx].currentLevel];
            if(PlayerGameManager.GetInstance == null)
            {
                return;
            }

            if(PlayerGameManager.GetInstance.playerData.coins >= coinCost)
            {
                PlayerGameManager.GetInstance.playerData.coins -= coinCost;
                TechnologyManager.GetInstance.UpgradeThisTech(currentTechs[selectedIdx]);

                SetTechPanelInformation();
            }
        }

        public void ShowPriceToolTip()
        {
            Parameters p = new Parameters();
            string curCoins = "Current: " + PlayerGameManager.GetInstance.playerData.coins.ToString(); 
            p.AddParameter<string>("Mesg", curCoins);
            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
        }
        public void HidePriceToolTip()
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }
    }

}