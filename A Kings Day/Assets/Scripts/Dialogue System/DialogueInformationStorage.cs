using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
public class DialogueInformationStorage : MonoBehaviour
{
    public List<ConversationInformationData> conversationList;

    public void Start()
    {
        if(conversationList == null)
        {
            conversationList = new List<ConversationInformationData>(); 
        }
    }
    public ConversationInformationData ObtainConversationByTitle(string thisTitle)
    {
        ConversationInformationData thisConversation = conversationList.Find(x => x.conversationTitle == thisTitle);

        return thisConversation;
    }
}
