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
    public bool unitsInCamp = false;
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
        unitsInCamp = false;

        SetVictoryPoints();
        BattlefieldSceneManager.GetInstance.battleUIInformation.dayTimer.StartTimer(minutePerDay, secondsPerDay, StopCurrentDayActions);
        BattlefieldSceneManager.GetInstance.battleUIInformation.InitializeBattlefieldUI();

    }

    public void GoToNextDay()
    {
        dayInProgress = true;
        unitsInCamp = false;

        BattlefieldSceneManager.GetInstance.battleUIInformation.dayTimer.StartTimer(minutePerDay, secondsPerDay, StopCurrentDayActions);
        BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUIInformation();
    }
    public void StopCurrentDayActions()
    {
        dayInProgress = false;
        if(currentDay < maxDays)
        {
            currentDay += 1;

            BattlefieldSpawnManager.GetInstance.RetreatAllUnits(true, BattlefieldSceneManager.GetInstance.EndTodaysBattle);
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

                attackerVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true, false);
                defenderVictoryPoints = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false, false);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);

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

    public void UpdateTotalVictoryPoints()
    {
        conquerableTiles = BattlefieldPathManager.GetInstance.ObtainConqueredTiles();

        switch (winCondition)
        {
            case BattlefieldWinCondition.ConquerOrEliminateAll:
                totalVictoryPoints = defenderVictoryPoints + attackerVictoryPoints;
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

        //Debug.Log("Panel Conquered By : " + newOwner);
        if(previousOwner == TeamType.Neutral)
        {
            switch (newOwner)
            {
                case TeamType.Defender:
                    IncreaseVictoryPoints(TeamType.Defender);
                    break;
                case TeamType.Attacker:
                    IncreaseVictoryPoints(TeamType.Attacker);
                    break;

                case TeamType.Neutral:
                default:
                    break;
            }

        }
        else if (previousOwner != newOwner)
        {
            switch (previousOwner)
            {
                case TeamType.Defender:
                    DecreaseVictoryPoints(TeamType.Defender);

                    if (newOwner == TeamType.Attacker)
                    {
                        IncreaseVictoryPoints(TeamType.Attacker);
                    }
                    break;
                case TeamType.Attacker:
                    DecreaseVictoryPoints(TeamType.Attacker);
                    if(newOwner == TeamType.Defender)
                    {
                        IncreaseVictoryPoints(TeamType.Defender);
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
    public void UnitKilled(TeamType thisTeam)
    {
        if(winCondition == BattlefieldWinCondition.ConquerAll)
        {
            return;
        }

        DecreaseVictoryPoints(thisTeam);

        if(winCondition == BattlefieldWinCondition.ConquerOrEliminateAll)
        {
            UpdateConqueredState();
        }
        UpdateTotalVictoryPoints();
    }

    public void UpdateConqueredState()
    {
        if (BattlefieldSpawnManager.GetInstance == null || BattlefieldPathManager.GetInstance == null)
        {
            return;
        }

        // Attackers
        int AtkaliveCount = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true, false);
        int DefaliveCount = BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false, false);

        if(AtkaliveCount <= 0 && DefaliveCount <= 0)
        {
            BattlefieldPathManager.GetInstance.ConvertAllToOneTeam(TeamType.Neutral);
        }
        else if(AtkaliveCount <= 0)
        {
            BattlefieldPathManager.GetInstance.ConvertAllToOneTeam(TeamType.Defender);
        }
        else if(DefaliveCount <= 0)
        {
            BattlefieldPathManager.GetInstance.ConvertAllToOneTeam(TeamType.Attacker);
        }

    }
    public void IncreaseVictoryPoints(TeamType thisTeam)
    {
        switch (thisTeam)
        {
            case TeamType.Neutral:
                break;
            case TeamType.Defender:
                defenderVictoryPoints += 1;
                break;
            case TeamType.Attacker:
                attackerVictoryPoints += 1;
                break;
            default:
                break;
        }

    }

    public void DecreaseVictoryPoints(TeamType thisTeam)
    {
        switch (thisTeam)
        {
            case TeamType.Neutral:
                break;
            case TeamType.Defender:
                defenderVictoryPoints -= 1;
                break;
            case TeamType.Attacker:
                attackerVictoryPoints -= 1;
                break;
            default:
                break;
        }
    }
    public void UpdateCurrentVictoryPoints()
    {
        BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateVictorySlider(totalVictoryPoints, defenderVictoryPoints);
    }

}
