using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;

namespace GameResource
{
    /// <summary>
    /// Food is the reason the village population increases
    /// food comes from granary and cattles, which are produced
    /// in an interval rate.
    /// </summary>
    public class FoodResourceBehavior : BaseResourceBehavior
    {
        // Grains
        public int curWeeksCounter = 0;

        public int curHarvestWeekCounter = 10;
        private int techHarvestWeekCounter = 0;
        public int GetHarvestTime
        {
            get
            {
                return curHarvestWeekCounter - techHarvestWeekCounter;
            }
        }

        public int foodStorageCounter = 50;
        private int techStorageCounter = 0;
        public int GetMaxFoodStorage
        {
            get
            {
                return foodStorageCounter + techStorageCounter + (2 * curPlayer.storageKeeperCount);
            }
        }

        public int curMaxVillageWorkers = 5;
        public int techHarvestProduce = 0;  // Number of Food Obtained per worker
        public int techAddVillageWorkers = 0;
        public int GetMaxVillageWorkers
        {
            get
            {
                return curMaxVillageWorkers + techAddVillageWorkers;
            }
        }

        // Cow Herd
        public bool canCowBirth = false;
        private int minimumCowsToAllowBirth = 2; 

        private float cowBirthChance = 0.30f; // chance of having a new cow.
        public float techBirthChance = 0.0f; 
        public float GetCowBirth
        {
            get
            {
                return cowBirthChance + techBirthChance + ((float)curPlayer.herdsmanCount * 0.01f);
            }
        }
        private float twinsBirthChance = 0.05f; // chance of having 2 new cows.
        private float tripleBirthChance = 0.01f; // chance of having 3 new cows.

        private int birthRollWeekInterval = 5; // Every 5 weeks roll dice for cowbirth.

        public int cowMeatPerKill = 3;
        public int techMeatPerKill = 0;

        public override void SetupResourceBehavior()
        {
            base.SetupResourceBehavior();

            if(PlayerGameManager.GetInstance == null)
            {
                return;
            }

            curPlayer = PlayerGameManager.GetInstance.playerData;

            CheckGrainCounter();

            CheckCowCounter();

            ImplementTechnology();
        }

        public override void UpdateWeeklyProgress()
        {
            CheckCowCounter();
            CheckGrainCounter();
        }

        public override void ImplementTechnology()
        {
            if(TechnologyManager.GetInstance == null)
            {
                return;
            }
            List<BaseTechnology> relatedTech = curPlayer.currentTechnologies.FindAll(x => x.improvedType == ResourceType.Food);

            foreach (BaseTechnology technology in relatedTech)
            {
                switch(technology.foodTechType)
                {
                    case FoodTechType.IncreaseProduce:
                        techHarvestProduce = technology.bonusIncrement * technology.currentLevel; // Food Produce every harvest
                        break;
                    case FoodTechType.IncreaseStorage:
                        techStorageCounter = technology.bonusIncrement * technology.currentLevel; // Food Storage
                        break;
                    case FoodTechType.lessWeekTotalInterval:
                        techHarvestWeekCounter = technology.bonusIncrement * technology.currentLevel; // Reduce Harvest Week cooldown
                        break;
                    case FoodTechType.IncreaseCowBirth:
                        techBirthChance = (technology.bonusIncrement * technology.currentLevel) *0.001f; // Increase Cow Birth.
                        break;
                    case FoodTechType.IncreaseCowMeat:
                        techMeatPerKill = technology.bonusIncrement * technology.currentLevel; // Increase Cow  Meat
                        break;
                }
            }

        }
        #region GRAIN COW CHECKER AND PRODUCTION

        /// GRAINS - PRODUCTION DOESNT SHOW UP ALL THE TIME.
        public void CheckGrainCounter()
        {
            curPlayer.curGrainWeeksCounter += 1;
            // GRAINS
            if (curPlayer.curGrainWeeksCounter >= curHarvestWeekCounter)
            {
                PlayerGameManager.GetInstance.playerData.curGrainWeeksCounter = 0;
                curPlayer.canReceiveGrainProduce = true;
            }

            if(curPlayer.canReceiveGrainProduce)
            {
                int addedFood = GetGrainProduction();
                if (ProductionManager.GetInstance != null)
                {
                    ProductionManager.GetInstance.ShowFoodNotif(addedFood);
                }

                curPlayer.foods += addedFood;
                curPlayer.canReceiveGrainProduce = false;
            }

        }
        public int GetGrainProduction()
        {
            int thisWeeksProduce = (curPlayer.farmerCount * (techHarvestProduce+1));

            return thisWeeksProduce;
        }

        // COWS
        public void CheckCowCounter()
        {
            if(curPlayer.cows > minimumCowsToAllowBirth)
            {
                curPlayer.curCowBirthCounter += 1;
                if (curPlayer.curCowBirthCounter >= birthRollWeekInterval)
                {
                    curPlayer.curCowBirthCounter = 0;
                    curPlayer.canReceiveNewCows = RollCowBirthDice();
                }
                else
                {
                    Debug.LogWarning("Cows Attempted to Give birth! unfortunately, the calf didnt survive.");
                }

            }

            if(curPlayer.canReceiveNewCows)
            {
                int cowsToAdd = 1;
                if(RollTwinsBirth())
                {
                    cowsToAdd += 1;
                }
                else if(RollTripletBirth())
                {
                    cowsToAdd += 2;
                }
                curPlayer.cows += cowsToAdd;
                if(ProductionManager.GetInstance != null)
                {
                    ProductionManager.GetInstance.ShowCowNotif(cowsToAdd);
                }

                curPlayer.canReceiveNewCows = false;
            }
        }
        public bool RollCowBirthDice()
        {
            bool giveBirth = false;
            float rollDice = UnityEngine.Random.Range(0.0f, 100.0f);

            if(rollDice <= (GetCowBirth * 100.0f))
            {
                giveBirth = true;
            }

            return giveBirth;
        }
        public bool RollTwinsBirth()
        {
            bool giveBirth = false;
            float rollDice = UnityEngine.Random.Range(0.0f, 100.0f);

            if (rollDice <= (twinsBirthChance * 100.0f))
            {
                giveBirth = true;
            }

            return giveBirth;
        }

        public bool RollTripletBirth()
        {
            bool giveBirth = false;
            float rollDice = UnityEngine.Random.Range(0.0f, 100.0f);

            if (rollDice <= (tripleBirthChance * 100.0f))
            {
                giveBirth = true;
            }

            return giveBirth;
        }

        public int GetCowMeat(int cowsKilled)
        {
            int totalMeat = cowsKilled * (techMeatPerKill + cowMeatPerKill);
            return totalMeat;
        }

        #endregion
    }
}
