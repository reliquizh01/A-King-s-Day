using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using GameItems;
using Kingdoms;

namespace Characters
{
    [System.Serializable]
    public class UnitIconData
    {
        public string unitName;
        public Sprite unitSprite;
    }

    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public Sprite skillSprite;
    }

    [System.Serializable]
    public class KingdomUnitStorage : MonoBehaviour
    {
        [Header("UNIT ICONS")]
        // MANUALLY ADDED 
        public List<UnitIconData> unitIconsList;

        [Header("Skill Icons")]
        public List<SkillData> skillIconList;

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

        public Sprite GetSkillIcon(string skillName)
        {
            Sprite icon = skillIconList[0].skillSprite;

            if (skillIconList.Find(x => x.skillName == skillName) != null)
            {
                icon = skillIconList.Find(x => x.skillName == skillName).skillSprite;
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

        public BaseHeroInformationData ObtainHeroInformation(string HeroName)
        {
            return heroStorage.Find(x => x.unitInformation.unitName == HeroName);
        }

        public BaseHeroInformationData ObtainHeroBaseInformation(WieldedWeapon weaponType)
        {
            return heroStorage.Find(x => x.isHeroBaseState && x.unitInformation.wieldedWeapon == weaponType);
        }
    }

}