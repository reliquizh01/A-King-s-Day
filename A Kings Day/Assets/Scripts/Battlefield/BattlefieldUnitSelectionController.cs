using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlefield
{
    public class BattlefieldUnitSelectionController : MonoBehaviour
    {
        public List<UnitSelectPanel> unitList;
        public UnitSelectPanel selectedUnit;
        public int currentSelectedIdx;

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetUnitPanelAsSelected(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetUnitPanelAsSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetUnitPanelAsSelected(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetUnitPanelAsSelected(3);
            }
        }

        public void SetUnitPanelAsSelected(int idx)
        {
            selectedUnit = unitList[idx];
            currentSelectedIdx = idx;

            for (int i = 0; i < unitList.Count; i++)
            {
                if(idx != i)
                {
                    unitList[i].UnSelect();
                }
                else
                {
                    unitList[idx].SetAsSelected();
                }
            }
        }
    }
}