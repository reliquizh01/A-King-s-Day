using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class OptionsController : MonoBehaviour
{
    public GameObject InGameOption;
    public GameObject OptionsPanel;

    public GameObject volumePanel;
    public GameObject controlPanel;

    public VolumeSliderController volumeControl;


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(TransitionManager.GetInstance.currentSceneManager.sceneType == SceneType.Opening)
            {
                return;
            }

            TransitionManager.GetInstance.ShowOptions(false);
        }
    }
    public void OpenInGameOptions()
    {
        InGameOption.SetActive(true);
    }

    public void OpenOptionPanel()
    {
        OptionsPanel.SetActive(true);
        volumePanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void ShowVolumePanel()
    {
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
        AudioManager.GetInstance.backgroundMusic.volume = volumeControl.bgmControl.value;
        AudioManager.GetInstance.sfx.volume = volumeControl.sfxControl.value;
    }
    public void ShowControlPanel()
    {
        volumePanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        InGameOption.SetActive(false);
        OptionsPanel.SetActive(false);
        TransitionManager.GetInstance.HideTabCover();
    }

    public void ReturnToMainMenu()
    {
        CloseOptions();
        TransitionManager.GetInstance.LoadScene(SceneType.Opening);
    }
}
