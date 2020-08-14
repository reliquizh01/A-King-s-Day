using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadTutorialController : MonoBehaviour
{
    public GameObject foodTut, coinTut, popTut, troopTut, WeekTut;

    public void ShowFoodTutorial()
    {
        foodTut.SetActive(true);
        coinTut.SetActive(false);
        popTut.SetActive(false);
        troopTut.SetActive(false);
        WeekTut.SetActive(false);
    }

    public void ShowCoinTutorial()
    {
        coinTut.SetActive(true);
        foodTut.SetActive(false);
        popTut.SetActive(false);
        troopTut.SetActive(false);
        WeekTut.SetActive(false);
    }

    public void ShowPopulationTutorial()
    {
        popTut.SetActive(true);
        foodTut.SetActive(false);
        coinTut.SetActive(false);
        troopTut.SetActive(false);
        WeekTut.SetActive(false);
    }

    public void ShowTroopTutorial()
    {
        troopTut.SetActive(true);
        foodTut.SetActive(false);
        coinTut.SetActive(false);
        popTut.SetActive(false);
        WeekTut.SetActive(false);
    }

    public void HideAllTutorial()
    {
        WeekTut.SetActive(false);
        foodTut.SetActive(false);
        coinTut.SetActive(false);
        popTut.SetActive(false);
        troopTut.SetActive(false);
    }

    public void ShowWeekTutorial()
    {
        WeekTut.SetActive(true);
        foodTut.SetActive(false);
        coinTut.SetActive(false);
        popTut.SetActive(false);
        troopTut.SetActive(false);
    }

}
