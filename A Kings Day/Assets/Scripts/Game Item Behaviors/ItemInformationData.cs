using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameItems
{
    public enum ItemType
    {
        Resources,
        Equipment,
    }

    public class ItemInformationData
    {
        public ItemType ItemType;
        public string itemName;
        public int damage, health, durability, speed;
        public int itemCount;
        public string itemDescription;
    }
}
