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

        public UnitInformationData GetUnitInformation(string genericName)
        {
            return basicUnitStorage.Find(x => x.unitName == genericName);
        }

        public List<TroopsInformation> GenerateBasicWarband(int unitCount)
        {
            List<int> troopTypes = new List<int>();
            troopTypes.Add(0);
            troopTypes.Add(0);
            troopTypes.Add(0);
            troopTypes.Add(0);

            for (int i = 0; i < unitCount; i++)
            {
                int rand = UnityEngine.Random.Range(0, troopTypes.Count);
                troopTypes[rand] += 1;
            }
            Debug.Log("[" + troopTypes[0] + "]" + "[" + troopTypes[1] + "]" + "[" + troopTypes[2] + "]" + "[" + troopTypes[3] + "]");

            TroopsInformation recruit = TroopsInformation.ConvertToTroopsInformation(GetUnitInformation("Recruit"), troopTypes[0]);
            TroopsInformation swordsman = TroopsInformation.ConvertToTroopsInformation(GetUnitInformation("Swordsman"), troopTypes[1]);
            TroopsInformation spearman = TroopsInformation.ConvertToTroopsInformation(GetUnitInformation("Spearman"), troopTypes[2]);
            TroopsInformation archer = TroopsInformation.ConvertToTroopsInformation(GetUnitInformation("Archer"), troopTypes[3]);

            List<TroopsInformation> tmp = new List<TroopsInformation>();
            tmp.Add(recruit);
            tmp.Add(swordsman);
            tmp.Add(spearman);
            tmp.Add(archer);

            return tmp;
        }

    }

}