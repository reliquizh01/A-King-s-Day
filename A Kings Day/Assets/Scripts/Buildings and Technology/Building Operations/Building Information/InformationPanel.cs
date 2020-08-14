using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using TMPro;
using UnityEngine.UI;
using GameItems;
using Characters;

public enum PanelType
{
    SingleCounter,
    GrowthCounter,
    MultiCounter,
    FlexibleCounter,
    HeroCounter,
}
public class InformationPanel : MonoBehaviour
{
    public PanelType panelType;
    public CountType countType;
    public Image panelIcon;

    public virtual void SetPanelIcon(Sprite newSprite)
    {
        panelIcon.sprite = newSprite;
    }
    public virtual void SetHeroSkillIcons(Sprite firstSkill, Sprite secondSkill)
    {

    }

    public virtual void SetSingleCounter(int newCount, string newDescription, string newTitle = "")
    {

    }

    public virtual void SetGrowthCounter(int newCount, int newGrowth,string newDescription, string newTitle = "")
    {

    }

    public virtual void SetMultiCounter(List<float> newCounts, string newTitle = "")
    {

    }

    public virtual void SetHeroCounter(List<int> healthCount, List<int> damageCount, List<int> speedCount, UnitAttackType attackType, string heroName = "")
    {

    }
    public virtual void ShowGrowth(int optionalGrowth = 0)
    {

    }
    public virtual void UpdateCount(int newCount)
    {

    }
    public virtual void UpdateCounts(List<int> newCounts)
    {

    }

    public virtual void SetFlexibleCounter(ItemInformationData itemInformation, string newTitle = "")
    {

    }

    public virtual string ObtainCountText(int amount)
    {
        string tmp = amount.ToString();
        switch (countType)
        {
            case CountType.Percentage:
                tmp = amount.ToString() + "%";
                break;

            case CountType.Number:
            default:
                break;
        }
        return tmp;
    }

    public virtual string ObtainCountText(float amount)
    {
        string tmp = amount.ToString();
        switch (countType)
        {
            case CountType.Percentage:
                tmp = amount.ToString() + "%";
                break;

            case CountType.Number:
            default:
                break;
        }
        return tmp;
    }

    public virtual void ResetCounter()
    {

    }
}
