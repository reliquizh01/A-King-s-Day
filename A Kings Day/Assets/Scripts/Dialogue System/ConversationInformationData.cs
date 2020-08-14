using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using System;
using Characters;
using Utilities;
using Drama;

namespace Dialogue
{

    public enum DialogueType
    {
        Single,
        Choices,
    }
    public enum DialogueReaction
    {
        Normal,
        Shocked,
        BattleStance,
    }
    [Serializable]
    public class DialogueIndexReaction
    {
        public int dialogueIndex = 0;
        public string reactionDescription;
        public Action potentialCallback;
        public bool moveDialogueUp;
        public bool moveDialogueDown;
    }

    [Serializable]
    public class DialogueBy
    {
        [Header("Speaker & Dialogue Data")]
        public string charName;
        public DialogueType type;
        public DialogueReaction reaction;

        [Header("Choices Dialogue")]
        public string single;
        public List<string> choiceDialogues;
        private int chosenIdx;

        public void SetAsChosenIndex(int newChoice)
        {
            chosenIdx = newChoice;
        }
        public string GetDialogue()
        {
            string text = "";

            switch (type)
            {
                case DialogueType.Single:
                    text = "[" + charName + "]:" + single;

                    break;
                case DialogueType.Choices:
                    text = "[" + charName + "]:" + choiceDialogues[chosenIdx];

                    break;
                default:
                    break;
            }

            return text;
        }
    }
    [Serializable]
    public class ConversationInformationData
    {
        [Header("Conversation Information")]
        public string conversationTitle;
        public List<string> characterNames;

        [Header("Conversation Mechanics")]
        public List<DialogueBy> dialoguePattern;
    }
}