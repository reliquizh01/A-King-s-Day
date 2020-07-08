using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Characters;
using TMPro;
using UnityEngine.UI;

namespace Battlefield
{


    public class BattlefieldUIHandler : MonoBehaviour
    {
        public TimerUI dayTimer;
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

        public void SetUnitPanels(BattlefieldCommander attacker, BattlefieldCommander defender)
        {
            attackerReportPanel.AssignCommander(attacker);
            defenderReportPanel.AssignCommander(defender);

            attackerPanel.warChestCount.targetCount = attacker.resourceAmount;
            defenderPanel.warChestCount.targetCount = defender.resourceAmount;

            for (int i = 0; i < attackerPanel.unitList.Count; i++)
            {
                attackerPanel.unitList[i].currentMaxCooldown = attacker.unitsCarried[i].unitInformation.unitCooldown;
                attackerPanel.unitList[i].countText.text = attacker.unitsCarried[i].totalUnitCount.ToString();
                if(attacker.unitsCarried[i].totalUnitCount <= 0)
                {
                    attackerPanel.unitList[i].DisablePanel();
                }
                else
                {
                    attackerPanel.unitList[i].EnablePanel();
                }
            }

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                defenderPanel.unitList[i].currentMaxCooldown = defender.unitsCarried[i].unitInformation.unitCooldown;
                defenderPanel.unitList[i].countText.text = defender.unitsCarried[i].totalUnitCount.ToString();

                if (defender.unitsCarried[i].totalUnitCount <= 0)
                {
                    defenderPanel.unitList[i].DisablePanel();
                }
                else
                {
                    defenderPanel.unitList[i].EnablePanel();
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
        public void UpdateUnitPanels()
        {
            if (BattlefieldSpawnManager.GetInstance == null)
            {
                return;
            }

            BattlefieldCommander attacker = BattlefieldSpawnManager.GetInstance.attackingCommander;
            BattlefieldCommander defender = BattlefieldSpawnManager.GetInstance.defendingCommander;

            attackerPanel.warChestCount.targetCount = attacker.resourceAmount;
            attackerPanel.warChestCount.startUpdating = true;
            defenderPanel.warChestCount.targetCount = defender.resourceAmount;
            defenderPanel.warChestCount.startUpdating = true;

            for (int i = 0; i < attackerPanel.unitList.Count; i++)
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

            for (int i = 0; i < defenderPanel.unitList.Count; i++)
            {
                defenderPanel.unitList[i].currentMaxCooldown = defender.unitsCarried[i].unitInformation.unitCooldown;
                defenderPanel.unitList[i].countText.text = defender.unitsCarried[i].totalUnitsAvailableForDeployment.ToString();
            }
        }


        public void UpdateVictorySlider(int totalVictoryPoints, int currentValue)
        {
            victorySlider.maxValue = totalVictoryPoints;
            victorySlider.value = currentValue;
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
                nextDayTimer.StartTimer(0, 5, StartNextDay);
            }
            else
            {
                nextDayTimer.startTimer = false;
                nextDayTimer.gameObject.SetActive(false);
            }
        }

        public void StartNextDay()
        {
            attackerReportPanel.myPanel.PlayCloseAnimation();
            defenderReportPanel.myPanel.PlayCloseAnimation();
            nextDayTimer.gameObject.SetActive(false);

            BattlefieldSystemsManager.GetInstance.StartDay();
        }
    }

}
