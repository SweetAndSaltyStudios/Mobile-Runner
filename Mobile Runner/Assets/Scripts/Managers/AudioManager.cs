using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singelton<AudioManager>
{
    #region VARIABLES

    public AudioMixer AudioMixer;

    public AudioClip[] MusicTracks;
    public AudioClip[] SoundEffects;

    private AudioSource musicTrackSource;

    #endregion VARIABLES

    #region PROPERTIES



    #endregion PROPERTIES

    #region UNITY_FUNCTIONS

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        PlayMusickTrack();
    }

    #endregion UNITY_FUNCTIONS

    #region CUSTOM_FUNCTIONS

    private void Initialize()
    {
        musicTrackSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    private void PlayMusickTrack()
    {
        musicTrackSource.clip = MusicTracks[Random.Range(0, MusicTracks.Length)];
        musicTrackSource.loop = true;
        musicTrackSource.Play();
    }

    public void PlaySfxAtPoint(string effectName, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(GetSoundEffect(effectName), position);
    }

    private AudioClip GetSoundEffect(string effectName)
    {
        for (int i = 0; i < SoundEffects.Length; i++)
        {
            if (SoundEffects[i].name == effectName)
            {
                return SoundEffects[i];
            }
        }

        Debug.LogError("!!!");
        return SoundEffects[0];
    }

    #endregion CUSTOM_FUNCTIONS

    #region COROUTINES



    #endregion COROUTINES
}
