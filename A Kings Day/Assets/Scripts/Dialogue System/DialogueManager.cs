using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Managers;

namespace Dialogue
{


    public class DialogueManager : MonoBehaviour
    {
        #region Singleton
        private static DialogueManager instance;
        public static DialogueManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (DialogueManager.GetInstance == null)
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
        public GameObject dialoguePanel;
        public Animation dialogueAnimation;
        public bool isUp;

        [Header("Dialogue Storage")]
        public DialogueInformationStorage dialogueStorage;

        [Header("Character Part")]
        public Image characterIcon;
        [Header("Dialogue Information")]
        public bool currentlyInConversation = false;
        public bool summoningAllSentences = false;
        public GameObject speechTextPrefab;
        public ConversationInformationData currentConversation;
        public int currentDialogueIdx = 0;
        public int currentBranchIdx = 0;
        [Header("Dialogue Mechanics")]
        public DialogueDeliveryType myDeliveryType;
        public float mesgSpeed = 1.25f;
        public GameObject speechParent;
        public GameObject enterKeyHighlight;
        public List<TypeWriterEffectUI> speechList;
        
        
        public List<Action> afterConversationCallBack;
        public List<DialogueIndexReaction> indexReactioncallBack;
        public void Start()
        {
            if(TransitionManager.GetInstance != null)
            {
                myDeliveryType = TransitionManager.GetInstance.optionController.dialogueControl.deliveryType;
                mesgSpeed = TransitionManager.GetInstance.optionController.dialogueControl.curDialogueSpeed;
            }
        }
        public void Update()
        {
            /*if(Input.GetKeyDown(KeyCode.Q))
            {
                ConversationInformationData tmp = dialogueStorage.ObtainConversationByTitle("Prologue - My Father's Last Order...");
                StartConversation(tmp);
            }*/

           
        }

        public void UpdateDialogueMechanics(DialogueDeliveryType deliveryType, float newSpeed)
        {
            myDeliveryType = deliveryType;
            mesgSpeed = newSpeed;

            if(speechList != null && speechList.Count > 0)
            {
                for (int i = 0; i < speechList.Count; i++)
                {
                    speechList[i].deliveryType = myDeliveryType;
                    speechList[i].intervalPerLetter = mesgSpeed;
                }
            }
        }

        public void StartConversation(ConversationInformationData thisConversation, Action callBack = null, List<DialogueIndexReaction> indexCallbacks = null)
        {
            Debug.Log("[Attempting to Start Conversation]");

            if(currentlyInConversation)
            {
                Debug.Log("[Currently in Conversation] Title:" + currentConversation.conversationTitle);
                return;
            }

            if(speechList != null && speechList.Count > 0)
            {
                ClearSpeechList();
            }

            Debug.Log("[Currently Not in Conversation]");
            dialoguePanel.SetActive(true);
            currentlyInConversation = true;
            currentDialogueIdx = 0;
            currentBranchIdx = 0;
            currentConversation = thisConversation;

            if(afterConversationCallBack == null)
            {
                afterConversationCallBack = new List<Action>();
            }

            if(callBack != null)
            {
                Debug.Log("Adding Callback To this Conversation : " + currentConversation.conversationTitle);
                afterConversationCallBack.Add(callBack);
            }

            indexReactioncallBack = indexCallbacks;

            SummonSentenceText();
        }
        public void MovePanelUp()
        {
            if (isUp)
                return;

            dialogueAnimation.Play("Move Up Dialogue");
            isUp = true;
        }

        public void MovePanelDown()
        {
            if (!isUp)
                return;

            dialogueAnimation.Play("Move Down Dialogue");
            isUp = false;
        }

