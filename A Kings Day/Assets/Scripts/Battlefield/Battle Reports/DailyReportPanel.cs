using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Battlefield;
using Managers;
using TMPro;
using Characters;
using UnityEngine.EventSystems;

namespace Battlefield
{
    public class DailyReportPanel : MonoBehaviour
    {
        public BasePanelBehavior myPanel;

        public CountingEffectUI warChestCount;
        public CountingEffectUI tileCount;

        public List<InjuredUnitCounter> injuredUnitsList;
        public InjuredUnitCounter currentSelectedInjuredUnits;
        public int selectedInjuredIdx;

        public TeamType reportTeam;
        public PlayerControlType controlType;
        [SerializeField]private BattlefieldCommander currentCommander;
        private bool isMultiPlayerControl;

        public Button healBtn;
        public bool isReady = false;
        public TextMeshProUGUI readinessText;
        public TextMeshProUGUI waitingText;
        public void Update()
        {
            if(isMultiPlayerControl)
            {
                MultiplayerControl();
            }
        }
        public void PanelControl()
        {
            SetAsSelectedPanel(injuredUnitsList[0]);

            if(!BattlefieldSceneManager.GetInstance.isCampaignMode)
            {
                isMultiPlayerControl = true;
                for (int i = 0; i < injuredUnitsList.Count; i++)
                {
                    injuredUnitsList[i].allowClick = false;
                }
            }
            else
            {
                isMultiPlayerControl = false;
                for (int i = 0; i < injuredUnitsList.Count; i++)
                {
                    injuredUnitsList[i].allowClick = true;
                }
            }
        }
        public void SetAsSelectedPanel(InjuredUnitCounter thisPanel)
        {
            if(currentSelectedInjuredUnits != null)
            {
                currentSelectedInjuredUnits.UnSelect();
            }

            currentSelectedInjuredUnits = thisPanel;
            selectedInjuredIdx = injuredUnitsList.FindIndex(x => x == currentSelectedInjuredUnits);

            currentSelectedInjuredUnits.SetAsSelected();
        }
        public void MultiplayerControl()
        {
            if(BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }
            if(!BattlefieldSystemsManager.GetInstance.unitsInCamp)
            {
                return;
            }

            switch (controlType)
            {
                case PlayerControlType.PlayerOne:
                    if(Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if(selectedInjuredIdx < injuredUnitsList.Count-1)
                        {
                            selectedInjuredIdx += 1;
                        }
                        else
                        {
                            selectedInjuredIdx = 0;
                        }
                    }
                    else if(Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (selectedInjuredIdx > 0)
                        {
                            selectedInjuredIdx -= 1;
                        }
                        else
                        {
                            selectedInjuredIdx = injuredUnitsList.Count-1;
                        }
                    }
                    else if(Input.GetKeyDown(KeyCode.Keypad0))
                    {
                        BaseEventData data = new BaseEventData(EventSystem.current);
                        ExecuteEvents.Execute(healBtn.gameObject, data, ExecuteEvents.submitHandler);
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        SwitchReadiness();
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        SwitchReadiness();
                    }
                    break;
                case PlayerControlType.PlayerTwo:
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (selectedInjuredIdx < injuredUnitsList.Count - 1)
                        {
                            selectedInjuredIdx += 1;
                        }
                        else
                        {
                            selectedInjuredIdx = 0;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (selectedInjuredIdx > 0)
                        {
                            selectedInjuredIdx -= 1;
                        }
                        else
                        {
                            selectedInjuredIdx = injuredUnitsList.Count - 1;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        BaseEventData data = new BaseEventData(EventSystem.current);
                        ExecuteEvents.Execute(healBtn.gameObject, data, ExecuteEvents.submitHandler);
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        SwitchReadiness();
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        SwitchReadiness();
                    }
                    break;
                default:
                    break;
            }
            SetAsSelectedPanel(injuredUnitsList[selectedInjuredIdx]);
        }

        public void SwitchReadiness()
        {
            isReady = !isReady;

            if(isReady)
            {
                readinessText.text = "READY";
                readinessText.color = Color.green;
            }
            else
            {
                readinessText.text = "PREPARING";
                readinessText.color = Color.yellow;
            }
            
            if(BattlefieldSceneManager.GetInstance != null)
            {
                BattlefieldSceneManager.GetInstance.CheckCommanderReadiness();
            }
        }

        public void ShowWaiting(bool showIt)
        {

        }
        public void HealThisUnit()
        {
            if(BattlefieldSpawnManager.GetInstance != null)
            {
                BattlefieldSpawnManager.GetInstance.HealUnitForThisCommander(reportTeam, selectedInjuredIdx);
                warChestCount.SetTargetCount(currentCommander.resourceAmount);
            }

            SetupDailyReportUnits();
            if(BattlefieldSceneManager.GetInstance != null)
            {
                BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();
            }
        }
        public void AssignCommander(BattlefieldCommander thisCommander)
        {
            currentCommander = thisCommander;
            PanelControl();
        }

        // ANIMATION REVELATIONS
        public void RevealTileCount()
        {
            int conqueredTileCount = BattlefieldPathManager.GetInstance.ObtainConqueredTiles(reportTeam).Count;

            tileCount.startUpdating = true;
            tileCount.SetTargetCount(conqueredTileCount);
        }

        public void RevealTotalChestCount()
        {

            warChestCount.startUpdating = true; 
            warChestCount.SetTargetCount(currentCommander.resourceAmount);


            if (BattlefieldSceneManager.GetInstance != null)
            {
                BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();
            }

            for (int i = 0; i < injuredUnitsList.Count; i++)
            {
                injuredUnitsList[i].unitIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(currentCommander.unitsCarried[i].unitInformation.unitName);
            }
        }

        public void SetupDailyReportUnits()
        {
            for (int i = 0; i < injuredUnitsList.Count; i++)
            {
                if(currentCommander.unitsCarried[i] == null)
                {
                    continue;
                }
                int injuredCount = currentCommander.unitsCarried[i].totalInjuredCount;
                injuredUnitsList[i].injuredCount.SetTargetCount(injuredCount);
                injuredUnitsList[i].injuredCount.startUpdating = true;
            }
        }


        public void AdjustInjuryPanelVisual()
        {
            for (int i = 0; i < injuredUnitsList.Count; i++)
            {
                if (injuredUnitsList[i].injuredCount.targetCount == 0)
                {
                    injuredUnitsList[i].DisablePanel();
                }
                else
                {
                    injuredUnitsList[i].EnablePanel();

                }
            }
            Debug.Log("Finish Preparation Speed");
        }
        public void ShowDailyReport()
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, AdjustInjuryPanelVisual));
        }

        public void HideDailyReport()
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, SwitchReadiness));
        }
    }
}
