using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kingdoms;
using Managers;

namespace ConversationControls
{
    public class ConversationController : MonoBehaviour
    {
        public bool isConversing = false;

        public void StartConversation()
        {
            isConversing = true;
            // CALL CONVERSATION HERE THEN END CONVERSATION;
            EndConversation(false);
        }
        public void LeavingConversation()
        {
            // CALL CONVERSATION HERE THEN END CONVERSATION;
            EndConversation(true);
        }


        public void EndConversation(bool isLeaving = false)
        {
            if(!isLeaving)
            {
                SpawnManager.GetInstance.StartCourt();
            }
            else
            {
                isConversing = false;
                SpawnManager.GetInstance.LeaveCourt();
            }
        }


    }
}
