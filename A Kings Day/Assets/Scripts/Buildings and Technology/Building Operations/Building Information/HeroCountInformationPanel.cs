using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Characters;

public class HeroCountInformationPanel : InformationPanel
{
    public Image firstSkill, secondSkill;

    public TextMeshProUGUI healthText, damageText, speedText, heroNameText, attackTypeText;

    public List<int> healthWithGrowth;
    public List<int> damageWithGrowth;
    public List<int> speedWithGrowth;

    public override void SetHeroCounter(List<int> healthCount, List<int> damageCount, List<int> speedCount, UnitAttackType attackType,string heroName = "")
    {

        base.SetHeroCounter(healthCount, damageCount, speedCount, attackType, heroName);

        healthWithGrowth = healthCount;
        damageWithGrowth = damageCount;
        speedWithGrowth = speedCount;

        attackTypeText.text = attackType.ToString();
        // HEALTH SPEED DAMAGE HERO NAME INFORMATIOn
        healthText.text = healthCount[0] + "/" + healthCount[0] + "<color=green>[" + healthCount[1] + "] </color>";
        damageText.text = damageCount[0] + "<color=green>[" + damageCount[1] + "] </color>";
        speedText.text = speedCount[0] + "<color=green>[" + speedCount[1] + "] </color>";

        heroNameText.text = heroName;
    }

    public override void ResetCounter()
    {
        base.ResetCounter();

        healthText.text = "??";
        damageText.text = "??";
        speedText.text = "??";
        heroNameText.text = "- No One -";
        attackTypeText.text = "??";
    }
}
