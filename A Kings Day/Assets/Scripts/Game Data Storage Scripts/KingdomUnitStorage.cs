using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;

namespace Characters
{
    [System.Serializable]
    public class KingdomUnitStorage : MonoBehaviour
    {
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