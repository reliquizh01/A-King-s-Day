using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Characters;
using TMPro;
using UnityEngine.UI;
using System;
using KingEvents;
using Maps;

namespace Battlefield
{


    public class BattlefieldUIHandler : MonoBehaviour
    {
        public TimerUI dayTimer;
        public TimerUI endDayOverTimer;
        public TextMeshProUGUI dayCounter;
        public BasePanelBehavior myPanel;
        public Slider victorySlider;

        public BattlefieldUnitSelectionController attackerPanel;
        public BattlefieldUnitSelectionController defenderPanel;

        public DailyReportPanel attackerReportPanel;
        public DailyReportPanel defenderReportPanel;

        public CountingEffectUI attackerWarChestCount;
        public CountingEffectUI defenderWarChestCount;

        public TimerUI nextDayTimer;
        public GameObject nextDayCover;


        public BasePanelBehavior defenderVictoryPanel, attackerVictoryPanel;
        public CampaignRewardsPanel winRewardsPanel, defeatRewardsPanel;
        public void SetUnitPanels(BattlefieldCommander attacker, BattlefieldCommander defender)
        {
            attackerReportPanel.AssignCommander(attacker);
            defenderReportPanel.AssignCommander(defender);

            attackerPanel.warChestCount.SetTargetCount(attacker.resourceAmount);
            defenderPanel.warChestCount.SetTargetCount(defender.resourceAmount);

            attackerPanel.SetCurrentCommander(attacker);
            defenderPanel.SetCurrentCommander(defender);

            for (int i = 0; i < attackerPanel.unitList.Count; i++)
            {
                if(i < attacker.unitsCarried.Count)
                {
                    attackerPanel.unitList[i].currentMaxCooldown = attacker.unitsCarried[i].unitInformation.unitCooldown;
                    attacker.unitsCarried[i].totalUnitsAvailableForDeployment = attacker.unitsCarried[i].totalUnitCount;
                    attackerPanel.unitList[i].countText.text = attacker.unitsCarried[i].totalUnitCount.ToString();
                    attackerPanel.unitList[i].unitImage.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(attacker.unitsCarried[i].unitInformation.unitName);
                    if (attacker.unitsCarried[i].totalUnitCount <= 0)
                    {
                        attackerPanel.unitList[i].DisablePanel();
                    }
                    else
                    {
                        attackerPanel.unitList[i].EnablePanel();
                    }
                }
                else
                {
                    attackerPanel.unitList[i].SetAsUnknown();
                }
            }

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                if (i < defender.unitsCarried.Count)
                {
                    defenderPanel.unitList[i].currentMaxCooldown = defender.unitsCarried[i].unitInformation.unitCooldown;
                    defender.unitsCarried[i].totalUnitsAvailableForDeployment = defender.unitsCarried[i].totalUnitCount;
                    defenderPanel.unitList[i].countText.text = defender.unitsCarried[i].totalUnitCount.ToString();
                    defenderPanel.unitList[i].unitImage.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(defender.unitsCarried[i].unitInformation.unitName);

                    if (defender.unitsCarried[i].totalUnitCount <= 0)
                    {
                        defenderPanel.unitList[i].DisablePanel();
                    }
                    else
                    {
                        defenderPanel.unitList[i].EnablePanel();
                    }
                }
                else
                {
                    defenderPanel.unitList[i].SetAsUnknown();
                }
            }

            UpdateUnitPanels();
        }

