using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Utilities;
using Drama;
using Kingdoms;
using SaveData;

namespace Managers
{
    /// <summary>
    /// Controls all the visuals and special effects in the Courtroom
    /// 
    /// like the lights going dark or the upgrades and so on.
    /// </summary>
    public class CourtroomSceneManager : BaseSceneManager
    {
        #region Singleton
        private static CourtroomSceneManager instance;
        public static CourtroomSceneManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            instance = this;
        }
        #endregion

        public bool guardsOutside = false;

        public BaseCharacter leftGuard, rightGuard;
        public ScenePointBehavior kingsSeat;
        public override void Start()
        {
            base.Start();
            if(TransitionManager.GetInstance != null)
            {
                if(TransitionManager.GetInstance.previousScene != sceneType)
                {
                    SetPositionFromTransition(TransitionManager.GetInstance.previousScene, false);
                }


                TransitionManager.GetInstance.SetAsNewSceneManager(this);
            }
            scenePointHandler.gameObject.SetActive(true);
        }

        public override void PreOpenManager()
        {
            base.PreOpenManager();
            if(TransitionManager.GetInstance.isNewGame)
            {
                interactionHandler.SwitchInteractableClickables(false);
                PlaceGuardOutside();
                Destroy(player.gameObject);

                DramaticActManager.GetInstance.FadeToClear(true, ()=> DramaticActManager.GetInstance.PlayScene("[Part 5]Prologue - The Lost Throne", FinishPrologueSetupTrueNewGame));
            }
            else if(PlayerGameManager.GetInstance != null)
            {
                if(PlayerGameManager.GetInstance.playerData.queuedDataEventsList == null)
                {
                    PlayerGameManager.GetInstance.playerData.queuedDataEventsList = new List<KingEvents.EventDecisionData>();
                }

                if (PlayerGameManager.GetInstance.playerData.queuedDataEventsList.Count <= 0 &&
                    PlayerGameManager.GetInstance.playerData.curDataEvent != null &&
                    string.IsNullOrEmpty(PlayerGameManager.GetInstance.playerData.curDataEvent.title))
                {
                    PlaceGuardOutside();
                }
            }
        }

        public void FinishPrologueSetupTrueNewGame()
        {
            // Player Data
            PlayerGameManager.GetInstance.ReceiveData(SaveData.SaveLoadManager.GetInstance.inheritanceData);
            // Campaign Data
            PlayerCampaignData temp = new PlayerCampaignData();
            temp.fileData = true;
            temp.travellerList = new List<BaseTravellerData>();
            temp.mapPointList = new List<Maps.MapPointInformationData>();
            temp.mapPointList.AddRange(TransitionManager.GetInstance.kingdomMapStorage.mapPointsStorage);
            // Save New Data
            PlayerGameManager.GetInstance.ReceiveCampaignData(temp);

            


            base.PreOpenManager();
            if (PlayerGameManager.GetInstance.playerData.queuedDataEventsList == null)
            {
                PlayerGameManager.GetInstance.playerData.queuedDataEventsList = new List<KingEvents.EventDecisionData>();
            }


            player = SpawnManager.GetInstance.spawnedCharacterUnits.Find(x => x.unitInformation.unitName == "Player");
            kingsSeat.OnMouseDown();

            if (PlayerGameManager.GetInstance.playerData.queuedDataEventsList == null)
            {
                PlayerGameManager.GetInstance.playerData.queuedDataEventsList = new List<KingEvents.EventDecisionData>();
            }

            TransitionManager.GetInstance.isNewGame = false;
            TransitionManager.GetInstance.playingPrologue = false;

            interactionHandler.SwitchInteractableClickables(true);

            if (AudioManager.GetInstance != null)
            {
                AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.courtroomDrama);
            }

            KingdomManager.GetInstance.AllowStartEvent();
            KingdomManager.GetInstance.StartWeekEvents();

            ResourceUI.ResourceInformationController.GetInstance.ShowResourcePanel(ResourceUI.ResourcePanelType.overhead);
            ResourceUI.ResourceInformationController.GetInstance.ShowWeekendPanel();
        }
        public override void StartManager()
        {
            base.StartManager();

            if(AudioManager.GetInstance != null)
            {
                AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.courtroomDrama);
            }

            if (KingdomManager.GetInstance != null && !TransitionManager.GetInstance.isNewGame)
            {
                if (PlayerGameManager.GetInstance != null && PlayerGameManager.GetInstance.playerData != null)
                {
                    if (PlayerGameManager.GetInstance.playerData.eventFinished < 3)
                    {
                        KingdomManager.GetInstance.AllowStartEvent();

                    }
                }
            }

        }
        public override void PreCloseManager()
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
            base.PreCloseManager();
        }
        public void MakeGuardLeave(Action callback = null)
        {
            leftGuard.OrderMovement(scenePointHandler.scenePoints[5]);
            StartCoroutine(DelayLeave(callback));

            guardsOutside = true;
        }
        IEnumerator DelayLeave(Action callback)
        {
            yield return new WaitForSeconds(0.5f);
            rightGuard.OrderMovement(scenePointHandler.scenePoints[5], callback);
        }
        public void PlaceGuardOutside()
        {
            leftGuard.SpawnInThisPosition(scenePointHandler.scenePoints[5], true);
            rightGuard.SpawnInThisPosition(scenePointHandler.scenePoints[5], true);
            guardsOutside = true;
        }

        public void MakeGuardShow(Action callBack = null)
        {
            if (!guardsOutside)
            {
                callBack();
                return;
            }

            leftGuard.OrderMovement(scenePointHandler.scenePoints[8]);
            StartCoroutine(DelayShow(callBack));
        }

        IEnumerator DelayShow(Action callback)
        {
            yield return new WaitForSeconds(0.5f);
            rightGuard.OrderMovement(scenePointHandler.scenePoints[9], callback);
        }



        public override void SetPositionFromTransition(SceneType prevScene, bool directToOffset = true)
        {
            base.SetPositionFromTransition(prevScene, directToOffset);
            ScenePointBehavior prevGate = scenePointHandler.scenePoints.Find(x => x.sceneLoader && x.SceneToLoad == prevScene);
            
            if(player != null)
            {
                if(prevGate != null)
                {
                    player.SpawnInThisPosition(prevGate, directToOffset);
                    player.OrderToFace(FacingDirection.Left);
                }
                else
                {
                    player.SpawnInThisPosition(kingsSeat);
                    player.OrderToFace(FacingDirection.Down);
                }
                if (GameUIManager.GetInstance != null)
                {
                    GameUIManager.GetInstance.PreOpenManager();
                }

            }
        }
    }
}
