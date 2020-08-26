using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public enum CountType
{
    Number,
    Percentage,
}
[RequireComponent(typeof(EventTrigger))]
public class GrowthCountInformationPanel : InformationPanel
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI growthText;

    private int count, growth;
    public override void SetGrowthCounter(int newCount, int newGrowth, string newDescription, string newTitle = "")
    {
        base.SetGrowthCounter(newCount, newGrowth, newDescription, newTitle);

        count = newCount;
        growth = newGrowth;

        titleText.text = newTitle;
        countText.text = newCount.ToString();
        descriptionText.text = newDescription;
        growthText.text = newGrowth.ToString();
    }

    public override void UpdateCount(int newCount)
    {
        base.UpdateCount(newCount);
        count = newCount;
        countText.text = newCount.ToString();
    }
    public override void ShowGrowth(int optionalGrowth = 0)
    {
        base.ShowGrowth(optionalGrowth);

        countText.text = (count + growth).ToString();
        countText.color = Color.green;
    }
}
