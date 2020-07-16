using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Utilities;
using Battlefield;

public class CustomBattlePanelHandler : MonoBehaviour
{
    public GameObject gameConditionPanel;
    public GameObject gameCustomPanel;

    BattlefieldCommander attackingCommander, defendingCommander;

    public List<MultiCountInformationPanel> attackersPanel;
    public List<MultiCountInformationPanel> defendersPanel;

    public CustomGameControllerChoice attackerControl, defenderControl;

    public bool increaseByTen = false;
    public void Start()
    {
        ResetCustomBattlePanel();
    }

    public void SwitchAttackerControl(PlayerControlType newControl)
    {
        if(BattlefieldSceneManager.GetInstance != null)
        {
            BattlefieldSceneManager.GetInstance.SwitchAttackerControls(newControl);
        }
    }

    public void SwitchDefenderControl(PlayerControlType newControl)
    {
        if (BattlefieldSceneManager.GetInstance != null)
        {
            BattlefieldSceneManager.GetInstance.SwitchDefenderControls(newControl);
        }
    }

    public void ResetCustomBattlePanel()
    {
        attackingCommander = new BattlefieldCommander();
        attackingCommander.SetupBasicCommander();
        defendingCommander = new BattlefieldCommander();
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
            List<int> tmp = new List<int>();
            tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.maxHealth);
            tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.maxDamage);
            tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.origSpeed);
            tmp.Add(0);

            attackersPanel[i].SetMultiCounter(tmp, attackingCommander.unitsCarried[i].unitInformation.unitName);
            attackersPanel[i].panelIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(attackingCommander.unitsCarried[i].unitInformation.unitName);

        }

        for (int i = 0; i < defendersPanel.Count; i++)
        {
            List<int> tmp = new List<int>();
            tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.maxHealth);
            tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.maxDamage);
            tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.origSpeed);
            tmp.Add(0);

            defendersPanel[i].SetMultiCounter(tmp, defendingCommander.unitsCarried[i].unitInformation.unitName);
            defendersPanel[i].panelIcon.sprite = BattlefieldSpawnManager.GetInstance.unitStorage.GetUnitIcon(defendingCommander.unitsCarried[i].unitInformation.unitName);
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

        attackersPanel[idx].multiCountPanels[attackersPanel[idx].multiCountPanels.Count - 1].text = attackingCommander.unitsCarried[idx].totalUnitCount.ToString();
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

        attackersPanel[idx].multiCountPanels[attackersPanel[idx].multiCountPanels.Count - 1].text = attackingCommander.unitsCarried[idx].totalUnitCount.ToString();
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

        defendersPanel[idx].multiCountPanels[attackersPanel[idx].multiCountPanels.Count - 1].text = defendingCommander.unitsCarried[idx].totalUnitCount.ToString();
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

        defendersPanel[idx].multiCountPanels[attackersPanel[idx].multiCountPanels.Count - 1].text = defendingCommander.unitsCarried[idx].totalUnitCount.ToString();

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
        BattlefieldSpawnManager.GetInstance.SetupAttackingCommander(attackingCommander);
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
