using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShmupBaby
{

    /// <summary>
    /// Data structure for Player Select Menu UI Clip.
    /// </summary>
    [System.Serializable]
    public class PlayerSelectUIAudioClip
    {
        #if UNITY_EDITOR

        /// <summary>
		/// Only used in the editor for organization purposes. 
        /// </summary>
        [Space]
        [Tooltip("Only used to keep things organized")]
        public string ClipName;

        #endif


        /// <summary>
		/// The UI event that will trigger the clip to play.
        /// </summary>
        [Space]
		[Tooltip("The UI event that will trigger the clip to play")]
        public PlayerSelectUIEvents UIEvent;

        /// <summary>
        /// Clip data.
        /// </summary>
        [Space]
		[Tooltip("Clip reference and volume.")]
        public ShmupAudioCip ClipSettings;

        /// <summary>
		/// Constructs a clip for the inspector.
        /// </summary>
        public PlayerSelectUIAudioClip(PlayerSelectUIEvents uIEvent)
        {
            UIEvent = uIEvent;
            ClipSettings = new ShmupAudioCip();

            #if UNITY_EDITOR

            ClipName = UIEvent.ToString();

            #endif

        }
    }

    /// <summary>
	/// Manages the audio playing in the Player Select scene. 
    /// </summary>
    [AddComponentMenu("Shmup Baby/Player Select/Player Select Audio Manager")]
    public class PlayerSelectAudioManager : AudioManager
    {

        /// <summary>
        /// The background music for this scene.
        /// </summary>
        [Header("BackgroundMusic")]
        [Space]
        [Tooltip("The music clip for the scene.")]
        public AudioClip BackgroundMusic;

        /// <summary>
		/// The Player Select UI audio clips.
        /// </summary>
        [Header("UI")]
        [Space]
        [Tooltip("a list of audio clips that will be played by UI events")]
        public PlayerSelectUIAudioClip[] UIAudioClips = new PlayerSelectUIAudioClip[]
        {
            new PlayerSelectUIAudioClip(PlayerSelectUIEvents.Accept),
            new PlayerSelectUIAudioClip(PlayerSelectUIEvents.Back),
            new PlayerSelectUIAudioClip(PlayerSelectUIEvents.OnSwitchPlayer)
        };

        /// <summary>
        /// Dictionary of the UI audio clips by UI events.
        /// </summary>
        private Dictionary<PlayerSelectUIEvents, ShmupAudioCip> _uiClipsDictionary;

        /// <summary>
		/// Start method is one of Unity messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
            MusicSource.loop = true;
            MusicSource.clip = BackgroundMusic;

            MusicSource.Play();

            InitializeUIAudio();
        }

        /// <summary>
		/// Populates the _uiClipsDictionary with the UI clip.
        /// </summary>
        private void InitializeUIAudio()
        {
            _uiClipsDictionary = new Dictionary<PlayerSelectUIEvents, ShmupAudioCip>();

            for (int i = 0; i < UIAudioClips.Length; i++)
            {
                if (!_uiClipsDictionary.ContainsKey(UIAudioClips[i].UIEvent))
                {
                    _uiClipsDictionary.Add(UIAudioClips[i].UIEvent, UIAudioClips[i].ClipSettings);
                }
                else
                {
                    Debug.Log("you have two audio clip that use the same event :" + UIAudioClips[i].UIEvent.ToString());
                }
            }

            PlayerSelectUIManager.OnUIEvent += PlayUISFX;
        }

        /// <summary>
		/// This is added to UI events, to play the designated clips in the UI events.
        /// </summary>
        private void PlayUISFX(ShmupEventArgs args)
        {
            PlayerSelectUIEvents uiEvent = ((PlayerSelectEventArg)args).UIEvent;

            if (_uiClipsDictionary.ContainsKey(uiEvent))
                PlayShmupClip(_uiClipsDictionary[uiEvent]);
        }

    }

}