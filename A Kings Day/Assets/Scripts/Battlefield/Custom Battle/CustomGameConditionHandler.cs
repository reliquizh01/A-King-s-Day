using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Battlefield
{
    public class CustomGameConditionHandler : MonoBehaviour
    {
        public CustomBattlePanelHandler myController;
        public BasePanelWindow myWindow;
        public BattlefieldWinCondition currentWinCondition;
        public TextMeshProUGUI winConditionButtonText;
        public TMP_InputField inputField;

        public TextMeshProUGUI gameDescriptionText;

        public void SwtichWinCondition()
        {
            switch (currentWinCondition)
            {
                case BattlefieldWinCondition.ConquerOrEliminateAll:
                    currentWinCondition = BattlefieldWinCondition.EliminateAll;
                    winConditionButtonText.text = "Eliminate all";
                    break;
                case BattlefieldWinCondition.EliminateAll:
                    currentWinCondition = BattlefieldWinCondition.ConquerAll;
                    winConditionButtonText.text = "Conquer all";
                    break;
                case BattlefieldWinCondition.ConquerAll:
                    currentWinCondition = BattlefieldWinCondition.ConquerOrEliminateAll;
                    winConditionButtonText.text = "Conquer or Eliminate all";
                    break;
                default:
                    break;
            }


            if(BattlefieldSystemsManager.GetInstance != null)
            {
                BattlefieldSystemsManager.GetInstance.winCondition = currentWinCondition;
            }

            UpdateDescriptionText();
        }

        public void UpdateDescriptionText()
        {
            switch (currentWinCondition)
            {
                case BattlefieldWinCondition.ConquerOrEliminateAll:
                    gameDescriptionText.text = "Conquer most Tiles or Eliminate all enemies within <color=green>"+BattlefieldSystemsManager.GetInstance.maxDays+" days</color>";
                    break;
                case BattlefieldWinCondition.EliminateAll:
                    gameDescriptionText.text = "Conquer ALL TILES or Eliminate all enemies within <color=green>" + BattlefieldSystemsManager.GetInstance.maxDays + " days</color>";
                    break;
                case BattlefieldWinCondition.ConquerAll:
                     gameDescriptionText.text = "Conquer most Tiles enemies within <color=green>" + BattlefieldSystemsManager.GetInstance.maxDays + " days</color>";
                    break;

                default:
                    break;
            }
        }
        public void AdjustDays()
        {
            if(BattlefieldSystemsManager.GetInstance != null)
            {
                BattlefieldSystemsManager.GetInstance.maxDays = Convert.ToInt32(inputField.text);
            }
            UpdateDescriptionText();
        }

        public void StartBattle()
        {
            if(PanelWindowManager.GetInstance != null)
            {
                myWindow.CloseWindow();
            }
            myController.StartBattle();
        }
    }
}