        public void SummonSentenceText()
        {
            if (currentConversation == null)
                return;

            if (currentDialogueIdx >= currentConversation.dialoguePattern.Count)
                return;

            GameObject tmp = (GameObject)Instantiate(speechTextPrefab);
            tmp.transform.SetParent(speechParent.transform);

            TypeWriterEffectUI tmpWriter = tmp.GetComponent<TypeWriterEffectUI>();
            speechList.Add(tmpWriter);

            if(indexReactioncallBack != null && indexReactioncallBack.Count >0)
            {
                DialogueIndexReaction reaction = indexReactioncallBack.Find(x => x.dialogueIndex == currentDialogueIdx);
                if (reaction != null)
                {
                    if(reaction.potentialCallback == null)
                    {
                        Debug.Log("Callback for reaction Index:" + reaction.dialogueIndex + " is Unavailable!");
                    }
                    else
                    {
                        reaction.potentialCallback();
                        if(reaction.moveDialogueUp)
                        {
                            MovePanelUp();
                        }
                        else if(reaction.moveDialogueDown)
                        {
                            MovePanelDown();
                        }

                        indexReactioncallBack.Remove(reaction);
                    }
                }
                   

            }

            if (currentConversation.dialoguePattern[currentDialogueIdx].type == DialogueType.Single)
            {
                tmpWriter.allowMesgControl = true;
                tmpWriter.isDialogueMesg = true;
                tmpWriter.deliveryType = myDeliveryType;
                tmpWriter.intervalPerLetter = mesgSpeed;

                Sprite talkingIcon = TransitionManager.GetInstance.unitStorage.GetUnitIcon(currentConversation.dialoguePattern[currentDialogueIdx].charName);
                characterIcon.sprite = talkingIcon;

                if(currentDialogueIdx < currentConversation.dialoguePattern.Count-1)
                {
                    tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true, () => FadeAndSummonLastSentence(tmpWriter), ShowEffects);
                }
                else
                {
                    if(summoningAllSentences)
                    {
                        tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true);
                    }
                    else if(currentDialogueIdx < currentConversation.dialoguePattern.Count-1)
                    {
                        tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true);
                    }
                    else if(currentDialogueIdx == (currentConversation.dialoguePattern.Count-1))
                    {
                        tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true, EndConversation);
                    }
                }
                if (currentDialogueIdx < currentConversation.dialoguePattern.Count)
                {
                    currentDialogueIdx += 1;
                }
            }
        }

        public void SummonAllSentence()
        {
            if (currentConversation == null)
                return;

            if (currentDialogueIdx >= currentConversation.dialoguePattern.Count)
                return;

            summoningAllSentences = true;

            for (int i = currentDialogueIdx; i < currentConversation.dialoguePattern.Count; i++)
            {
                GameObject tmp = (GameObject)Instantiate(speechTextPrefab);
                tmp.transform.SetParent(speechParent.transform);

                TypeWriterEffectUI tmpWriter = tmp.GetComponent<TypeWriterEffectUI>();
                speechList.Add(tmpWriter);
                tmpWriter.deliveryType = myDeliveryType;
                tmpWriter.intervalPerLetter = mesgSpeed;

                if (currentDialogueIdx < currentConversation.dialoguePattern.Count - 1)
                {
                    tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true, () => FadeAndSummonLastSentence(tmpWriter), ShowEffects);
                }
                else
                {
                    tmpWriter.SetTypeWriterMessage(currentConversation.dialoguePattern[currentDialogueIdx].GetDialogue(), true, EndConversation);
                }

                if (currentDialogueIdx < currentConversation.dialoguePattern.Count)
                {
                    currentDialogueIdx += 1;
                }
            }
        }
        public void ShowEffects()
        {
            if(myDeliveryType == DialogueDeliveryType.EnterAfterEachSpeech)
            {
                enterKeyHighlight.SetActive(true);
            }
        }

        public void HideEffects()
        {
            if (myDeliveryType == DialogueDeliveryType.EnterAfterEachSpeech)
            {
                enterKeyHighlight.SetActive(false);
            }
        }
        public void EndConversation()
        {
            dialoguePanel.SetActive(false);
            currentlyInConversation = false;
            ClearSpeechList();
            currentConversation = null;
            indexReactioncallBack = null;
            summoningAllSentences = false;

            Debug.Log("Ending Conversation, AfterCallback Count: " + afterConversationCallBack.Count);

            if (afterConversationCallBack != null && afterConversationCallBack.Count > 0)
            {
                List<Action> copyCallbacks = new List<Action>();
                
                copyCallbacks.AddRange(afterConversationCallBack);
                afterConversationCallBack.Clear();

                for (int i = 0; i < copyCallbacks.Count; i++)
                {
                    copyCallbacks[i].Invoke();
                }
            }
        }
        public void ClearSpeechList()
        {
            for (int i = 0; i < speechList.Count; i++)
            {
                speechList[i].afterMessageCallback = null;
                DestroyImmediate(speechList[i].gameObject);
            }
            speechList.Clear();
        }
        public void FadeAndSummonLastSentence(TypeWriterEffectUI thisMesg)
        {
            if(Time.timeScale == 0.0f)
            {
                return;
            }

            HideEffects();
            if (currentDialogueIdx + 1 < currentConversation.dialoguePattern.Count)
            {
                speechList[currentDialogueIdx - 1].SetMessageAsFade();
                if (currentDialogueIdx - 2 >= 0)
                {
                    speechList[currentDialogueIdx - 2].SetMessageAsFadest();
                }
            }

            SummonSentenceText();
        }
    }
}