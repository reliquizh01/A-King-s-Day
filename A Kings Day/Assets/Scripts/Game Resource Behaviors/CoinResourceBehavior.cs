using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Managers;
using Technology;

namespace GameResource
{
    public class CoinResourceBehavior : BaseResourceBehavior
    {
        public int techTechDiscount = 0;
        public int techDiscountCoinPenalty = 0;
        public int GetTechDiscount
        {
            get { return techTechDiscount; }
        }

        public int baseMerchantArrival = 20;
        public int techMerchantArrival = 0;

        public int techSecurityIncome = 0;

        public int baseMonthlyIncome = 5;
        public int techMonthlyIncome = 0;

        public int GetTotalMonthlyIncome
        {
            get { return baseMonthlyIncome + techMonthlyIncome; }
        }

        public override void SetupResourceBehavior()
        {
            base.SetupResourceBehavior();

            if(PlayerGameManager.GetInstance == null)
            {
                return;
            }

            curPlayer = PlayerGameManager.GetInstance.playerData;


            ImplementTechnology();

            CheckMonthlyCounter();

            UpdateWarningMechanics();
        }
        public override void UpdateWeeklyProgress()
        {
            base.UpdateWeeklyProgress();
            curPlayer.curMonthTaxCounter += 1;
            CheckMonthlyCounter();
            if(curPlayer.canReceiveTax)
            {
                ProductionManager.GetInstance.ShowCoinNotif(GetTotalTax(), "Tax");
                PlayerGameManager.GetInstance.ReceiveResource(GetTotalTax(), ResourceType.Coin);
                curPlayer.canReceiveTax = false;
            }
        }

        public void CheckMonthlyCounter()
        {
            if(curPlayer.curMonthTaxCounter >= curPlayer.maxMonthTaxCount)
            {
                curPlayer.canReceiveTax = true;
                curPlayer.curMonthTaxCounter = 0;
            }

        }

        public int GetTotalTax()
        {
            int totalTax = GetTotalMonthlyIncome + (techSecurityIncome - techDiscountCoinPenalty); 
            return totalTax;
        }
        public override void ImplementTechnology()
        {
            base.ImplementTechnology();

            List<BaseTechnologyData> relatedTech = curPlayer.currentTechnologies.FindAll(x=> x.improvedType == ResourceType.Coin);

            foreach (BaseTechnologyData technology in relatedTech)
            {
                switch(technology.coinTechType)
                {
                    case CoinTechType.IncreaseDiscount:
                        techDiscountCoinPenalty = technology.currentLevel;
                        techTechDiscount = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case CoinTechType.IncreaseMerchantArrival:
                        techMerchantArrival = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case CoinTechType.IncreaseSecurityIncome:
                        techSecurityIncome = technology.bonusIncrement * technology.currentLevel;
                        break;
                    case CoinTechType.IncreaseTaxMonthlyIncome:
                        techMonthlyIncome = technology.bonusIncrement * technology.currentLevel;
                        break;
                }
            }
        }
    }
}