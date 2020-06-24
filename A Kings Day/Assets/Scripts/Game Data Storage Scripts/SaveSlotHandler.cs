using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using Kingdoms;
using SaveData;
using TMPro;
using UnityEngine.UI;

public class SaveSlotHandler : MonoBehaviour
{
    public GameObject savePanelPrefab;
    public GameObject savePanelParents;

    public List<SaveSlotPanel> savePanelList;

    public TextMeshProUGUI nokingdomText;
    public Scrollbar scrollBar;


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

        nokingdomText.gameObject.SetActive(false);
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
            }
        }
    }
}
