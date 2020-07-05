using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class CustomBattlePanelHandler : MonoBehaviour
{
    BattlefieldCommander attackingCommander, defendingCommander;

    public List<MultiCountInformationPanel> attackersPanel;
    public List<MultiCountInformationPanel> defendersPanel;

    public bool increaseByTen = false;
    public void Start()
    {
        attackingCommander = new BattlefieldCommander();
        attackingCommander.SetupBasicCommander();
        defendingCommander = new BattlefieldCommander();
        defendingCommander.SetupBasicCommander();

        SetupCustomBattlePanel();
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            increaseByTen = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            increaseByTen = false;
        }

    }
    public void SetupCustomBattlePanel()
    {
        for (int i = 0; i < attackersPanel.Count; i++)
        {
            List<int> tmp = new List<int>();
            if(i != (attackersPanel.Count-1))
            {
                tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.maxHealth);
                tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.maxDamage);
                tmp.Add((int)attackingCommander.unitsCarried[i].unitInformation.origSpeed);
                tmp.Add(0);

                attackersPanel[i].SetMultiCounter(tmp, attackingCommander.unitsCarried[i].unitInformation.unitName);
            }
            else
            {
                tmp.Add((int)attackingCommander.heroesCarried[0].unitInformation.maxHealth);
                tmp.Add((int)attackingCommander.heroesCarried[0].unitInformation.maxDamage);
                tmp.Add((int)attackingCommander.heroesCarried[0].unitInformation.origSpeed);

                attackersPanel[i].SetMultiCounter(tmp, attackingCommander.heroesCarried[0].unitInformation.unitName);
            } 
        }

        for (int i = 0; i < defendersPanel.Count; i++)
        {
            List<int> tmp = new List<int>();
            if (i != (attackersPanel.Count - 1))
            {
                tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.maxHealth);
                tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.maxDamage);
                tmp.Add((int)defendingCommander.unitsCarried[i].unitInformation.origSpeed);
                tmp.Add(0);

                defendersPanel[i].SetMultiCounter(tmp, defendingCommander.unitsCarried[i].unitInformation.unitName);
            }
            else
            {
                tmp.Add((int)defendingCommander.heroesCarried[0].unitInformation.maxHealth);
                tmp.Add((int)defendingCommander.heroesCarried[0].unitInformation.maxDamage);
                tmp.Add((int)defendingCommander.heroesCarried[0].unitInformation.origSpeed);

                defendersPanel[i].SetMultiCounter(tmp, defendingCommander.heroesCarried[0].unitInformation.unitName);
            }
        }

    }

    public void IncreaseUnitIndexAttack(int idx)
    {
        int increase = 1;
        if(increaseByTen)
        {
            increase = 10;
        }

        attackingCommander.unitsCarried[idx].totalUnitCount += increase;
        attackingCommander.unitsCarried[idx].totalUnitsAvailableForDeployment += increase;

        attackersPanel[idx].multiCountPanels[attackersPanel[idx].multiCountPanels.Count - 1].text = attackingCommander.unitsCarried[idx].totalUnitCount.ToString();
    }
    public void DecreaseUnitIndexAttack(int idx)
    {
        if(attackingCommander.unitsCarried[idx].totalUnitCount == 0)
        {
            return;
        }

        int decrease = 1;
        if (increaseByTen)
        {
            decrease = 10;

            if((attackingCommander.unitsCarried[idx].totalUnitCount - 10) < 0)
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


    public void StartBattle()
    {
        BattlefieldSpawnManager.GetInstance.SetupAttackingCommander(attackingCommander);
        BattlefieldSpawnManager.GetInstance.SetupDefendingCommander(defendingCommander);
        BattlefieldSceneManager.GetInstance.PreBattleStart();

    }
}
