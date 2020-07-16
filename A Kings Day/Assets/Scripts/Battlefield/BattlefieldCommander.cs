using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Characters;
using Managers;

[Serializable]
public class TroopsInformation
{
    public UnitInformationData unitInformation;
    public int totalUnitsAvailableForDeployment;
    public int totalUnitCount;
    public int totalInjuredCount; // Send units here during battle
    public int totalReturningUnitCount; // Transfer Injured units here after day falls
    public int totalDeathCount;
}

[Serializable]
public class BattlefieldCommander
{
    public List<TroopsInformation> unitsCarried;
    public List<BaseHeroInformationData> heroesCarried;
    public int resourceAmount = 20;

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

    public int CheckTotalTroopsCount()
    {
        int tmp = 0;

        for (int i = 0; i < unitsCarried.Count; i++)
        {
            tmp += unitsCarried[i].totalUnitCount;
        }

        tmp += heroesCarried.Count;

        return tmp;
    }
}
