using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using KingEvents;
using Utilities;

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
                if(transform.parent == null)
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
        [SerializeField]private PlayerKingdomData curPlayer;
        [SerializeField] private PlayerCampaignData curCampaign;

        public NotificationHandler notifHandler;

        public override void Start()
        {
            base.Start();

            if(PlayerGameManager.GetInstance != null)
            {
                curPlayer = PlayerGameManager.GetInstance.playerData;
                curCampaign = PlayerGameManager.GetInstance.campaignData;
            }

            EventBroadcaster.Instance.AddObserver(EventNames.WEEKLY_UPDATE, WeeklyProductionProgress);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.WEEKLY_UPDATE, WeeklyProductionProgress);
        }

        public void WeeklyProductionProgress(Parameters p = null)
        {

        }

        public void ShowPopNotif(int amount, string fromDescription)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Population, amount, false, fromDescription);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Population, amount, true, fromDescription);
            }
        }
        public void ShowTroopNotif(int amount, string fromDescription)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Troops, amount, false, fromDescription);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Troops, amount, true, fromDescription);
            }
        }
        public void ShowFoodNotif(int amount, string fromDescription)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Food, amount, false, fromDescription);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Food, amount, true, fromDescription);
            }
        }
        public void ShowCoinNotif(int amount, string fromDescription)
        {
            if (amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Coin, amount, false, fromDescription);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Coin, amount, true, fromDescription);
            }
        }
        public void ShowCowNotif(int amount, string fromDescription)
        {
            if(amount >= 0)
            {
                notifHandler.RevealResourceNotification(ResourceType.Cows, amount, false, fromDescription);
            }
            else
            {
                notifHandler.RevealResourceNotification(ResourceType.Cows, amount, true, fromDescription);
            }
        }
    }
}
