using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace Managers
{
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



        public void PlayDecisionHover()
        {
            sfx.Play();
        }
    }

}