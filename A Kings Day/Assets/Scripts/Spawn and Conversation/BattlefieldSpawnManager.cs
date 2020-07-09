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

        public List<BaseCharacter> attackerSpawnedUnits;
        public List<BaseCharacter> defenderSpawnedUnits;

        private bool waitingForAttackerRetreat = false;
        private Action attackerRetreatCallback;

        private bool waitingForDefenderRetreat = false;
        private Action defenderRetreatCallBack;

        private bool waitingForAllRetreat = false;
        private Action endCurrentBattleCallback;

        // Campaign Mode
        public void SetupPlayerCommander(PlayerKingdomData playerKingdomData, bool isAttacker = true)
        {
            BattlefieldCommander currentCommander = new BattlefieldCommander();

            TroopsInformation tmp = new TroopsInformation();
            tmp.unitInformation = unitStorage.basicUnitStorage.Find(x => x.unitName == "Recruit");
            tmp.totalUnitCount = playerKingdomData.recruits;
            tmp.totalUnitsAvailableForDeployment = tmp.totalUnitCount;

            TroopsInformation tmp1 = new TroopsInformation();
            tmp1.unitInformation = unitStorage.basicUnitStorage.Find(x => x.unitName == "Swordsman");
            tmp1.totalUnitCount = playerKingdomData.swordsmenCount;
            tmp.totalUnitsAvailableForDeployment = tmp.totalUnitCount;

            TroopsInformation tmp2 = new TroopsInformation();
            tmp2.unitInformation = unitStorage.basicUnitStorage.Find(x => x.unitName == "Spearman");
            tmp2.totalUnitCount = playerKingdomData.spearmenCount;
            tmp2.totalUnitsAvailableForDeployment = tmp2.totalUnitCount;

            TroopsInformation tmp3 = new TroopsInformation();
            tmp3.unitInformation = unitStorage.basicUnitStorage.Find(x => x.unitName == "Archer");
            tmp3.totalUnitCount = playerKingdomData.archerCount;
            tmp3.totalUnitsAvailableForDeployment = tmp3.totalUnitCount;

            currentCommander.unitsCarried.Add(tmp); currentCommander.unitsCarried.Add(tmp1);
            currentCommander.unitsCarried.Add(tmp2); currentCommander.unitsCarried.Add(tmp3);

            currentCommander = ImplementTechnology(currentCommander);

            if(isAttacker)
            {
                attackingCommander = currentCommander;
            }
            else
            {
                defendingCommander = currentCommander;
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

            if (atkUnit != null)
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

            Debug.Log(attackerEmpty + " / " + defenderEmpty);
            if (defenderEmpty && attackerEmpty && !BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                Debug.Log("ENDING ALL!");
                endCurrentBattleCallback();
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
                }
                else
                {
                    for (int i = 0; i < defendingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += defendingCommander.unitsCarried[i].totalUnitCount;
                        totalCount += defendingCommander.unitsCarried[i].totalInjuredCount;
                        totalCount += defendingCommander.unitsCarried[i].totalDeathCount;
                    }
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
                }
                else
                {
                    for (int i = 0; i < defendingCommander.unitsCarried.Count; i++)
                    {
                        totalCount += defendingCommander.unitsCarried[i].totalUnitCount;
                    }
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

            GameObject tmp;
            if (isAttacker)
            {
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

                    attackerSpawnedUnits.Add(tmp.GetComponent<BaseCharacter>());
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].SpawnInThisPosition(spawnPoint);
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].SetupCharacter(attackingCommander.unitsCarried[unitIdx].unitInformation);
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].OrderMovement(targetPoint);
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].isFighting = true;
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].canReturnToCamp = true;
                    attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].teamType = TeamType.Attacker;

                }
                else
                {
                    Debug.Log("Unit Is Null : " + unitPath);
                }

            }
            else
            {
                defendingCommander.unitsCarried[unitIdx].totalUnitsAvailableForDeployment -= 1;
                string unitPath = defendingCommander.unitsCarried[unitIdx].unitInformation.prefabDataPath.Split('.')[0];
                unitPath = unitPath.Replace("Assets/Resources/", "");

                tmp = (GameObject)Resources.Load(unitPath);
                if (tmp != null)
                {
                    tmp = GameObject.Instantiate(tmp, spawnPoint.transform.position, Quaternion.identity, null);
                    if (defenderSpawnedUnits == null)
                    {
                        defenderSpawnedUnits = new List<BaseCharacter>();
                    }

                    defenderSpawnedUnits.Add(tmp.GetComponent<BaseCharacter>());
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].SpawnInThisPosition(spawnPoint);
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].SetupCharacter(defendingCommander.unitsCarried[unitIdx].unitInformation);
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].OrderMovement(targetPoint);
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].isFighting = true;
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].canReturnToCamp = true;
                    defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].teamType = TeamType.Defender;
                }
                else
                {
                    Debug.Log("Unit Is Null : " + defendingCommander.unitsCarried[unitIdx].unitInformation.prefabDataPath);
                }

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
            ScenePointBehavior returnToThisPoint;
            endCurrentBattleCallback = newRetreatAll;

            if(attackerSpawnedUnits.Count <= 0 && defenderSpawnedUnits.Count <= 0)
            {
                endCurrentBattleCallback();
            }
            else
            {
                for (int i = 0; i < attackerSpawnedUnits.Count; i++)
                {
                    if (attackerSpawnedUnits[i].unitInformation.curhealth <= 0)
                    {
                        continue;
                    }
                    column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(attackerSpawnedUnits[i].myMovements.currentTargetPoint);
                    returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, true);

                    attackerSpawnedUnits[i].unitInformation.curhealth = attackerSpawnedUnits[i].unitInformation.maxHealth;
                    attackerSpawnedUnits[i].unitInformation.curhealth = attackerSpawnedUnits[i].unitInformation.maxHealth;
                    attackerSpawnedUnits[i].myRange.enemiesInRange.Clear();
                    attackerSpawnedUnits[i].myMovements.pathToTargetPoint.Clear();
                    attackerSpawnedUnits[i].OrderMovement(returnToThisPoint);
                    attackerSpawnedUnits[i].myMovements.speed = attackerSpawnedUnits[i].unitInformation.RealSpeed + 0.5f;
                    if (fullheal)
                    {
                        attackerSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                    }
                }

                // DEFENDERS
                for (int i = 0; i < defenderSpawnedUnits.Count; i++)
                {
                    if (defenderSpawnedUnits[i].unitInformation.curhealth <= 0)
                    {
                        continue;
                    }
                    column = BattlefieldPathManager.GetInstance.ObtainColumnByPoint(defenderSpawnedUnits[i].myMovements.currentTargetPoint);
                    returnToThisPoint = BattlefieldPathManager.GetInstance.ObtainSpawnPoint(column, false);

                    defenderSpawnedUnits[i].unitInformation.curhealth = defenderSpawnedUnits[i].unitInformation.maxHealth;
                    defenderSpawnedUnits[i].myRange.enemiesInRange.Clear();
                    defenderSpawnedUnits[i].myMovements.pathToTargetPoint.Clear();
                    defenderSpawnedUnits[i].OrderMovement(returnToThisPoint);
                    defenderSpawnedUnits[i].myMovements.speed = defenderSpawnedUnits[i].unitInformation.RealSpeed + 0.5f;
                    if (fullheal)
                    {
                        defenderSpawnedUnits[i].unitInformation.currentState = UnitState.Healthy;
                    }
                }
            }
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
