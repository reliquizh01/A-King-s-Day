﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Kingdoms;
using Utilities;
using Managers;
using UnityEngine.EventSystems;
using ResourceUI;
using KingEvents;

namespace Buildings
{
    public enum BuildingCondition
    {
        Ruins,
        Functioning
    }
    [Serializable]
    public class BuildingInformationData
    {
        public string BuildingName;
        public BuildingType buildingType;

        public List<BuildingCardData> buildingCard;
        
        [Header("Flavor Messages")]
        public List<string> introductionMessages;

        public List<string> ObtainCardPositiveFlavorText(int idx)
        {
            return buildingCard[idx].cardPosMesg;
        }
        public List<string> ObtainCardNegativeFlavorText(int idx)
        {
            return buildingCard[idx].cardNegMesg;
        }
        public List<string> ObtainActionPositiveFlavorText(int cardIdx, int actionIdx)
        {
            return buildingCard[cardIdx].actionTypes[actionIdx].AcceptMesg;
        }
        public List<string> ObtainActionNegativeFlavorText(int cardIdx, int actionIdx)
        {
            return buildingCard[cardIdx].actionTypes[actionIdx].DenyMesg;
        }
    }

    [Serializable]
    public class BuildingCardData
    {
        public string cardName;
        public Sprite cardIcon;
        public List<CardActiondata> actionTypes;

        [Header("Flavor Mesg")]
        public List<string> cardPosMesg;
        public List<string> cardNegMesg;
    }
    [Serializable]
    public class CardActiondata
    {
        public CardActionType actionType;
        public bool costResource;
        public Sprite logoIcon;
        public string message;

        [Header("Action Information")]
        public bool hasCooldown;
        public int cooldownCount;

        [Header("Reward List")]
        public List<ResourceReward> rewardList;

        [Header("Flavor Messages")]
        public List<string> AcceptMesg;
        public List<string> DenyMesg;
    }
}