using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Kingdoms;
using KingEvents;
using TMPro;

public class BaseNotification : MonoBehaviour
{
    public bool isShowing = false;
    public BasePanelBehavior myPanel;
    public CountingEffectUI text;
    public GameObject foodGo, troopGo, popGo, coinGo, cowGo;


    public void SetAsReduce()
    {
        text.numberColorText = "<color=red>";
    }

    public void SetAsAdd()
    {
        text.numberColorText = "<color=green>";
    }


    public void SetIconTo(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                foodGo.SetActive(true);
                troopGo.SetActive(false);
                popGo.SetActive(false);
                coinGo.SetActive(false);
                cowGo.SetActive(false);
                break;
            case ResourceType.Troops:
                foodGo.SetActive(false);
                troopGo.SetActive(true);
                popGo.SetActive(false);
                coinGo.SetActive(false);
                cowGo.SetActive(false);
                break;
            case ResourceType.Population:
                foodGo.SetActive(false);
                troopGo.SetActive(false);
                popGo.SetActive(true);
                coinGo.SetActive(false);
                cowGo.SetActive(false);
                break;
            case ResourceType.Coin:
                foodGo.SetActive(false);
                troopGo.SetActive(false);
                popGo.SetActive(false);
                coinGo.SetActive(true);
                cowGo.SetActive(false);
                break;
            case ResourceType.Cows:
                foodGo.SetActive(false);
                troopGo.SetActive(false);
                popGo.SetActive(false);
                coinGo.SetActive(false);
                cowGo.SetActive(true);
                break;
        }

    }


    public void PreCloseAnim()
    {
        StartCoroutine(DelayRemoval());
    }
    public IEnumerator DelayRemoval()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName,()=> SetShowing(false)));
    }
    public void SetShowing(bool newStatus)
    {
        isShowing = newStatus;
    }
}
