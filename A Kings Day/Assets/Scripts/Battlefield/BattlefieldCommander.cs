using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Characters;
using Managers;
using Kingdoms;
using Battlefield;
using Maps;

[Serializable]
public class TroopsInformation
{
    public UnitInformationData unitInformation;
    public int totalUnitsAvailableForDeployment;
    public int totalUnitCount;
    public int totalInjuredCount; // Send units here during battle
    public int totalReturningUnitCount; // Transfer Injured units here after day falls
    public int totalDeathCount;

    public static TroopsInformation ConvertToTroopsInformation(UnitInformationData unitData, int availableCount)
    {
        TroopsInformation tmp = new TroopsInformation();
        tmp.unitInformation = new UnitInformationData();
        tmp.unitInformation = unitData;

        tmp.totalUnitsAvailableForDeployment = availableCount;
        tmp.totalUnitCount = availableCount;

        return tmp;
    }
}

[Serializable]
public class BattlefieldCommander
{
    public ChooseUnitMindset unitMindset = ChooseUnitMindset.AggressiveSpawning;
    public TerritoryOwners teamAffiliation;
    public List<TroopsInformation> unitsCarried;
    public List<BaseHeroInformationData> heroesCarried;
    public int resourceAmount = 20;
    public List<BaseBuffInformationData> spawnBuffsList;

    public void SetupBasicCommander()
    {
        if(BattlefieldSceneManager.GetInstance == null)
        {
            return;
        }

        unitsCarried = new List<TroopsInformation>();
        heroesCarried = new List<BaseHeroInformationData>();

        TroopsInformation tmp = new TroopsInformation();
        tmp.unitInformation = BattlefieldSceneManager.GetInstance.spawnManager.unitStorage.basicUnitStorage.Find(x => x.unitName == "Recruit");
        tmp.totalUnitCount = 0;

        TroopsInformation tmp1 = new TroopsInformation();
        tmp1.unitInformation = BattlefieldSceneManager.GetInstance.spawnManager.unitStorage.basicUnitStorage.Find(x => x.unitName == "Swordsman");
        tmp1.totalUnitCount = 0;

        TroopsInformation tmp2 = new TroopsInformation();
        tmp2.unitInformation = BattlefieldSceneManager.GetInstance.spawnManager.unitStorage.basicUnitStorage.Find(x => x.unitName == "Spearman");
        tmp2.totalUnitCount = 0;

        TroopsInformation tmp3 = new TroopsInformation();
        tmp3.unitInformation = BattlefieldSceneManager.GetInstance.spawnManager.unitStorage.basicUnitStorage.Find(x => x.unitName == "Archer");
        tmp3.totalUnitCount = 0;

        unitsCarried.Add(tmp); unitsCarried.Add(tmp1); unitsCarried.Add(tmp2);
        unitsCarried.Add(tmp3);

        //heroesCarried.Add(BattlefieldSceneManager.GetInstance.spawnManager.unitStorage.heroStorage[0]);
    }

    public static BattlefieldCommander ConvertTravellerToCommander(MapPointInformationData thisPoint)
    {
        BattlefieldCommander tmp = new BattlefieldCommander();
        tmp.heroesCarried = new List<BaseHeroInformationData>();

        tmp.unitsCarried = new List<TroopsInformation>();
        tmp.unitsCarried.AddRange(thisPoint.troopsStationed);
        tmp.unitMindset = thisPoint.aiMindset;

        if (thisPoint.leaderUnit != null)
        {
            tmp.heroesCarried.Add(thisPoint.leaderUnit);
        }

        return tmp;
    }

    public static BattlefieldCommander ConvertTravellerToCommander(BaseTravellerData thisTraveller)
    {
        BattlefieldCommander tmp = new BattlefieldCommander();
        tmp.heroesCarried = new List<BaseHeroInformationData>();

        tmp.unitsCarried = new List<TroopsInformation>();
        tmp.unitsCarried.AddRange(thisTraveller.troopsCarried);
        tmp.unitMindset = thisTraveller.leaderMindset;
        if(thisTraveller.leaderUnit != null)
        {
            tmp.heroesCarried.AddRange(thisTraveller.leaderUnit);
        }

        return tmp;
    }

    public int CheckUnitCount(string troopName)
    {
        int tmp = 0;

        tmp += unitsCarried.Find(x => x.unitInformation.unitName == troopName).totalUnitCount;
        tmp += unitsCarried.Find(x => x.unitInformation.unitName == troopName).totalInjuredCount;
        return tmp;
    }

    public int CheckUnitDeathCount(string troopName)
    {
        int tmp = 0;

        tmp += unitsCarried.Find(x => x.unitInformation.unitName == troopName).totalDeathCount;

        return tmp;
    }

    public int CheckTotalDeadTroopsCount()
    {
        int tmp = 0;

        if (unitsCarried != null && unitsCarried.Count > 0)
        {
            for (int i = 0; i < unitsCarried.Count; i++)
            {
                tmp += unitsCarried[i].totalDeathCount;
            }
        }

        return tmp;
    }
    public int CheckTotalTroopsCount()
    {
        int tmp = 0;

        if (unitsCarried != null && unitsCarried.Count > 0)
        {
            for (int i = 0; i < unitsCarried.Count; i++)
            {
                tmp += unitsCarried[i].totalUnitCount;
            }
        }

        if(heroesCarried != null && heroesCarried.Count > 0)
        {
            tmp += heroesCarried.Count;
        }

        return tmp;
    }
}
