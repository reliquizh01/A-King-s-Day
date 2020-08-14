using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;


namespace Characters
{

    public enum SoundEffectsType
    {
        SendDamage,
        TalkCalmly,
        ReceiveDamage,
    }
    [System.Serializable]
    public class ObjectAudioGenerator
    {
        public List<AudioClip> sfxClipList;
    }
    public class CharacterAudioControl : MonoBehaviour
    {
        public AudioSource audioSource;
        public ObjectAudioGenerator sendDamageList;
        public ObjectAudioGenerator receiveProjectileList;
        public ObjectAudioGenerator receiveMeleeList;
        public ObjectAudioGenerator receiveDeathInjuredList;

        public ObjectAudioGenerator projectileBlockList;
        public ObjectAudioGenerator meleeBlockList;

        public void Start()
        {
            if(AudioManager.GetInstance != null)
            {
                if(audioSource != null)
                {
                    audioSource.volume = AudioManager.GetInstance.sfx.volume;
                }
            }
        }
        public void PlaySendDamageAudio()
        {
            int rand = UnityEngine.Random.Range(0, sendDamageList.sfxClipList.Count - 1);

            audioSource.clip = sendDamageList.sfxClipList[rand];
            audioSource.Play();
        }

        public void PlayReceiveProjectile()
        {
            int rand = UnityEngine.Random.Range(0, receiveProjectileList.sfxClipList.Count - 1);

            audioSource.clip = receiveProjectileList.sfxClipList[rand];
            audioSource.Play();
        }

        public void PlayReceiveMelee()
        {
            int rand = UnityEngine.Random.Range(0, receiveMeleeList.sfxClipList.Count - 1);

            audioSource.clip = receiveMeleeList.sfxClipList[rand];
            audioSource.Play();
        }

        public void PlayInjuredOrDead()
        {
            int rand = UnityEngine.Random.Range(0, receiveDeathInjuredList.sfxClipList.Count - 1);

            audioSource.clip = receiveDeathInjuredList.sfxClipList[rand];
            audioSource.Play();
        }

        public void PlayBlockProjectile()
        {
            int rand = UnityEngine.Random.Range(0, projectileBlockList.sfxClipList.Count - 1);

            audioSource.clip = projectileBlockList.sfxClipList[rand];
            audioSource.Play();
        }

        public void PlayBlockMelee()
        {
            int rand = UnityEngine.Random.Range(0, meleeBlockList.sfxClipList.Count - 1);

            audioSource.clip = meleeBlockList.sfxClipList[rand];
            audioSource.Play();
        }
    }

}