using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    public GameObject soundFire;

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
    public float maxDistance = 100f;
    public float spatialBlend = 1f;
    public float dopplerLevel = 0;
    public float delayWalkSound = 0f;
    Dictionary<Sound, float> soundTimerDictionary;

    [Header("BGM musics")]
    public MusicAudioClip[] MusicBGMList;

    [Header("audios enemy movements")]
    public AudioClip[] enemyMovementList;

    [Header("Audioclip list")]
    public SoundAudioClip[] soundAudioClipList;

    private SoundAudioClip currentCreatedClip;
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();

        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.EnemyMove] = 0f;
    }

    #region Sounds
    // To play a sound from another class : SoundManager.PlaySound(Sound.name, position);
    public void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound : " + currentCreatedClip.sound.ToString());
            soundGameObject.transform.position = position;

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = currentCreatedClip.audioClip;

            audioSource.maxDistance = maxDistance;
            audioSource.spatialBlend = spatialBlend;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = dopplerLevel;
            audioSource.volume = currentCreatedClip.volume;

            audioSource.Play();
            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    // Test if a sound can be played or need some time and store it if it exist
    private bool CanPlaySound(Sound sound)
    {
        //Check if the sound exist
        foreach (SoundAudioClip soundAudioClip in soundAudioClipList)
        {
            if (soundAudioClip.sound == sound)
            {
                if (soundAudioClip.audioClip != null)
                {
                    currentCreatedClip = soundAudioClip;

                    // If it exist, check if that sound have a cooldown or not (and is in cooldown or not)
                    switch (sound)
                    {
                        case Sound.EnemyMove:
                            if (soundTimerDictionary.ContainsKey(sound))
                            {
                                float lastTimePlayed = soundTimerDictionary[sound];
                                float playerMoveTimerMax = delayWalkSound;
                                if (lastTimePlayed + playerMoveTimerMax < Time.time)
                                {
                                    soundTimerDictionary[sound] = Time.time;

                                    // Randomize the sound of walk
                                    var random = Random.value;

                                    if (random >= 0.75)
                                    {
                                        currentCreatedClip.audioClip = enemyMovementList[0];
                                    }
                                    if (random >= 0.50 && random < 0.75)
                                    {
                                        currentCreatedClip.audioClip = enemyMovementList[1];
                                    }
                                    if (random >= 0.25 && random < 0.50)
                                    {
                                        currentCreatedClip.audioClip = enemyMovementList[2];
                                    }
                                    if (random < 0.25)
                                    {
                                        currentCreatedClip.audioClip = enemyMovementList[3];
                                    }

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            break;

                        case Sound.none:
                            return false;

                        default:
                            return true;
                    }
                }
                else
                {
                    Debug.LogWarning("AudioClip not found");
                    return false;
                }
            }
        }
        return false;
    }
    #endregion

    #region Musics

    public void ActiveFire()
    {
        soundFire.SetActive(true);
    }

    public void PlayMusic(Music music)
    {
        foreach( MusicAudioClip musicAudioClip in MusicBGMList)
        {
            if (musicAudioClip.music == music && musicAudioClip.audioClip != null)
            {

                if (musicAudioClip.music == Music.defeat || musicAudioClip.music == Music.Win)
                {
                    audioSource.loop = false;
                }

                audioSource.clip = musicAudioClip.audioClip;
                audioSource.Play();
            }
        }
    }

    #endregion
}

// List of all the sounds
public enum Sound
{
    ThrowDisc,
    RecallDisc,
    PlayerGetHit,
    PlayerMovement,
    ExplosionDisc,
    ShockwaveDisc,
    EnemyDamaged,
    EnemyDeath,
    EnemyMove,
    CultistATK,
    TouniATK,
    ShieldGetHit,
    WallGetHit,
    PlayerTeleport,
    SelectCompetence,
    NotEnoughActionPoint,
    statueCrack,
    none,
}



// Group a name and an audioclip
[System.Serializable]
public class SoundAudioClip
{
    public Sound sound;
    public AudioClip audioClip;
    [Range(0,1)]
    public float volume;
}

public enum Music
{
    Menu,
    InGame,
    Win,
    defeat,
    InBoss,
    fireLightUp,
}

// Group a name and an audioclip
[System.Serializable]
public class MusicAudioClip
{
    public Music music;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume;
}
