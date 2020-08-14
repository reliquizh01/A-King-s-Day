using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public GameObject tabCover;
    [Header("Control Button")]
    public Button audioBtn;
    public Button keyboardBtn; 
    public Button dialogueBtn;
    [Header("Panel Windows")]
    public BasePanelWindow InGameOption;
    public BasePanelWindow OptionsPanel;

    [Header("Control Panels")]
    public GameObject volumePanel;
    public GameObject controlPanel;
    public GameObject dialoguePanel;

    public VolumeSliderController volumeControl;
    public DialogueController dialogueControl;

    public void Start()
    {
        ShowControlPanel();

        InGameOption.parentOpenCallback = ()=> SwitchCover(true);
        InGameOption.parentCloseCallback = ()=> SwitchCover(false);

        OptionsPanel.parentOpenCallback = () => SwitchCover(true);
        OptionsPanel.parentCloseCallback = () => SwitchCover(false);
    }
    public void SwitchCover(bool open)
    {
        if(open == false)
        {
            if(!InGameOption.gameObject.activeSelf && !OptionsPanel.gameObject.activeSelf)
            {
                tabCover.SetActive(false);
            }
        }
        else
        {
            tabCover.SetActive(true);
        }
    }
    public void OpenInGameOptions()
    {
        tabCover.SetActive(true);
        InGameOption.gameObject.SetActive(true);
    }

    public void OpenOptionPanel()
    {
        tabCover.SetActive(true);
        OptionsPanel.gameObject.SetActive(true);

        controlPanel.SetActive(true);
        volumePanel.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    public void ShowVolumePanel()
    {
        keyboardBtn.image.color = keyboardBtn.colors.normalColor;
        audioBtn.image.color = audioBtn.colors.pressedColor;
        dialogueBtn.image.color = dialogueBtn.colors.normalColor;

        volumePanel.SetActive(true);
        controlPanel.SetActive(false);
        dialoguePanel.SetActive(false);

        SetupVolume();
    }
    public void ShowDialogueControl()
    {
        dialogueBtn.image.color = dialogueBtn.colors.pressedColor;
        audioBtn.image.color = audioBtn.colors.normalColor;
        keyboardBtn.image.color = keyboardBtn.colors.normalColor;

        dialoguePanel.SetActive(true);
        volumePanel.SetActive(false);
        controlPanel.SetActive(false);
    }
    public void ShowControlPanel()
    {
        keyboardBtn.image.color = keyboardBtn.colors.pressedColor;
        audioBtn.image.color = audioBtn.colors.normalColor;
        dialogueBtn.image.color = dialogueBtn.colors.normalColor;

        volumePanel.SetActive(false);
        controlPanel.SetActive(true);
        dialoguePanel.SetActive(false);
    }


    public void SetupVolume()
    {
        volumeControl.bgmControl.value = AudioManager.GetInstance.backgroundMusic.volume;
        volumeControl.sfxControl.value = AudioManager.GetInstance.sfx.volume;
    }

    public void UpdateVolume()
    {
        AudioManager.GetInstance.SetBGMVolume(volumeControl.bgmControl.value);
        AudioManager.GetInstance.SetSFXVolume(volumeControl.sfxControl.value);
    }

    public void CloseOptions()
    {
        InGameOption.CloseWindow();
        InGameOption.gameObject.SetActive(false);
        OptionsPanel.CloseWindow();
        OptionsPanel.gameObject.SetActive(false);
        tabCover.SetActive(false);
        TransitionManager.GetInstance.HideTabCover();
    }

    public void ReturnToMainMenu()
    {
        CloseOptions();
        TransitionManager.GetInstance.LoadScene(SceneType.Opening);
        
    }
}
