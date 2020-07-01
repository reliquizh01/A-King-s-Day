using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameItems
{
    public enum ItemType
    {
        Resources,
        Equipment,
    }

    [Serializable]
    public class ItemInformationData
    {
        public ItemType ItemType;
        public string itemName;
        public int damage, health, durability, speed;
        public int itemCount;
        public string itemDescription;
        public int itemPrice;
    }
}
