using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
	/// Data structure for Main Menu UI Clip.
    /// </summary>
    [System.Serializable]
    public class MainMenuUIAudioClip
    {
        #if UNITY_EDITOR

        /// <summary>
		/// Only used in the editor for organization purposes. 
        /// </summary>
        [Space]
        [Tooltip("only used to keep things organized")]
        public string ClipName;

        #endif


        /// <summary>
		/// The UI event that will trigger the clip to play.
        /// </summary>
        [Space]
        [Tooltip("The UI event that will trigger the clip to play")]
        public MainUIEvents UIEvent;

        /// <summary>
        /// Clip data.
        /// </summary>
        [Space]
        [Tooltip("Clip reference and volume.")]
        public ShmupAudioCip ClipSettings;

        /// <summary>
		/// Constructs a clip for the inspector.
        /// </summary>
        public MainMenuUIAudioClip(MainUIEvents uIEvent)
        {
            UIEvent = uIEvent;
            ClipSettings = new ShmupAudioCip();

            #if UNITY_EDITOR

            ClipName = UIEvent.ToString();

            #endif

        }
    }

    /// <summary>
    /// Manages the audio playing in the Main Menu scene. 
    /// </summary>
    [AddComponentMenu("Shmup Baby/Main Menu/Main Menu Audio Manager")]
    public class MainMenuAudioManager : AudioManager
    {

        /// <summary>
		/// The background music for this scene.
        /// </summary>
        [Header("BackgroundMusic")]
        [Space]
        [Tooltip("The background music clip for the scene.")]
        public AudioClip BackgroundMusic;

        /// <summary>
		/// The Main Menu UI audio clips.
        /// </summary>
        [Header("UI")]
        [Space]
        [Tooltip("A list of audio clips that will be played by UI events")]
        public MainMenuUIAudioClip[] UIAudioClips = new MainMenuUIAudioClip[]
        {
            new MainMenuUIAudioClip(MainUIEvents.ButtonPressed),
            new MainMenuUIAudioClip(MainUIEvents.ToggleChange)
        };

        /// <summary>
        /// Dictionary of the UI audio clips by UI events.
        /// </summary>
        private Dictionary<MainUIEvents, ShmupAudioCip> _uiClipsDictionary;

        /// <summary>
		/// Start method is one of Unity's messages that gets called when a new object is instantiated.
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
            _uiClipsDictionary = new Dictionary<MainUIEvents, ShmupAudioCip>();

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

            MainMenuUIManager.OnUIEvent += PlayUISFX;
        }

        /// <summary>
		/// This is added to UI events, to play the designated clips in the UI events.
        /// </summary>
        private void PlayUISFX(ShmupEventArgs args)
        {
            MainUIEvents uiEvent = ((MainUIEventArg)args).UIEvent;

            if (_uiClipsDictionary.ContainsKey(uiEvent))
                PlayShmupClip(_uiClipsDictionary[uiEvent]);
        }

    }

}
