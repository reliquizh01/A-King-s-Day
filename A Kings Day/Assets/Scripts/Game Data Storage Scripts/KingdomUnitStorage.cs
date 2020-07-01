using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;

namespace Characters
{
    public class KingdomUnitStorage : MonoBehaviour
    {
        public List<BaseHeroInformationData> heroStorage;
        public List<BaseMerchantInformationData> merchantStorage;

        // SKILLS
        public List<BaseSkillInformationData> skillStorage;
        public List<BaseBuffInformationData> buffStorage;

    }

}