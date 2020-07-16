using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlefield;

public class CustomGameControllerChoice : MonoBehaviour
{
    public CustomBattlePanelHandler myController;
    public Image keyboardIcon, numpadIcon, aiIcon;
    public PlayerControlType currentControlType;
    public bool isAttacker;

    public void SwitchToAIController(bool userInteraction)
    {
        aiIcon.gameObject.SetActive(true);
        numpadIcon.gameObject.SetActive(false);
        keyboardIcon.gameObject.SetActive(false);
        currentControlType = PlayerControlType.Computer;

        if(isAttacker)
        {
            myController.SwitchAttackerControl(currentControlType);
        }
        else
        {
            myController.SwitchDefenderControl(currentControlType);
        }
    }

    public void SwitchToNumpadController(bool userInteraction)
    {

        aiIcon.gameObject.SetActive(false);
        numpadIcon.gameObject.SetActive(true);
        keyboardIcon.gameObject.SetActive(false);
        currentControlType = PlayerControlType.PlayerTwo;

        if (currentControlType == PlayerControlType.Computer)
            return;


        if (isAttacker)
        {
            if (userInteraction)
            {
                myController.defenderControl.SwitchToKeyboardController(false);
            }
            myController.SwitchAttackerControl(currentControlType);
        }
        else
        {
            if (userInteraction)
            {
                myController.attackerControl.SwitchToKeyboardController(false);
            }
            myController.SwitchDefenderControl(currentControlType);
        }
    }

    public void SwitchToKeyboardController(bool userInteraction)
    {

        aiIcon.gameObject.SetActive(false);
        numpadIcon.gameObject.SetActive(false);
        keyboardIcon.gameObject.SetActive(true);
        currentControlType = PlayerControlType.PlayerOne;


        if (currentControlType == PlayerControlType.Computer)
            return;

        if (isAttacker)
        {
            if (userInteraction)
            {
                myController.defenderControl.SwitchToNumpadController(false);
            }
            myController.SwitchAttackerControl(currentControlType);
        }
        else
        {
            if (userInteraction)
            {
                myController.attackerControl.SwitchToNumpadController(false);
            }
            myController.SwitchDefenderControl(currentControlType);
        }
    }
}
