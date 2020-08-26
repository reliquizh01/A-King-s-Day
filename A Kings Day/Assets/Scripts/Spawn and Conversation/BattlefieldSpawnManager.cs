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
        public void SetupPlayerCommander(BaseTravellerData troopsInformations, bool isAttacker = true)
        {
            BattlefieldCommander currentCommander = new BattlefieldCommander();
            currentCommander.unitsCarried = new List<TroopsInformation>();

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
                    if(attackingCommander.spawnBuffsList != null && attackingCommander.spawnBuffsList.Count > 0)
                    {
                        for (int i = 0; i < attackingCommander.spawnBuffsList.Count; i++)
                        {
                            attackerSpawnedUnits[attackerSpawnedUnits.Count - 1].AddBuff(attackingCommander.spawnBuffsList[i]);
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

                    if (defendingCommander.spawnBuffsList != null && defendingCommander.spawnBuffsList.Count > 0)
                    {
                        for (int i = 0; i < attackingCommander.spawnBuffsList.Count; i++)
                        {
                            defenderSpawnedUnits[defenderSpawnedUnits.Count - 1].AddBuff(defendingCommander.spawnBuffsList[i]);
                        }
                    }

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
            BattlefieldSystemsManager.GetInstance.IncreaseVictoryPoints(thisTeam);
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

            if(attackerSpawnedUnits.Count <= 0 && defenderSpawnedUnits.Count <= 0)
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
