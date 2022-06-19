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
            BossMusic,
            LichMusic,
            MainHubAmbient
        }

        public enum SFX
        {
            StarShardDrop,
            ElectrumDrop,

            SwordWoosh,

            TimelineResetDeath,
            TimelineResetRespawn,

            FellInSlimePit,

            ElevatorUp,
            ElevatorDown,

            DrinkPotion,

            GearSwap,
            ItemPurchase,

            CaptainsLogAlert,
            CaptainsLogOpen,
            CaptainsLogClose,

            ButtonHover,
            ButtonClick,

            Tick,
            Tock
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
        [SerializeField][FMODUnity.EventRef] private string lichMusicTrack;
        [SerializeField][FMODUnity.EventRef] private string mainHubAmbientTrack;

        [Header("SFX")]
        [SerializeField][FMODUnity.EventRef] private string starShardDrop;
        [SerializeField][FMODUnity.EventRef] private string electrumDrop;
        [SerializeField][FMODUnity.EventRef] private string swordWoosh;
        [SerializeField][FMODUnity.EventRef] private string timelineResetDeath;
        [SerializeField][FMODUnity.EventRef] private string timelineResetRespawn;
        [SerializeField][FMODUnity.EventRef] private string fellInSlimePit;
        [SerializeField][FMODUnity.EventRef] private string elevatorUp;
        [SerializeField][FMODUnity.EventRef] private string elevatorDown;
        [SerializeField][FMODUnity.EventRef] private string gearSwap;
        [SerializeField][FMODUnity.EventRef] private string itemPurchase;
        [SerializeField][FMODUnity.EventRef] private string drinkPotion;

        [Header("UI SFX")]
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteAngry;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteBlush;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteHappy;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteHeart;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteQuestion;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteSad;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteSurprise;
        [SerializeField][FMODUnity.EventRef] private string dialogueEmoteSweat;
        [SerializeField][FMODUnity.EventRef] private string captainsLogAlert;
        [SerializeField][FMODUnity.EventRef] private string buttonHover;
        [SerializeField][FMODUnity.EventRef] private string buttonClick;
        [SerializeField][FMODUnity.EventRef] private string tickSFX;
        [SerializeField][FMODUnity.EventRef] private string tockSFX;
        [SerializeField][FMODUnity.EventRef] private string captainsLogOpen;
        [SerializeField][FMODUnity.EventRef] private string captainsLogClose;

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

            case MusicTrack.LichMusic:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance.release();
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(lichMusicTrack);
                break;
            case MusicTrack.MainHubAmbient:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance.release();
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(mainHubAmbientTrack);
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

    public void setLichHallwayProgress(float progress)
    {
        musicInstance.setParameterByName("HallwayProgress", progress);
    }


    public void setLichMusicStage(int stage)
    {
        musicInstance.setParameterByName("LichMusicStage", stage);
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

    #region Play SFX
        // public void SetupSFXOnStart(string SFXName, FMOD.Studio.EventInstance FMODevent, GameObject source)
        // {
        //     if(SFXName != ""){
        //         FMODevent = FMODUnity.RuntimeManager.CreateInstance(SFXName);
        //         FMODevent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(source));
        //         // FMODUnity.RuntimeManager.AttachInstanceToGameObject(hitSFXEvent, transform);
        //     }
        // }

        public void PlaySFX(string SFXName, FMOD.Studio.EventInstance FMODevent)
        {
            if(SFXName == ""){
                return;
            }

            // FMODUnity.RuntimeManager.AttachInstanceToGameObject(FMODevent, source.transform);
            FMODevent.start();
            FMODevent.release();
        }
        
        public void PlaySFX(string SFXName, FMOD.Studio.EventInstance FMODevent, GameObject source)
        {
            if(SFXName == ""){
                return;
            }

            // FMODUnity.RuntimeManager.AttachInstanceToGameObject(FMODevent, source.transform, GetComponent<Rigidbody>()); do we need the rigidbody?
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FMODevent, source.transform);
            
            FMODevent.start();
            FMODevent.release();
        }

        public void PlaySFX(string SFXName)
        {
            if(SFXName == ""){
                return;
            }
            FMODUnity.RuntimeManager.PlayOneShot(SFXName);
        }

        public void PlaySFX(string SFXName, GameObject source)
        {
            if(SFXName == ""){
                return;
            }
            FMODUnity.RuntimeManager.PlayOneShot(SFXName, source.transform.position);
        }

        public void PlaySFX(SFX sfx, GameObject source)
        {
            switch(sfx){
                // Currency
                case SFX.StarShardDrop:
                    FMODUnity.RuntimeManager.PlayOneShot(starShardDrop, source.transform.position);
                    return;
                case SFX.ElectrumDrop:
                    FMODUnity.RuntimeManager.PlayOneShot(electrumDrop, source.transform.position);
                    return;

                // Gear
                case SFX.SwordWoosh:
                    FMODUnity.RuntimeManager.PlayOneShot(swordWoosh, source.transform.position);
                    return;

                // Death stuff
                case SFX.TimelineResetDeath:
                    FMODUnity.RuntimeManager.PlayOneShot(timelineResetDeath, source.transform.position);
                    return;
                case SFX.TimelineResetRespawn:
                    FMODUnity.RuntimeManager.PlayOneShot(timelineResetRespawn, source.transform.position);
                    return;

                // Traps
                case SFX.FellInSlimePit:
                    FMODUnity.RuntimeManager.PlayOneShot(fellInSlimePit, source.transform.position);
                    return;

                case SFX.ElevatorUp:
                    FMODUnity.RuntimeManager.PlayOneShot(elevatorUp, source.transform.position);
                    return;
                case SFX.ElevatorDown:
                    FMODUnity.RuntimeManager.PlayOneShot(elevatorDown, source.transform.position);
                    return;
            }
            Debug.LogWarning("No SFX found for " + sfx);
        }

        public void PlaySFX(SFX sfx)
        {
            switch(sfx){
                case SFX.CaptainsLogAlert:
                    FMODUnity.RuntimeManager.PlayOneShot(captainsLogAlert);
                    return;
                case SFX.CaptainsLogOpen:
                    FMODUnity.RuntimeManager.PlayOneShot(captainsLogOpen);
                    return;
                case SFX.CaptainsLogClose:
                    FMODUnity.RuntimeManager.PlayOneShot(captainsLogClose);
                    return;

                case SFX.ButtonHover:
                    FMODUnity.RuntimeManager.PlayOneShot(buttonHover);
                    return;
                case SFX.ButtonClick:
                    FMODUnity.RuntimeManager.PlayOneShot(buttonClick);
                    return;
                
                case SFX.GearSwap:
                    FMODUnity.RuntimeManager.PlayOneShot(gearSwap);
                    return;
                case SFX.ItemPurchase:
                    FMODUnity.RuntimeManager.PlayOneShot(itemPurchase);
                    return;

                case SFX.DrinkPotion:
                    FMODUnity.RuntimeManager.PlayOneShot(drinkPotion);
                    return;
                
                case SFX.Tick:
                    FMODUnity.RuntimeManager.PlayOneShot(tickSFX);
                    return;
                case SFX.Tock:
                    FMODUnity.RuntimeManager.PlayOneShot(tockSFX);
                    return;
            }
            Debug.LogWarning("No UI SFX found for " + sfx);
        }

        public void PlaySFX(DialogueEmoteType emoteType)
        {
            switch(emoteType){
                case DialogueEmoteType.angry:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteAngry);
                    return;
                case DialogueEmoteType.blush:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteBlush);
                    return;
                case DialogueEmoteType.happy:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteHappy);
                    return;
                case DialogueEmoteType.heart:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteHeart);
                    return;
                case DialogueEmoteType.question:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteQuestion);
                    return;
                case DialogueEmoteType.sad:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteSad);
                    return;
                case DialogueEmoteType.surprise:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteSurprise);
                    return;
                case DialogueEmoteType.sweat:
                    FMODUnity.RuntimeManager.PlayOneShot(dialogueEmoteSweat);
                    return;
            }
            Debug.LogWarning("No SFX found for dialogue emote " + emoteType);
        }
    #endregion
}
