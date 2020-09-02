using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Dialogue;
using System;
using Characters;
using Managers;

namespace Drama
{
    public enum DramaSceneType
    {
        LetOtherUnitsStay,
        BanishAllActors,
        ClearAllCharacters,
    }

    public enum MethodOfPositioningActors
    {
        IfExistFadeToPosition,
        TeleportToPosition,
        WalkFromCurrentToPosition,
        SummonToInitialPosition,
    }

    public enum DramaActionType
    {
        MakeActorMove,
        ShowConversation,
        BanishActor,
        ShowTabCover,
        HideTabCover,
        ShowActor,
        FadeToDark,
        FadeToClear,
        LoadThisScene,
    }
    [Serializable]
    public class DramaActor
    {
        [Header("Character Information")]
        public string characterName;
        public string characterPrefabPath;
        public DramaActorType actorType;

        [Header("Actor Mechanics")]
        public MethodOfPositioningActors positionActorsMethod;
        public GameObject currentActor;
    }
    [Serializable]
    public class DramaAction
    {
        public DramaActionType actionType;
        public DramaActor thisActor;

        [Header("Action Mechanics")]
        public bool actionFinish = false;
        public float delayBeforeStart;
        public SceneType loadThisScene = (SceneType)0;
        [Header("Actor Mechanics")]
        public bool stayOnLastState = false;
        public int actorPositionIdx;
        public List<CharacterStates> characterStates;
        public List<float> facingDirection;
        public List<Vector3> actorsPosition;

        [Header("Conversation Mechanics")]
        public string conversationTitle;
        public bool hasLineActions;
        public List<DialogueIndexReaction> indexReactionList;
        public void FinishCurrentAction()
        {
            switch (actionType)
            {
                case DramaActionType.MakeActorMove:
                    if(actorPositionIdx >= actorsPosition.Count-1)
                    {
                        actionFinish = true;
                    }
                    else
                    {
                        actorPositionIdx += 1;
                    }
                    break;
                case DramaActionType.ShowConversation:
                    actionFinish = true;
                    break;

                case DramaActionType.FadeToClear:
                case DramaActionType.FadeToDark:
                case DramaActionType.ShowActor:
                case DramaActionType.BanishActor:
                    actionFinish = true;
                    break;
                default:
                    break;
            }
        }
    }
    [Serializable]
    public class DramaFrame
    {
        public string frameName;
        public string description;
        public List<DramaAction> actionsOnFrameList;

        public bool callNextStory;
        public string nextDramaTitle;
    }
    [Serializable]
    public class DramaScenario
    {
        [Header("Scenario Information")]
        public string scenarioName;
        public DramaSceneType sceneType;

        [Header("Actors")]
        public List<DramaActor> actors;

        [Header("Frames")]
        public int currentFrameIdx;
        public List<DramaFrame> actionsPerFrame;



        public bool CurrentFrameFinish()
        {
            bool isFinish = true;

            if(actionsPerFrame == null || actionsPerFrame.Count <= 0)
            {
                return isFinish;
            }

            for (int i = 0; i < actionsPerFrame[currentFrameIdx].actionsOnFrameList.Count; i++)
            {
                if (!actionsPerFrame[currentFrameIdx].actionsOnFrameList[i].actionFinish)
                {
                    isFinish = false;
                    continue;
                }
            }
            return isFinish;
        }

        public void UpdateFrameCurrentActors(string actorName, GameObject currentActor)
        {
            if(actionsPerFrame == null)
            {
                return;
            }

            for (int i = 0; i < actionsPerFrame.Count; i++)
            {
                for (int x = 0; x < actionsPerFrame[i].actionsOnFrameList.Count; x++)
                {
                    if(actionsPerFrame[i].actionsOnFrameList[x].thisActor.characterName == actorName)
                    {
                        actionsPerFrame[i].actionsOnFrameList[x].thisActor.currentActor = currentActor;
                    }
                }
            }
        }
    }
}
