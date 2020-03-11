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
    [SerializeField] Dictionary<Sound, float> soundTimerDictionary;

    //[Header("BGM music")]
    //[SerializeField] float BGMFadeSpead;
    //[SerializeField] AudioClip BGM;

    [Header("Audioclip list")]
    public SoundAudioClip[] soundAudioClipList;


    public void Initialized()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerMove] = 0f;
    }

    // To play a sound from another class : SoundManagerScript.PlaySound("NameOfTheSound");
    public void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
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

    // Test if a sound can be played or need some time
    private bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            // Sound that shouldn't be repeated to fast
            case Sound.PlayerMove:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 0.05f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;

            default:
                return true;
        }
        return true;
    }

    // Get the sound given from the list of stored sound
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


    // List of all the sounds
    public enum Sound
    {
        PlayerMove,
        PlayerAttack,
        EnnemyHit,
        EnnemyDie,
        GainGold
    }

    // Group a name and an audioclip
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
