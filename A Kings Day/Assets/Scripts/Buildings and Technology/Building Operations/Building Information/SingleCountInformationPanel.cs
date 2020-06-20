using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleCountInformationPanel : InformationPanel
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI descriptionText;

    private int count;

    public override void ShowGrowth(int optionalGrowth = 0)
    {
        base.ShowGrowth(optionalGrowth);
        if(optionalGrowth > 0)
        {
            countText.text = ObtainCountText((count + optionalGrowth));
        }
        else
        {
            countText.text = ObtainCountText((count + 1));
        }
        countText.color = Color.green;

    }
    public override void SetSingleCounter(int newCount, string newDescription, string newTitle = "")
    {
        base.SetSingleCounter(newCount, newDescription, newTitle);

        titleText.text = newTitle;
        countText.text = ObtainCountText(newCount);
        descriptionText.text = newDescription;
    }
}
