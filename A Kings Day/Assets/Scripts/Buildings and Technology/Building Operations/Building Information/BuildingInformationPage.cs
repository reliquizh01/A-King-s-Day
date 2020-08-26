using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using UnityEngine.EventSystems;

public class BuildingInformationPage : MonoBehaviour
{

    public bool hasUniqueRequirements = false;
    public List<InformationPanel> informationPanelList;

    public void InitializeInformationPanels(InformationActionHandler revealer)
    {
        for (int i = 0; i < informationPanelList.Count; i++)
        {
            informationPanelList[i].myController = revealer;
            informationPanelList[i].localIdx = i;
        }
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
