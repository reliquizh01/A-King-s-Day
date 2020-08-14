using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiCountInformationPanel : InformationPanel
{
    public TextMeshProUGUI titleText;
    public List<TextMeshProUGUI> multiCountPanels;

    public List<float> currentCounts;
    public override void SetMultiCounter(List<float> newCounts, string newTitle = "")
    {
        base.SetMultiCounter(newCounts, newTitle);
        currentCounts = newCounts;

        titleText.text = newTitle;

        for (int i = 0; i < currentCounts.Count; i++)
        {
            multiCountPanels[i].text = ObtainCountText(currentCounts[i]);
        }
    }
}
