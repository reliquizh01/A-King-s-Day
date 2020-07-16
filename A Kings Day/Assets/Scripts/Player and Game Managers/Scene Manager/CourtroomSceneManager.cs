using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

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
        public override void Start()
        {
            base.Start();
            if(TransitionManager.GetInstance.previousScene != sceneType)
            {
                SetPositionFromTransition(TransitionManager.GetInstance.previousScene);
            }

            TransitionManager.GetInstance.SetAsNewSceneManager(this);
            scenePointHandler.gameObject.SetActive(true);
        }

        public override void PreOpenManager()
        {
            base.PreOpenManager();

            if(PlayerGameManager.GetInstance != null)
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

        public override void StartManager()
        {
            base.StartManager();

            if(AudioManager.GetInstance != null)
            {
                AudioManager.GetInstance.PlayThisBackGroundMusic(BackgroundMusicType.courtroomDrama);
            }
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
            leftGuard.SpawnInThisPosition(scenePointHandler.scenePoints[5]);
            rightGuard.SpawnInThisPosition(scenePointHandler.scenePoints[5]);
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


        public override void OrderCharacterToMove(ScenePointBehavior toThisPoint)
        {
            base.OrderCharacterToMove(toThisPoint);
            if (toThisPoint.sceneLoader)
            {
                king.OrderMovement(toThisPoint, () => TransitionManager.GetInstance.LoadScene(toThisPoint.SceneToLoad));
            }
            else
            {
                king.OrderMovement(toThisPoint);
            }
        }

        public override void SetPositionFromTransition(SceneType prevScene)
        {
            base.SetPositionFromTransition(prevScene);
            ScenePointBehavior prevGate = scenePointHandler.scenePoints.Find(x => x.sceneLoader && x.SceneToLoad == prevScene);
            
            if(king != null)
            {
                if(prevGate != null)
                {
                    if(prevScene != SceneType.Opening)
                    {
                        Debug.Log("Previous Scene : " + prevScene);
                        king.SpawnInThisPosition(prevGate);
                    }

                    if (GameUIManager.GetInstance != null)
                    {
                        GameUIManager.GetInstance.PreOpenManager();
                    }
                }
            }
        }
    }
}
