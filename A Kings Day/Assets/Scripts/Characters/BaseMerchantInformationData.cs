using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameItems;

namespace Characters
{
    public enum MerchantType
    {
        Goods,
        Equipments,
        Exotic,
    }

    [Serializable]
    public class BaseMerchantInformationData
    {
        public string merchantName;
        public MerchantType merchantType;
        public bool isRandomGenerated;
        public UnitInformationData unitInformation;
        public List<ItemInformationData> itemsSold;
    }
}