        public void InitializeBattlefieldUI()
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, StartCounting));
        }

        public void ShowdailyReportPanel()
        {
            if(BattlefieldSpawnManager.GetInstance == null)
            {
                return;
            }

            Debug.Log("[Showing Daily Report Panel]");
            attackerReportPanel.gameObject.SetActive(true);
            attackerReportPanel.ShowDailyReport();
            defenderReportPanel.gameObject.SetActive(true);
            defenderReportPanel.ShowDailyReport();
        }
        public void UpdateUIInformation()
        {
            dayCounter.text = "Day " + BattlefieldSystemsManager.GetInstance.currentDay;
            StartCounting();
            UpdateUnitPanels();
        }
        public void StartCounting()
        {
            for (int i = 0; i < attackerPanel.unitList.Count; i++)
            {
                attackerPanel.unitList[i].currentCooldownCounter = 0;
                attackerPanel.unitList[i].startCounting = true;
            }

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                defenderPanel.unitList[i].currentCooldownCounter = 0;
                defenderPanel.unitList[i].startCounting = true;
            }
        }

        public void ResetCounters()
        {
            for (int i = 0; i < attackerPanel.unitList.Count; i++)
            {
                attackerPanel.unitList[i].currentCooldownCounter = 0;
                attackerPanel.unitList[i].startCounting = false;
            }

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                defenderPanel.unitList[i].currentCooldownCounter = 0;
                defenderPanel.unitList[i].startCounting = false;
            }
        }
        public void UpdateUnitPanels()
        {
            if (BattlefieldSpawnManager.GetInstance == null)
            {
                return;
            }

            BattlefieldCommander attacker = BattlefieldSpawnManager.GetInstance.attackingCommander;
            BattlefieldCommander defender = BattlefieldSpawnManager.GetInstance.defendingCommander;

            attackerPanel.warChestCount.SetTargetCount(attacker.resourceAmount);
            attackerPanel.warChestCount.startUpdating = true;
            defenderPanel.warChestCount.SetTargetCount(defender.resourceAmount);
            defenderPanel.warChestCount.startUpdating = true;

            for (int i = 0; i < attackerPanel.unitList.Count; i++)
            {
                if(i < attacker.unitsCarried.Count)
                {
                    attackerPanel.unitList[i].currentMaxCooldown = attacker.unitsCarried[i].unitInformation.unitCooldown;
                    attackerPanel.unitList[i].countText.text = attacker.unitsCarried[i].totalUnitsAvailableForDeployment.ToString();

                    if (attacker.unitsCarried[i].totalUnitsAvailableForDeployment <= 0)
                    {
                        attackerPanel.unitList[i].DisablePanel();
                    }
                    else
                    {
                        attackerPanel.unitList[i].EnablePanel();
                    }
                }
            }

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                if(i < defender.unitsCarried.Count)
                {
                    defenderPanel.unitList[i].currentMaxCooldown = defender.unitsCarried[i].unitInformation.unitCooldown;
                    defenderPanel.unitList[i].countText.text = defender.unitsCarried[i].totalUnitsAvailableForDeployment.ToString();

                    if (defender.unitsCarried[i].totalUnitsAvailableForDeployment <= 0)
                    {
                        defenderPanel.unitList[i].DisablePanel();
                    }
                    else
                    {
                        defenderPanel.unitList[i].EnablePanel();
                    }
                }
            }
        }


        public void UpdateVictorySlider(int totalVictoryPoints, int currentValue)
        {
            victorySlider.maxValue = totalVictoryPoints;
            victorySlider.value = currentValue;
        }

        public void ShowVictorious(TeamType thisTeam, Action callBack = null)
        {
            switch (thisTeam)
            {
                case TeamType.Neutral:

                    break;
                case TeamType.Defender:
                    defenderVictoryPanel.gameObject.SetActive(true);
                    defenderVictoryPanel.gameObject.GetComponent<AudioSource>().Play();
                    StartCoroutine(defenderVictoryPanel.WaitAnimationForAction(defenderVictoryPanel.openAnimationName, () => StartCoroutine(defenderVictoryPanel.WaitAnimationForAction(defenderVictoryPanel.closeAnimationName, callBack))));
                    break;
                case TeamType.Attacker:
                    attackerVictoryPanel.gameObject.SetActive(true);
                    attackerVictoryPanel.gameObject.GetComponent<AudioSource>().Play();
                    StartCoroutine(attackerVictoryPanel.WaitAnimationForAction(attackerVictoryPanel.openAnimationName, () => StartCoroutine(attackerVictoryPanel.WaitAnimationForAction(attackerVictoryPanel.closeAnimationName, callBack))));
                    break;
                default:
                    break;
            }
        }

        public void ShowCampaignRewards(bool playerHasWon, List<ResourceReward> rewards, TerritoryOwners enemyOwner, BattlefieldCommander playerCommander)
        {
            CampaignRewardsPanel thisPanel = (playerHasWon) ? winRewardsPanel : defeatRewardsPanel;

            thisPanel.playerCommander = playerCommander;
            if(thisPanel.playerRewards == null)
            {
                thisPanel.playerRewards = new List<ResourceReward>();
            }
            thisPanel.playerRewards = rewards;
            thisPanel.enemyOwner = enemyOwner;

            if(TransitionManager.GetInstance != null && TransitionManager.GetInstance.attackedPointInformationData != null)
            {
                thisPanel.territoryName.text = TransitionManager.GetInstance.attackedPointInformationData.pointName;
            }

            thisPanel.gameObject.SetActive(true);
        }
        public void RewardButtonLoadPreviousScene()
        {
            TransitionManager.GetInstance.LoadScene(TransitionManager.GetInstance.previousScene);
        }
        public bool CheckOverLapping()
        {
            return attackerPanel.playerPlacement == defenderPanel.playerPlacement;
        }

        public void CheckReadiness()
        {
            if(attackerReportPanel.isReady && defenderReportPanel.isReady)
            {
                nextDayTimer.gameObject.SetActive(true);
                nextDayCover.gameObject.SetActive(true);
                nextDayTimer.StartTimer(0, 5, StartNextDay);
            }
            else
            {
                nextDayTimer.startTimer = false;
                nextDayTimer.gameObject.SetActive(false);
                nextDayCover.gameObject.SetActive(false);
            }
        }

        public void StartNextDay()
        {
            attackerReportPanel.HideDailyReport();
            defenderReportPanel.HideDailyReport();
            nextDayTimer.gameObject.SetActive(false);

            BattlefieldSystemsManager.GetInstance.GoToNextDay();

            if (attackerPanel.isComputer)
            {
                attackerPanel.ComputerPlayerControl();
            }

            if (defenderPanel.isComputer)
            {
                defenderPanel.ComputerPlayerControl();
            }
        }

        public void EndBattle()
        {
            ResetCounters();
            nextDayTimer.ResetTimer();
        }
    }

}
