using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using Kingdoms;
using Characters;
using Drama;
using ResourceUI;

namespace Managers
{
    public class CreationSceneManager : BaseSceneManager
    {
        #region Singleton
        private static CreationSceneManager instance;
        public static CreationSceneManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (CreationSceneManager.GetInstance == null)
            {
                instance = this;
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }

            if (TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsNewSceneManager(this);
                TransitionManager.GetInstance.SetAsCurrentManager(SceneType.Creation);
            }
        }
        #endregion

        [Header("Creation Manager")]
        public KingdomCreationManager kingdomCreationManager;

        [Header("Game Introduction")]
        public string prologueSceneTitle;
        public string partTwoprologueSceneTitle;
        public List<InfiniteScrollScript> scrollingPathList;
        public override void PreOpenManager()
        {
            base.PreOpenManager();
            Loaded = true;
            kingdomCreationManager.PreOpenManager();

            if(TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.SetAsCurrentManager(SceneType.Creation);
            }
        }

        public void StartPrologue()
        {
            PlayerGameManager.GetInstance.ReceiveTroops(25, "Recruit");
            PlayerGameManager.GetInstance.ReceiveTroops(15, "Archer");
            PlayerGameManager.GetInstance.ReceiveTroops(20, "Swordsman");
            PlayerGameManager.GetInstance.ReceiveTroops(20, "Spearman");

            kingdomCreationManager.creationView.myPanel.PlayCloseAnimation();
            kingdomCreationManager.creationView.playPrologueTab.CloseWindow();
            StopPathScrolling();
            if (DramaticActManager.GetInstance != null)
            {
                TransitionManager.GetInstance.playingPrologue = true;
                DramaticActManager.GetInstance.PlayScene(prologueSceneTitle, IntroduceResources);
            }
        }
        public void IntroduceResources()
        {
            if(TransitionManager.GetInstance == null)
            {
                return;
            }
            ResourceInformationController.GetInstance.StartOverheadTutorial(true);

        }
        public void StopPathScrolling()
        {
            for (int i = 0; i < scrollingPathList.Count; i++)
            {
                scrollingPathList[i].startmoving = false;
            }
        }

        public void EndPrologueScene(PlayerKingdomData temporaryKingdom)
        {
            if (SaveLoadManager.GetInstance != null)
            {
                PlayerCampaignData temp = new PlayerCampaignData();
                temp.fileData = true;
                temp.travellerList = new List<BaseTravellerData>();
                temp.mapPointList = new List<Maps.MapPointInformationData>();
                temp.mapPointList.AddRange(TransitionManager.GetInstance.kingdomMapStorage.mapPointsStorage);

                Debug.Log("RESOURCE: " + temporaryKingdom.coins);
                SaveLoadManager.GetInstance.SetNewSaveData(temporaryKingdom, temp);
                temporaryKingdom._fileName = SaveLoadManager.GetInstance.saveDataList[SaveLoadManager.GetInstance.saveDataList.Count - 1]._fileName;
                temp._fileName = SaveLoadManager.GetInstance.saveCampaignDataList[SaveLoadManager.GetInstance.saveCampaignDataList.Count - 1]._fileName;

                PlayerGameManager.GetInstance.ReceiveData(temporaryKingdom);
                PlayerGameManager.GetInstance.ReceiveCampaignData(temp);
            }

            if (TransitionManager.GetInstance != null)
            {
                TransitionManager.GetInstance.LoadScene(SceneType.Courtroom);
            }
        }
    }

}
