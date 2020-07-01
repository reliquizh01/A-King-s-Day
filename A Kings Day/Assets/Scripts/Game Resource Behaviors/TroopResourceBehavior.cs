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

        public int curMorale = 100;
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

            curPlayer.recruits += 1;
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
