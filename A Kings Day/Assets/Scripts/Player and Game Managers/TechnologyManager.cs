using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Technology;
using Kingdoms;

namespace Managers
{

    public class TechnologyManager : BaseManager
    {
        #region Singleton
        private static TechnologyManager instance;
        public static TechnologyManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (TechnologyManager.GetInstance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        public KingdomTechnologyStorage techStorage;
        public PlayerKingdomData curPlayer;
        [SerializeField]private bool playerInitialized = false;
        public override void Start()
        {
            base.Start();
        }
        public void OnEnable()
        {
            if(PlayerGameManager.GetInstance != null)
            {
                curPlayer = PlayerGameManager.GetInstance.playerData;
            }
        }

        public int ObtainTechUpgradePrice(BaseTechnology thisTech)
        {
            int price = 100;
            // APPLY DISCOUNTS
            price = thisTech.goldLevelRequirements[thisTech.currentLevel];

            BaseTechnology discountTech = PlayerGameManager.GetInstance.playerData.currentTechnologies.Find(x => x.improvedType == ResourceType.Coin && x.coinTechType == CoinTechType.IncreaseDiscount);

            int discountAmount = discountTech.bonusIncrement * discountTech.currentLevel;

            price -= discountAmount;
            return price;
        }
        public void UpgradeThisTech(BaseTechnology thisTech)
        {
            curPlayer.currentTechnologies.Find(x => x.technologyName == thisTech.technologyName).currentLevel += 1;
        }
        public void InitializePlayerTech()
        {
            if(playerInitialized)
            {
                return;
            }
            playerInitialized = true;
            if (PlayerGameManager.GetInstance != null)
            {
                PlayerGameManager.GetInstance.playerData.currentTechnologies.AddRange(techStorage.technologies);
            }
        }
    }
}
