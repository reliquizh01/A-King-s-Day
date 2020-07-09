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


        public Sprite GetUnitIcon(string unitName)
        {
            Sprite icon = unitIconsList[0].unitSprite;

            if(unitIconsList.Find(x => x.unitName == unitName) != null)
            {
                icon = unitIconsList.Find(x => x.unitName == unitName).unitSprite;
            }

            return icon;
        }

    }

}