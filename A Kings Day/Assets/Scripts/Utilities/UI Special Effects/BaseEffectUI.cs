using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum SpecialEffectsType
{
    Shaking,
    EasingIn,
    EasingOut,
}
public class BaseEffectUI : MonoBehaviour
{
    public SpecialEffectsType effectType;
}
