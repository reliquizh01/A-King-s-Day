using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using Kingdoms;
using TMPro;
using UnityEngine.UI;

public class SaveSlotHandler : MonoBehaviour
{
    public GameObject savePanelPrefab;
    public GameObject savePanelParents;

    public List<SaveSlotPanel> savePanelList;
    public SaveSlotPanel currentPanel;

    public TextMeshProUGUI nokingdomText;
    public Scrollbar scrollBar;

    public Button loadBtn, deleteBtn;

    public int selectedIndex;
    public void Start()
    {
        scrollBar.value = 0;
    }
    public void SetSavePanels(List<PlayerKingdomData> dataFiles)
    {
        for (int i = 0; i < dataFiles.Count; i++)
        {
            SetupThisPanel(dataFiles[i], i);
        }

        SwitchButtonInteraction(false);
        nokingdomText.gameObject.SetActive(false);
    }

    public void UpdatePanels(List<PlayerKingdomData> dataFiles)
    {
        Debug.Log("COUNT : " + dataFiles.Count);

        if(dataFiles == null || dataFiles.Count <= 0)
        {
            for (int i = 0; i < savePanelList.Count; i++)
            {
                savePanelList[i].gameObject.SetActive(false);
            }

            SwitchButtonInteraction(false);
            nokingdomText.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < savePanelList.Count; i++)
            {
                if(i < dataFiles.Count)
                {
                    SetupThisPanel(dataFiles[i], i);
                }
                else
                {
                    savePanelList[i].gameObject.SetActive(false);
                }
            }
            ResetPanels();
        }
    }
    public void SwitchButtonInteraction(bool switchTo)
    {
        loadBtn.interactable = switchTo;
        deleteBtn.interactable = switchTo;
    }

    public void SetupThisPanel(PlayerKingdomData thisData,int idx)
    {
        savePanelList[idx].gameObject.SetActive(true);

        savePanelList[idx].kingdomName.text = thisData.kingdomsName;

        savePanelList[idx].coinsCount.text = thisData.coins.ToString();
        savePanelList[idx].populationCount.text = thisData.population.ToString();
        savePanelList[idx].foodCount.text = thisData.foods.ToString();
        savePanelList[idx].troopsCount.text = thisData.GetTotalTroops.ToString();
        savePanelList[idx].weeksCount.text = thisData.weekCount.ToString();
        savePanelList[idx].myController = this;
    }

    public void SetAsSelectedPanel(SaveSlotPanel thisPanel)
    {
        selectedIndex = savePanelList.IndexOf(thisPanel);
        for (int i = 0; i < savePanelList.Count; i++)
        {
            if(thisPanel != savePanelList[i])
            {
                savePanelList[i].myBg.sprite = savePanelList[i].normalSprite;
            }
            else
            {
                savePanelList[i].myBg.sprite = savePanelList[i].highlightSprite;
                currentPanel = savePanelList[i];
                selectedIndex = i;
                SwitchButtonInteraction(true);
            }
        }
    }
    public void ResetPanels()
    {
        for (int i = 0; i < savePanelList.Count; i++)
        {
            savePanelList[i].myBg.sprite = savePanelList[i].normalSprite;
        }
        SwitchButtonInteraction(false);
    }
}
