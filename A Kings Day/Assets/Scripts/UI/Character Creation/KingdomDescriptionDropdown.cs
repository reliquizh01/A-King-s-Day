using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KingdomDescriptionDropdown : MonoBehaviour
{
    public KingdomCreationUiV2 myController;
    public TMP_Dropdown dropDown;

    public List<string> descriptionOption;

    public void Start()
    {
        SetupDropDownOptions();
    }
    public void SetupDropDownOptions()
    {
        dropDown.AddOptions(descriptionOption);
    }

    public void UpdateFamilyOrigin()
    {
        myController.AdjustFamilyOrigin(this);
    }
}
