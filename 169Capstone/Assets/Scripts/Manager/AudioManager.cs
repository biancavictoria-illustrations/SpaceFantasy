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
        private bool isInCombat;
        private Coroutine combatTransitionRoutine;
    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
        masterVCA.setVolume(masterVolume);

        musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
        musicVCA.setVolume(musicVolume);

        sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
        sfxVCA.setVolume(sfxVolume);

        playMusic(MusicTrack.Level1, false);
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
    }

    public void toggleCombat(bool isInCombat)
    {
        if(this.isInCombat == isInCombat)
            return;

        Debug.Log("Set combat to: " + isInCombat);
        this.isInCombat = isInCombat;
        musicInstance.setParameterByName("Combat", isInCombat ? 1 : 0);
    }
}
