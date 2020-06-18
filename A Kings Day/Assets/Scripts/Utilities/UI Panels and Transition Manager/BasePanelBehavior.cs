﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

[RequireComponent(typeof(Animation))]
public class BasePanelBehavior : MonoBehaviour
{
    public Animation myAnim;
    public string openAnimationName;
    public string closeAnimationName;

    public bool playOpeningOnAwake = false;
    public bool isPlaying = false;
    public void Awake()
    {
        myAnim = this.GetComponent<Animation>();
        if (playOpeningOnAwake)
        {
            PlayOpenAnimation();
        }
    }

    public void PlayOpenAnimation()
    {
        //Debug.Log(this.gameObject.name);
        if (string.IsNullOrEmpty(openAnimationName))
        {
            return;
        }
        if (!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
        }

        myAnim.Play(openAnimationName);
        isPlaying = true;
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(CallTransitionOff());
        }
    }

    public void PlayCloseAnimation()
    {
        if (string.IsNullOrEmpty(closeAnimationName))
        {
            return;
        }
        myAnim.Play(closeAnimationName);
        isPlaying = true;

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(ClosePanel());
        }
    }

    public IEnumerator CallTransitionOff()
    {
        yield return new WaitForSeconds(myAnim.GetClip(openAnimationName).length);

    }
    public void PlayCloseAnimation(bool closePanel = false)
    {
        if (string.IsNullOrEmpty(closeAnimationName))
        {
            return;
        }
        myAnim.Play(closeAnimationName);

        if (closePanel)
        {
            Debug.Log("Me");
            StartCoroutine(ClosePanel());
        }
    }

    public IEnumerator ClosePanel()
    {
        if (string.IsNullOrEmpty(closeAnimationName))
        {
            this.gameObject.SetActive(false);
            yield return null;
        }
        yield return new WaitForSeconds(myAnim.GetClip(closeAnimationName).length);

        isPlaying = false;
        this.gameObject.SetActive(false);
    }

    public IEnumerator WaitAnimationForAction(string animName, Action callBack = null)
    {
        if(!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
        }

        if (myAnim.GetClip(animName) == null)
        {
          //  Debug.Log("Animation : " + animName + " is null");
            yield return null;
        }
        //Debug.Log("Start Playing : " + animName);
        myAnim.Play(animName);
        isPlaying = true;
        yield return new WaitForSeconds(myAnim.GetClip(animName).length);


        isPlaying = false;
        if (callBack != null)
        {
            callBack();
        }
    }
}
