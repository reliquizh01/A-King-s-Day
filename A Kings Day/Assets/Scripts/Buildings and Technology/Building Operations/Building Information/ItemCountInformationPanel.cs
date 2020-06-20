using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;
using UnityEngine.UI;
using GameItems;

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


    public override void SetFlexibleCounter(ItemInformationData itemInformation, string newTitle = "")
    {
        base.SetFlexibleCounter(itemInformation, newTitle);

    }
}
