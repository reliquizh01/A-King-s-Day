using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    }

}