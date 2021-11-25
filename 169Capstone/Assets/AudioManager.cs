using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Music & SFX Enums
        public enum MusicTrack
        {
            Level1
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
        [SerializeField][FMODUnity.EventRef] private string level1MusicTrack;

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

    #region Public Properties
        [Header("Volume Levels")]
        [Range(0, 1)] public float masterVolume = 1;
        [Range(0, 1)] public float musicVolume = 1;
        [Range(0, 1)] public float sfxVolume = 1;
    #endregion

    #region Private Properties
        //Combat Music Transition Properties
        private const float combatTransitionDuration = 1;
        private float combatTransitionProgress = 0;
        private Coroutine combatTransitionRoutine;
    #endregion

    void Start()
    {
        Instance = this;

        masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
        masterVCA.setVolume(masterVolume);

        musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
        musicVCA.setVolume(musicVolume);

        sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
        sfxVCA.setVolume(sfxVolume);
    }

    void Update()
    {
        //A short demo of the music system
        if(Time.time > 30)
        {
            FMOD.Studio.PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);
            if(state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
                stopMusic(true);
        }
        else if(Time.time > 20)
        {
            if(combatTransitionProgress == 1)
                toggleCombat(false);
        }
        else if(Time.time > 10)
        {
            if(combatTransitionProgress == 0)
                toggleCombat(true);
        }
        else
        {
            FMOD.Studio.PLAYBACK_STATE state;
            musicInstance.getPlaybackState(out state);
            if(state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                playMusic(MusicTrack.Level1, false);
        }
    }

    public void playMusic(MusicTrack track, bool isCombat)
    {
        switch(track)
        {
            case MusicTrack.Level1:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(level1MusicTrack);
                break;
        }

        combatTransitionProgress = isCombat ? 1 : 0;
        musicInstance.setParameterByName("Combat", combatTransitionProgress);
        musicInstance.start();
    }

    public void stopMusic(bool allowFade)
    {
        var stopMode = allowFade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
        musicInstance.stop(stopMode);
        StopAllCoroutines();
    }

    public void toggleCombat(bool isInCombat)
    {
        if(combatTransitionRoutine != null)
            StopCoroutine(combatTransitionRoutine);

        StartCoroutine(CombatMusicTransitionRoutine(isInCombat));
    }

    private IEnumerator CombatMusicTransitionRoutine(bool isInCombat)
    {
        bool hasCompletedTransition = false;
        while(!hasCompletedTransition)
        {
            if(isInCombat && combatTransitionProgress < 1)
                combatTransitionProgress += Time.deltaTime / combatTransitionDuration;

            if(!isInCombat && combatTransitionProgress > 0)
                combatTransitionProgress -= Time.deltaTime / combatTransitionDuration;

            if(isInCombat && combatTransitionProgress >= 1)
            {
                combatTransitionProgress = 1;
                hasCompletedTransition = true;
            }
            if(!isInCombat && combatTransitionProgress <= 0)
            {
                combatTransitionProgress = 0;
                hasCompletedTransition = true;
            }

            musicInstance.setParameterByName("Combat", combatTransitionProgress);

            yield return null;
        }
    }
}
