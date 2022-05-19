using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Music & SFX Enums
        public enum MusicTrack
        {
            TitleMusic,
            Level1,
            BossMusic
        }
    #endregion

    #region Singleton Stuff
        public static AudioManager Instance
        {
            get
            {
                return instance;
            }

            set
            {
                if(instance == null)
                    instance = value;
                else
                    Destroy(value);
            }
        }
        private static AudioManager instance;
    #endregion

    #region Event Paths
        //Music & SFX
        [Header("Music Tracks")]
        [SerializeField][FMODUnity.EventRef] private string titleMusic;
        [SerializeField][FMODUnity.EventRef] private string level1MusicTrack;
        [SerializeField][FMODUnity.EventRef] private string bossMusicTrack;

        //VCAs
        private const string masterVCAPath = "vca:/Master";
        private const string musicVCAPath = "vca:/Music";
        private const string sfxVCAPath = "vca:/SFX";
    #endregion

    #region FMOD Instances
        //Events
        private FMOD.Studio.EventInstance musicInstance;

        //VCAs
        private FMOD.Studio.VCA masterVCA;
        private FMOD.Studio.VCA musicVCA;
        private FMOD.Studio.VCA sfxVCA;
    #endregion

    #region Private Properties
        [Header("Volume Levels")]
        [SerializeField] [Range(0, 1)] private float masterVolume = 1;
        [SerializeField] [Range(0, 1)] private float musicVolume = 1;
        [SerializeField] [Range(0, 1)] private float sfxVolume = 1;

        //Combat Music Transition Properties
        private const float combatTransitionDuration = 1;
        private float combatTransitionProgress = 0;
        private bool isInCombat;
        private Coroutine queuedMusicRoutine;
    #endregion

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
        masterVCA.setVolume(masterVolume);

        musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
        musicVCA.setVolume(musicVolume);

        sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
        sfxVCA.setVolume(sfxVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        masterVCA.setVolume(masterVolume);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicVCA.setVolume(musicVolume);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxVCA.setVolume(sfxVolume);
    }

    public void playMusic(MusicTrack track, bool isCombat = false)
    {
        switch(track)
        {
            case MusicTrack.TitleMusic:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance.release();
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(titleMusic);
                break;

            case MusicTrack.Level1:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance.release();
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(level1MusicTrack);
                combatTransitionProgress = isCombat ? 1 : 0;
                musicInstance.setParameterByName("Combat", combatTransitionProgress);
                break;

            case MusicTrack.BossMusic:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance.release();
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(bossMusicTrack);
                break;
        }
        musicInstance.start();
    }

    public void stopMusic(bool allowFade)
    {
        var stopMode = allowFade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
        musicInstance.stop(stopMode);
        musicInstance.release();
    }

    public void toggleCombat(bool isInCombat)
    {
        if(this.isInCombat == isInCombat)
            return;

        // Debug.Log("Set combat to: " + isInCombat);
        this.isInCombat = isInCombat;
        musicInstance.setParameterByName("Combat", isInCombat ? 1 : 0);
    }

    public void queueMusicAfterFadeOut(MusicTrack track, bool isCombat = false)
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();

        if(queuedMusicRoutine != null)
            StopCoroutine(queuedMusicRoutine);

        queuedMusicRoutine = StartCoroutine(queueMusicWatcherRoutine(track, isCombat));
    }

    private IEnumerator queueMusicWatcherRoutine(MusicTrack track, bool isCombat = false)
    {
        FMOD.Studio.PLAYBACK_STATE state = FMOD.Studio.PLAYBACK_STATE.PLAYING;
        musicInstance.getPlaybackState(out state);

        while(state != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            yield return null;
	        musicInstance.getPlaybackState(out state);
        }

        playMusic(track, isCombat);
    }
}
