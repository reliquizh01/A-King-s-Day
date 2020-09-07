using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace Managers
{
    public enum SoundEffectsType
    {
        monoHover1,
    }

    public enum BackgroundMusicType
    {
        openingTheme,
        battlefieldPreparation1,
        courtroomDrama,
        DefeatInBattle,
        WinInBattle,
        BalconyVibes,
    }

    [System.Serializable]
    public class SpecialEffectsClass
    {
        public SoundEffectsType sfxType;
        public AudioClip myAudioClip;
    }
    [System.Serializable]
    public class BackgroundMusicClass
    {
        public BackgroundMusicType bgmType;
        public AudioClip myAudioClip;
    }

    public class AudioManager : BaseManager
    {
        #region Singleton
        private static AudioManager instance;
        public static AudioManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if (AudioManager.GetInstance == null)
            {
                if (transform.parent == null)
                {
                   DontDestroyOnLoad(this.gameObject);
                }
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        public AudioClip nextClip;
        public bool fadeForNextAudio;
        public AudioSource backgroundMusic;
        public float curBgmVol;
        public AudioSource sfx;
        public float curSfxVol;
        public List<BackgroundMusicClass> bgmList;
        public List<SpecialEffectsClass> sfxList;

        public void Start()
        {
            curBgmVol = backgroundMusic.volume;
            curSfxVol = sfx.volume;
        }
        public void PlayDecisionHover()
        {
            sfx.Play();
        }

        public void Update()
        {
            if(fadeForNextAudio)
            {
                if(nextClip != null)
                {
                    backgroundMusic.volume -= 0.002f;
                    if(backgroundMusic.volume <= 0)
                    {
                        backgroundMusic.clip = nextClip;
                        backgroundMusic.Play();
                        nextClip = null;
                    }
                }
                else
                {
                    backgroundMusic.volume += 0.002f;
                    if(backgroundMusic.volume >= curBgmVol)
                    {
                        backgroundMusic.volume = curBgmVol;
                        fadeForNextAudio = false;
                    }
                }
            }
        }
        public override void PlayThisBackGroundMusic(BackgroundMusicType thisType)
        {
            if(bgmList.Find(x => x.bgmType == thisType) != null)
            {
                nextClip = bgmList.Find(x => x.bgmType == thisType).myAudioClip;
                fadeForNextAudio = true;
            }
        }

        public void SetBGMVolume(float newValue)
        {
            curBgmVol = newValue;
            backgroundMusic.volume = curBgmVol;
        }

        public void SetSFXVolume(float newValue)
        {
            curSfxVol = newValue;
            sfx.volume = curSfxVol;
        }
        public void SetVolumeAsBackground()
        {
            backgroundMusic.volume = 0.0005f;
        }

        public void NormalizeBackgroundVolume()
        {
            backgroundMusic.volume = curBgmVol;
        }
    }

}