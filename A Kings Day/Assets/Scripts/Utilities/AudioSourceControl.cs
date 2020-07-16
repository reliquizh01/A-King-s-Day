using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public enum AudioPlayType
{
    Normal,
    SlowlyFade,
    Loop,
}
[RequireComponent(typeof(AudioSource))]
public class AudioSourceControl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioPlayType playType;
    public float initialVolume = 0.5f;
    
    public float curDelayCount = 0.0f;
    public float delayPlay = 0.0f;
    public bool delayOn = false;
    public bool isPlaying = false;

    public void Start()
    {
        if(AudioManager.GetInstance != null)
        {
            initialVolume = AudioManager.GetInstance.sfx.volume;
        }
        audioSource.volume = initialVolume;

        if(delayPlay > 0)
        {
            delayOn = true;
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            PlayAudio();
        }

        if(isPlaying)
        {
            if(delayOn)
            {
                curDelayCount += Time.deltaTime;
                if(curDelayCount > delayPlay)
                {
                    audioSource.Play();
                    delayOn = false;
                }
            }
            else
            {
                switch (playType)
                {
                    case AudioPlayType.SlowlyFade:
                        UpdateSlowlyFade();
                        break;
                    case AudioPlayType.Loop:
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void PlayAudio(AudioClip newClip = null)
    {
        isPlaying = true;
        audioSource.volume = initialVolume;

        switch (playType)
        {
            case AudioPlayType.SlowlyFade:
                InitializeSlowlyFade();
                break;
            case AudioPlayType.Loop:
                InitializeLoop();
                break;
            default:
                break;
        }
        if(newClip != null)
        {
            audioSource.clip = newClip;
        }

        if(delayPlay <= 0)
        {
            audioSource.Play();
        }
    }
    public void InitializeSlowlyFade()
    {
        audioSource.volume = initialVolume;
    }
    public void InitializeLoop()
    {
        audioSource.loop = true;
    }

    public void UpdateSlowlyFade()
    {
        if(audioSource.volume > 0.001f)
        {
            audioSource.volume -= 0.002f;
        }
        else
        {
            isPlaying = false;
        }
    }
}
