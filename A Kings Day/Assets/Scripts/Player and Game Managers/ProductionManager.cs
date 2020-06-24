using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using KingEvents;

namespace Managers
{
    /// <summary>
    /// Handles all the production computation and how they're improved thru technology.
    /// </summary>
    public class ProductionManager : BaseManager
    {
        #region Singleton
        private static ProductionManager instance;
        public static ProductionManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (ProductionManager.GetInstance == null)
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
        private PlayerKingdomData curPlayer;

        public NotificationHandler notifHandler;

        public override void Start()
        {
            base.Start();

            if(PlayerGameManager.GetInstance != null)
            {
                curPlayer = PlayerGameManager.GetInstance.playerData;
            }
        }

        public void ShowPopNotif(int amount)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Population, amount);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Population, amount, true);
            }
        }
        public void ShowTroopNotif(int amount)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Troops, amount);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Troops, amount, true);
            }
        }
        public void ShowFoodNotif(int amount)
        {
            Debug.Log("FUCUkasdkaka");
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Food, amount);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Food, amount, true);
            }
        }
        public void ShowCoinNotif(int amount)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Coin, amount);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Coin, amount, true);
            }
        }
        public void ShowCowNotif(int amount)
        {
            if(amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Cows, amount);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Cows, amount, true);
            }
        }
    }
}
