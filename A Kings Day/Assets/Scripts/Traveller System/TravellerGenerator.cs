using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;
using Characters;
using Managers;

namespace Kingdoms
{
    public class TravellerGenerator : MonoBehaviour
    {
        public TravellingSystem myParent;

        public BaseTravellerData GenerateRandomMerchantTraveller(int unitCount, float newRelationship)
        {
            BaseTravellerData tmp = new BaseTravellerData();

            // INITIALIZATION OF DATA
            tmp.weekSpawned = ObtainPlayerWeeklyCount();
            tmp.troopsCarried = new List<TroopsInformation>();

            // LEADER INFORMATION
            int randLdrIdx = UnityEngine.Random.Range(0, myParent.unitStorage.merchantStorage.Count);
            tmp.leaderUnit = new BaseHeroInformationData();
            tmp.leaderUnit.unitInformation = new UnitInformationData();
            tmp.leaderUnit.unitInformation = myParent.unitStorage.merchantStorage[randLdrIdx].unitInformation;
            tmp.UpdateRelationship(newRelationship);
            // TRAVELLER SPEED
            tmp.travellerSpeed = 0.025f;


            // TROOPS CARRIED
            tmp.troopsCarried.AddRange(myParent.unitStorage.GenerateBasicWarband(unitCount));

            TravellerFlavourPhrase flavourTmp = new TravellerFlavourPhrase();
            flavourTmp.relationshipGauge = 0;
            flavourTmp.flavourText = "We're here to make some profits!";

            tmp.flavourTexts = new List<TravellerFlavourPhrase>();
            tmp.flavourTexts.Add(flavourTmp);

            return tmp;
        }


        public BaseTravellerData GenerateRandomWarbandTraveller(int unitCount, float newRelationship)
        {
            BaseTravellerData tmp = new BaseTravellerData();

            tmp.weekSpawned = ObtainPlayerWeeklyCount();
            tmp.troopsCarried = new List<TroopsInformation>();

            int randLdrIdx = UnityEngine.Random.Range(0, myParent.unitStorage.heroStorage.Count);
            tmp.leaderUnit = new BaseHeroInformationData();
            tmp.leaderUnit.unitInformation = new UnitInformationData();
            tmp.leaderUnit.unitInformation = myParent.unitStorage.heroStorage[randLdrIdx].unitInformation;
            tmp.UpdateRelationship(newRelationship);

            // TroopTypes
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


            TroopsInformation recruit = TroopsInformation.ConvertToTroopsInformation(myParent.unitStorage.GetUnitInformation("Recruit"), troopTypes[0]);
            if (myParent.unitStorage.GetUnitInformation("Recruit") == null)
            {
                Debug.Log("It's Empty");
            }
            else
            {
                Debug.Log("Max Health: " + recruit.unitInformation.maxHealth + " Unit Name: " + recruit.unitInformation.unitGenericName);
            }
            TroopsInformation swordsman = TroopsInformation.ConvertToTroopsInformation(myParent.unitStorage.GetUnitInformation("Swordsman"), troopTypes[1]);
            TroopsInformation spearman = TroopsInformation.ConvertToTroopsInformation(myParent.unitStorage.GetUnitInformation("Spearman"), troopTypes[2]);
            TroopsInformation archer = TroopsInformation.ConvertToTroopsInformation(myParent.unitStorage.GetUnitInformation("Archer"), troopTypes[3]);

            tmp.troopsCarried.Add(recruit);
            tmp.troopsCarried.Add(swordsman);
            tmp.troopsCarried.Add(spearman);
            tmp.troopsCarried.Add(archer);

            TravellerFlavourPhrase flavourTmp = new TravellerFlavourPhrase();
            flavourTmp.relationshipGauge = -50;
            flavourTmp.flavourText = "100 coins is not enough, we know you're hiding more there!";

            TravellerFlavourPhrase flavourTmp1 = new TravellerFlavourPhrase();
            flavourTmp1.relationshipGauge = -30;
            flavourTmp1.flavourText = "100 coins, you should give more!";

            TravellerFlavourPhrase flavourTmp2 = new TravellerFlavourPhrase();
            flavourTmp2.relationshipGauge = 0;
            flavourTmp2.flavourText = "100 coins might make us stay a week or two..";

            tmp.flavourTexts = new List<TravellerFlavourPhrase>();
            tmp.flavourTexts.Add(flavourTmp);
            tmp.flavourTexts.Add(flavourTmp1);
            tmp.flavourTexts.Add(flavourTmp2);
            return tmp;
        }

        public int ObtainPlayerWeeklyCount()
        {
            if (PlayerGameManager.GetInstance != null)
            {
                return PlayerGameManager.GetInstance.playerData.weekCount;
            }
            else;
            {
                return 0;
            }
        }
    }
}
