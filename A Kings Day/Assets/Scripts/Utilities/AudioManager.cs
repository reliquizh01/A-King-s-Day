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
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        public AudioSource backgroundMusic;
        public AudioSource sfx;

        public List<BackgroundMusicClass> bgmList;
        public List<SpecialEffectsClass> sfxList;

        public void PlayDecisionHover()
        {
            sfx.Play();
        }

        public override void PlayThisBackGroundMusic(BackgroundMusicType thisType)
        {
            if(bgmList.Find(x => x.bgmType == thisType) != null)
            {
                backgroundMusic.clip = bgmList.Find(x => x.bgmType == thisType).myAudioClip;
                backgroundMusic.Play();
            }
        }
    }

}