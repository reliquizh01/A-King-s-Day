using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;
using Maps;

public class MapWeeklyBehavior : MonoBehaviour
{
    public PlayerCampaignData playerCampaignData;
    public PlayerKingdomData playerKingdomData;
    public void Start()
    {

    }

    public void SetupMapBehavior()
    {
        if (PlayerGameManager.GetInstance != null)
        {
            playerCampaignData = PlayerGameManager.GetInstance.campaignData;
            playerKingdomData = PlayerGameManager.GetInstance.playerData;
        }
    }

    public void UpdateWeeklyProgress()
    {
        if(playerCampaignData.mapPointList == null && playerCampaignData.mapPointList.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < playerCampaignData.mapPointList.Count; i++)
        {

        }
    }
}
