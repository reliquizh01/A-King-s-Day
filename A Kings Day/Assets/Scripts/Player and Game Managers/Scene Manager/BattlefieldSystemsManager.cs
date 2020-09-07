using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;
using Managers;
using Kingdoms;
using SaveData;
using KingEvents;
using Maps;
using Dialogue;

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
    public int defenderConqured = 0;

    [Header("Victory Condition Information")]
    public int totalVictoryPoints = 0;
    public int defenderVictoryPoints = 0;
    public int defenderTroopPoints = 0;
    public int attackerVictoryPoints = 0;
    public int attackerTroopPoints = 0;
    public int conquerableTiles = 0;
    public bool someoneWon = false;

    [Header("Campaign Mechanics")]
    public TeamType playerTeam;
    public TerritoryOwners enemyTerritoryName;
    public bool playerWon = false;
    List<ResourceReward> coinRewards;

    [Header("Sound Effects")]
    public List<AudioSourceControl> startDaySfxList;

    public void StartDay()
    {

        if(BattlefieldSceneManager.GetInstance == null)
        {
            return;
        }
        dayInProgress = true;
        someoneWon = false;
        unitsInCamp = false;
        currentDay = 0;
        currentDay += 1;

        SetVictoryPoints();
        BattlefieldSceneManager.GetInstance.battleUIInformation.dayTimer.StartTimer(minutePerDay, secondsPerDay, StartOverTimeNoSpawn);
        BattlefieldSceneManager.GetInstance.battleUIInformation.InitializeBattlefieldUI();
        BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateUIInformation();

        if (startDaySfxList != null && startDaySfxList.Count > 0)
        {
            for (int i = 0; i < startDaySfxList.Count; i++)
            {
                startDaySfxList[i].PlayAudio();
            }
        }

        if(AudioManager.GetInstance != null)
        {
            AudioManager.GetInstance.SetVolumeAsBackground();
        }
    }

    public void StartOverTimeNoSpawn()
    {

        dayInProgress = false;
        BattlefieldUIHandler uiHandler = BattlefieldSceneManager.GetInstance.battleUIInformation;

        uiHandler.attackerPanel.playerPlacement.battleTile.HideHoverTile();
        uiHandler.defenderPanel.playerPlacement.battleTile.HideDefenderTile();
        
        uiHandler.endDayOverTimer.gameObject.SetActive(true);
        uiHandler.endDayOverTimer.StartTimer(0, 5, StopCurrentDayActions);

        uiHandler.attackerPanel.leaderSlotHandler.cdCounter.PauseTimer();
        uiHandler.defenderPanel.leaderSlotHandler.cdCounter.PauseTimer();

        uiHandler.attackerPanel.skillSlotHandler.PauseCooldown();
        uiHandler.defenderPanel.skillSlotHandler.PauseCooldown();
    }
    public void GoToNextDay()
    {
        dayInProgress = true;
        unitsInCamp = false;


        BattlefieldUIHandler uiHandler = BattlefieldSceneManager.GetInstance.battleUIInformation;

        uiHandler.attackerPanel.leaderSlotHandler.ContinueCooldown();
        uiHandler.defenderPanel.leaderSlotHandler.ContinueCooldown();
        
        uiHandler.attackerPanel.skillSlotHandler.ContinueCooldown();
        uiHandler.defenderPanel.skillSlotHandler.ContinueCooldown();
        
        uiHandler.dayTimer.StartTimer(minutePerDay, secondsPerDay, StartOverTimeNoSpawn);
        uiHandler.UpdateUIInformation();

        if(uiHandler.attackerPanel.controlType != PlayerControlType.Computer)
        {
            uiHandler.attackerPanel.playerPlacement.battleTile.ShowHoverTile();
        }
        else
        {
            uiHandler.attackerPanel.ComputerPlayerControl();
        }

        if (uiHandler.defenderPanel.controlType != PlayerControlType.Computer)
        {
            uiHandler.defenderPanel.playerPlacement.battleTile.ShowDefenderTile();
        }
        else
        {
            uiHandler.defenderPanel.ComputerPlayerControl();
        }

        if (startDaySfxList != null && startDaySfxList.Count > 0)
        {
            for (int i = 0; i < startDaySfxList.Count; i++)
            {
                startDaySfxList[i].PlayAudio();
            }
        }

        if (AudioManager.GetInstance != null)
        {
            AudioManager.GetInstance.SetVolumeAsBackground();
        }

    }
    public void StopCurrentDayActions()
    {
        Debug.Log("[Stopping All Actions]");

        dayInProgress = false;
        if (currentDay < maxDays)
        {
            currentDay += 1;

            BattlefieldUIHandler uiHandler = BattlefieldSceneManager.GetInstance.battleUIInformation;

            uiHandler.attackerPanel.leaderSlotHandler.PauseCooldown();
            uiHandler.defenderPanel.leaderSlotHandler.PauseCooldown();

            uiHandler.attackerPanel.skillSlotHandler.PauseCooldown();
            uiHandler.defenderPanel.skillSlotHandler.PauseCooldown();

            BattlefieldSpawnManager.GetInstance.RetreatAllUnits(true, BattlefieldSceneManager.GetInstance.EndTodaysBattle);
        }
        else
        {
            Debug.Log("[Checking All Victorious!]");

            BattlefieldSpawnManager.GetInstance.RetreatAllUnits(true, CheckVictorious);
        }

        if (AudioManager.GetInstance != null)
        {
            AudioManager.GetInstance.NormalizeBackgroundVolume();
        }

        BattlefieldSceneManager.GetInstance.battleUIInformation.endDayOverTimer.gameObject.SetActive(false);
    }

    public void CheckVictorious()
    {
        if(someoneWon)
        {
            return;
        }
        UpdateTroopPoints();

        attackerConquered = BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Attacker).Count;
        defenderConqured = BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Defender).Count;

        int allTiles = BattlefieldPathManager.GetInstance.ObtainPathCount();
        TeamType winner = TeamType.Neutral;
        switch (winCondition)
        {
            case BattlefieldWinCondition.ConquerOrEliminateAll:
                if(attackerConquered >= allTiles || defenderTroopPoints <= 0)
                {
                    winner = TeamType.Attacker;
                    if(playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if(defenderConqured >= allTiles || attackerTroopPoints <= 0)
                {
                    winner = TeamType.Defender;
                    if (playerTeam == TeamType.Defender)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if(attackerConquered > defenderConqured && currentDay >= maxDays && !dayInProgress)
                {
                    winner = TeamType.Attacker;
                    if (playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if(defenderConqured > attackerConquered && currentDay >= maxDays && !dayInProgress)
                {
                    winner = TeamType.Defender;
                    if (playerTeam == TeamType.Defender)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                break;
            case BattlefieldWinCondition.EliminateAll:
                Debug.Log("Checkign Eliminate All");
                if(attackerTroopPoints <= 0)
                {

                    Debug.Log("All Attackers Down!");
                    winner = TeamType.Defender;
                    if (playerTeam == TeamType.Defender)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if(defenderTroopPoints <= 0)
                {
                    Debug.Log("All Defenders Down!");
                    winner = TeamType.Attacker;
                    if (playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                break;
            case BattlefieldWinCondition.ConquerAll:
                if (attackerConquered >= allTiles)
                {
                    winner = TeamType.Attacker;
                    if (playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if (defenderConqured >= allTiles)
                {
                    winner = TeamType.Defender;
                    if (playerTeam == TeamType.Defender)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if (attackerConquered > defenderConqured && currentDay >= maxDays && !dayInProgress)
                {
                    winner = TeamType.Attacker;
                    if (playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                else if (defenderConqured > attackerConquered && currentDay >= maxDays && !dayInProgress)
                {
                    winner = TeamType.Defender;
                    if (playerTeam == TeamType.Attacker)
                    {
                        playerWon = true;
                    }
                    else
                    {
                        playerWon = false;
                    }
                    someoneWon = true;
                }
                break;

            default:
                break;
        }

        if(someoneWon)
        {
            BattlefieldSceneManager.GetInstance.battleUIInformation.ShowVictorious(winner, CheckPostVictorious);
            dayInProgress = false;
        }
        else
        {
            if(currentDay >= maxDays && !dayInProgress)
            {
                maxDays += 1;
                dayInProgress = true;
            }
        }
    }

    public void PlayPrologueResultScene()
    {
        if (playerWon)
        {
            BattlefieldSpawnManager.GetInstance.HideAllUnitsHealthBar();
            BattlefieldUIHandler uiHandler = BattlefieldSceneManager.GetInstance.battleUIInformation;
            uiHandler.attackerPanel.skillSlotHandler.myPanel.PlayCloseAnimation();
            uiHandler.defenderPanel.skillSlotHandler.myPanel.PlayCloseAnimation();

            uiHandler.attackerPanel.leaderSlotHandler.myPanel.PlayCloseAnimation();
            uiHandler.defenderPanel.leaderSlotHandler.myPanel.PlayCloseAnimation();

            uiHandler.myPanel.PlayCloseAnimation();
            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Prologue - Making Peace");
            Drama.DramaticActManager.GetInstance.FadeToDark(true, () => DialogueManager.GetInstance.StartConversation(tmp, () => TransitionManager.GetInstance.LoadScene(SceneType.Courtroom)));

        }
        else
        {
            TransitionManager.GetInstance.LoadScene(SceneType.Battlefield);
        }
    }
    public void CheckPostVictorious()
    {
        BattlefieldSceneManager.GetInstance.battleUIInformation.dayTimer.PauseTimer();

        if(playerWon)
        {
            AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.WinInBattle);
        }
        else
        {
            AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.DefeatInBattle);
        }

        if (!BattlefieldSceneManager.GetInstance.isCampaignMode)
        {
            BattlefieldSceneManager.GetInstance.customBattlePanel.ResetCustomBattlePanel();
            BattlefieldSceneManager.GetInstance.customBattlePanel.gameCustomPanel.SetActive(true);
            BattlefieldSceneManager.GetInstance.battleUIInformation.EndBattle();
            BattlefieldPathManager.GetInstance.ResetAllPaths();
            BattlefieldSpawnManager.GetInstance.RetreatAllUnits();
            unitsInCamp = true;
        }
        else
        {
            BattlefieldPathManager.GetInstance.ResetAllPaths();
            BattlefieldSpawnManager.GetInstance.RetreatAllUnits();
            unitsInCamp = true;

            if (TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                ReturnUnitsToPlayerData();
                ReturnUnitsToEnemyData();

                ObtaincoinRewards();
            }
            if (TransitionManager.GetInstance != null && TransitionManager.GetInstance.isEngagedWithMapPoint)
            {
                ObtainTerritoryRewards();

                BattlefieldCommander playerUnits = new BattlefieldCommander();
                if (TransitionManager.GetInstance.isPlayerAttacker)
                {
                    playerUnits = BattlefieldSpawnManager.GetInstance.attackingCommander;
                }
                else
                {
                    playerUnits = BattlefieldSpawnManager.GetInstance.defendingCommander;
                }

                BattlefieldSceneManager.GetInstance.battleUIInformation.ShowCampaignRewards(playerWon, coinRewards, enemyTerritoryName, playerUnits);
            }
            else
            {
                // TUTORIAL
                if (TransitionManager.GetInstance != null && TransitionManager.GetInstance.isNewGame)
                {

                    PlayPrologueResultScene();
                }
                else // TRAVELLER
                {

                }
            }
        }

    }

    #region CAMPAIGN_SYSTEM_MECHANICS
    public void ReturnUnitsToEnemyData()
    {
        BattlefieldCommander enemyUnits = new BattlefieldCommander();
        if (TransitionManager.GetInstance.isPlayerAttacker)
        {
            enemyUnits = BattlefieldSpawnManager.GetInstance.defendingCommander;
        }
        else
        {
            enemyUnits = BattlefieldSpawnManager.GetInstance.attackingCommander;
        }


        if (TransitionManager.GetInstance.isEngagedWithTraveller)
        {
            if(!playerWon)
            {
                TransitionManager.GetInstance.attackedTravellerData.troopsCarried = new List<TroopsInformation>();
                TransitionManager.GetInstance.attackedTravellerData.troopsCarried.AddRange(enemyUnits.unitsCarried);
            }
            else
            {
                PlayerGameManager.GetInstance.campaignData.travellerList.Remove(TransitionManager.GetInstance.attackedTravellerData);
            }
        }
        else if(TransitionManager.GetInstance.isEngagedWithMapPoint)
        {
            enemyTerritoryName = TransitionManager.GetInstance.attackedPointInformationData.ownedBy;

            Debug.Log("[Victory While Engaged Against a Map Point]");
            if(!playerWon)
            {
                Debug.Log("[Map Point Still Has Troops Around: " + enemyUnits.CheckTotalTroopsCount() + "]");
                TransitionManager.GetInstance.attackedPointInformationData.troopsStationed = new List<TroopsInformation>();
                TransitionManager.GetInstance.attackedPointInformationData.troopsStationed.AddRange(enemyUnits.unitsCarried);

                TerritoryOwners pastOwner = TransitionManager.GetInstance.attackedPointInformationData.ownedBy;
                TransitionManager.GetInstance.attackedPointInformationData.previousOwner = pastOwner;
                TransitionManager.GetInstance.attackedPointInformationData.ownedBy = enemyUnits.teamAffiliation;
            }
            else
            {
                TransitionManager.GetInstance.attackedPointInformationData.troopsStationed = new List<TroopsInformation>();

                TerritoryOwners pastOwner = TransitionManager.GetInstance.attackedPointInformationData.ownedBy;
                TransitionManager.GetInstance.attackedPointInformationData.previousOwner = pastOwner;

                TransitionManager.GetInstance.attackedPointInformationData.ownedBy = Maps.TerritoryOwners.Player;
            }
        }

        SaveLoadManager.GetInstance.SaveCurrentCampaignData();
    }

    public void ReturnUnitsToPlayerData()
    {
        BattlefieldCommander playerUnits = new BattlefieldCommander();
        if (TransitionManager.GetInstance.isPlayerAttacker)
        {
            playerUnits = BattlefieldSpawnManager.GetInstance.attackingCommander;
        }
        else
        {
            playerUnits = BattlefieldSpawnManager.GetInstance.defendingCommander;
        }

        // CHANGE PLAYER DATA COUNT STUFF TO UNITS CARRIED (SO PLAYER CAN USE OTHER UNITS LATER ON).
        for (int i = 0; i < playerUnits.unitsCarried.Count; i++)
        {
            TroopsInformation tmp = PlayerGameManager.GetInstance.playerData.troopsList.Find(x => x.unitInformation.unitName == playerUnits.unitsCarried[i].unitInformation.unitName);
            tmp.totalReturningUnitCount = playerUnits.CheckUnitCount(playerUnits.unitsCarried[i].unitInformation.unitName);
            tmp.totalUnitCount -= (tmp.totalReturningUnitCount + playerUnits.CheckUnitDeathCount(playerUnits.unitsCarried[i].unitInformation.unitName));
        }

        SaveLoadManager.GetInstance.SaveCurrentData();
    }

    public void ObtaincoinRewards()
    {
        BattlefieldCommander enemyUnits = new BattlefieldCommander();
        BattlefieldCommander playerUnits = new BattlefieldCommander();
        if (TransitionManager.GetInstance.isPlayerAttacker)
        {
            enemyUnits = BattlefieldSpawnManager.GetInstance.defendingCommander;
            playerUnits = BattlefieldSpawnManager.GetInstance.attackingCommander;
        }
        else
        {
            enemyUnits = BattlefieldSpawnManager.GetInstance.attackingCommander;
            playerUnits = BattlefieldSpawnManager.GetInstance.defendingCommander;
        }


        coinRewards = new List<ResourceReward>();
        // Obtain Salvaged Prize
        int salvagedCoins = 0;

        for (int i = 0; i < enemyUnits.unitsCarried.Count; i++)
        {
            salvagedCoins += enemyUnits.CheckUnitDeathCount(enemyUnits.unitsCarried[i].unitInformation.unitName);
        }
        salvagedCoins += playerUnits.CheckTotalDeadTroopsCount();
        salvagedCoins *= UnityEngine.Random.Range(0,3);


        // Obtain Injured Prize
        int deadCount = playerUnits.CheckTotalDeadTroopsCount();

        ResourceReward deadPenalty = new ResourceReward();
        deadPenalty.resourceTitle = "Dead Penalty";
        deadPenalty.resourceType = ResourceType.Coin;
        deadPenalty.rewardAmount = -deadCount;

        ResourceReward salvagedPrize = new ResourceReward();
        salvagedPrize.resourceTitle = "Salvaged Prize";
        salvagedPrize.resourceType = ResourceType.Coin;
        salvagedPrize.rewardAmount = salvagedCoins;

        coinRewards.Add(salvagedPrize);
        coinRewards.Add(deadPenalty);

        if(PlayerGameManager.GetInstance != null)
        {
            for (int i = 0; i < coinRewards.Count; i++)
            {
                PlayerGameManager.GetInstance.ReceiveResource(coinRewards[i].rewardAmount, coinRewards[i].resourceType);
            }
            SaveLoadManager.GetInstance.SaveCurrentData();
        }

    }

    public void ObtainTerritoryRewards()
    {
        int taxMoney = TransitionManager.GetInstance.attackedPointInformationData.coinTax;
        // Tax Money
        if (playerWon)
        {

            ResourceReward taxPrize = new ResourceReward();
            taxPrize.resourceTitle = "Tax Prize";
            taxPrize.resourceType = ResourceType.Coin;
            taxPrize.rewardAmount = taxMoney;
            coinRewards.Add(taxPrize);
            PlayerGameManager.GetInstance.campaignData.totalWeeklyTax += taxMoney;
        }
        else if(TransitionManager.GetInstance.attackedPointInformationData.ownedBy == TerritoryOwners.Player)
        {
            PlayerGameManager.GetInstance.campaignData.totalWeeklyTax -= taxMoney;
        }


        // Obtain Map Point
        if (PlayerGameManager.GetInstance != null && PlayerGameManager.GetInstance.campaignData != null)
        {
            MapPointInformationData tmp = TransitionManager.GetInstance.attackedPointInformationData;

            if (PlayerGameManager.GetInstance.campaignData.mapPointList.Find(x => x.pointName == tmp.pointName) != null)
            {
                if (playerWon)
                {
                    PlayerGameManager.GetInstance.campaignData.mapPointList.Find(x => x.pointName == tmp.pointName).ownedBy = Maps.TerritoryOwners.Player;
                }
                else
                {
                    PlayerGameManager.GetInstance.campaignData.mapPointList.Find(x => x.pointName == tmp.pointName).ownedBy = enemyTerritoryName;
                }
            }

            SaveLoadManager.GetInstance.SaveCurrentCampaignData();
        }
    }
    #endregion

    #region BATTLE_SYSTEM_MECHANICS
    public void SetVictoryPoints()
    {
        conquerableTiles = BattlefieldPathManager.GetInstance.ObtainPathCount();
        totalVictoryPoints = 0;

        attackerConquered = BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Attacker).Count;
        defenderConqured = BattlefieldPathManager.GetInstance.ObtainConqueredTiles(TeamType.Defender).Count;

        UpdateTroopPoints();
        switch (winCondition)
        {
            case BattlefieldWinCondition.ConquerOrEliminateAll:
                // CURRENT POINTS BASED ON TOTAL UNITS + CONQUERED TILES
                attackerVictoryPoints = attackerTroopPoints + attackerConquered;
                defenderVictoryPoints = defenderTroopPoints + defenderConqured;
                

                // TOTAL VICTORY = UNITS + CONQUERABLE TILES
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(true);
                totalVictoryPoints += BattlefieldSpawnManager.GetInstance.CountCommanderTroops(false);

                break;
            case BattlefieldWinCondition.EliminateAll:


                attackerVictoryPoints = attackerTroopPoints;
                totalVictoryPoints += attackerTroopPoints;

                defenderVictoryPoints = defenderTroopPoints;
                totalVictoryPoints += defenderTroopPoints;

                break;
            case BattlefieldWinCondition.ConquerAll:
                totalVictoryPoints = conquerableTiles;
                break;
            default:
                break;
        }
        UpdateCurrentVictoryPoints();
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
            totalVictoryPoints += 1;
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
    }

    public void UpdateTroopPoints()
    {
        attackerTroopPoints = BattlefieldSpawnManager.GetInstance.attackingCommander.CheckTotalTroopsCount();
        defenderTroopPoints = BattlefieldSpawnManager.GetInstance.defendingCommander.CheckTotalTroopsCount();
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

        UpdateCurrentVictoryPoints();
        UpdateTroopPoints();
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

        UpdateCurrentVictoryPoints();
        UpdateTroopPoints();
    }
    public void UpdateCurrentVictoryPoints()
    {

        BattlefieldSceneManager.GetInstance.battleUIInformation.UpdateVictorySlider(totalVictoryPoints, defenderVictoryPoints);
    }

    #endregion

}
