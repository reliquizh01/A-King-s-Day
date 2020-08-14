using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using Utilities;
using Dialogue;
using System;

namespace Drama
{

    public class DramaticActManager : MonoBehaviour
    {
        #region Singleton
        private static DramaticActManager instance;
        public static DramaticActManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (DramaticActManager.GetInstance == null)
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
        public DramaStorage dramaStorage;

        public DramaScenario currentDrama;
        public DarkFaderEffectsUI darkFader;

        [Header("Drama Mechanics")]
        public int curActiveFrameIdx = 0;
        public int curActionIdx = 0;
        [Header("Action Delay Mechanics")]
        public bool addDelayForAction = false;
        public float curActionDelay = 0.0f;
        private float actionDelayCounter = 0.0f;

        Action afterCurrentSceneCallback;
        public void PlayScene(string sceneTitle, Action newAfterCallBack = null)
        {
            if(currentDrama != null && !string.IsNullOrEmpty(currentDrama.scenarioName))
            {
                Debug.Log("WARNING : ATTEMPTED TO PLAY : " + sceneTitle + " WHILE DRAMA : " + currentDrama.scenarioName + " IS PLAYING.");
                return;
            }

            // Drama Storage
            currentDrama = dramaStorage.ObtainDramaByTitle(sceneTitle);
            curActiveFrameIdx = 0;

            SummonActors();
            AdjustScene();
            PlayFrame();
            afterCurrentSceneCallback = newAfterCallBack;
        }

