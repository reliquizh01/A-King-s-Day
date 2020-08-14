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
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
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

            BaseTechnologyData discountTech = PlayerGameManager.GetInstance.playerData.currentTechnologies.Find(x => x.improvedType == ResourceType.Coin && x.coinTechType == CoinTechType.IncreaseDiscount);

            int discountAmount = discountTech.bonusIncrement * discountTech.currentLevel;

            price -= discountAmount;
            return price;
        }
        public void UpgradeThisTech(BaseTechnology thisTech)
        {
            curPlayer.currentTechnologies.Find(x => x.technologyName == thisTech.technologyName).currentLevel += 1;

            if(TransitionManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame && SaveData.SaveLoadManager.GetInstance != null)
            {
                SaveData.SaveLoadManager.GetInstance.SaveCurrentData();
            }
        }

        public List<BaseTechnologyData> InitializePlayerTech()
        {

            List<BaseTechnologyData> techData = new List<BaseTechnologyData>();
            for (int i = 0; i < techStorage.technologies.Count; i++)
            {
                BaseTechnologyData tmp = new BaseTechnologyData().ConverToData(techStorage.technologies[i]);
                techData.Add(tmp);
            }

            return techData;
        }
    }
}
