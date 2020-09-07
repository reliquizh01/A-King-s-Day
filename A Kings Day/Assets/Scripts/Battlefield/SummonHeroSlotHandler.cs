using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Battlefield;

public class SummonHeroSlotHandler : MonoBehaviour
{
    public BattlefieldUnitSelectionController myController;
    public BasePanelBehavior myPanel;
    public TextMeshProUGUI heroSummonText;
    public Image spawnIcon;
    public Sprite EmptyIcon;
    public Sprite heroSummonIcon;

    public TeamType myTeam;
    public bool isClickable;

    public TimerUI cdCounter;
    public bool startCounting;

    public bool allowSpawning;

    public int maxCounter;

    public void SetupHeroSpawn(int newUnitCooldown)
    {
        maxCounter = newUnitCooldown;

        cdCounter.gameObject.SetActive(true);
        cdCounter.StartTimer(0, maxCounter, AllowHeroSpawn);
        switch (myController.controlType)
        {
            case PlayerControlType.PlayerOne:
                heroSummonText.text = "R";
                break;
            case PlayerControlType.PlayerTwo:
                heroSummonText.text = "6";
                break;
            case PlayerControlType.Computer:
                heroSummonText.text = "-";
                break;
            default:
                break;
        }
    }
    public void AllowHeroSpawn()
    {
        allowSpawning = true;
        cdCounter.gameObject.SetActive(false);

        if(myController.controlType == PlayerControlType.Computer)
        {
            myController.ComputerSummonLeaderControl();
        }
    }

    public void PauseCooldown()
    {
        cdCounter.startTimer = false;
    }

    public void ContinueCooldown()
    {
        if(!allowSpawning)
        {
            cdCounter.startTimer = true;
        }
    }
    public void SummonHero()
    {
        if(!allowSpawning)
        {
            return;
        }
        allowSpawning = false;

        cdCounter.gameObject.SetActive(true);
        cdCounter.timerText.text = "In Battle";

    }
}
