using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Characters;

public class HeroTrainInformationPage : BuildingInformationPage
{
    [Header("Player Hero List")]
    public List<BaseHeroInformationData> playerHeroList;
    [Header("Current Hero")]
    public BaseHeroInformationData currentHero;
    public int currentIdx;

    public void Start()
    {
        if(PlayerGameManager.GetInstance.playerData.myHeroes == null || PlayerGameManager.GetInstance.playerData.myHeroes.Count <= 0)
        {
            return;
        }
        if (playerHeroList != null && playerHeroList.Count > 0)
        {
            playerHeroList.Clear();
        }

        playerHeroList = new List<BaseHeroInformationData>();
        playerHeroList.AddRange(PlayerGameManager.GetInstance.playerData.myHeroes);

        currentIdx = 0;
        currentHero = playerHeroList[currentIdx];
    }
    public void OnEnable()
    {
        if(playerHeroList != null && playerHeroList.Count > 0)
        {
            playerHeroList.Clear();
        }

        playerHeroList = new List<BaseHeroInformationData>();
        playerHeroList.AddRange(PlayerGameManager.GetInstance.playerData.myHeroes);
    }

    public override void ImplementPageAction(int idx)
    {
        currentHero.upgradesAdded += 1;
        base.ImplementPageAction(idx);
        switch (idx)
        {
            case 0: // HEALTH
                currentHero.unitInformation.curhealth += 1;
                currentHero.unitInformation.maxHealth += 1;
                break;
            case 1: // DAMAGE
                currentHero.unitInformation.minDamage += 1;
                currentHero.unitInformation.maxDamage += 1;
                break;
            case 2: // SPEED
                currentHero.unitInformation.origSpeed += 0.1f;
                break;
            default:
                break;
        }
    }

    public override int ObtainRewardMultiplier()
    {
        if(!string.IsNullOrEmpty(currentHero.unitInformation.unitName))
        {
            return 0;
        }

        return currentHero.upgradesAdded;
    }
    public void UpdateCurrentHero()
    {
        currentHero = playerHeroList[currentIdx];
    }

    public void ArrowLeft()
    {
        if(currentIdx > 0)
        {
            currentIdx -= 1;
        }
        else
        {
            currentIdx = playerHeroList.Count - 1;
        }
    }

    public void ArrowRight()
    {
        if (currentIdx < playerHeroList.Count)
        {
            currentIdx += 1;
        }
        else
        {
            currentIdx = 0;
        }
    }
    public override bool HasMetRequirements()
    {
        if(currentHero == null || string.IsNullOrEmpty(currentHero.unitInformation.unitName))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
