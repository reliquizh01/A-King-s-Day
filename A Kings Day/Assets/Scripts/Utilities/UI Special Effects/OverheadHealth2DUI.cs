using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class OverheadHealth2DUI : MonoBehaviour
{

    public Transform mTransform;
    public Transform mTextOverTransform;

    [Header("Health Mechanics")]
    public bool showHealthbar = false;
    public GameObject healthBarParent;

    [Header("Health Counter")]
    public float incrementSpeed = 0.0075f;

    public bool initialBarUpdate;
    public Image mHealthBarOverHead;

    public bool secondBarUpdate;
    public Image mHealthAfterDamageOverhead;

    public float curHealthCount;
    public float targetHealthCount;

    public bool isReceivingDamage = false;
    public float afterDamageDelayCounter = 0;
    public float delayOffset = 1.5f;
    void Awake()
    {
        mTransform = transform;
        if(!showHealthbar)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if(initialBarUpdate)
        {
            if(mHealthBarOverHead.fillAmount < targetHealthCount)
            {
                mHealthBarOverHead.fillAmount += incrementSpeed;

                if(mHealthBarOverHead.fillAmount >= targetHealthCount)
                {
                    mHealthBarOverHead.fillAmount = targetHealthCount;
                    initialBarUpdate = false;
                }
            }
            else if (mHealthBarOverHead.fillAmount > targetHealthCount)
            {
                mHealthBarOverHead.fillAmount -= incrementSpeed;

                if (mHealthBarOverHead.fillAmount <= targetHealthCount)
                {
                    mHealthBarOverHead.fillAmount = targetHealthCount;
                    initialBarUpdate = false;
                }
            }
        }
        else if(!initialBarUpdate && isReceivingDamage)
        {
            afterDamageDelayCounter += Time.deltaTime;
            if(afterDamageDelayCounter >= delayOffset)
            {
                isReceivingDamage = false;
                secondBarUpdate = true;
            }
        }
        else if(!initialBarUpdate && !isReceivingDamage && secondBarUpdate)
        {
            if (mHealthAfterDamageOverhead.fillAmount < targetHealthCount)
            {
                mHealthAfterDamageOverhead.fillAmount += incrementSpeed;

                if (mHealthAfterDamageOverhead.fillAmount >= targetHealthCount)
                {
                    mHealthAfterDamageOverhead.fillAmount = targetHealthCount;
                    secondBarUpdate = false;
                }
            }
            else if (mHealthAfterDamageOverhead.fillAmount > targetHealthCount)
            {
                mHealthAfterDamageOverhead.fillAmount -= incrementSpeed;

                if (mHealthAfterDamageOverhead.fillAmount <= targetHealthCount)
                {
                    mHealthAfterDamageOverhead.fillAmount = targetHealthCount;
                    secondBarUpdate = false;
                }
            }

            if (mHealthAfterDamageOverhead.fillAmount <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    void LateUpdate()
    {
        if (showHealthbar)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(mTransform.position);
            // add a tiny bit of height?
            screenPos.y += 2; // adjust as you see fit.
            mTextOverTransform.position = screenPos;
        }
    }

    public void UpdateHealthBar(float newCurHealth, float maxhealth, bool damageReceive = false)
    {
        curHealthCount = newCurHealth;

        if(curHealthCount < 0)
        {
            curHealthCount = 0;
        }

        targetHealthCount = curHealthCount / maxhealth;

        initialBarUpdate = true;
        isReceivingDamage = damageReceive;

        if(isReceivingDamage)
        {
            afterDamageDelayCounter = 0;
        }
    }
    public void SetupHealthBar(float curHealth, float maxHealth)
    {
        showHealthbar = true;
        curHealthCount = curHealth;

        mHealthBarOverHead.fillAmount = curHealth / maxHealth;
        mHealthAfterDamageOverhead.fillAmount = curHealth / maxHealth;
    }
    public void HideHealthBar()
    {
        showHealthbar = false;
    }
}
