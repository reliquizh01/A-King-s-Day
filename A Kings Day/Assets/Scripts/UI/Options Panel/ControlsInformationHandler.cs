using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;

public class ControlsInformationHandler : MonoBehaviour
{
    public GameObject playerOne, playerTwo;
    public PlayerControlType currentControlShown;
    public bool isShowingControls;


    public GameObject initialDetail;
    public GameObject unitSelection, unitSummon, tileSwap;

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
            tileSwap.gameObject.SetActive(true);
            unitSummon.gameObject.SetActive(false);
            unitSelection.gameObject.SetActive(false);
        }
        else if (idx == 4)
        {
            tileSwap.gameObject.SetActive(false);
            unitSummon.gameObject.SetActive(true);
            unitSelection.gameObject.SetActive(false);
        }
        else if(idx > 4 && idx <= 8)
        {
            tileSwap.gameObject.SetActive(false);
            unitSummon.gameObject.SetActive(false);
            unitSelection.gameObject.SetActive(true);
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
        currentControlShown = PlayerControlType.PlayerOne;
        playerOne.SetActive(true);
        playerTwo.SetActive(false);
    }

    public void ShowPlayerTwo()
    {
        currentControlShown = PlayerControlType.PlayerTwo;
        playerOne.SetActive(false);
        playerTwo.SetActive(true);
    }
}
