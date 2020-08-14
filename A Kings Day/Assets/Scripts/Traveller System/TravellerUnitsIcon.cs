using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TravellerUnitsIcon : MonoBehaviour
{
    [Header("Default Icon")]
    public Sprite unknownImage;
    [Header("Icon List")]
    public List<Image> unitsIcons;


    public void SetAsUnknownIcon(int idx)
    {
        ShowIcon(idx);
        unitsIcons[idx].sprite = unknownImage;
    }

    public void SetAsNewicon(int idx,Sprite iconName)
    {
        ShowIcon(idx);
        unitsIcons[idx].sprite = iconName;
    }

    public void ShowIcon(int idx)
    {
        unitsIcons[idx].gameObject.SetActive(true);
    }
    public void HideIcon(int idx)
    {
        unitsIcons[idx].gameObject.SetActive(false);
    }
}
