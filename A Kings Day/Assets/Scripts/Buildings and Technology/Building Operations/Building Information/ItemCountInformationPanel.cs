using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;
using UnityEngine.UI;
using GameItems;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ItemCountInformationPanel : InformationPanel
{
    public TextMeshProUGUI merchantNameText;

    public TextMeshProUGUI itemNameText;

    [Header("Equipment")]
    public GameObject equipmentPanel;
    public List<TextMeshProUGUI> equipmentCounterList;

    [Header("Resource Item")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;


    public override void SetPanelIcon(Sprite newSprite)
    {
        panelIcon.gameObject.SetActive(true);
        base.SetPanelIcon(newSprite);

    }
    public override void SetFlexibleCounter(ItemInformationData itemInformation, string newTitle = "")
    {
        base.SetFlexibleCounter(itemInformation, newTitle);

        switch (itemInformation.ItemType)
        {
            case ItemType.Resources:
                equipmentPanel.SetActive(false);
                descriptionPanel.SetActive(true);

                descriptionText.text = itemInformation.itemDescription;
                break;
            case ItemType.Equipment:
                equipmentPanel.SetActive(true);
                descriptionPanel.SetActive(false);

                equipmentCounterList[0].text = itemInformation.health.ToString();
                equipmentCounterList[1].text = itemInformation.damage.ToString();
                equipmentCounterList[2].text = itemInformation.speed.ToString();
                equipmentCounterList[3].text = itemInformation.durability.ToString();
                break;
            default:
                break;
        }
    }

    public override void ResetCounter()
    {
        base.ResetCounter();
        equipmentPanel.SetActive(false);
        descriptionPanel.SetActive(true);

        descriptionText.text = "Empty.";
        merchantNameText.text = "- No One -";
        itemNameText.text = "- Empty -";
        panelIcon.gameObject.SetActive(false);
    }
}
