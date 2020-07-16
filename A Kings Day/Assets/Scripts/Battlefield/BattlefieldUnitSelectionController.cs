using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Characters;
using UnityEngine.UI;
using Utilities;

namespace Battlefield
{
    public enum ClickedMovement
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum PlayerControlType
    {
        PlayerOne,
        PlayerTwo,
        Computer,
    }
    public class BattlefieldUnitSelectionController : MonoBehaviour
    {
        public List<UnitSelectPanel> unitList;
        public UnitSelectPanel selectedUnit;

        public PlayerControlType controlType;
        public ClickedMovement lastClicked;

        private BattlefieldCommander currentCommander;
        [Header("Computer AI")]
        public BattlefieldCommanderComputer computerAI;
        public bool canChangeSummon = true;

        [Header("Resource Information")]
        public CountingEffectUI warChestCount;

        [Header("Selected Information")]
        public int currentSelectedIdx;
        public int curColumnIdx;
        public int curRowIdx;

        private int defenderSelectedIdx = 0;
        private int defenderColumnIdx = 4;
        private int defenderRowIdx = 11;

        private int attackerSelectedIdx = 0;
        private int attackerColumnIdx = 0;
        private int attackerRowIdx = 0;
        public ScenePointBehavior playerPlacement;

        [SerializeField] private float curCountDelaySelectMove;
        private float delaySelectMoveTime = 0.25f;
        [SerializeField]private bool startCounting = false;
        [SerializeField]private bool startContinuousShift = false;

        public bool isAttacker = false;
        public bool isComputer = false;
        public void Start()
        {
            if(unitList != null && unitList.Count > 0)
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    unitList[i].myController = this;
                }
            }

