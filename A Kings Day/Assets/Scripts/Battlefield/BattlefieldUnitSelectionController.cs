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
        public TeamType teamType;

        private BattlefieldCommander currentCommander;
        [Header("Computer AI")]
        public BattlefieldCommanderComputer computerAI;
        public bool canChangeSummon = true;
        [Header("Skill Slot Mechanics")]
        public BattlefieldSkillsHandler skillSlotHandler;
        public SummonHeroSlotHandler leaderSlotHandler;

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
            skillSlotHandler.myController = this;
            leaderSlotHandler.myController = this;

            if (BattlefieldSceneManager.GetInstance != null && !BattlefieldSceneManager.GetInstance.isCampaignMode)
            {

                if(isAttacker)
                {
                    curColumnIdx = attackerColumnIdx;
                    curRowIdx = attackerRowIdx;

                    SetPointBehavior();

                }
                else
                {
                    curColumnIdx = defenderColumnIdx;
                    curRowIdx = defenderRowIdx;

                    SetPointBehavior();

                }

                unitList[0].SetAsSelected();
                selectedUnit = unitList[0];
            }

            skillSlotHandler.myPanel.PlayOpenAnimation();
        }

        public void Update()
        {
            if(BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                // Later on Add ON-LINE Controls (Multiplayer is only good for Campaign and Local 1v1)
                MultiplayerControl();
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
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!leaderSlotHandler.allowSpawning)
                {
                    return;
                }
                SummonLeader();
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
            SetPointBehavior();
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

            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (!leaderSlotHandler.allowSpawning)
                {
                    return;
                }
                SummonLeader();
            }
        }   

        public void ComputerPlayerControl()
        {
            if(!BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }
            

            if(canChangeSummon)
            {
                curColumnIdx = computerAI.ChooseLane();
                SetPointBehavior();

                int idx = computerAI.ChooseNextUnitIndex();

                if(idx >= 0)
                {
                    if(currentCommander.unitsCarried[idx].totalUnitsAvailableForDeployment > 0)
                    {
                        SetUnitPanelAsSelected(idx);
                    }
                    else
                    {
                        idx = computerAI.ChooseNextUnitIndex();
                    }
                }
                canChangeSummon = false;
            }
            else
            {
                canChangeSummon = true;
                SummonUnit();
            }

            ComputerPlayerSkillControl(true);
        }

        public void ComputerPlayerSkillControl(bool disregardSummon = false)
        {
            if(currentCommander.heroesCarried == null ||
                currentCommander.heroesCarried.Count <= 0)
            {
                return;
            }

            if(!BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }

            for (int i = 0; i < currentCommander.heroesCarried[0].skillsList.Count; i++)
            {
                BaseSkillInformationData thisSkill = currentCommander.heroesCarried[0].skillsList[i];

                if(skillSlotHandler.skillSlotList[i].cdCounter.gameObject.activeInHierarchy)
                {
                    continue;
                }

                switch (thisSkill.skillType)
                {
                    // SKILLS THAT REQUIRE KNOWLEDGE
                    case SkillType.Defensive:
                    case SkillType.DefensiveBuff:
                        if (thisSkill.targetType == TargetType.UnitOnTiles)
                        {
                            // Check how many Tiles have units on it.
                            int tileWithAllies = 0;
                            switch (teamType)
                            {
                                case TeamType.Defender:
                                    tileWithAllies = BattlefieldPathManager.GetInstance.pathsWithDefender.Count;
                                    break;
                                case TeamType.Attacker:
                                    tileWithAllies = BattlefieldPathManager.GetInstance.pathsWithAttacker.Count;
                                    break;
                                default:
                                    break;
                            }

                            int halfOfMaxTarget = (thisSkill.maxRange / 2);
                            if (tileWithAllies > halfOfMaxTarget)
                            {
                                skillSlotHandler.curSkillIdx = i;
                                skillSlotHandler.ActivateThisSkill(i);
                            }
                        }
                        else if(thisSkill.targetType == TargetType.UnitOnly)
                        {
                            int unitsLeft = BattlefieldSpawnManager.GetInstance.CountSpawnedUnits(isAttacker, false);
                            if(unitsLeft > 0)
                            {
                                skillSlotHandler.curSkillIdx = i;
                                skillSlotHandler.ActivateThisSkill(i);
                            }
                        }
                        else
                        {
                            skillSlotHandler.curSkillIdx = i;
                            skillSlotHandler.ActivateThisSkill(i);
                        }
                        break;
                    case SkillType.Offensive:
                    case SkillType.OffensiveBuff:
                        if (thisSkill.targetType == TargetType.UnitOnTiles)
                        {
                            // Check how many Tiles have units on it.
                            int tilesWithEnemies = 0;
                            switch (teamType)
                            {
                                case TeamType.Neutral:
                                    tilesWithEnemies = BattlefieldPathManager.GetInstance.pathsWithAttacker.Count + BattlefieldPathManager.GetInstance.pathsWithDefender.Count;
                                    break;
                                case TeamType.Defender:
                                    tilesWithEnemies = BattlefieldPathManager.GetInstance.pathsWithAttacker.Count;
                                    break;
                                case TeamType.Attacker:
                                    tilesWithEnemies = BattlefieldPathManager.GetInstance.pathsWithDefender.Count;
                                    break;
                                default:
                                    break;
                            }

                            int halfOfMaxTarget = (thisSkill.maxRange / 2);
                            if (tilesWithEnemies > halfOfMaxTarget)
                            {
                                skillSlotHandler.curSkillIdx = i;
                                skillSlotHandler.ActivateThisSkill(i);
                            }
                        }
                        else if(thisSkill.targetType == TargetType.UnitOnly)
                        {
                            int unitsLeft = BattlefieldSpawnManager.GetInstance.CountSpawnedUnits(!isAttacker, false);
                            if (unitsLeft > 0)
                            {
                                skillSlotHandler.curSkillIdx = i;
                                skillSlotHandler.ActivateThisSkill(i);
                            }
                        }
                        else
                        {
                            skillSlotHandler.curSkillIdx = i;
                            skillSlotHandler.ActivateThisSkill(i);
                        }
                        break;
                        


                        break;

                    // SKILL THAT CAN BE QUICK CASTED.
                    case SkillType.SummonUnits:

                        skillSlotHandler.curSkillIdx = i;
                        skillSlotHandler.ActivateThisSkill(i);

                        break;
                    default:
                        break;
                }

            }

            if(!disregardSummon)
            {
                ComputerPlayerControl();
            }
            ComputerSummonLeaderControl();
        }

        public void ComputerSummonLeaderControl()
        {
            if(!leaderSlotHandler.allowSpawning)
            {
                return;
            }

            if(!BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }

            BaseCharacter computerLeader = null;

            switch (teamType)
            {
                case TeamType.Defender:
                    computerLeader = BattlefieldSpawnManager.GetInstance.defenderHeroLeader;
                    break;
                case TeamType.Attacker:
                    computerLeader = BattlefieldSpawnManager.GetInstance.attackerHeroLeader;
                    break;
                default:
                    break;
            }

            float checkHpByHalf = computerLeader.unitInformation.curhealth * 0.5f;

            if(computerLeader.unitInformation.curhealth > checkHpByHalf)
            {
                SummonLeader();
            }
        }
        public void SummonLeader()
        {
            if(!TransitionManager.GetInstance.isNewGame)
            {
                if (!BattlefieldSystemsManager.GetInstance.dayInProgress)
                {
                    return;
                }

                if (BattlefieldSpawnManager.GetInstance == null)
                {
                    return;
                }
            }

            ScenePointBehavior spawn = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(curColumnIdx, isAttacker);
            ScenePointBehavior target = BattlefieldPathManager.GetInstance.ObtainTargetPoint(curColumnIdx, isAttacker);

            BattlefieldSpawnManager.GetInstance.SpawnLeaderToBattle(teamType, spawn, target);
            leaderSlotHandler.SummonHero();
        }
        public void SummonUnit()
        {
            if (!BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                return;
            }

            if (BattlefieldSpawnManager.GetInstance == null)
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

            Debug.Log("Summoning Unit");

            bool canSpawn = BattlefieldSpawnManager.GetInstance.CheckIfUnitsAvailable(isAttacker, currentSelectedIdx);
            if(canSpawn)
            {
                if(BattlefieldPathManager.GetInstance == null)
                {
                    return;
                }
                ScenePointBehavior spawn = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(curColumnIdx, isAttacker);
                ScenePointBehavior target = BattlefieldPathManager.GetInstance.ObtainTargetPoint(curColumnIdx, isAttacker);

                BattlefieldSpawnManager.GetInstance.SpawnUnit(currentSelectedIdx, spawn, target, isAttacker);
                for (int i = 0; i < unitList.Count; i++)
                {
                    unitList[i].ResetCooldown();
                }
                BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();
            }
            /*
            // Check if Can Spawn
            if(canSpawn)
            {
                Debug.Log("OBtaining Spawn Points");


                BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUnitPanels();

                canChangeSummon = true;
            }
            else
            {
                // Create Reaction if unit is wrongly clicked.
            }*/

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
            teamType = (isAttacker) ? TeamType.Attacker : TeamType.Defender;
            BattlefieldUIHandler myHandler = BattlefieldSceneManager.GetInstance.battleUIInformation;
            if (controlType == PlayerControlType.Computer)
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
                    SetPointBehavior();
                }
                else
                {
                    curColumnIdx = defenderColumnIdx;
                    curRowIdx = defenderRowIdx;

                    SetPointBehavior();
                }


                unitList[0].SetAsSelected();
                selectedUnit = unitList[0];
            }

            skillSlotHandler.SetupSkillSlots();
        }
        public void SetPointBehavior()
        {
            bool isAttacker = (teamType == TeamType.Attacker)? true: false;
            
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