        public void Update()
        {
            
        }
        public void PlayFrame()
        {
            for (int i = 0; i < currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].actionsOnFrameList.Count; i++)
            {
                EnactAction(currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].actionsOnFrameList[i]);
            }
        }
        public void NextFrame()
        {
            if(currentDrama == null)
            {
                return;
            }

            if(!currentDrama.CurrentFrameFinish())
            {
                return;
            }

            if (curActiveFrameIdx < currentDrama.actionsPerFrame.Count-1)
            {
                curActiveFrameIdx += 1;
                currentDrama.currentFrameIdx += 1;
                if(currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].actionsOnFrameList == null)
                {
                    return;
                }
                for (int i = 0; i < currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].actionsOnFrameList.Count; i++)
                {
                    EnactAction(currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].actionsOnFrameList[i]);
                }
            }
            else if(curActiveFrameIdx >= currentDrama.actionsPerFrame.Count-1)
            {
                if(currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].callNextStory)
                {
                    PlayScene(currentDrama.actionsPerFrame[currentDrama.currentFrameIdx].nextDramaTitle);
                }

                if(afterCurrentSceneCallback != null)
                {
                    afterCurrentSceneCallback();
                    afterCurrentSceneCallback = null;
                }
                curActiveFrameIdx = 0;
                currentDrama = null;
            }

        }
        public void EnactAction(DramaAction thisAction)
        {
            if(thisAction.delayBeforeStart > 0.0f)
            {
                float time = new float();
                time = thisAction.delayBeforeStart;
                thisAction.delayBeforeStart = 0;
                StartCoroutine(DelayThisAction(thisAction, time));
            }
            else
            {
                // Check if Actor Exist
                if(thisAction.actionType == DramaActionType.MakeActorMove)
                {
                    if (thisAction.thisActor.actorType != DramaActorType.SFX && thisAction.thisActor.actorType != DramaActorType.Tools)
                    {
                        //Debug.Log("Making Actor Move!");
                        GameObject actionActor = thisAction.thisActor.currentActor;

                        if (actionActor.GetComponent<BaseCharacter>() == null)
                            return;

                        BaseCharacter thisChar = actionActor.GetComponent<BaseCharacter>();
                        if(thisChar == null)
                        {
                           // Debug.LogError("BaseCharacter Not Found, Check Unit");
                            return;
                        }

                        if(ScenePointPathfinder.GetInstance == null)
                        {
                          //  Debug.LogError("Need ScenePoint PathFinder");
                            return;
                        }

                        if(thisAction.actorPositionIdx == 0)
                        {
                            ScenePointBehavior tmp = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(thisAction.actorsPosition[0]);
                            thisChar.SpawnInThisPosition(tmp);
                            CheckNextMove(thisChar, thisAction);
                        }
                        else
                        {
                            ScenePointBehavior tmp = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(thisAction.actorsPosition[1]);
                            thisChar.OrderMovement(tmp, () => CheckNextMove(thisChar, thisAction));
                        }
                    }
                    else if(thisAction.thisActor.actorType == DramaActorType.Tools)
                    {
                        BaseToolBehavior tmp = thisAction.thisActor.currentActor.GetComponent<BaseToolBehavior>();

                        if (tmp == null)
                            return;

                        if(thisAction.actorPositionIdx == 0)
                        {
                            tmp.OrderMovement(thisAction.actorsPosition[0], () => CheckNextMove(tmp, thisAction));
                        }
                    }
                }
                else if(thisAction.actionType == DramaActionType.BanishActor)
                {
                    GameObject actionActor = thisAction.thisActor.currentActor;

                    if (actionActor.GetComponent<BaseCharacter>() == null)
                        return;

                    BaseCharacter thisChar = actionActor.GetComponent<BaseCharacter>();

                    thisChar.OrderToBanish();
                    CheckNextMove(thisAction);
                }
                else if(thisAction.actionType == DramaActionType.ShowActor)
                {
                    GameObject actionActor = thisAction.thisActor.currentActor;

                    if (actionActor.GetComponent<BaseCharacter>() == null)
                        return;

                    BaseCharacter thisChar = actionActor.GetComponent<BaseCharacter>();

                    thisChar.OrderToReveal();
                    CheckNextMove(thisAction);
                }
                else if(thisAction.actionType == DramaActionType.ShowBriefConversation)
                {
                    if (DialogueManager.GetInstance == null)
                        return;

                    ConversationInformationData thisConversation = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle(thisAction.conversationTitle);
                    DialogueManager.GetInstance.StartConversation(thisConversation, ()=> CheckNextMove(thisAction));
                }
                else if(thisAction.actionType == DramaActionType.ShowTabCover)
                {
                    if (TransitionManager.GetInstance == null)
                        return;

                    TransitionManager.GetInstance.ShowTabCover();
                }
                else if (thisAction.actionType == DramaActionType.HideTabCover)
                {
                    if (TransitionManager.GetInstance == null)
                        return;

                    TransitionManager.GetInstance.HideTabCover();
                }
                else if (thisAction.actionType == DramaActionType.FadeToDark)
                {
                    darkFader.FadeToDark(() => CheckNextMove(thisAction));
                }
                else if (thisAction.actionType == DramaActionType.FadeToClear)
                {
                    darkFader.FadeToClear(() => CheckNextMove(thisAction));
                }
                else if (thisAction.actionType == DramaActionType.LoadThisScene)
                {
                    if (TransitionManager.GetInstance == null)
                        return;

                    currentDrama = null;
                    DialogueManager.GetInstance.afterConversationCallBack.Clear();
                    
                    TransitionManager.GetInstance.LoadScene(thisAction.loadThisScene);
                }
            }
        }

        public void FadeToDark(bool fadeAllCharacters, Action callBack)
        {

            if(fadeAllCharacters)
            {
                if(SpawnManager.GetInstance != null)
                {
                    if(SpawnManager.GetInstance.spawnedCharacterUnits != null && SpawnManager.GetInstance.spawnedCharacterUnits.Count > 0)
                    {
                        for (int i = 0; i < SpawnManager.GetInstance.spawnedCharacterUnits.Count; i++)
                        {
                            SpawnManager.GetInstance.spawnedCharacterUnits[i].OrderToBanish();
                        }
                    }
                }
            }

            darkFader.FadeToDark(callBack);
        }

        public void FadeToClear(bool revealAllCharacters, Action callBack)
        {
            if (revealAllCharacters)
            {
                if (SpawnManager.GetInstance != null)
                {
                    if (SpawnManager.GetInstance.spawnedCharacterUnits != null && SpawnManager.GetInstance.spawnedCharacterUnits.Count > 0)
                    {
                        for (int i = 0; i < SpawnManager.GetInstance.spawnedCharacterUnits.Count; i++)
                        {
                            SpawnManager.GetInstance.spawnedCharacterUnits[i].OrderToReveal();
                        }
                    }
                }
            }

            darkFader.FadeToClear(callBack);
        }
        IEnumerator DelayThisAction(DramaAction thisAction, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            EnactAction(thisAction);
        }

        public void CheckNextMove(DramaAction thisAction)
        {
            thisAction.FinishCurrentAction();
            if(thisAction.actionFinish)
            {
                NextFrame();
                return;
            }
        }
        public void CheckNextMove(BaseCharacter thisCharacter, DramaAction thisAction)
        {
            thisAction.FinishCurrentAction();
            if(thisAction.actionFinish)
            {
                NextFrame();
                return;
            }
            ScenePointBehavior tmp = ScenePointPathfinder.GetInstance.ObtainNearestScenePoint(thisAction.actorsPosition[thisAction.actorPositionIdx]);
            thisCharacter.OrderMovement(tmp, () => CheckNextMove(thisCharacter, thisAction));
        }

        public void CheckNextMove(BaseToolBehavior thisTool, DramaAction thisAction)
        {
            thisAction.FinishCurrentAction();
            if (thisAction.actionFinish)
            {
                NextFrame();
                return;
            }
            thisTool.OrderMovement(thisAction.actorsPosition[thisAction.actorPositionIdx], () => CheckNextMove(thisTool, thisAction));
        }
        public void SummonActors()
        {
            if (currentDrama == null)
                return;

            if (currentDrama.actors != null && currentDrama.actors.Count > 0 )
            {
                List<DramaActor> characterActors = currentDrama.actors.FindAll(x => x.actorType != DramaActorType.Tools && x.actorType != DramaActorType.SFX);
                for (int i = 0; i < characterActors.Count; i++)
                {
                    string unitPath = characterActors[i].characterPrefabPath.Split('.')[0];
                    unitPath = unitPath.Replace("Assets/Resources/", "");
                    if (SpawnManager.GetInstance != null)
                    {
                        GameObject curDramaActorPrefab = null;
                        if(SpawnManager.GetInstance.spawnedCharacterUnits != null && SpawnManager.GetInstance.spawnedCharacterUnits.Count > 0)
                        {
                            if(SpawnManager.GetInstance.spawnedCharacterUnits.Find(x => x.unitInformation.unitName == characterActors[i].characterName) != null)
                            {
                                curDramaActorPrefab = SpawnManager.GetInstance.spawnedCharacterUnits.Find(x => x.unitInformation.unitName == characterActors[i].characterName).gameObject;
                            }
                        }

                        if (curDramaActorPrefab != null)
                        {
                            characterActors[i].currentActor = curDramaActorPrefab;
                            currentDrama.UpdateFrameCurrentActors(characterActors[i].characterName, curDramaActorPrefab);
                        }
                        else
                        {
                            curDramaActorPrefab = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), Vector3.zero, Quaternion.identity, null);
                            characterActors[i].currentActor = curDramaActorPrefab;
                            currentDrama.UpdateFrameCurrentActors(characterActors[i].characterName, curDramaActorPrefab);

                            SpawnManager.GetInstance.spawnedCharacterUnits.Add(curDramaActorPrefab.GetComponent<BaseCharacter>());
                        }
                    }
                    else
                    {
                        GameObject tmp = GameObject.Instantiate((GameObject)Resources.Load(unitPath, typeof(GameObject)), Vector3.zero, Quaternion.identity, null);
                        characterActors[i].currentActor = tmp;
                        currentDrama.UpdateFrameCurrentActors(characterActors[i].characterName, tmp);
                    }
                }
            }
        }

        public void AdjustScene()
        {
            if (currentDrama == null)
                return;

            List<DramaActor> nonCharacterActors = currentDrama.actors.FindAll(x => x.actorType != DramaActorType.Generic && x.actorType != DramaActorType.Unique);

            if(nonCharacterActors != null && nonCharacterActors.Count > 0)
            {
                for (int i = 0; i < nonCharacterActors.Count; i++)
                {
                    if(nonCharacterActors[i].characterName == "Camera")
                    {
                        nonCharacterActors[i].currentActor = GameObject.FindGameObjectWithTag("MainCamera");
                        currentDrama.UpdateFrameCurrentActors(nonCharacterActors[i].characterName, nonCharacterActors[i].currentActor);
                    }
                }
            }
        }
    }
}