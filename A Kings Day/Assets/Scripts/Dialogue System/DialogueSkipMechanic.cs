using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogue
{
    public class DialogueSkipMechanic : MonoBehaviour
    {
        public bool editorMode = false;
        public bool isClicked = false;
        public bool clickedOnce = false;
        public Image skipFill;
        public float curCount;
        public float targetCount = 2;

        public void Start()
        {
            #if UNITY_EDITOR
            editorMode = true;
            #endif
        }
        public void Update()
        {
            if (DialogueManager.GetInstance.currentlyInConversation)
            {
                if(Input.GetKey(UtilitiesCommandObserver.GetInstance.GetKey("SKIP_WHOLE_CONVERSATION")))
                {
                    isClicked = true;
                }
                else
                {
                    isClicked = false;
                    curCount = 0;
                    skipFill.fillAmount = 0;
                }

                if(isClicked)
                {
                    if(editorMode)
                    {
                        curCount += 0.025f;
                        skipFill.fillAmount = curCount / targetCount;
                        if(curCount >= targetCount)
                        {
                            DialogueManager.GetInstance.SummonAllSentence();
                            isClicked = false;
                            curCount = 0;
                            skipFill.fillAmount = 0;
                        }
                    }
                }
            }
        }
    }

}