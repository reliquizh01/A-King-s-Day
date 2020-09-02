using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using Managers;
using Kingdoms;
using Buildings;

public class BalconyTutorialController : MonoBehaviour
{
    public GameObject barracksTut, farmTut, tavernTut, shopTut, blacksmithTut, travelTut, HousingTut, marketTut, enemyTut;
    public Canvas tutorialCanvas;
    public BaseTravellerBehavior prologueBandit;
    public BaseBuildingBehavior barracks;

    public void StartBalconyTutorial(bool fromCreationScene)
    {
        List<DialogueIndexReaction> callBacks = new List<DialogueIndexReaction>();

        DialogueIndexReaction temp0 = new DialogueIndexReaction();
        temp0.dialogueIndex = 1;
        temp0.potentialCallback = () => DialogueManager.GetInstance.MovePanelUp();

        DialogueIndexReaction temp1 = new DialogueIndexReaction();
        temp1.dialogueIndex = 2;
        temp1.potentialCallback = () => ShowBarracksTutorial();

        DialogueIndexReaction temp2 = new DialogueIndexReaction();
        temp2.dialogueIndex = 5;
        temp2.potentialCallback = () => ShowFarmTutorial();

        DialogueIndexReaction temp3 = new DialogueIndexReaction();
        temp3.dialogueIndex = 9;
        temp3.potentialCallback = () => ShowTavernTutorial();
        temp3.moveDialogueUp = true;

        DialogueIndexReaction temp4 = new DialogueIndexReaction();
        temp4.dialogueIndex = 13;
        temp4.potentialCallback = () => ShowShopTutorial();

        DialogueIndexReaction temp5 = new DialogueIndexReaction();
        temp5.dialogueIndex = 17;
        temp5.potentialCallback = () => ShowBlacksmithTutorial();

        DialogueIndexReaction temp6 = new DialogueIndexReaction();
        temp6.dialogueIndex = 20;
        temp6.potentialCallback = () => ShowTravelTutorial();
        temp6.moveDialogueDown = true;

        DialogueIndexReaction temp7 = new DialogueIndexReaction();
        temp7.dialogueIndex = 23;
        temp7.potentialCallback = () => ShowHousingTutorial();

        DialogueIndexReaction temp8 = new DialogueIndexReaction();
        temp8.dialogueIndex = 27;
        temp8.potentialCallback = () => ShowMarketTutorial();
        temp8.moveDialogueUp = true;


        callBacks.Add(temp0);
        callBacks.Add(temp1);
        callBacks.Add(temp2);
        callBacks.Add(temp3);
        callBacks.Add(temp4);
        callBacks.Add(temp5);
        callBacks.Add(temp6);
        callBacks.Add(temp7);
        callBacks.Add(temp8);

        if(TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.HideTabCover();
        }

        if (DialogueManager.GetInstance != null)
        {


            if (fromCreationScene)
            {
                // Summon Monster
                BalconySceneManager.GetInstance.travelSystem.SummonRandomTraveller(TravelLocation.ForestOfRetsnom, TravellerType.Invader, 2); // 20
                prologueBandit = BalconySceneManager.GetInstance.travelSystem.spawnedUnits[BalconySceneManager.GetInstance.travelSystem.spawnedUnits.Count - 1];
                prologueBandit.clickDetector.isClickable = false;
            }
            else
            {
                // Optional : You might be able to use this part in case you want to 're-run' the tutorial
                // post-creation
            }

            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Introduce the buildings");
            Debug.Log("[STARTING CONVERSATION] Introduce the buildings");
            DialogueManager.GetInstance.StartConversation(tmp, HideAllTutorial, callBacks);
        }

    }
    public void ShowBarracksTutorial()
    {
        barracksTut.SetActive(true);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        shopTut.SetActive(false);
        blacksmithTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowFarmTutorial()
    {
        farmTut.SetActive(true);
        barracksTut.SetActive(false);
        tavernTut.SetActive(false);
        shopTut.SetActive(false);
        blacksmithTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowTavernTutorial()
    {
        tavernTut.SetActive(true);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        shopTut.SetActive(false);
        blacksmithTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowShopTutorial()
    {
        shopTut.SetActive(true);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        blacksmithTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowBlacksmithTutorial()
    {
        blacksmithTut.SetActive(true);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    //travelTut, HousingTut, marketTut;

    public void ShowTravelTutorial()
    {
        tutorialCanvas.sortingOrder = 5;
        travelTut.SetActive(true);
        blacksmithTut.SetActive(false);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowHousingTutorial()
    {
        HousingTut.SetActive(true);
        blacksmithTut.SetActive(false);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        travelTut.SetActive(false);
        marketTut.SetActive(false);
    }

    public void ShowMarketTutorial()
    {
        marketTut.SetActive(true);
        blacksmithTut.SetActive(false);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
    }

    public void HideAllTutorial()
    {
        tutorialCanvas.sortingOrder = 3;
        blacksmithTut.SetActive(false);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);

        if(DialogueManager.GetInstance != null)
        {
            DialogueManager.GetInstance.MovePanelDown();
        }

        if (TransitionManager.GetInstance.isNewGame)
        {
            //Call Dialogue for Invasion here.
            StartUpgradeTutorial(true);
        }
        else
        {
            BalconySceneManager.GetInstance.interactionHandler.SwitchInteractableClickables(true);
        }

    }


    public void StartUpgradeTutorial(bool fromCreationScene)
    {
        if (BalconySceneManager.GetInstance == null)
            return;

        List<DialogueIndexReaction> callBacks = new List<DialogueIndexReaction>();



        if (DialogueManager.GetInstance != null)
        {
            ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Prologue - Upgrades");

            if (fromCreationScene)
            {
                Debug.Log("Starting Upgrade Tutorial");
                DialogueIndexReaction temp1 = new DialogueIndexReaction();
                temp1.dialogueIndex = 8;
                temp1.potentialCallback = () => ShowEnemyInvasionTut();

                DialogueIndexReaction temp2 = new DialogueIndexReaction();
                temp2.dialogueIndex = 10;
                temp2.potentialCallback = () => ShowBarracksInvasionTut();

                callBacks.Add(temp1);
                callBacks.Add(temp2);

            }
            else
            {
                DialogueIndexReaction temp1 = new DialogueIndexReaction();
                temp1.dialogueIndex = 7;
                temp1.potentialCallback = () => SkipInvasionDialogue();

                DialogueIndexReaction temp2 = new DialogueIndexReaction();
                temp2.dialogueIndex = 10;
                temp2.potentialCallback = () => ShowBarracksInvasionTut();

                callBacks.Add(temp1);
                callBacks.Add(temp2);
                // Optional : You might be able to use this part in case you want to 're-run' the tutorial
                // post-creation
            }
            DialogueManager.GetInstance.StartConversation(tmp, HideUpgradesTutorial, callBacks);

        }
    }

    public void ShowBarracksInvasionTut()
    {
        barracksTut.SetActive(true);
        enemyTut.SetActive(false);
    }

    public void SkipInvasionDialogue()
    {
        DialogueManager.GetInstance.currentDialogueIdx = 10;
    }
    public void ShowEnemyInvasionTut()
    {
        enemyTut.SetActive(true);
        barracksTut.SetActive(false);
    }

    public void HideUpgradesTutorial()
    {
        Debug.Log("Hiding Upgrades Tutorial");

        enemyTut.SetActive(false);
        barracksTut.SetActive(false);
        blacksmithTut.SetActive(false);
        shopTut.SetActive(false);
        barracksTut.SetActive(false);
        farmTut.SetActive(false);
        tavernTut.SetActive(false);
        travelTut.SetActive(false);
        HousingTut.SetActive(false);
        marketTut.SetActive(false);

        barracks.isClickable = true;
        barracks.optionHandler.buildingOptions.Find(x => x.type == BuildingOptionType.Upgrade).SetInteraction(false);
        barracks.optionHandler.buildingOptions.Find(x => x.type == BuildingOptionType.Use).SetInteraction(false);

    }

    public void StartInvasionTutorial()
    {
        ShowEnemyInvasionTut();
        ConversationInformationData tmp = DialogueManager.GetInstance.dialogueStorage.ObtainConversationByTitle("Prologue - Engage Enemy");

        DialogueManager.GetInstance.StartConversation(tmp, HideInvasionTutorial);
    }

    public void HideInvasionTutorial()
    {
        prologueBandit.clickDetector.isClickable = true;
        enemyTut.SetActive(false);
        barracksTut.SetActive(false);
    }
}
