using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Managers;
using Kingdoms;
using Drama;
using SaveData;
using UnityEngine.UI;

public class KingdomCreationManager : BaseManager
{
    public KingdomCreationUiV2 creationView;
    public BasePanelBehavior saveSlots;
    public BasePanelWindow saveSlotWindow;
    public SaveSlotHandler saveSlotHandler;
    
    [Header("Introduction Mechanic")]
    public BasePanelBehavior introductionPanel;
    public string introDrama;

    public override void PreOpenManager()
    {
        base.PreOpenManager();
        if(introductionPanel != null)
        {
            introductionPanel.gameObject.SetActive(true);
            StartCoroutine(introductionPanel.WaitAnimationForAction(introductionPanel.openAnimationName, GoToCreationView));
        }
        else
        {
            TransitionManager.GetInstance.RemoveLoading();
        }
    }
    public override void StartManager()
    {
        base.StartManager();

        if(TransitionManager.GetInstance != null && TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Opening)
        {
            if (SaveData.SaveLoadManager.GetInstance.saveDataList != null &&
               SaveData.SaveLoadManager.GetInstance.saveDataList.Count > 0)
            {
                if(saveSlots.gameObject != null)
                {
                    saveSlots.gameObject.SetActive(true);
                    saveSlots.PlayOpenAnimation();
                    saveSlotWindow.parentCloseCallback = TransitionToOpenScene;
                    saveSlotHandler.SetSavePanels(SaveData.SaveLoadManager.GetInstance.saveDataList);
                }
            }
            else
            {
                saveSlotHandler.nokingdomText.gameObject.SetActive(true);
                creationView.gameObject.SetActive(true);
            }
        }
    }
    public override void PreCloseManager()
    {
        StartCoroutine(saveSlots.WaitAnimationForAction(saveSlots.closeAnimationName, () => TransitionManager.GetInstance.RemoveLoading()));        
        base.PreCloseManager();
    }

    public void GoToCreationView()
    {
        if (TransitionManager.GetInstance != null) 
        {
            if(TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Opening)
            {
                saveSlots.gameObject.SetActive(false);
            }
        }
        if(DramaticActManager.GetInstance != null)
        {
            DramaticActManager.GetInstance.PlayScene(introDrama);
        }
        creationView.gameObject.SetActive(true);
        creationView.myPanel.PlayOpenAnimation();
    }
    public override void CloseManager()
    {
        base.CloseManager();
        if (TransitionManager.GetInstance != null)
        {
            TransitionManager.GetInstance.HideTabCover();
        }
        if(creationView != null)
        {
            creationView.gameObject.SetActive(false);
        }
    }

    public void LoadThisData()
    {
        if(saveSlotHandler.currentPanel == null)
        {
            return;
        }
        if(saveSlotHandler.isLoading)
        {
            return;
        }
        saveSlotHandler.isLoading = true;

        if (PlayerGameManager.GetInstance != null)
        {
            PlayerGameManager.GetInstance.ReceiveData(SaveData.SaveLoadManager.GetInstance.saveDataList[saveSlotHandler.selectedIndex]);

            PlayerCampaignData tmp = new PlayerCampaignData();
            tmp = SaveData.SaveLoadManager.GetInstance.saveCampaignDataList[saveSlotHandler.selectedIndex];
            PlayerGameManager.GetInstance.ReceiveCampaignData(tmp);
        }
        TransitionManager.GetInstance.isNewGame = false;
        saveSlotWindow.parentCloseCallback = null;
        Debug.Log("Removign Save Slot Window!");
        StartCoroutine(saveSlots.WaitAnimationForAction(saveSlots.closeAnimationName, LoadDataToGame));
    }
    public void DeleteThisData()
    {
        if(SaveData.SaveLoadManager.GetInstance != null)
        {
            SaveData.SaveLoadManager.GetInstance.DeleteData();
            SaveData.SaveLoadManager.GetInstance.DeleteCampaignData();
        }
        saveSlotHandler.UpdatePanels(SaveData.SaveLoadManager.GetInstance.saveDataList);
    }
    public void LoadDataToGame()
    {
        TransitionManager.GetInstance.LoadScene(SceneType.Courtroom);
        TransitionManager.GetInstance.TransitionToNextGameView(SceneType.Courtroom);
    }
    public void TransitionToOpenScene()
    {
        Debug.Log("Transitioning to Open Creation");
        if(TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Opening)
        {
            saveSlotWindow.parentCloseCallback = null;
            saveSlotHandler.ResetPanels();

            TransitionManager.GetInstance.SetAsCurrentManager(SceneType.Opening);
        }
        else
        {
            TransitionManager.GetInstance.TransitionToNextGameView(SceneType.Opening);
        }
    }

    public void TransitionToKingdomCreationScene()
    {
        if (SaveLoadManager.GetInstance != null)
        {
            if (SaveLoadManager.GetInstance.saveDataList != null
                && SaveLoadManager.GetInstance.saveDataList.Count > 0)
            {
                TransitionManager.GetInstance.TransitionToNextGameView(SceneType.Creation);
            }
            else
            {
                TransitionManager.GetInstance.LoadScene(SceneType.Creation);
            }
        }
        else
        {
            TransitionManager.GetInstance.LoadScene(SceneType.Creation);
        }
    }

    public void CreateNewKingdom()
    {
        saveSlotWindow.parentCloseCallback = null;
        saveSlotWindow.CloseWindow();
        TransitionManager.GetInstance.LoadScene(SceneType.Creation);
    }
}
