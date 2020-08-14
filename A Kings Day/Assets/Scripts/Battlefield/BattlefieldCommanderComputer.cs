using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;

namespace Battlefield
{
    public enum ChooseUnitMindset
    {
        AggressiveSpawning,  // Will Summon any units available as soon as possible
        CautiousSpawning,   // Will Summon against units closest to base.
        ObjectiveSpawning, 
    }

    public enum UnitPlacementMindSet
    {
        ToeToToe,
        FindWeakLane,
        BackupLane,
    }

    public enum CommanderComputerActions
    {
        SummonTroops,
        SummonOnWeakLane,
        ObserveAndGetWeakLane,
    }
    public class BattlefieldCommanderComputer : MonoBehaviour
    {
        public bool isAttacker;
        public ChooseUnitMindset chooseUnitMindset;
        public UnitPlacementMindSet unitPlacementMindset;
        public List<CommanderComputerActions> actionList = new List<CommanderComputerActions>();

        [Header("Commander Information")]
        public BattlefieldCommander commanderInformation;

        [Header("Battlefield Information")]
        public int chosenUnitIdx = 0;
        public int battlefieldPathCount = 0;


        public int ChooseNextUnitIndex()
        {
            int idx = 0;
            List<int> indexWithAvailableTroops = new List<int>();

            if(commanderInformation.unitsCarried != null && commanderInformation.unitsCarried.Count > 0)
            {
                for (int i = 0; i < commanderInformation.unitsCarried.Count; i++)
                {
                    if(commanderInformation.unitsCarried[i].totalUnitsAvailableForDeployment > 0)
                    {
                        indexWithAvailableTroops.Add(i);
                    }
                }

                if(indexWithAvailableTroops.Count > 0)
                {
                    switch (chooseUnitMindset)
                    {
                        case ChooseUnitMindset.AggressiveSpawning:
                            idx = indexWithAvailableTroops[UnityEngine.Random.Range(0, indexWithAvailableTroops.Count)];
                            break;
                        case ChooseUnitMindset.CautiousSpawning:
                            break;
                        case ChooseUnitMindset.ObjectiveSpawning:
                            break;
                        default:
                            break;
                    }
                }
            }

            return idx;
        }

        public int ChooseLane()
        {
            if (!BattlefieldSystemsManager.GetInstance.dayInProgress)
            {
                
                return -1;
            }

            int idx = 0;
            bool mindsetDecided = false;

            List<int> enemyCount = ObserveUnitsInAllPaths((isAttacker) ? TeamType.Attacker : TeamType.Defender);
            List<int> allyCount = ObserveUnitsInAllPaths((isAttacker) ? TeamType.Attacker : TeamType.Defender);
            battlefieldPathCount = BattlefieldPathManager.GetInstance.fieldPaths.Count;

            switch (unitPlacementMindset)
            {
                case UnitPlacementMindSet.ToeToToe:
                    for (int i = 0; i < BattlefieldPathManager.GetInstance.fieldPaths.Count; i++)
                    {
                        if(allyCount[i] < enemyCount[i])
                        {
                            idx = i;
                            mindsetDecided = true;
                            break;
                        }
                    }
                    break;
                case UnitPlacementMindSet.FindWeakLane:
                    break;
                case UnitPlacementMindSet.BackupLane:
                    break;
                default:
                    break;
            }

            if(!mindsetDecided)
            {
                idx = UnityEngine.Random.Range(0, battlefieldPathCount);
            }

            return idx;
        }

        public List<int> ObserveUnitsInAllPaths(TeamType thisTeamUnits)
        {
            List<int> unitCount = new List<int>();

            if (BattlefieldPathManager.GetInstance != null)
            {
                for (int i = 0; i < BattlefieldPathManager.GetInstance.fieldPaths.Count; i++)
                {
                    List<BaseCharacter> charactersInLane = BattlefieldPathManager.GetInstance.ObtainCharactersOnThisPath(i);

                    int enemyCount = 0;

                    for (int x = 0; x < charactersInLane.Count; x++)
                    {
                        if (charactersInLane[x].teamType == thisTeamUnits)
                        {
                            enemyCount += 1;
                        }
                    }

                    unitCount.Add(enemyCount);
                }
            }

            return unitCount;
        }
    }
}
