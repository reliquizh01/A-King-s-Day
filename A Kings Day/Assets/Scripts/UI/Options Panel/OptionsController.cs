using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{

    public Button audioBtn, keyboardBtn;
    public BasePanelWindow InGameOption;
    public BasePanelWindow OptionsPanel;

    public GameObject volumePanel;
    public GameObject controlPanel;

    public VolumeSliderController volumeControl;

    public void Start()
    {
        ShowControlPanel();
    }
    public void OpenInGameOptions()
    {
        InGameOption.gameObject.SetActive(true);
    }

    public void OpenOptionPanel()
    {
        OptionsPanel.gameObject.SetActive(true);
        volumePanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void ShowVolumePanel()
    {
        keyboardBtn.image.color = keyboardBtn.colors.normalColor;
        audioBtn.image.color = audioBtn.colors.pressedColor;
        volumePanel.SetActive(true);
        controlPanel.SetActive(false);

        SetupVolume();
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
    public void ShowControlPanel()
    {
        keyboardBtn.image.color = keyboardBtn.colors.pressedColor;
        audioBtn.image.color = audioBtn.colors.normalColor;
        volumePanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        InGameOption.gameObject.SetActive(false);
        OptionsPanel.gameObject.SetActive(false);
        TransitionManager.GetInstance.HideTabCover();
    }

    public void ReturnToMainMenu()
    {
        CloseOptions();
        TransitionManager.GetInstance.LoadScene(SceneType.Opening);
        
    }
}
