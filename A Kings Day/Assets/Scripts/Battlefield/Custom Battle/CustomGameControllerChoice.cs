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

    public void SwitchToThisControl(PlayerControlType playerControlType)
    {
        switch (playerControlType)
        {
            case PlayerControlType.PlayerOne:
                keyboardIcon.gameObject.SetActive(true);
                numpadIcon.gameObject.SetActive(false);
                aiIcon.gameObject.SetActive(false);
                break;
            case PlayerControlType.PlayerTwo:
                keyboardIcon.gameObject.SetActive(false);
                numpadIcon.gameObject.SetActive(true);
                aiIcon.gameObject.SetActive(false);
                break;
            case PlayerControlType.Computer:
                keyboardIcon.gameObject.SetActive(false);
                numpadIcon.gameObject.SetActive(false);
                aiIcon.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
