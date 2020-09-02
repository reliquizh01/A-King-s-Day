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

        public bool computerControlled = false;
        public bool canComputerHeal = false;

        public TextMeshProUGUI readinessText;
        public TextMeshProUGUI waitingText;


        public bool isAttacker = false;

        [Header("Help Tool Mechanics")]
        public GameObject readyTooltipObject;
        public TextMeshProUGUI readyLeft;
        public TextMeshProUGUI readyRight;

        public void Update()
        {
            if (!computerControlled)
            {
                MultiplayerControl();
            }
            else
            {
                ComputerControl();
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

        public void SetControlType(PlayerControlType thisControl)
        {
            controlType = thisControl;
            if(controlType == PlayerControlType.Computer)
            {
                computerControlled = true;
                readyTooltipObject.SetActive(false);
            }

            if (controlType == PlayerControlType.PlayerTwo)
            {
                readyTooltipObject.SetActive(true);
                readyLeft.text = "<-";
                readyRight.text = "->";
            }
            else if(controlType == PlayerControlType.PlayerOne)
            {
                readyTooltipObject.SetActive(true);
                readyLeft.text = "A";
                readyRight.text = "D";
            }
        }

        public void ComputerControl()
        {
            if (BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }

            if(!BattlefieldSystemsManager.GetInstance.unitsInCamp)
            {
                return;
            }

            if(canComputerHeal)
            {
                List<int> selectionAmountCount = new List<int>();
                if(injuredUnitsList != null && injuredUnitsList.Count > 0)
                {
                    for (int i = 0; i < injuredUnitsList.Count; i++)
                    {
                        selectionAmountCount.Add(currentCommander.unitsCarried[i].totalInjuredCount);
                    }
                    selectedInjuredIdx = 0;

                    if(canComputerHeal)
                    {
                        canComputerHeal = false;
                        SetAsSelectedPanel(injuredUnitsList[selectedInjuredIdx]);
                        StartCoroutine(StartClickingPanels(selectionAmountCount));
                    }
                }
            }
        }

        public IEnumerator StartClickingPanels(List<int> selectionAmountCount)
        {
            yield return new WaitForSeconds(0.25f);

            if(selectionAmountCount[selectedInjuredIdx] > 0)
            {
                BaseEventData data = new BaseEventData(EventSystem.current);
                ExecuteEvents.Execute(healBtn.gameObject, data, ExecuteEvents.submitHandler);

                selectionAmountCount[selectedInjuredIdx] -= 1;

                Debug.Log("Healing Panel : " + selectedInjuredIdx + " Selection Count :" + selectionAmountCount.Count);
            }

            if(selectionAmountCount[selectedInjuredIdx] == 0)
            {
                if(selectedInjuredIdx < 3)
                {
                    selectedInjuredIdx += 1;
                    SetAsSelectedPanel(injuredUnitsList[selectedInjuredIdx]);
                    StartCoroutine(StartClickingPanels(selectionAmountCount));
                }
                else
                {
                    SwitchReadiness();
                }
            }
            else
            {
                StartCoroutine(StartClickingPanels(selectionAmountCount));
            }
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
                case PlayerControlType.PlayerTwo:

                    if (Input.GetKeyDown(KeyCode.DownArrow))
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
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
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
                    else if (Input.GetKeyDown(KeyCode.Keypad0))
                    {
                        BaseEventData data = new BaseEventData(EventSystem.current);
                        ExecuteEvents.Execute(healBtn.gameObject, data, ExecuteEvents.submitHandler);
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        SwitchReadiness();
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        SwitchReadiness();
                    }
                    break;
                case PlayerControlType.PlayerOne:

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
                case PlayerControlType.Computer:

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
                if(i <= (currentCommander.unitsCarried.Count-1))
                {
                    injuredUnitsList[i].unitIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(currentCommander.unitsCarried[i].unitInformation.unitName);
                }
            }
        }

        public void SetupDailyReportUnits()
        {
            for (int i = 0; i < currentCommander.unitsCarried.Count; i++)
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

            if(computerControlled)
            {
                canComputerHeal = true;
            }
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
