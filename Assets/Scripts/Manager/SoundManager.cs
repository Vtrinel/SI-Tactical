using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    [Header("Parameters")]
    [SerializeField] static float maxDistance = 100f;
    [SerializeField] static float spatialBlend = 1f;
    [SerializeField] static float dopplerLevel = 0;

    [Header("BGM music")]
    [SerializeField] float BGMFadeSpead;
    [SerializeField] AudioClip BGM;

    [Header("Audioclip list")]
    public SoundAudioClip[] soundAudioClipList;

    // To play a sound from another class : SoundManagerScript.PlaySound("NameOfTheSound");
     public static void PlaySound(Sound sound, Vector3 position)
     {
         if (canPlaySound(sound))
         {
             GameObject soundGameObject = new GameObject("sound");
             soundGameObject.transform.position = position;
             AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
             audioSource.clip = GetAudioClip(sound);
             audioSource.maxDistance = maxDistance;
             audioSource.spatialBlend = spatialBlend;
             audioSource.rolloffMode = AudioRolloffMode.Linear;
             audioSource.dopplerLevel = dopplerLevel;
             audioSource.Play();

             Object.Destroy(soundGameObject, audioSource.clip.length);
         }
     }
     
    private AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in soundAudioClipList)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

            return null;
    }


    public enum Sound
    {
        PlayerMove,
        PlayerAttack,
        EnnemyHit,
        EnnemyDie,
        GainGold
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
