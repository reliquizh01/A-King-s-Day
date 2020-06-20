using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiCountInformationPanel : InformationPanel
{
    public TextMeshProUGUI titleText;
    public List<TextMeshProUGUI> multiCountPanels;

    public List<int> currentCounts;
    public override void SetMultiCounter(List<int> newCounts, string newTitle = "")
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
