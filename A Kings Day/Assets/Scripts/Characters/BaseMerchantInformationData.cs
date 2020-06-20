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
    }

    [Serializable]
    public class BaseMerchantInformationData
    {
        public List<ItemInformationData> itemsSold;
    }
}