using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Buildings;
using Utilities;
using Kingdoms;
using Battlefield;
using System;

namespace Managers
{
    public class BattlefieldSpawnManager : BaseManager
    {
        #region Singleton
        private static BattlefieldSpawnManager instance;
        public static BattlefieldSpawnManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            instance = this;
        }
        #endregion


        public KingdomUnitStorage unitStorage;
        public BattlefieldCommander attackingCommander, defendingCommander;

        [Header("Spawned Units")]
        public BaseCharacter attackerHeroLeader;
        public List<BaseCharacter> attackerSpawnedUnits;
        public BaseCharacter defenderHeroLeader;
        public List<BaseCharacter> defenderSpawnedUnits;

        private bool waitingForAttackerRetreat = false;
        private Action attackerRetreatCallback;

        private bool waitingForDefenderRetreat = false;
        private Action defenderRetreatCallBack;

        private bool waitingForAllRetreat = false;
        private Action endCurrentBattleCallback;

        [Header("Test Mode")]
        public bool testModeDebug;

        // Campaign Mode
        public void SetupPlayerCommander(BaseTravellerData troopsInformations, bool isAttacker = true)
        {
            BattlefieldCommander currentCommander = new BattlefieldCommander();
            currentCommander.unitsCarried = new List<TroopsInformation>();
            currentCommander.teamAffiliation = Maps.TerritoryOwners.Player;

            currentCommander.heroesCarried = new List<BaseHeroInformationData>();
            currentCommander.heroesCarried.AddRange(troopsInformations.leaderUnit);

            for (int i = 0; i < troopsInformations.troopsCarried.Count; i++)
            {
                currentCommander.unitsCarried.Add(troopsInformations.troopsCarried[i]);
            }

            currentCommander = ImplementTechnology(currentCommander);

            currentCommander.spawnBuffsList = new List<BaseBuffInformationData>();
            currentCommander.spawnBuffsList.AddRange(CheckCampaignPlayerBuffPenalties());

            if(isAttacker)
            {
                attackingCommander = new BattlefieldCommander();
                attackingCommander = currentCommander;
            }
            else
            {
                defendingCommander = new BattlefieldCommander();
                defendingCommander = currentCommander;
            }

        }


