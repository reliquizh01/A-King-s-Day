using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using Battlefield;
using Characters;
using System;

public class CustomBattlePanelHandler : MonoBehaviour
{
    public GameObject gameConditionPanel;
    public GameObject gameCustomPanel;

    public BattlefieldCommander attackingCommander, defendingCommander;

    [Header("Attackers")]
    public int curAtkIdx;
    public int attackerControlIdx;
    public CustomHeroPanel attackingLeader;
    public List<CustomUnitPanel> attackersPanel;
    [Header("Defenders")]
    public int curDefIdx;
    public int defenderControlIdx;
    public CustomHeroPanel defendingLeader;
    public List<CustomUnitPanel> defendersPanel;


    public CustomGameControllerChoice attackerControl, defenderControl;

    public bool increaseByTen = false;
    public void Start()
    {
        ResetCustomBattlePanel();
    }

    public void SwitchAttackerControl(bool next)
    {
        if (next)
        {
            if (attackerControlIdx < Enum.GetNames(typeof(PlayerControlType)).Length - 1)
            {
                attackerControlIdx += 1;
            }
            else
            {
                attackerControlIdx = 0;
            }
        }
        else
        {
            if (attackerControlIdx <= 0)
            {
                attackerControlIdx = Enum.GetNames(typeof(PlayerControlType)).Length - 1;
            }
            else
            {
                attackerControlIdx -= 1;
            }
        }

        if (BattlefieldSceneManager.GetInstance != null)
        {
            BattlefieldSceneManager.GetInstance.SwitchAttackerControls((PlayerControlType)attackerControlIdx);
        }
        attackerControl.SwitchToThisControl((PlayerControlType)attackerControlIdx);
    }

    public void SwitchDefenderControl(bool next)
    {
        if(next)
        {
            if (defenderControlIdx < Enum.GetNames(typeof(PlayerControlType)).Length-1)
            {
                defenderControlIdx += 1;
            }
            else
            {
                defenderControlIdx = 0;
            }
        }
        else
        {
            if (defenderControlIdx <= 0)
            {
                defenderControlIdx = Enum.GetNames(typeof(PlayerControlType)).Length - 1;
            }
            else
            {
                defenderControlIdx -= 1;
            }
        }
        if (BattlefieldSceneManager.GetInstance != null)
        {
            BattlefieldSceneManager.GetInstance.SwitchDefenderControls((PlayerControlType)defenderControlIdx);
        }

        defenderControl.SwitchToThisControl((PlayerControlType)defenderControlIdx);
    }

    public void NextHeroInformation(bool isAttacker)
    {
        
        if(isAttacker)
        {
            if(curAtkIdx >= TransitionManager.GetInstance.unitStorage.heroStorage.Count-1)
            {
                curAtkIdx = 0;
            }
            else
            {
                curAtkIdx += 1;
            }
            BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curAtkIdx];
            // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
            newHero.unitInformation.unitName = "Player";
            newHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";


            attackingLeader.SetupHero(newHero);
        }
        else
        {
            if (curDefIdx >= TransitionManager.GetInstance.unitStorage.heroStorage.Count - 1)
            {
                curDefIdx = 0;
            }
            else
            {
                curDefIdx += 1;
            }
            BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curDefIdx];
            // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
            newHero.unitInformation.unitName = "Player";
            newHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";


