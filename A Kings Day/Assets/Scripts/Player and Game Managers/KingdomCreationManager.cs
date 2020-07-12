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
    public override void PreCloseManager()
    {
        StartCoroutine(saveSlots.WaitAnimationForAction(saveSlots.closeAnimationName, () => TransitionManager.GetInstance.RemoveLoading()));        
        base.PreCloseManager();
    }
    public void UpdateSlotHandler()
    {

    }
    public void GoToCreationView()
    {
        saveSlots.gameObject.SetActive(false);
        if(TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.ShowTabCover();
        }
        creationView.gameObject.SetActive(true);
    }
    public override void CloseManager()
    {
        base.CloseManager();
        if (TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.HideTabCover();
        }
        creationView.gameObject.SetActive(false);
    }

    public void LoadThisData()
    {
        if(saveSlotHandler.currentPanel == null)
        {
            return;
        }
        if (PlayerGameManager.GetInstance != null)
        {
            PlayerGameManager.GetInstance.ReceiveData(SaveData.SaveLoadManager.GetInstance.saveDataList[saveSlotHandler.selectedIndex]);
        }
        TransitionManager.GetInstance.isNewGame = false;
        StartCoroutine(saveSlots.WaitAnimationForAction(saveSlots.closeAnimationName, LoadDataToGame));
    }
    public void DeleteThisData()
    {
        if(SaveData.SaveLoadManager.GetInstance != null)
        {
            SaveData.SaveLoadManager.GetInstance.DeleteData();
        }
        saveSlotHandler.UpdatePanels(SaveData.SaveLoadManager.GetInstance.saveDataList);
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
