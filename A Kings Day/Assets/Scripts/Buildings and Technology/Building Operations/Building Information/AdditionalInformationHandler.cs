using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Buildings;

public class AdditionalInformationHandler : MonoBehaviour
{
    public TextMeshProUGUI addedInfoText;

    public List<GameObject> actionButtons;
    

    public void ShowOnAddedInfoAction(int idx, CardActiondata thisDecision)
    {
        addedInfoText.text = thisDecision.AdditionalInformationMessage;
        actionButtons[idx].SetActive(true);
    }

    public void ShowOnAddedInfoPanels(int idx, string additionalInformation)
    {
        HideOnAddedInfoAction(false);
        addedInfoText.text = additionalInformation;

    }
    public void HideOnAddedInfoAction(bool hideWhole = true)
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].SetActive(false);
        }

        if(hideWhole)
        {
            this.gameObject.SetActive(false);
        }
    }
}