        public void SummonTeamHeroes()
        {
            Debug.Log("Initialize Team heroes");
            GameObject tmp;

            if (attackingCommander.heroesCarried != null && attackingCommander.heroesCarried.Count > 0)
            {
                if (!string.IsNullOrEmpty(attackingCommander.heroesCarried[0].unitInformation.unitName))
                {
                    Debug.Log("Trying to Summon Attacking Commander Hero!");
                    string unitPath = attackingCommander.heroesCarried[0].unitInformation.prefabDataPath.Split('.')[0];
                    unitPath = unitPath.Replace("Assets/Resources/", "");

                    tmp = (GameObject)Resources.Load(unitPath, typeof(GameObject));
                    if (tmp != null)
                    {
                        Debug.Log("Found Hero Prefab!");
                        ScenePointBehavior atkSpawnPoint = BattlefieldPathManager.GetInstance.attackerHeroSpawnpoint;
                        tmp = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), atkSpawnPoint.transform.position, Quaternion.identity, null);

                        BaseCharacter heroCharacter = tmp.GetComponent<BaseCharacter>();
                        heroCharacter.OrderToFace(FacingDirection.Left);
                        heroCharacter.SpawnInThisPosition(atkSpawnPoint);
                        heroCharacter.OrderMovement(atkSpawnPoint.neighborPoints[0]);
                        heroCharacter.isLeadingHero = true;

                        heroCharacter.teamType = TeamType.Attacker;
                        BattlefieldSceneManager.GetInstance.battleUIInformation.attackerPanel.leaderSlotHandler.SetupHeroSpawn(heroCharacter.unitInformation.unitCooldown);
                        attackerHeroLeader = heroCharacter;
                        heroCharacter.SetupCharacter(attackingCommander.heroesCarried[0].unitInformation);
                    }
                }
            }
            if(defendingCommander.heroesCarried != null && defendingCommander.heroesCarried.Count > 0)
            {
                if (!string.IsNullOrEmpty(defendingCommander.heroesCarried[0].unitInformation.unitName))
                {
                    string unitPath = defendingCommander.heroesCarried[0].unitInformation.prefabDataPath.Split('.')[0];
                    unitPath = unitPath.Replace("Assets/Resources/", "");

                    tmp = (GameObject)Resources.Load(unitPath, typeof(GameObject));
                    if (tmp != null)
                    {
                        ScenePointBehavior defSpawnPoint = BattlefieldPathManager.GetInstance.defenderHeroSpawnpoint;
                        tmp = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), defSpawnPoint.transform.position, Quaternion.identity, null);

                        BaseCharacter heroCharacter = tmp.GetComponent<BaseCharacter>();
                        heroCharacter.OrderToFace(FacingDirection.Right);
                        heroCharacter.SpawnInThisPosition(defSpawnPoint);
                        heroCharacter.OrderMovement(defSpawnPoint.neighborPoints[0]);
                        heroCharacter.isLeadingHero = true;

                        heroCharacter.teamType = TeamType.Defender;
                        BattlefieldSceneManager.GetInstance.battleUIInformation.defenderPanel.leaderSlotHandler.SetupHeroSpawn(heroCharacter.unitInformation.unitCooldown);
                        defenderHeroLeader = heroCharacter;
                        heroCharacter.SetupCharacter(defendingCommander.heroesCarried[0].unitInformation);
                    }
                }
            }
        }

        public void SpawnLeaderToBattle(TeamType thisTeamLeader, ScenePointBehavior spawnPoint, ScenePointBehavior targetPoint)
        {
            BaseCharacter thisLeader = null;
            switch (thisTeamLeader)
            {
                case TeamType.Defender:
                    if (defenderHeroLeader == null) return;

                    thisLeader = defenderHeroLeader;

                    if(defendingCommander.spawnBuffsList != null &&
                        defendingCommander.spawnBuffsList.Count > 0)
                    {
                        thisLeader.unitInformation.buffList = new List<BaseBuffInformationData>();

                        for (int i = 0; i < defendingCommander.spawnBuffsList.Count; i++)
                        {
                            thisLeader.AddBuff(defendingCommander.spawnBuffsList[i]);
                        }
                    }
                    defenderSpawnedUnits.Add(thisLeader);
                    break;
                case TeamType.Attacker:
                    if (attackerHeroLeader == null) return;

                    thisLeader = attackerHeroLeader;

                    if (attackingCommander.spawnBuffsList != null &&
                        attackingCommander.spawnBuffsList.Count > 0)
                    {
                        thisLeader.unitInformation.buffList = new List<BaseBuffInformationData>();

                        for (int i = 0; i < attackingCommander.spawnBuffsList.Count; i++)
                        {
                            thisLeader.AddBuff(attackingCommander.spawnBuffsList[i]);
                        }
                    }
                    attackerSpawnedUnits.Add(thisLeader);
                    break;

                case TeamType.Neutral:
                default:
                    break;
            }

            if(thisLeader != null)
            {
                Debug.Log("This Leader Name:" + thisLeader.gameObject.name);
                thisLeader.SpawnInThisPosition(spawnPoint, true);
                thisLeader.isFighting = true;
                thisLeader.OrderMovement(targetPoint);
                thisLeader.canReturnToCamp = true;
                thisLeader.isFighting = true;
                thisLeader.overheadHealthbar.gameObject.SetActive(true);
                thisLeader.overheadHealthbar.SetupHealthBar(thisLeader.unitInformation.curhealth, thisLeader.unitInformation.maxHealth);
                thisLeader.myRange.UpdateTotalRange();
            }
        }
        public BattlefieldCommander ImplementTechnology(BattlefieldCommander commander)
        {
            for (int i = 0; i < commander.unitsCarried.Count; i++)
            {
                // DAMAGE
                commander.unitsCarried[i].unitInformation.minDamage += PlayerGameManager.GetInstance.troopBehavior.techDmg;
                commander.unitsCarried[i].unitInformation.maxDamage += PlayerGameManager.GetInstance.troopBehavior.techDmg;

                // HEALTH
                commander.unitsCarried[i].unitInformation.curhealth += PlayerGameManager.GetInstance.troopBehavior.techHealth;
                commander.unitsCarried[i].unitInformation.maxHealth += PlayerGameManager.GetInstance.troopBehavior.techHealth;

                // MORALE
                commander.unitsCarried[i].unitInformation.morale = PlayerGameManager.GetInstance.troopBehavior.baseMorale +
                                                                   PlayerGameManager.GetInstance.troopBehavior.techMorale;
            }

            return commander;
        }

        public List<BaseBuffInformationData> CheckCampaignPlayerBuffPenalties()
        {
            List<BaseBuffInformationData> penaltyBuffs = new List<BaseBuffInformationData>();
            if(PlayerGameManager.GetInstance.playerData.foods <= 0)
            {
                BaseBuffInformationData tmp = new BaseBuffInformationData();
                tmp.targetStats = TargetStats.health;
                tmp.effectAmount = -1;
                tmp.permanentBuff = true;
                tmp.buffName = "Hunger";

                penaltyBuffs.Add(tmp);
            }

            return penaltyBuffs;
        }
        public void SetupAttackingCommander(BattlefieldCommander thisCommander)
        {
            attackingCommander = thisCommander;
        }

        public void SetupDefendingCommander(BattlefieldCommander thisCommander)
        {
            defendingCommander = thisCommander;
        }


        public void RemoveThisUnit(BaseCharacter thisUnit, UnitState lastState)
        {
            int idx;
            BaseCharacter atkUnit = attackerSpawnedUnits.Find(x => x == thisUnit);
            BaseCharacter defUnit = defenderSpawnedUnits.Find(x => x == thisUnit);

            if(thisUnit == attackerHeroLeader)
            {
                int unitCooldown = thisUnit.unitInformation.unitCooldown;
                ScenePointBehavior spawnPoint = BattlefieldPathManager.GetInstance.attackerHeroSpawnpoint;
                thisUnit.ReceiveHealing(0.1f, UnitAttackType.SPELL, TargetStats.health);

                if (lastState == UnitState.Dead || lastState == UnitState.Injured)
                {
                    unitCooldown += 30;
                    thisUnit.unitInformation.currentState = UnitState.Healthy;
                }

                BattlefieldSceneManager.GetInstance.battleUIInformation.attackerPanel.leaderSlotHandler.SetupHeroSpawn(unitCooldown);
                thisUnit.SpawnInThisPosition(spawnPoint, true);
                thisUnit.OrderMovement(spawnPoint.neighborPoints[0]);

                thisUnit.canRegenerate = true;
                thisUnit.isFighting = false;
                attackerSpawnedUnits.Remove(thisUnit);
            }
            else if(thisUnit == defenderHeroLeader)
            {
                int unitCooldown = thisUnit.unitInformation.unitCooldown;
                ScenePointBehavior spawnPoint = BattlefieldPathManager.GetInstance.defenderHeroSpawnpoint;
                thisUnit.ReceiveHealing(0.1f, UnitAttackType.SPELL, TargetStats.health);

                if (lastState == UnitState.Dead)
                {
                    unitCooldown += 30;
                    thisUnit.UpdateCharacterState(CharacterStates.Injured_State);
                }


                BattlefieldSceneManager.GetInstance.battleUIInformation.defenderPanel.leaderSlotHandler.SetupHeroSpawn(unitCooldown);
                thisUnit.SpawnInThisPosition(spawnPoint, true);
                thisUnit.OrderMovement(spawnPoint.neighborPoints[0]);

                thisUnit.canRegenerate = true;
                thisUnit.isFighting = false;
                defenderSpawnedUnits.Remove(thisUnit);
            }
            else if (atkUnit != null)
            {
                idx = attackingCommander.unitsCarried.FindIndex(x => x.unitInformation.unitName == atkUnit.unitInformation.unitGenericName);
                if(idx >= 0)
                {
                    switch (lastState)
                    {
                        case UnitState.Healthy:
                            attackingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment += 1;
                            attackerSpawnedUnits.Remove(thisUnit);
                            DestroyImmediate(thisUnit.gameObject);
                            if(BattlefieldSceneManager.GetInstance.battleUIInformation.attackerPanel.isComputer)
                            {
                                BattlefieldSceneManager.GetInstance.battleUIInformation.attackerPanel.ComputerPlayerControl();
                            }
                            break;

                        case UnitState.Injured:
                            attackingCommander.unitsCarried[idx].totalUnitCount -= 1;
                            attackingCommander.unitsCarried[idx].totalInjuredCount += 1;
                            BattlefieldSystemsManager.GetInstance.UnitKilled(TeamType.Attacker);
                            break;

                        case UnitState.Dead:
                            attackingCommander.unitsCarried[idx].totalUnitCount -= 1;
                            attackingCommander.unitsCarried[idx].totalDeathCount += 1;
                            BattlefieldSystemsManager.GetInstance.UnitKilled(TeamType.Attacker);
                            break;

                        default:
                            break;
                    }
                }
            }
            else if(defUnit != null)
            {
                idx = defendingCommander.unitsCarried.FindIndex(x => x.unitInformation.unitName == thisUnit.unitInformation.unitGenericName);
                if (idx >= 0)
                {
                    switch (lastState)
                    {
                        case UnitState.Healthy:
                            defendingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment += 1;
                            defenderSpawnedUnits.Remove(thisUnit);
                            DestroyImmediate(thisUnit.gameObject);
                            if (BattlefieldSceneManager.GetInstance.battleUIInformation.defenderPanel.isComputer)
                            {
                                BattlefieldSceneManager.GetInstance.battleUIInformation.defenderPanel.ComputerPlayerControl();
                            }
                            break;

                        case UnitState.Injured:
                            defendingCommander.unitsCarried[idx].totalUnitCount -= 1;
                            defendingCommander.unitsCarried[idx].totalInjuredCount += 1;
                            BattlefieldSystemsManager.GetInstance.UnitKilled(TeamType.Defender);
                            break;

                        case UnitState.Dead:
                            defendingCommander.unitsCarried[idx].totalUnitCount -= 1;
                            defendingCommander.unitsCarried[idx].totalDeathCount += 1;
                            BattlefieldSystemsManager.GetInstance.UnitKilled(TeamType.Defender);
                            break;

                        default:
                            break;
                    }
                }
            }


            bool attackerEmpty = false, defenderEmpty = false;
            if(attackerSpawnedUnits != null && attackerSpawnedUnits.Find(x => x.unitInformation.currentState == UnitState.Healthy) == null)
            {
                attackerEmpty = true;
                if(attackerRetreatCallback != null)
                {
                    attackerRetreatCallback();
                }
            }

            if(defenderSpawnedUnits != null && defenderSpawnedUnits.Find(x => x.unitInformation.currentState == UnitState.Healthy) == null)
            {
                defenderEmpty = true;
                if(defenderRetreatCallBack != null)
                {
                    defenderRetreatCallBack();
                }
            }

            if(BattlefieldSystemsManager.GetInstance.dayInProgress && !BattlefieldSystemsManager.GetInstance.unitsInCamp
                && BattlefieldSystemsManager.GetInstance.winCondition != BattlefieldWinCondition.ConquerAll)
            {
                BattlefieldSystemsManager.GetInstance.CheckVictorious();
            }
            else
            {
                if (defenderEmpty && attackerEmpty)
                {
                    if(endCurrentBattleCallback != null)
                    {
                        endCurrentBattleCallback();
                    }
                }
            }

        }
        public bool CheckIfUnitsAvailable(bool isAttacker, int idx)
        {
            if(isAttacker)
            {
                if (attackingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment <= 0)
                {
                    return false;
                }

            }
            else
            {
                if (defendingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public int CountCommanderTroops(bool isAttacker, bool countDead = true)
        {
            int totalCount = 0;
            if(countDead)
            {
                if(isAttacker)
                {
                    for (int i = 0; i < attackingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += attackingCommander.unitsCarried[i].totalUnitCount;
                        totalCount += attackingCommander.unitsCarried[i].totalInjuredCount;
                        totalCount += attackingCommander.unitsCarried[i].totalDeathCount;
                    }

                    totalCount += attackerSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                else
                {
                    for (int i = 0; i < defendingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += defendingCommander.unitsCarried[i].totalUnitCount;
                        totalCount += defendingCommander.unitsCarried[i].totalInjuredCount;
                        totalCount += defendingCommander.unitsCarried[i].totalDeathCount;
                    }
                    totalCount += defenderSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                return totalCount;
            }
            else
            {
                if (isAttacker)
                {
                    for (int i = 0; i < attackingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += attackingCommander.unitsCarried[i].totalUnitCount;
                    }
                    totalCount += attackerSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                else
                {
                    for (int i = 0; i < defendingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += defendingCommander.unitsCarried[i].totalUnitCount;
                    }
                    totalCount += defenderSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                return totalCount;
            }
        }
        public void SpawnUnit(int unitIdx, ScenePointBehavior spawnPoint, ScenePointBehavior targetPoint, bool isAttacker = true)
        {
            if(BattlefieldSystemsManager.GetInstance != null)
            {
                if(!BattlefieldSystemsManager.GetInstance.dayInProgress)
                {
                    return;
                }
            }

            GameObject tmp = null;
            if (isAttacker)
            {
                if(unitIdx != -1 && unitIdx >= attackingCommander.unitsCarried.Count)
                {
                    return;
                }

                attackingCommander.unitsCarried[unitIdx].totalUnitsAvailableForDeployment -= 1;
                string unitPath = attackingCommander.unitsCarried[unitIdx].unitInformation.prefabDataPath.Split('.')[0];
                unitPath = unitPath.Replace("Assets/Resources/", "");

                tmp = (GameObject)Resources.Load(unitPath, typeof(GameObject));
                if(tmp != null)
                {
                    tmp = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), spawnPoint.transform.position, Quaternion.identity, null);
                    if(attackerSpawnedUnits == null)
                    {
                        attackerSpawnedUnits = new List<BaseCharacter>();
                    }

                    if(tmp.GetComponent<BaseCharacter>() != null)
                    {
                        attackerSpawnedUnits.Add(tmp.GetComponent<BaseCharacter>());
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].SpawnInThisPosition(spawnPoint, true);
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].SetupCharacter(attackingCommander.unitsCarried[unitIdx].unitInformation);
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].isFighting = true;
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].overheadHealthbar.SetupHealthBar(attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].unitInformation.curhealth, attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].unitInformation.maxHealth);
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].canReturnToCamp = true;
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].teamType = TeamType.Attacker;
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].overheadHealthbar.gameObject.SetActive(true);
                        attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].OrderMovement(targetPoint);
                        if(attackingCommander.spawnBuffsList != null && attackingCommander.spawnBuffsList.Count > 0)
                        {
                            for (int i = 0; i < attackingCommander.spawnBuffsList.Count; i++)
                            {
                                attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].AddBuff(attackingCommander.spawnBuffsList[i]);
                            }
                        }

                    }

                }
                else
                {
                    Debug.Log("Unit Is Null : " + unitPath);
                }

            }
            else
            {
                if (unitIdx != -1 && unitIdx >= defendingCommander.unitsCarried.Count)
                {
                    return;
                }

                defendingCommander.unitsCarried[unitIdx].totalUnitsAvailableForDeployment -= 1;
                string unitPath = defendingCommander.unitsCarried[unitIdx].unitInformation.prefabDataPath.Split('.')[0];
                unitPath = unitPath.Replace("Assets/Resources/", "");

                tmp = (GameObject)Resources.Load(unitPath, typeof(GameObject));
                if (tmp != null)
                {
                    tmp = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), spawnPoint.transform.position, Quaternion.identity, null);
                    if (defenderSpawnedUnits == null)
                    {
                        defenderSpawnedUnits = new List<BaseCharacter>();
                    }

                    if (tmp.GetComponent<BaseCharacter>() != null)
                    {
                        defenderSpawnedUnits.Add(tmp.GetComponent<BaseCharacter>());
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].SpawnInThisPosition(spawnPoint, true);
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].SetupCharacter(defendingCommander.unitsCarried[unitIdx].unitInformation);
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].overheadHealthbar.gameObject.SetActive(true);
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].overheadHealthbar.SetupHealthBar(defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].unitInformation.curhealth, defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].unitInformation.maxHealth);
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].canReturnToCamp = true;
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].teamType = TeamType.Defender;
                        defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].isFighting = true;


                        Debug.Log("TargetPoint: " + targetPoint.gameObject.name + " Parent: " + targetPoint.transform.parent.gameObject.name);

                        if(testModeDebug)
                        {
                            List<ScenePointBehavior> pathPoints = ScenePointPathfinder.GetInstance.ObtainScenePath(spawnPoint, targetPoint);

                            Debug.Log("Path Point Count: " + pathPoints.Count);
                        }

                         defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].OrderMovement(targetPoint);
                        if (defendingCommander.spawnBuffsList != null && defendingCommander.spawnBuffsList.Count > 0)
                        {
                            for (int i = 0; i < defendingCommander.spawnBuffsList.Count; i++)
                            {
                                defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].AddBuff(defendingCommander.spawnBuffsList[i]);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Unit Is Null : " + unitPath);
                }

            }

        }
        public void SpawnSkillUnits(string prefabPath, TeamType teamWith, int columnPoint)
        {
            GameObject tmp;
            tmp = (GameObject)Resources.Load(prefabPath);
            ScenePointBehavior spawnPoint = null;
            ScenePointBehavior targetPoint = null;

            List<BattlefieldPathHandler> fieldPaths = BattlefieldPathManager.GetInstance.fieldPaths;
            switch (teamWith)
            {
                case TeamType.Neutral: // MONSTERS SPAWNED, CAN be IMPROVED LATER ON
                    int rand = UnityEngine.Random.Range(0, fieldPaths[columnPoint].scenePoints.Count);
                    spawnPoint = fieldPaths[columnPoint].scenePoints[rand];
                    if(rand < (fieldPaths[columnPoint].scenePoints.Count/2))
                    {
                        targetPoint = fieldPaths[columnPoint].attackerSpawnPoint;
                    }
                    else
                    {
                        targetPoint = fieldPaths[columnPoint].defenderSpawnPoint;
                    }

                    break;
                case TeamType.Defender:
                    spawnPoint = fieldPaths[columnPoint].defenderSpawnPoint;
                    targetPoint = fieldPaths[columnPoint].attackerSpawnPoint;
                    break;
                case TeamType.Attacker:
                    spawnPoint = fieldPaths[columnPoint].attackerSpawnPoint;
                    targetPoint = fieldPaths[columnPoint].defenderSpawnPoint;
                    break;
                default:
                    break;
            }

            tmp = GameObject.Instantiate(tmp, spawnPoint.transform.position, Quaternion.identity, null);
            BaseCharacter tmpCharacter = tmp.GetComponent<BaseCharacter>();


            tmpCharacter.SpawnInThisPosition(spawnPoint);
            tmpCharacter.SetupCharacter(unitStorage.GetUnitInformation(tmpCharacter.unitInformation.unitName));
            tmpCharacter.OrderMovement(targetPoint);
            tmpCharacter.isFighting = true;
            tmpCharacter.overheadHealthbar.gameObject.SetActive(true);
            tmpCharacter.overheadHealthbar.SetupHealthBar(tmpCharacter.unitInformation.curhealth, tmpCharacter.unitInformation.maxHealth);
            tmpCharacter.canReturnToCamp = false;
            tmpCharacter.teamType = teamWith;

            switch (teamWith)
            {
                case TeamType.Defender:
                    if(defendingCommander.spawnBuffsList != null && defendingCommander.spawnBuffsList.Count > 0)
                    {
                        for (int i = 0; i < defendingCommander.spawnBuffsList.Count; i++)
                        {
                            tmpCharacter.AddBuff(defendingCommander.spawnBuffsList[i]);
                        }
                    }
                    defenderSpawnedUnits.Add(tmpCharacter);
                    break;

                case TeamType.Attacker:
                    if (attackingCommander.spawnBuffsList != null && attackingCommander.spawnBuffsList.Count > 0)
                    {
                        for (int i = 0; i < attackingCommander.spawnBuffsList.Count; i++)
                        {
                            tmpCharacter.AddBuff(attackingCommander.spawnBuffsList[i]);
                        }
                    }
                    attackerSpawnedUnits.Add(tmpCharacter);
                    break;
                default:
                    break;
            }
        }
        public void HealUnitForThisCommander(TeamType thisTeam, int unitIdx)
        {
            BattlefieldCommander thisCommander = null;
            switch (thisTeam)
            {

                case TeamType.Defender:
                    thisCommander = defendingCommander;
                    break;

                case TeamType.Attacker:
                    thisCommander = attackingCommander;
                    break;

                case TeamType.Neutral:
                default:
                    break;
            }

            if(thisCommander.resourceAmount >= thisCommander.unitsCarried[unitIdx].unitInformation.healcost)
            {
                if(thisCommander.unitsCarried[unitIdx].totalInjuredCount > 0)
                {
                    thisCommander.resourceAmount -= thisCommander.unitsCarried[unitIdx].unitInformation.healcost;

                    thisCommander.unitsCarried[unitIdx].totalInjuredCount -= 1;
                    thisCommander.unitsCarried[unitIdx].totalUnitCount += 1;
                    thisCommander.unitsCarried[unitIdx].totalUnitsAvailableForDeployment += 1;
                    BattlefieldSystemsManager.GetInstance.IncreaseVictoryPoints(thisTeam);
                }

            }
        }
        public void RetreatTeamUnits(TeamType thisTeam, bool fullheal = false, Action newRetreatCallback = null)
        {
            if(BattlefieldPathManager.GetInstance == null)
            {
                return;
            }
            int column = -1;
            ScenePointBehavior returnToThisPoint;
            switch (thisTeam)
            {
                case TeamType.Defender:
                    defenderRetreatCallBack = newRetreatCallback;
                    for (int i = 0; i < defenderSpawnedUnits.Count; i++)
                    {
                        if(defenderSpawnedUnits[i].unitInformation.curhealth <= 0)
                        {
                            continue;
                        }
                        column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(defenderSpawnedUnits[i].myMovements.currentTargetPoint);
                        returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, false);

                        defenderSpawnedUnits[i].unitInformation.curhealth = defenderSpawnedUnits[i].unitInformation.maxHealth;
                        defenderSpawnedUnits[i].myRange.enemiesInRange.Clear();
                        defenderSpawnedUnits[i].OrderMovement(returnToThisPoint);

                        if (defenderSpawnedUnits[i].myMovements.currentPoint == defenderSpawnedUnits[i].myMovements.currentTargetPoint)
                        {
                            RemoveThisUnit(defenderSpawnedUnits[i], UnitState.Healthy);
                        }
                        else
                        {
                        }


                        if (fullheal)
                        {
                            defenderSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                        }
                    }
                    break;
                case TeamType.Attacker:
                    attackerRetreatCallback = newRetreatCallback;
                    for (int i = 0; i < attackerSpawnedUnits.Count; i++)
                    {
                        if (attackerSpawnedUnits[i].unitInformation.curhealth <= 0)
                        {
                            continue;
                        }
                        column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(attackerSpawnedUnits[i].myMovements.currentTargetPoint);
                        returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, true);

                        attackerSpawnedUnits[i].unitInformation.curhealth = attackerSpawnedUnits[i].unitInformation.maxHealth;
                        attackerSpawnedUnits[i].myRange.enemiesInRange.Clear();
                        attackerSpawnedUnits[i].OrderMovement(returnToThisPoint);

                        if (attackerSpawnedUnits[i].myMovements.currentPoint == attackerSpawnedUnits[i].myMovements.currentTargetPoint)
                        {
                            RemoveThisUnit(attackerSpawnedUnits[i], UnitState.Healthy);
                        }
                        

                        if (fullheal)
                        {
                            attackerSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                        }
                    }
                    break;
                case TeamType.Neutral:

                default:
                    break;
            }
        }

        public void RetreatAllUnits(bool fullheal = false, Action newRetreatAll = null)
        {
            int column = -1;
            ScenePointBehavior returnToThisPoint = null;
            endCurrentBattleCallback = newRetreatAll;

            if(attackerSpawnedUnits.Count > 0 && defenderSpawnedUnits.Count > 0
                && attackerSpawnedUnits.Find(x => x.unitInformation.curhealth > 0) == null
                && defenderSpawnedUnits.Find(x => x.unitInformation.curhealth > 0) == null)
                
            {
                if(endCurrentBattleCallback != null)
                {
                    endCurrentBattleCallback();
                }
            }
            else
            {
                for (int i = 0; i < attackerSpawnedUnits.Count; i++)
                {
                    if(attackerSpawnedUnits[i].unitInformation.curhealth > 0)
                    {
                        column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(attackerSpawnedUnits[i].myMovements.currentTargetPoint);
                        returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, true);


                        attackerSpawnedUnits[i].unitInformation.curhealth = attackerSpawnedUnits[i].unitInformation.maxHealth;
                        attackerSpawnedUnits[i].myRange.enemiesInRange.Clear();
                        attackerSpawnedUnits[i].myMovements.pathToTargetPoint.Clear();
                        attackerSpawnedUnits[i].OrderMovement(returnToThisPoint);
                        attackerSpawnedUnits[i].myMovements.speed = attackerSpawnedUnits[i].unitInformation.RealSpeed + 0.5f;
                        attackerSpawnedUnits[i].isLeaving = true;
                        if (attackerSpawnedUnits[i].myMovements.CheckDistance(returnToThisPoint.transform.position) <= attackerSpawnedUnits[i].myMovements.distForReach)
                        {
                            attackerSpawnedUnits[i].myMovements.FinishPathMovement();
                        }
                        if (fullheal)
                        {
                            attackerSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                        }
                    }

                }

                // DEFENDERS
                for (int i = 0; i < defenderSpawnedUnits.Count; i++)
                {

                    if (defenderSpawnedUnits[i].unitInformation.curhealth > 0)
                    {
                        column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(defenderSpawnedUnits[i].myMovements.currentTargetPoint);
                        returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, false);

                        defenderSpawnedUnits[i].unitInformation.curhealth = defenderSpawnedUnits[i].unitInformation.maxHealth;
                        defenderSpawnedUnits[i].myRange.enemiesInRange.Clear();
                        defenderSpawnedUnits[i].myMovements.pathToTargetPoint.Clear();
                        defenderSpawnedUnits[i].OrderMovement(returnToThisPoint);
                        defenderSpawnedUnits[i].myMovements.speed = defenderSpawnedUnits[i].unitInformation.RealSpeed + 0.85f;
                        defenderSpawnedUnits[i].isLeaving = true;
                        if (defenderSpawnedUnits[i].myMovements.CheckDistance(returnToThisPoint.transform.position) <= defenderSpawnedUnits[i].myMovements.distForReach)
                        {
                            defenderSpawnedUnits[i].myMovements.FinishPathMovement();
                        }
                        if (fullheal)
                        {
                            defenderSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                        }
                    }
                }

                /*
                List<ScenePointBehavior> tmp = BattlefieldPathManager.GetInstance.ObtainSpawnPoints();
                Debug.Log("SCENE POINTS COUNT: " + tmp.Count);
                if(tmp != null && tmp.Count > 0)
                {
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        if(tmp[i].battleTile.characterStepping != null &&
                            tmp[i].battleTile.characterStepping.Count > 0)
                        {
                            for (int x = 0; x < tmp[i].battleTile.characterStepping.Count; x++)
                            {
                                tmp[i].battleTile.characterStepping[x].myMovements.FinishPathMovement();
                            }
                        }
                        if(tmp[i].battleTile.lastCharacterToStepIn != null)
                        {
                            tmp[i].battleTile.lastCharacterToStepIn.myMovements.FinishPathMovement();
                        }

                    }
                }*/
            }
        }
        public int CountSpawnedUnits(bool isAttacker, bool countDead = false)
        {
            int tmp = 0;

            if(isAttacker)
            {
                if (!countDead)
                {
                    tmp = attackerSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                else;
                {
                    tmp = attackerSpawnedUnits.Count;
                }
            }
            else
            {
                if (!countDead)
                {
                    tmp = defenderSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0).Count;
                }
                else;
                {
                    tmp = defenderSpawnedUnits.Count;
                }
            }

            return tmp;
        }
        public void UpdateCommanderResources()
        {
            attackingCommander.resourceAmount += BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Attacker).Count;
            defendingCommander.resourceAmount += BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Defender).Count;

        }
        public void SpawnHero(BaseHeroInformationData heroInformation, ScenePointBehavior spawnPoint)
        {

        }
    }
}
