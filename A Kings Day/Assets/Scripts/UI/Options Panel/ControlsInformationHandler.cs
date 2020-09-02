using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Battlefield;

public class ControlsInformationHandler : MonoBehaviour
{
    public GameObject playerOne, playerTwo;
    public Button playerOneBtn, playerTwoBtn;
    public PlayerControlType currentControlShown;
    public bool isShowingControls;


    public GameObject initialDetail;
    public GameObject unitSelection, unitSummon, tileSwap, skillSelect, leaderSpawn;

    public void Start()
    {
        ShowPlayerOne();
    }
    public void Update()
    {

    }

    public void ShowThisVisuals(int idx, PlayerControlType thisControl)
    {
        if(currentControlShown != thisControl)
        {
            return;
        }

        initialDetail.gameObject.SetActive(false);
        if (idx <= 3)
        {
            tileSwap.SetActive(true);
            unitSummon.SetActive(false);
            unitSelection.SetActive(false);
            skillSelect.SetActive(false);
            leaderSpawn.SetActive(false);
        }
        else if (idx == 4)
        {
            tileSwap.SetActive(false);
            unitSummon.SetActive(true);
            unitSelection.SetActive(false);
            skillSelect.SetActive(false);
            leaderSpawn.SetActive(false);
        }
        else if(idx > 4 && idx <= 8)
        {
            tileSwap.SetActive(false);
            unitSummon.SetActive(false);
            unitSelection.SetActive(true);
            skillSelect.SetActive(false);
            leaderSpawn.SetActive(false);
        }
        else if(idx == 9)
        {
            tileSwap.SetActive(false);
            unitSummon.SetActive(false);
            unitSelection.SetActive(false);
            skillSelect.SetActive(false);
            leaderSpawn.SetActive(true);
        }
        else if(idx >= 10 && idx <= 12)
        {
            tileSwap.SetActive(false);
            unitSummon.SetActive(false);
            unitSelection.SetActive(false);
            skillSelect.SetActive(true);
            leaderSpawn.SetActive(false);

        }
    }

    public void ShowControls()
    {
        if(!isShowingControls)
        {
            isShowingControls = true;
            ShowPlayerOne();
        }
    }

    public void ResetVisuals()
    {
        ShowPlayerOne();
        tileSwap.gameObject.SetActive(false);
        unitSummon.gameObject.SetActive(false);
        unitSelection.gameObject.SetActive(false);
        initialDetail.gameObject.SetActive(true);
    }
    public void ShowPlayerOne()
    {
        playerOneBtn.image.color = playerOneBtn.colors.pressedColor;
        playerTwoBtn.image.color = playerTwoBtn.colors.normalColor;
        currentControlShown = PlayerControlType.PlayerOne;
        playerOne.SetActive(true);
        playerTwo.SetActive(false);
    }

    public void ShowPlayerTwo()
    {
        playerOneBtn.image.color = playerOneBtn.colors.normalColor;
        playerTwoBtn.image.color = playerTwoBtn.colors.pressedColor;

        currentControlShown = PlayerControlType.PlayerTwo;
        playerOne.SetActive(false);
        playerTwo.SetActive(true);
    }
}
