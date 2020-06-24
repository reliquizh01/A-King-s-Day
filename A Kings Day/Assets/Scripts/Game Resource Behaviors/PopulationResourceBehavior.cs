using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;

namespace GameResource
{
    public class PopulationResourceBehavior : BaseResourceBehavior
    {
        public int uncleanWeeks = 0;
        public int baseDeathChance = 4;
        public int GetDeathRate
        {
            get { return baseDeathChance + uncleanWeeks; }
        }

        public int baseBirthrate = 20;
        public int techBirthrate = 0;
        public int GetBirthRate
        {
            get { return baseBirthrate + techBirthrate; }
        }

        public int settlerChance = 10;
        public int techSettlerChance = 0;
        public int GetSettlerChance
        {
            get { return settlerChance + techSettlerChance; }
        }

        public int PopPerFood = 4;
        public int techPopPerFood = 0;

        public int GetPopPerFood
        {
            get
            {
                return PopPerFood + techPopPerFood;
            }
        }

        public int maxPopulation = 50;
        public int techMaxPopulation = 0;

        public int GetMaxPopulation
        {
            get { return techMaxPopulation + maxPopulation; }
        }
        public int coinPerPop = 1;
        public int techCoinPerPop = 0;
        public int GetTaxPerPop
        {
            get { return coinPerPop + techCoinPerPop; }
        }
        public int taxInterval = 4;

        public override void SetupResourceBehavior()
        {
            base.SetupResourceBehavior();
            if(PlayerGameManager.GetInstance == null)
            {
                return;
            }

            CheckTaxCounter();

            ImplementTechnology();
        }

        public override void ImplementTechnology()
        {
            base.ImplementTechnology();

            List<BaseTechnology> relatedTech = curPlayer.currentTechnologies.FindAll(x => x.improvedType== ResourceType.Population);

            foreach (BaseTechnology technology in relatedTech)
            {
                switch(technology.popTechType)
                {
                    case PopulationTechType.IncreaseBirthrate:
                        techBirthrate = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case PopulationTechType.IncreaseFoodSupport:
                        techPopPerFood = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case PopulationTechType.IncreaseMaxVillagers:
                        techMaxPopulation = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case PopulationTechType.IncreaseSettlerChance:
                        techSettlerChance = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case PopulationTechType.IncreaseTaxCollected:
                        techCoinPerPop = technology.bonusIncrement * technology.currentLevel;
                        break;
                }
            }
        }

        public void CheckTaxCounter()
        {
            if(curPlayer.curTaxWeeksCounter < taxInterval)
            {
                return;
            }

            curPlayer.curTaxWeeksCounter = 0;
            curPlayer.canReceiveTax = true;

            curPlayer.coins += GetTaxPerPop * curPlayer.population;
        }
    }
}