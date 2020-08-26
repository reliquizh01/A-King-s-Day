using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;

namespace GameResource
{
    /// <summary>
    /// Troops maintains peace and creates oppurtunity of income
    /// by protecting its subjects, they can only be trained
    /// by converting a villager and a fixed amount of coin.
    /// </summary>
    public class TroopResourceBehavior : BaseResourceBehavior
    {
        public int troopHealth = 10;
        public int techHealth = 0;
        public int GetMaxHealth
        {
            get
            {
                return troopHealth + techHealth;
            }
        }
        public int troopDmg = 1;
        public int techDmg = 0;
        public int GetMaxDamage
        {
            get
            {
                return troopHealth + techDmg;
            }
        }
        public int recruitCoin = 10;
        public int techCoin = 0;
        public int GetRecruitCoins
        {
            get
            {
                return recruitCoin - techCoin;
            }
        }

        public int maxTroops = 50;
        public int techMaxTroops = 0;

        public int baseMorale = 30;
        public int techMorale = 0;



        public override void SetupResourceBehavior()
        {
            base.SetupResourceBehavior();

            if (PlayerGameManager.GetInstance == null)
            {
                return;
            }

            curPlayer = PlayerGameManager.GetInstance.playerData;

            ImplementTechnology();

            UpdateWarningMechanics();
        }

        public void RecruitTroops()
        {
            if(curPlayer.coins < GetRecruitCoins && curPlayer.population < 1)
            {
                Debug.LogWarning("Trying to recruit a troop but you seem to be lacking in resources [ Coins:" + curPlayer.coins + "] [Pop:" + curPlayer.population + "]");
                return;
            }

            curPlayer.coins -= GetRecruitCoins;
            curPlayer.SetPopulation(-1);

            int idx = -1;
            if (curPlayer.troopsList == null)
            {
                curPlayer.troopsList = new List<TroopsInformation>();
            }
            if (curPlayer.troopsList.Find(x => x.unitInformation.unitName == "Recruit") != null)
            {
                idx = curPlayer.troopsList.FindIndex(x => x.unitInformation.unitName == "Recruit");
            }

            if(idx != -1)
            {
                curPlayer.troopsList[idx].totalUnitCount += 1;
            }
            else
            {
                curPlayer.troopsList.Add(TroopsInformation.ConvertToTroopsInformation(TransitionManager.GetInstance.unitStorage.GetUnitInformation("Recruit"), 0));
                curPlayer.troopsList[curPlayer.troopsList.Count - 1].totalUnitCount += 1;
            }
        }

        public override void UpdateWeeklyProgress()
        {
            base.UpdateWeeklyProgress();

            ReturnTroopsNextWeek();
        }

        public void ReturnTroopsNextWeek()
        {
            if(PlayerGameManager.GetInstance.playerData.troopsList == null)
            {
                return;
            }

            int totalReturningUnits = 0;
            for (int i = 0; i < PlayerGameManager.GetInstance.playerData.troopsList.Count; i++)
            {
                if(PlayerGameManager.GetInstance.playerData.troopsList[i].totalReturningUnitCount > 0)
                {
                    totalReturningUnits += PlayerGameManager.GetInstance.playerData.troopsList[i].totalReturningUnitCount;
                    PlayerGameManager.GetInstance.playerData.troopsList[i].totalUnitCount += PlayerGameManager.GetInstance.playerData.troopsList[i].totalReturningUnitCount;
                    PlayerGameManager.GetInstance.playerData.troopsList[i].totalReturningUnitCount = 0;
                }
            }

            if(totalReturningUnits > 0)
            {
                ProductionManager.GetInstance.ShowTroopNotif(totalReturningUnits, "Units Returned");
            }
        }
        public override void ImplementTechnology()
        {
            List<BaseTechnologyData> relatedTech = curPlayer.currentTechnologies.FindAll(x => x.improvedType == ResourceType.Food);


            foreach (BaseTechnologyData technology in relatedTech)
            {
                switch(technology.troopTechType)
                {
                    case TroopTechType.DecreaseCoinRecruit:
                        techCoin = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case TroopTechType.IncreaseDamage:
                        techDmg = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case TroopTechType.IncreaseHealth:
                        techHealth = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case TroopTechType.IncreaseMaxTroops:
                        techMaxTroops = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case TroopTechType.IncreaseMorale:
                        techMorale = technology.bonusIncrement * technology.currentLevel;
                        break;
                }
            }
        }
    }
}
