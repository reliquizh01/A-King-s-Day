using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
using ResourceUI;
using GameItems;
using Characters;
using Managers;

namespace Buildings
{
    public class BarracksInformationHandler : CardInformationHandler
    {
        public List<SubInformationHandler> troopPanelList;
        public SubInformationHandler currentPanel;

        [Header("Train Hero")]
        public List<BaseHeroInformationData> heroInformation;
        public int curHeroIdx;

        [Header("Trained Troops Information")]
        public int trainedSwordsman;
        public int trainedSpearman;
        public int trainedArcher;

        public override void SetupCardInformation(Parameters p = null)
        {
            base.SetupCardInformation(p);

            if(PlayerGameManager.GetInstance != null)
            {
                if(PlayerGameManager.GetInstance.playerData.myHeroes != null && PlayerGameManager.GetInstance.playerData.myHeroes.Count > 0)
                {
                    heroInformation = PlayerGameManager.GetInstance.playerData.myHeroes;
                }
                trainedSwordsman = PlayerGameManager.GetInstance.playerData.swordsmenCount;
                trainedSpearman = PlayerGameManager.GetInstance.playerData.spearmenCount;
                trainedArcher = PlayerGameManager.GetInstance.playerData.archerCount;
            }
        }

        public override void HideCardInformation()
        {
            base.HideCardInformation();
            for (int i = 0; i < troopPanelList.Count; i++)
            {
                troopPanelList[i].gameObject.SetActive(false);
            }

        }
        public override void ChangeCardAction(int idx)
        {
            base.ChangeCardAction(idx);

            for (int i = 0; i < troopPanelList.Count; i++)
            {
                if(i == idx)
                {
                    myController.HideInfoBlocker();
                    troopPanelList[i].gameObject.SetActive(true);
                    currentPanel = troopPanelList[i];
                    currentCardIdx = i;
                }
                else
                {
                    troopPanelList[i].gameObject.SetActive(false);
                }
            }
            InitializeCurrentPanel();
        }
        public override void InitializeCurrentPanel()
        {
            base.InitializeCurrentPanel();
            if(currentPanel == troopPanelList[2])
            {
                SetupHeroPanel();
            }
            // Train Troops
            if(currentPanel == troopPanelList[1])
            {
                SetupTrainTroopsPanel();
            }
        }
        public void SetupTrainTroopsPanel()
        {
            List<CardPanelHandler> trainedTroopsPanel = currentPanel.informationPanels;

            for (int i = 0; i < trainedTroopsPanel.Count; i++)
            {
                Parameters p = new Parameters();

                if(i == 0)
                {
                    p.AddParameter<string>("Count", trainedSwordsman.ToString());
                }
                else if(i == 1)
                {
                    p.AddParameter<string>("Count", trainedSpearman.ToString());
                }
                else if(i == 2)
                {
                    p.AddParameter<string>("Count", trainedArcher.ToString());
                }

                trainedTroopsPanel[i].InitializePanel(p);
            }
        }

        public void SetupHeroPanel()
        {
            List<CardPanelHandler> heroPanel = currentPanel.informationPanels;

            if(heroInformation != null && heroInformation.Count > 0)
            {
                for (int i = 0; i < heroPanel.Count; i++)
                {
                    Parameters p = new Parameters();

                    if(i == 0)
                    {
                        p.AddParameter<string>("Count", heroInformation[curHeroIdx].unitInformation.maxHealth.ToString());
                        p.AddParameter<string>("Title", heroInformation[curHeroIdx].heroName);
                        p.AddParameter<string>("Growth", heroInformation[curHeroIdx].healthGrowthRate.ToString());
                    }
                    else if(i == 1)
                    {
                        string dmg = heroInformation[curHeroIdx].unitInformation.minDamage.ToString() + "-" + heroInformation[curHeroIdx].unitInformation.maxDamage.ToString();
                        p.AddParameter<string>("Count", dmg);
                        p.AddParameter<string>("Growth", heroInformation[curHeroIdx].damageGrowthRate.ToString());
                    }
                    else if(i == 2)
                    {
                        p.AddParameter<string>("Count", heroInformation[curHeroIdx].unitInformation.origSpeed.ToString());
                        p.AddParameter<string>("Growth", heroInformation[curHeroIdx].speedGrowthRate.ToString());
                    }

                    heroPanel[i].InitializePanel(p);
                }
            }
            else
            {
                myController.ShowInfoBlocker("Recruit a Hero First, in the tavern.");
            }
        }


    }
}