            defendingLeader.SetupHero(newHero);

        }
    }

    public void PreviousHeroInformation(bool isAttacker)
    {

        if (isAttacker)
        {
            if (curAtkIdx <= 0)
            {
                curAtkIdx = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage.Count - 1;
            }
            else
            {
                curAtkIdx -= 1;
            }

            BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curAtkIdx];
            attackingLeader.SetupHero(newHero);
            // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
            newHero.unitInformation.unitName = "Player";
            newHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";
        }
        else
        {
            if (curDefIdx <= 0)
            {
                curDefIdx = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage.Count - 1;
            }
            else
            {
                curDefIdx -= 1;
            }
            BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curDefIdx];
            defendingLeader.SetupHero(newHero);
            // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
            newHero.unitInformation.unitName = "Player";
            newHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";

        }
    }
    public void ResetCustomBattlePanel()
    {

        attackingCommander = new BattlefieldCommander();
        curAtkIdx = 0;
        attackerControlIdx = 2;
        attackerControl.SwitchToThisControl((PlayerControlType)attackerControlIdx);

        BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curAtkIdx];
        // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
        newHero.unitInformation.unitName = "Player";
        newHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";

        attackingLeader.SetupHero(newHero);
        attackingCommander.SetupBasicCommander();


        defendingCommander = new BattlefieldCommander();
        curAtkIdx = 0;
        defenderControlIdx = 0;
        defenderControl.SwitchToThisControl((PlayerControlType)defenderControlIdx);

        BaseHeroInformationData startHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curDefIdx];
        // TEMPORARY UNTIL PREFABS FOR UNIQUE HEROES ARE CREATED
        startHero.unitInformation.unitName = "Player";
        startHero.unitInformation.prefabDataPath = "Assets/Resources/Prefabs/Unit and Items/Player.prefab";

        defendingLeader.SetupHero(startHero);
        defendingCommander.SetupBasicCommander();


        gameConditionPanel.SetActive(false);

        SetupCustomBattlePanel();
    }
    public void Update()
    {
        if (UtilitiesCommandObserver.GetInstance == null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                increaseByTen = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                increaseByTen = false;
            }
        }
        else
        {
            if (Input.GetKey(UtilitiesCommandObserver.GetInstance.GetKey(UtilitiesControlActionNames.INCREASE_COUNT_INCREMENT)))
            {
                increaseByTen = true;
            }
            else if (Input.GetKeyUp(UtilitiesCommandObserver.GetInstance.GetKey(UtilitiesControlActionNames.INCREASE_COUNT_INCREMENT)))
            {
                increaseByTen = false;
            }
        }

    }
    public void SetupCustomBattlePanel()
    {
        for (int i = 0; i < attackersPanel.Count; i++)
        {
            List<float> tmp = new List<float>();
            tmp.Add((float)attackingCommander.unitsCarried[i].unitInformation.maxHealth);
            tmp.Add((float)attackingCommander.unitsCarried[i].unitInformation.maxDamage);
            tmp.Add((float)attackingCommander.unitsCarried[i].unitInformation.origSpeed);
            tmp.Add((float)attackingCommander.unitsCarried[i].unitInformation.range);
            tmp.Add((float)attackingCommander.unitsCarried[i].totalUnitCount);

            attackersPanel[i].multiCounter.SetMultiCounter(tmp, attackingCommander.unitsCarried[i].unitInformation.unitName);
            attackersPanel[i].multiCounter.panelIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(attackingCommander.unitsCarried[i].unitInformation.unitName);

        }

        for (int i = 0; i < defendersPanel.Count; i++)
        {
            List<float> tmp = new List<float>();
            tmp.Add((float)defendingCommander.unitsCarried[i].unitInformation.maxHealth);
            tmp.Add((float)defendingCommander.unitsCarried[i].unitInformation.maxDamage);
            tmp.Add((float)defendingCommander.unitsCarried[i].unitInformation.origSpeed);
            tmp.Add((float)defendingCommander.unitsCarried[i].unitInformation.range);
            tmp.Add((float)defendingCommander.unitsCarried[i].totalUnitCount);
            Debug.Log("Setting it up for defender: " + i + " Count: " + defendingCommander.unitsCarried[i].totalUnitCount);
            defendersPanel[i].multiCounter.SetMultiCounter(tmp, defendingCommander.unitsCarried[i].unitInformation.unitName);
            defendersPanel[i].multiCounter.panelIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(defendingCommander.unitsCarried[i].unitInformation.unitName);
        }

    }

    public void IncreaseUnitIndexAttack(int idx)
    {
        int increase = 1;
        if (increaseByTen)
        {
            increase = 10;
        }

        attackingCommander.unitsCarried[idx].totalUnitCount += increase;
        attackingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment += increase;

        attackersPanel[idx].multiCounter.multiCountPanels[attackersPanel[idx].multiCounter.multiCountPanels.Count - 1].text = attackingCommander.unitsCarried[idx].totalUnitCount.ToString();
    }                      
    public void DecreaseUnitIndexAttack(int idx)
    {
        if (attackingCommander.unitsCarried[idx].totalUnitCount == 0)
        {
            return;
        }

        int decrease = 1;
        if (increaseByTen)
        {
            decrease = 10;

            if ((attackingCommander.unitsCarried[idx].totalUnitCount - 10) < 0)
            {
                decrease = attackingCommander.unitsCarried[idx].totalUnitCount;
            }
        }

        attackingCommander.unitsCarried[idx].totalUnitCount -= decrease;
        attackingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment -= decrease;

        attackersPanel[idx].multiCounter.multiCountPanels[attackersPanel[idx].multiCounter.multiCountPanels.Count - 1].text = attackingCommander.unitsCarried[idx].totalUnitCount.ToString();
    }

    public void IncreaseUnitIndexDefenders(int idx)
    {
        int increase = 1;
        if (increaseByTen)
        {
            increase = 10;
        }

        defendingCommander.unitsCarried[idx].totalUnitCount += increase;
        defendingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment += increase;

        defendersPanel[idx].multiCounter.multiCountPanels[attackersPanel[idx].multiCounter.multiCountPanels.Count - 1].text = defendingCommander.unitsCarried[idx].totalUnitCount.ToString();
    }

    public void DecreaseUnitIndexDefenders(int idx)
    {
        if (defendingCommander.unitsCarried[idx].totalUnitCount == 0)
        {
            return;
        }

        int decrease = 1;
        if (increaseByTen)
        {
            decrease = 10;

            if ((defendingCommander.unitsCarried[idx].totalUnitCount - 10) < 0)
            {
                decrease = defendingCommander.unitsCarried[idx].totalUnitCount;
            }
        }

        defendingCommander.unitsCarried[idx].totalUnitCount -= decrease;
        defendingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment -= decrease;

        defendersPanel[idx].multiCounter.multiCountPanels[attackersPanel[idx].multiCounter.multiCountPanels.Count - 1].text = defendingCommander.unitsCarried[idx].totalUnitCount.ToString();

    }

    public void ShowBattleConditionPanel()
    {
        bool oneSideIsEmpty = false;

        if(attackingCommander.CheckTotalTroopsCount() <= 0)
        {
            oneSideIsEmpty = true;
        }
        else if(defendingCommander.CheckTotalTroopsCount() <= 0)
        {
            oneSideIsEmpty = true;
        }

        if(oneSideIsEmpty)
        {
            return;
        }

        gameConditionPanel.SetActive(true);
    }
    public void StartBattle()
    {
        gameConditionPanel.SetActive(false);
        BaseHeroInformationData thisHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curAtkIdx];
        attackingCommander.heroesCarried.Add(thisHero);
        BattlefieldSpawnManager.GetInstance.SetupAttackingCommander(attackingCommander);

        BaseHeroInformationData newHero = BattlefieldSpawnManager.GetInstance.unitStorage.heroStorage[curDefIdx];
        defendingCommander.heroesCarried.Add(newHero);
        BattlefieldSpawnManager.GetInstance.SetupDefendingCommander(defendingCommander);
        BattlefieldSceneManager.GetInstance.PreBattleStart();
    }

    public void ReturnToMenu()
    {
        if (TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.currentSceneManager.PlayThisBackGroundMusic(BackgroundMusicType.openingTheme);
            TransitionManager.GetInstance.LoadScene(SceneType.Opening);

        }
    }

    public void ShowControls()
    {
        if (TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.ShowOptions(true);
        }
    }
}