            if(BattlefieldSceneManager.GetInstance != null && !BattlefieldSceneManager.GetInstance.isCampaignMode)
            {

                if(isAttacker)
                {
                    curColumnIdx = attackerColumnIdx;
                    curRowIdx = attackerRowIdx;

                    SetPointBehavior(isAttacker);

                }
                else
                {
                    curColumnIdx = defenderColumnIdx;
                    curRowIdx = defenderRowIdx;

                    SetPointBehavior(isAttacker);

                }

                unitList[0].SetAsSelected();
                selectedUnit = unitList[0];
            }
        }

        public void Update()
        {
            if(BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                if(BattlefieldSceneManager.GetInstance.isCampaignMode)
                {
                    CampaignControls();
                }
                else
                {
                    MultiplayerControl();
                }
            }
        }

        public void MultiplayerControl()
        {
            switch (controlType)
            {
                case PlayerControlType.PlayerOne:
                    PlayerOneControls();
                        break;
                case PlayerControlType.PlayerTwo:
                    PlayerTwoControls();
                    break;
                case PlayerControlType.Computer:

                    break;
                default:
                    break;
            }

            if(startCounting)
            {
                curCountDelaySelectMove += Time.deltaTime;
                if(curCountDelaySelectMove > delaySelectMoveTime)
                {
                    startContinuousShift = true;
                }
                else
                {
                    startContinuousShift = false;
                }
            }
            if(startContinuousShift)
            {
                AdjustMovement(lastClicked);
            }
        }
        public void PlayerOneControls()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentSelectedIdx = 0;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentSelectedIdx = 1;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentSelectedIdx = 2;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentSelectedIdx = 3;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                lastClicked = ClickedMovement.Up;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if(Input.GetKeyUp(KeyCode.W))
            {
                if (lastClicked == ClickedMovement.Up)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                lastClicked = ClickedMovement.Down;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                if(lastClicked == ClickedMovement.Down)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                lastClicked = ClickedMovement.Left;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                if (lastClicked == ClickedMovement.Left)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                lastClicked = ClickedMovement.Right;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                if (lastClicked == ClickedMovement.Right)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }

            // SUMMON CONTROLS
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SummonUnit();
            }
        }

        public void AdjustMovement(ClickedMovement lastMove)
        {
            switch (lastMove)
            {
                case ClickedMovement.Up:
                    if (curColumnIdx <= 0)
                    {
                        curColumnIdx = 0;
                    }
                    else
                    {
                        curColumnIdx -= 1;
                    }
                    break;
                case ClickedMovement.Down:

                    if (curColumnIdx >= BattlefieldPathManager.GetInstance.fieldPaths.Count - 1)
                    {
                        curColumnIdx = BattlefieldPathManager.GetInstance.fieldPaths.Count - 1;
                    }
                    else
                    {
                        curColumnIdx += 1;
                    }

                    break;
                case ClickedMovement.Left:

                    if (curRowIdx >= BattlefieldPathManager.GetInstance.fieldPaths[curColumnIdx].scenePoints.Count - 1)
                    {
                        curRowIdx = BattlefieldPathManager.GetInstance.fieldPaths[curColumnIdx].scenePoints.Count - 1;
                    }
                    else
                    {
                        curRowIdx += 1;
                    }
                    break;
                case ClickedMovement.Right:

                    if (curRowIdx <= 0)
                    {
                        curRowIdx = 0;
                    }
                    else
                    {
                        curRowIdx -= 1;
                    }
                    break;
                default:
                    break;
            }
            curCountDelaySelectMove = 0;
            SetPointBehavior(isAttacker);
        }
        public void PlayerTwoControls()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                currentSelectedIdx = 0;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                currentSelectedIdx = 1;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                currentSelectedIdx = 2;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                currentSelectedIdx = 3;
                SetUnitPanelAsSelected(currentSelectedIdx);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                lastClicked = ClickedMovement.Up;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (lastClicked == ClickedMovement.Up)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                lastClicked = ClickedMovement.Down;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (lastClicked == ClickedMovement.Down)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                lastClicked = ClickedMovement.Left;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (lastClicked == ClickedMovement.Left)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                lastClicked = ClickedMovement.Right;
                AdjustMovement(lastClicked);
                startCounting = true;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (lastClicked == ClickedMovement.Right)
                {
                    curCountDelaySelectMove = 0;
                    startCounting = false;
                    startContinuousShift = false;
                }
            }

            // SUMMON CONTROLS
            if(Input.GetKeyDown(KeyCode.Keypad0))
            {
                SummonUnit();
            }
        }

        public void ComputerPlayerControl()
        {
            curColumnIdx = computerAI.ChooseLane();
            SetPointBehavior(isAttacker);

            if(canChangeSummon)
            {
                int idx = computerAI.ChooseNextUnitIndex();

                if(idx >= 0)
                {
                    SetUnitPanelAsSelected(idx);
                }
            }

            SummonUnit();
        }

        public void CampaignControls()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetUnitPanelAsSelected(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetUnitPanelAsSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetUnitPanelAsSelected(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetUnitPanelAsSelected(3);
            }

        }
        public void SummonUnit()
        {
            if(BattlefieldSpawnManager.GetInstance == null)
            {
                return;
            }
            if(!unitList[currentSelectedIdx].cooldownFinish)
            {
                if(computerAI)
                {
                    canChangeSummon = false;
                }
                return;
            }

            if(BattlefieldSceneManager.GetInstance.isCampaignMode)
            {
                // Player Only
            }
            else
            {
                bool canSpawn = BattlefieldSpawnManager.GetInstance.CheckIfUnitsAvailable(isAttacker, currentSelectedIdx);

                // Check if Can Spawn
                if(canSpawn)
                {
                    ScenePointBehavior spawn = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(curColumnIdx, isAttacker);
                    ScenePointBehavior target = BattlefieldPathManager.GetInstance.ObtainTargetPoint(curColumnIdx, isAttacker);

                    BattlefieldSpawnManager.GetInstance.SpawnUnit(currentSelectedIdx, spawn, target, isAttacker);
                    BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();

                    for (int i = 0; i < unitList.Count; i++)
                    {
                        unitList[i].ResetCooldown();
                    }
                    canChangeSummon = true;
                }
                else
                {

                }

            }

        }

        public void SetCurrentCommander(BattlefieldCommander thisCommander)
        {
            currentCommander = thisCommander;

            for (int i = 0; i < unitList.Count; i++)
            {
                unitList[i].ResetCooldown();
            }

            if (isComputer)
            {
                computerAI.commanderInformation = currentCommander;
            }

        }
        public void SetControlType(PlayerControlType curController,bool isAttacker)
        {
            controlType = curController;
            if(controlType == PlayerControlType.Computer)
            {
                isComputer = true;
            }
            else
            {
                isComputer = false;
            }
            if (BattlefieldSceneManager.GetInstance != null && !BattlefieldSceneManager.GetInstance.isCampaignMode)
            {
                if(isAttacker)
                {
                    curColumnIdx = attackerColumnIdx;
                    curRowIdx = attackerRowIdx;
                    SetPointBehavior(isAttacker);
                }
                else
                {
                    curColumnIdx = defenderColumnIdx;
                    curRowIdx = defenderRowIdx;

                    SetPointBehavior(isAttacker);
                }


                unitList[0].SetAsSelected();
                selectedUnit = unitList[0];
            }
        }
        public void SetPointBehavior(bool isAttacker)
        {
            ScenePointBehavior tmp = null;
            ScenePointBehavior prev = null;
            if (!isAttacker)
            {
                tmp = BattlefieldPathManager.GetInstance.ObtainPath(curColumnIdx, curRowIdx);
                if (tmp != null)
                {
                    if (playerPlacement != null)
                    {
                        playerPlacement.battleTile.HideDefenderTile();
                        prev = playerPlacement;
                    }

                    playerPlacement = tmp;

                    playerPlacement.battleTile.ShowDefenderTile();
                    if (prev != null)
                    {
                        prev.battleTile.CheckOverlapping();
                    }
                }
            }
            else
            {
                tmp = BattlefieldPathManager.GetInstance.ObtainPath(curColumnIdx, curRowIdx);
                if (tmp != null)
                {
                    if(playerPlacement != null)
                    {
                        playerPlacement.battleTile.HideHoverTile();
                        prev = playerPlacement;
                    }

                    playerPlacement = tmp;

                    playerPlacement.battleTile.ShowHoverTile();

                    if (prev != null)
                    {
                        prev.battleTile.CheckOverlapping();
                    }
                }

            }
            
        }
        public void SetUnitPanelAsSelected(int idx)
        {
            selectedUnit = unitList[idx];
            currentSelectedIdx = idx;

            for (int i = 0; i < unitList.Count; i++)
            {
                if(idx != i)
                {
                    unitList[i].UnSelect();
                }
                else
                {
                    unitList[idx].SetAsSelected();
                }
            }
        }
    }
}