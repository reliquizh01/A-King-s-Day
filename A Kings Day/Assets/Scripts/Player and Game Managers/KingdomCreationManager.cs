using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Managers;
using Kingdoms;

public class KingdomCreationManager : BaseManager
{
    public GameObject creationView;
    public BasePanelBehavior saveSlots;
    public SaveSlotHandler saveSlotHandler;

    public PlayerKingdomData CreateKingdom()
    {
        PlayerKingdomData tmpKingdom = new PlayerKingdomData();
            

        return tmpKingdom;
    }
    public override void PreOpenManager()
    {
        base.PreOpenManager();

    }
    public override void StartManager()
    {
        Debug.Log("Starting Kingdom Creation Manager");
        base.StartManager();

        if (SaveData.SaveLoadManager.GetInstance.saveDataList != null &&
           SaveData.SaveLoadManager.GetInstance.saveDataList.Count > 0)
        {
            saveSlots.gameObject.SetActive(true);
            saveSlots.PlayOpenAnimation();

            saveSlotHandler.SetSavePanels(SaveData.SaveLoadManager.GetInstance.saveDataList);
        }
        else
        {
            saveSlotHandler.nokingdomText.gameObject.SetActive(true);
            creationView.gameObject.SetActive(true);
        }
    }

    public void GoToCreationView()
    {
        saveSlots.gameObject.SetActive(false);
        creationView.gameObject.SetActive(true);
    }
    public override void CloseManager()
    {
        base.CloseManager();
        creationView.gameObject.SetActive(false);
    }

    public void LoadThisData()
    {

        if (PlayerGameManager.GetInstance != null)
        {
            PlayerGameManager.GetInstance.ReceiveData(SaveData.SaveLoadManager.GetInstance.saveDataList[saveSlotHandler.selectedIndex]);
        }
        StartCoroutine(saveSlots.WaitAnimationForAction(saveSlots.closeAnimationName, LoadDataToGame));
        TransitionManager.GetInstance.isNewGame = false;
    }

    public void LoadDataToGame()
    {

        TransitionManager.GetInstance.LoadScene(SceneType.Courtroom);
        TransitionManager.GetInstance.TransitionToNextGameView(GameViews.CourtroomView);
    }
    public void TransitionToOpenScene()
    {
        Debug.Log("Transitioning to Open Creation");
        TransitionManager.GetInstance.TransitionToNextGameView(GameViews.OpeningView);
    }
}
