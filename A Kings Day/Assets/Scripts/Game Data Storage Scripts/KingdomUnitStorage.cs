using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;

namespace Characters
{
    [System.Serializable]
    public class UnitIconData
    {
        public string unitName;
        public Sprite unitSprite;
    }
    [System.Serializable]
    public class KingdomUnitStorage : MonoBehaviour
    {
        [Header("UNIT ICONS")]
        // MANUALLY ADDED 
        public List<UnitIconData> unitIconsList;
        [Header("UNIT INFORMATIONS")]
        // CHARACTERS
        public List<BaseHeroInformationData> heroStorage;
        public List<BaseMerchantInformationData> merchantStorage;
        // UNIT
        public List<UnitInformationData> basicUnitStorage;

        // SKILLS
        public List<BaseSkillInformationData> skillStorage;
        public List<BaseBuffInformationData> buffStorage;



    }

}