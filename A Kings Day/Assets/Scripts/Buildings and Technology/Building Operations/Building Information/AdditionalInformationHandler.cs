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

    public void HideOnAddedInfoAction()
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].SetActive(false);
        }

        this.gameObject.SetActive(false);
    }
}
