using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;
using Managers;


public enum BattlefieldWinCondition
{
    ConquerOrEliminateAll,
    EliminateAll,
    ConquerAll,
}
public class BattlefieldSystemsManager : MonoBehaviour
{
    #region Singleton
    private static BattlefieldSystemsManager instance;
    public static BattlefieldSystemsManager GetInstance
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

    public int currentDay;
    public int maxDays = 7;
    public int minutePerDay = 2, secondsPerDay = 0;
    public bool dayInProgress;

    [Header("Victory Type")]
    public BattlefieldWinCondition winCondition;
    public int attackerConquered = 0;

    [Header("Victory Condition Information")]
    public int totalVictoryPoints = 0;
    public int defenderVictoryPoints = 0;
    public int attackerVictoryPoints = 0;
    public int conquerableTiles = 0;

    public void StartDay()
    {

        if(BattlefieldSceneManager.GetInstance == null)
        {
            return;
        }
        dayInProgress = true;

        SetVictoryPoints();
        BattlefieldSceneManager.GetInstance.battleUIInformation.dayTimer.StartTimer(minutePerDay, secondsPerDay, StopCurrentDayActions);
        BattlefieldSceneManager.GetInstance.battleUIInformation.InitializeBattlefieldUI();
    }

    public void StopCurrentDayActions()
    {
        dayInProgress = false;
        if(currentDay < maxDays)
        {
            currentDay += 1;
        }
    }


    public void SetVictoryPoints()
    {
        conquerableTiles = BattlefieldPathManager.GetInstance.ObtainConqueredTiles();
        totalVictoryPoints = 0;

        switch (winCondition)
        {
            case BattlefieldWinCondition.ConquerOrEliminateAll:
                totalVictoryPoints += conquerableTiles;

                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);
                attackerVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                defenderVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);

                break;
            case BattlefieldWinCondition.EliminateAll:

                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);
                attackerVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                defenderVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);

                break;
            case BattlefieldWinCondition.ConquerAll:
                totalVictoryPoints = conquerableTiles;
                break;
            default:
                break;
        }
        UpdateCurrentVictoryPoints();
    }

    public void UpdateTileVictoryPoints()
    {
        int newTileCount = BattlefieldPathManager.GetInstance.ObtainConqueredTiles();

        switch (winCondition)
        {
            case BattlefieldWinCondition.ConquerOrEliminateAll:

                totalVictoryPoints = newTileCount;
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);

                break;
            case BattlefieldWinCondition.EliminateAll:

                break;
            case BattlefieldWinCondition.ConquerAll:
                totalVictoryPoints = conquerableTiles;
                break;

            default:
                break;
        }
    }
    public void PanelConquered(TeamType newOwner, TeamType previousOwner)
    {
        if(winCondition == BattlefieldWinCondition.EliminateAll)
        {
            return;
        }

        switch (newOwner)
        {
            case TeamType.Defender:
                defenderVictoryPoints += 1;
                break;
            case TeamType.Attacker:
                attackerVictoryPoints += 1;
                break;

            case TeamType.Neutral:
            default:
                break;
        }

        if(previousOwner != newOwner)
        {
            switch (previousOwner)
            {
                case TeamType.Defender:
                    defenderVictoryPoints -= 1;
                    if (newOwner == TeamType.Attacker)
                    {
                        attackerVictoryPoints += 1;
                    }
                    break;
                case TeamType.Attacker:
                    attackerVictoryPoints -= 1;

                    if(newOwner == TeamType.Defender)
                    {
                        defenderVictoryPoints += 1;
                    }
                    break;

                case TeamType.Neutral:
                    break;

                default:
                    break;
            }
        }

        UpdateCurrentVictoryPoints();
    }
    public void UnitKilled(bool fromAttackers)
    {
        if(winCondition == BattlefieldWinCondition.ConquerAll)
        {
            return;
        }

        if(fromAttackers)
        {
            attackerVictoryPoints -= 1;
            defenderVictoryPoints += 1;
        }
        else
        {
            defenderVictoryPoints -= 1;
            attackerVictoryPoints += 1;
        }

    }

    public void UpdateCurrentVictoryPoints()
    {
        BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateVictorySlider(totalVictoryPoints, defenderVictoryPoints);
    }

}
