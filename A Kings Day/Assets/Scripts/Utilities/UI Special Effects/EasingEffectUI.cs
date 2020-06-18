using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class EasingEffectUI : BaseEffectUI
{
    public RectTransform myRect;
    public SpecialEffectsType endEffectType;
    public float easeSpeed;
    public bool startEasing = false;
    public Vector2 targetPoint;
    public Vector2 startPoint;

    public void Start()
    {
        startPoint = new Vector2(myRect.offsetMax.x,myRect.offsetMin.y);
    }

    public void Update()
    {
        if(startEasing)
        {
            myRect.offsetMax += new Vector2(0, easeSpeed * Time.deltaTime);
            myRect.offsetMin += new Vector2(0, easeSpeed * Time.deltaTime);

            if(myRect.offsetMax == new Vector2(0,targetPoint.x) && myRect.offsetMin == new Vector2(0, targetPoint.y))
            {
                startEasing = false;
            }
        }
    }

    public void StartEasingIn()
    {
        startEasing = true;
    }

}
