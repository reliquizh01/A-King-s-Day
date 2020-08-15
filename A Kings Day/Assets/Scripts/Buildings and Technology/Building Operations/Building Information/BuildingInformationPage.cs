using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;

public class BuildingInformationPage : MonoBehaviour
{
    public bool hasUniqueRequirements = false;
    public List<InformationPanel> informationPanelList;

    public void HighlightThisPanel(int idx)
    {

    }

    public virtual bool HasMetRequirements()
    {
        return false;
    }
    public virtual void ImplementPageAction(int idx)
    {

    }

    public virtual int ObtainRewardMultiplier()
    {
        return 1;
    }
}
