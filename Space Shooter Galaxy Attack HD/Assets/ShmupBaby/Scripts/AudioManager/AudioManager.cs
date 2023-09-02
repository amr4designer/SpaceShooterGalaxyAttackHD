using System;
using UnityEngine;
using UnityEngine.Audio;

namespace ShmupBaby
{
	/// <summary>
	/// Base class for the audio manager.
	/// </summary>
    [AddComponentMenu("")]
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// the master mixer of the game.
        /// </summary>
        [Tooltip("the master mixer of the game.")]
        [HideInInspector]
        public AudioMixer MasterMixer;
        /// <summary>
		/// Reference to the audio manager's music audio source.
        /// </summary>
        [HideInInspector]
        public AudioSource MusicSource;
        /// <summary>
        /// Reference to the audio manager's SFX audio source.
        /// </summary>
        [HideInInspector]
        public AudioSource SFXSource;

        /// <summary>
        /// the master volume for the audio manager.
        /// </summary>
        public float MasterVolume
        {
            get
            {
                return _masterVolume;
            }
            set
            {
                if (MasterGroup != null)
                {
                    if (value <= -20)
                        value = -80;
                    _masterVolume = value;
                    MasterGroup.audioMixer.SetFloat(MasterVolumeParmaterName, _masterVolume);
                }
            }
        }
        /// <summary>
        /// the master volume for the audio manager.
        /// </summary>
        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                if (MusicGroup != null)
                {
                    if (value <= -20)
                        value = -80;
                    _musicVolume = value;
                    MusicGroup.audioMixer.SetFloat(MusicVolumeParmaterName, _musicVolume);
                }
            }
        }
        /// <summary>
        /// the master volume for the audio manager.
        /// </summary>
        public float SFXVolume
        {
            get
            {
                return _SFXVolume;
            }
            set
            {
                if (SFXGroup != null)
                {
                    if (value <= -20)
                        value = -80;
                    _SFXVolume = value;
                    SFXGroup.audioMixer.SetFloat(SFXVolumeParmaterName, _SFXVolume);
                }
            }
        }

        /// <summary>
        /// back-end field for SFX volume multiplier.
        /// </summary>
        private float _SFXVolume = 1;
        /// <summary>
        /// back-end field for Music volume multiplier.
        /// </summary>
        private float _musicVolume = 1;
        /// <summary>
        /// back-end field for Music volume multiplier.
        /// </summary>
        private float _masterVolume = 1;

        /// <summary>
        /// the name of the master group
        /// </summary>
        private const string MasterGroupName = "Master";
        /// <summary>
        /// the name of the parameter that control the master volume
        /// </summary>
        private const string MasterVolumeParmaterName = "MasterVol";
        /// <summary>
        /// the name of the music group
        /// </summary>
        private const string MusicGroupName = "Music";
        /// <summary>
        /// the name of the parameter that control the music volume
        /// </summary>
        private const string MusicVolumeParmaterName = "MusicVol";
        /// <summary>
        /// the name of the SFX group
        /// </summary>
        private const string SFXGroupName = "SFX";
        /// <summary>
        /// the name of the parameter that control the SFX volume
        /// </summary>
        private const string SFXVolumeParmaterName = "SFXVol";

        /// <summary>
        /// the master audio group.
        /// </summary>
        protected AudioMixerGroup MasterGroup;
        /// <summary>
        /// the music audio group.
        /// </summary>
        protected AudioMixerGroup MusicGroup;
        /// <summary>
        /// the SFX audio group.
        /// </summary>
        protected AudioMixerGroup SFXGroup;

        /// <summary>
		/// Unity message, begins before the start function.
		/// </summary>
        protected override void Awake()
        {
            base.Awake();

            MasterMixer = Resources.Load<AudioMixer>("Audio/Mixers/MasterMixer");

            if(MasterMixer == null)
            {
                Debug.Log("master mixer couldn't be found in Resources/Audio/Mixers/MasterMixer please make sure that you placed it there.");
            }

            AudioMixerGroup[] groups = MasterMixer.FindMatchingGroups("");

            MasterGroup = GetGroupByName(groups, MasterGroupName);
            MusicGroup = GetGroupByName(groups, MusicGroupName);
            SFXGroup = GetGroupByName(groups, SFXGroupName);

            MusicSource = gameObject.AddComponent<AudioSource>();
            SFXSource = gameObject.AddComponent<AudioSource>();

            MusicSource.outputAudioMixerGroup = MusicGroup;
            SFXSource.outputAudioMixerGroup = SFXGroup;

            LevelInitializer.Instance.SubscribeToStage(4, InitializeAudioManager);
            
        }

        /// <summary>
        /// get AudioMixerGroup by it's name from AudioMixerGroup array.
        /// </summary>
        private AudioMixerGroup GetGroupByName (AudioMixerGroup[] groups, string name)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i].name == name)
                    return groups[i];
            }
            Debug.Log(" the audio mixer you have doesn't have groups that match the shmup baby naming convention");
            return null;
        }

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        protected virtual void InitializeAudioManager()
        {
            
            if (GameManager.IsInitialize)
            {
                MasterVolume = GameManager.Instance.GameSettings.MasterVolume;
                MusicVolume = GameManager.Instance.GameSettings.MusicVolume;
                SFXVolume = GameManager.Instance.GameSettings.SFXVolume;
            }
        }

        /// <summary>
        /// pause the background music.
        /// </summary>
        public void PauseMusic ()
        {
            MusicSource.Pause();
            SFXSource.Pause();
        }

        /// <summary>
        /// UnPause the background music.
        /// </summary>
        public void UnPauseMusic()
        {
            MusicSource.UnPause();
            SFXSource.UnPause();
        }

        /// <summary>
		/// Plays the package audio clips.
        /// </summary>
        /// <param name="clip">clip to play.</param>
        public void PlayShmupClip(ShmupAudioCip clip)
        {
            if (SFXSource == null)
                return;

            if (clip.Clip != null)
                SFXSource.PlayOneShot(clip.Clip, clip.Volume);
        }

        /// <summary>
		/// Plays a default agent clip.
        /// </summary>
        /// <param name="agentType">The agent type : player , enemy , ..</param>
        /// <param name="agentEvent">The agent event</param>
        public virtual void PlayDefaultAgentClip(Type agentType, AllAgentEvents agentEvent)
        {

        }

    }

}
