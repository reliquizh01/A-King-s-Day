using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Maps;
using KingEvents;
using Battlefield;
public class CampaignRewardsPanel : MonoBehaviour
{
    public BasePanelBehavior myPanel;
    [Header("Panel Data")]
    public BattlefieldCommander playerCommander;
    public List<ResourceReward> playerRewards;
    public TerritoryOwners enemyOwner;

    [Header("Territory Rewards")]
    public BasePanelBehavior territoryObject;
    public TextMeshProUGUI territoryName;
    public Image conquerorsIcon;
    public List<Sprite> conquerorSprites;

    [Header("Coin Rewards")]
    public BasePanelBehavior coinRewardObject;
    public CountingEffectUI taxEarning;
    public CountingEffectUI salvagedEarning;
    public CountingEffectUI woundedEarning;
    public CountingEffectUI totalEarning;

    [Header("Death Report")]
    public BasePanelBehavior casualtyRewardObject;
    public GameObject sentencePrefab;
    public Transform sentenceParent;
    public List<TypeWriterEffectUI> sentencesCreated;

    [Header("Confirm Button")]
    public GameObject confirmButton;

    public void OnEnable()
    {
        StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, CheckTerritory));
    }

    public void CheckTerritory()
    {
        if(playerRewards.Find(x => x.resourceTitle == "Tax Prize") != null)
        {
            territoryObject.gameObject.SetActive(true);
            StartCoroutine(territoryObject.WaitAnimationForAction(territoryObject.openAnimationName, CheckCoinReward));
        }
        else
        {
            CheckCoinReward();
        }
    }

    public void CheckCoinReward()
    {
        coinRewardObject.gameObject.SetActive(true);
        StartCoroutine(coinRewardObject.WaitAnimationForAction(coinRewardObject.openAnimationName, CheckCasualties));

        taxEarning.SetTargetCount(playerRewards.Find(x => x.resourceTitle == "Tax Prize").rewardAmount);
        salvagedEarning.SetTargetCount(playerRewards.Find(x => x.resourceTitle == "Salvaged Prize").rewardAmount);
        woundedEarning.SetTargetCount(playerRewards.Find(x => x.resourceTitle == "Dead Penalty").rewardAmount);

        int totalEarned = 0;
        for (int i = 0; i < playerRewards.Count; i++)
        {
            totalEarned += playerRewards[i].rewardAmount;
        }
        totalEarning.SetTargetCount(totalEarned);

    }

    public void CheckCasualties()
    {
        casualtyRewardObject.gameObject.SetActive(true);
        StartCoroutine(casualtyRewardObject.WaitAnimationForAction(casualtyRewardObject.openAnimationName, EnableButton));

        if (sentencesCreated == null)
        {
            sentencesCreated = new List<TypeWriterEffectUI>();
        }
        for (int i = 0; i < playerCommander.unitsCarried.Count; i++)
        {
            string text = playerCommander.unitsCarried[i].totalDeathCount + " " + playerCommander.unitsCarried[i].unitInformation.unitName + " died";

            GameObject tmp = (GameObject)Instantiate(sentencePrefab);
            tmp.transform.SetParent(sentenceParent);

            TypeWriterEffectUI typeWriter = tmp.GetComponent<TypeWriterEffectUI>();
            typeWriter.SetTypeWriterMessage(text, true);
            sentencesCreated.Add(typeWriter);

        }
    }

    public void EnableButton()
    {
        confirmButton.SetActive(true);
    }
}
