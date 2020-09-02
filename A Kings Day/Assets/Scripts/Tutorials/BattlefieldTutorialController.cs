using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using Managers;
using Kingdoms;
using Buildings;

public class BattlefieldTutorialController : MonoBehaviour
{
    public GameObject victorySliderTut, summonSpawnTut, warChestTut, skillsTut;

    public void StartBattlefieldTutorial(bool fromCreationScene)
    {
        List<DialogueIndexReaction> callBacks = new List<DialogueIndexReaction>();

        DialogueIndexReaction temp0 = new DialogueIndexReaction();
        temp0.dialogueIndex = 1;
        temp0.potentialCallback = () => DialogueManager.GetInstance.MovePanelUp();

        DialogueIndexReaction temp1 = new DialogueIndexReaction();
        temp1.dialogueIndex = 4;
        temp1.potentialCallback = () => DialogueManager.GetInstance.MovePanelDown();

        DialogueIndexReaction temp2 = new DialogueIndexReaction();
        temp2.dialogueIndex = 5;
        temp2.potentialCallback = () => ShowVictorySliderTutorial();

        DialogueIndexReaction temp3 = new DialogueIndexReaction();
        temp3.dialogueIndex = 8;
        temp3.potentialCallback = () => ShowWarChestTutorial();

        DialogueIndexReaction temp4 = new DialogueIndexReaction();
        temp4.dialogueIndex = 12;
        temp4.potentialCallback = () => DialogueManager.GetInstance.MovePanelUp();

        DialogueIndexReaction temp5 = new DialogueIndexReaction();
        temp5.dialogueIndex = 12;
        temp5.potentialCallback = () => ShowSkillsTutorial();

        DialogueIndexReaction temp6 = new DialogueIndexReaction();
        temp6.dialogueIndex = 15;
        temp6.potentialCallback = () => DialogueManager.GetInstance.MovePanelDown();

        DialogueIndexReaction temp7 = new DialogueIndexReaction();
        temp7.dialogueIndex = 15;
        temp7.potentialCallback = () => ShowSummonSpawnTutorial();


        callBacks.Add(temp0); callBacks.Add(temp1); callBacks.Add(temp2); callBacks.Add(temp3);
        callBacks.Add(temp4); callBacks.Add(temp5); callBacks.Add(temp6); callBacks.Add(temp7);


        if (TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.HideTabCover();
        }

        if (DialogueManager.GetInstance != null)
        {


            if (fromCreationScene)
            {
                // Pause Everything First.
            }
            else
            {
                // Optional : You might be able to use this part in case you want to 're-run' the tutorial
                // post-creation
            }

            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Remember the Battlefield");
            Debug.Log("[STARTING CONVERSATION] Remembering the Battlefield");
            DialogueManager.GetInstance.StartConversation(tmp, HideAllTutorial, callBacks);
        }

    }

    public void ShowVictorySliderTutorial()
    {
        victorySliderTut.SetActive(true);
        summonSpawnTut.SetActive(false);
        warChestTut.SetActive(false);
        skillsTut.SetActive(false);
    }

    public void ShowSummonSpawnTutorial()
    {
        victorySliderTut.SetActive(false);
        summonSpawnTut.SetActive(true);
        warChestTut.SetActive(false);
        skillsTut.SetActive(false);
    }

    public void ShowWarChestTutorial()
    {
        victorySliderTut.SetActive(false);
        summonSpawnTut.SetActive(false);
        warChestTut.SetActive(true);
        skillsTut.SetActive(false);
    }

    public void ShowSkillsTutorial()
    {
        victorySliderTut.SetActive(false);
        summonSpawnTut.SetActive(false);
        warChestTut.SetActive(false);
        skillsTut.SetActive(true);
    }

    public void HideAllTutorial()
    {
        victorySliderTut.SetActive(false);
        summonSpawnTut.SetActive(false);
        warChestTut.SetActive(false);
        skillsTut.SetActive(false);

        BattlefieldSystemsManager.GetInstance.StartDay();
    }
}
