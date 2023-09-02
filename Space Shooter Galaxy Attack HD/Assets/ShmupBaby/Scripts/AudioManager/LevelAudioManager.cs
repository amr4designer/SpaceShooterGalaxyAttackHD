using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
	/// Audio Clip Object used throughout the package.
    /// </summary>
    [System.Serializable]
    public class ShmupAudioCip
    {
        /// <summary>
		/// Clip that will be played by PlayClip.
        /// </summary>
        [Space]
        [Tooltip("reference to the clip")]
        public AudioClip Clip;

        /// <summary>
        /// Clip play volume.
        /// </summary>
        [Space]
        [Range(0,10f)]
        [Tooltip("Clip play volume")]
        public float Volume ;


        public ShmupAudioCip()
        {
            Volume = 1f;
        }

        /// <summary>
		/// This method matches the ShmupDelegate Signature,
		/// Play the Clip of this object.
        /// </summary>
        public void PlayClip(ShmupEventArgs args)
        {
            LevelAudioManager.Instance.PlayShmupClip(this);
        }
    }

    /// <summary>
	/// Data structure for the agent audio clip.
    /// </summary>
    [System.Serializable]
    public class AgentClip
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
		/// The agent event that will trigger the clip to play.
        /// </summary>
        [Space]
        [Tooltip("the agent event that will trigger the clip to play")]
        public AllAgentEvents Event;

        /// <summary>
		/// The clip data.
        /// </summary>
        [Space]
        [Tooltip("clip reference and volume.")]
        public ShmupAudioCip ClipSettings;

        /// <summary>
		/// The type of agent that will trigger this clip,
		/// Its used to decide what is the default clip for that agent.
        /// </summary>
        private Type _agentType;

        /// <summary>
		/// Sets the type of the agent that will play this clip.
        /// </summary>
        /// <param name="agentType">the type of the agent that will play this clip.</param>
        public void SetAgentType(Type agentType)
        {
            _agentType = agentType;
        }

        /// <summary>
		/// Constructs the agent clip for the inspector.
        /// </summary>
       public AgentClip(AllAgentEvents agentEvent)
        {
            Event = agentEvent;

            ClipSettings = new ShmupAudioCip();

            #if UNITY_EDITOR

            ClipName = agentEvent.ToString();

            #endif
        }

        /// <summary>
		/// This method matches the ShmupDelegate Signature,
        /// play the Clip of this object.
        /// </summary>
        public void PlayAgentClip(ShmupEventArgs args)
        {
			//If there is no clip; we play the default one if it existed.
            if (ClipSettings.Clip != null)
            {
                LevelAudioManager.Instance.PlayShmupClip(ClipSettings);
            }
            else
            {
                if (_agentType != null)
                    LevelAudioManager.Instance.PlayDefaultAgentClip(_agentType, Event);
            }
        }

    }

    /// <summary>
	/// Data structure for Level UI Clip.
    /// </summary>
    [System.Serializable]
    public class LevelUIAudioClip
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
        public LevelUIEvents UIEvent;

        /// <summary>
        /// Clip data.
        /// </summary>
        [Space]
        [Tooltip("Clip reference and volume.")]
        public ShmupAudioCip ClipSettings;

        /// <summary>
		/// Constructs the clip for the inspector.
        /// </summary>
        public LevelUIAudioClip(LevelUIEvents uIEvent)
        {
            UIEvent = uIEvent;
            ClipSettings = new ShmupAudioCip();

            #if UNITY_EDITOR

            ClipName = UIEvent.ToString();

            #endif

        }
    }

    /// <summary>
	/// Manages the audio playing in the Level scenes. 
    /// </summary>
    [AddComponentMenu("Shmup Baby/Level/LevelAudioManager")]
    public class LevelAudioManager : AudioManager
    {
        /// <summary>
		/// The background music for this level.
        /// </summary>
        [Header("BackgroundMusic")]
        [Space]
        [Tooltip("The background music clip for the level.")]
        public AudioClip BackgroundMusic;

        /// <summary>
		/// The level UI audio clips.
        /// </summary>
        [Header("UI")]
        [Space]
        [Tooltip("A list of audio clips that will be played by UI events")]
        public LevelUIAudioClip[] UIAudioClips = new LevelUIAudioClip[]
        {
            //initialize the field to populate the inspector.
            new LevelUIAudioClip ( LevelUIEvents.GameOverCanvasAppear ),
            new LevelUIAudioClip ( LevelUIEvents.LevelCompleteAppear ),
            new LevelUIAudioClip ( LevelUIEvents.PauseButtonPressed ),
            new LevelUIAudioClip ( LevelUIEvents.ResumeButtonPressed ),
            new LevelUIAudioClip ( LevelUIEvents.RetryButtonPressed ),
            new LevelUIAudioClip ( LevelUIEvents.MainMenuButtonPressed ),
            new LevelUIAudioClip ( LevelUIEvents.NextLevelButtonPressed ),
        };

        /// <summary>
		/// The default player audio clips.
        /// </summary>
		/// We are still working on finding better ways to handle the default sounds.
        [Header("Player")]
        [Space]
        [HideInInspector]
        public AgentClip[] PlayerClips = new AgentClip[]
        {
            //Initialize the field to populate the inspector.
            new AgentClip(AllAgentEvents.Destroy),
            new AgentClip(AllAgentEvents.TakeHealthDamage),
            new AgentClip(AllAgentEvents.TakeShieldDamage),
            new AgentClip(AllAgentEvents.ImmunityActivate),
            new AgentClip(AllAgentEvents.TakeCollision),
            new AgentClip(AllAgentEvents.PickUp)
        };

        /// <summary>
        /// The default enemy audio clips
        /// </summary>
		/// We are still working on finding better ways to handle the default sounds.
        [Header("Enemy")]
        [Space]
        [HideInInspector]
        public AgentClip[] EnemyClips = new AgentClip[]
        {
            //initialize the field to populate the inspector.
            new AgentClip(AllAgentEvents.Destroy),
            new AgentClip(AllAgentEvents.TakeHealthDamage),
            new AgentClip(AllAgentEvents.TakeShieldDamage),
            new AgentClip(AllAgentEvents.DealCollision),
            new AgentClip(AllAgentEvents.Drop),
            new AgentClip(AllAgentEvents.DetonationStart)
        };
        
        /// <summary>
        /// Dictionary of the ui audio clips by UI events.
        /// </summary>
        private Dictionary<LevelUIEvents, ShmupAudioCip> _uiClipsDictionary;
        /// <summary>
        /// Dictionary of the player default audio clips by UI events.
        /// </summary>
        private Dictionary<AllAgentEvents, ShmupAudioCip> _playerClipsDictionary;
        /// <summary>
        /// Dictionary of the enemy default audio clips by UI events.
        /// </summary>
        private Dictionary<AllAgentEvents, ShmupAudioCip> _enemyClipsDictionary;

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        protected override void InitializeAudioManager()
        {
            base.InitializeAudioManager();

            InitializeUIAudio();

            InitializePlayerAudio();

            InitializeEnemyAudio();

            MusicSource.loop = true;
            MusicSource.clip = BackgroundMusic;

            MusicSource.Play();

        }

        /// <summary>
        /// Populates the _playerClipsDictionary with the default player clip.
        /// </summary>
        private void InitializePlayerAudio()
        {
            _playerClipsDictionary = new Dictionary<AllAgentEvents, ShmupAudioCip>();

            for (int i = 0; i < PlayerClips.Length; i++)
            {
                if (!_playerClipsDictionary.ContainsKey(PlayerClips[i].Event))
                {
                    _playerClipsDictionary.Add(PlayerClips[i].Event, PlayerClips[i].ClipSettings);
                }
                else
                {
                    Debug.Log("you have two audio clip that use the same event :" + PlayerClips[i].Event.ToString());
                }
            }
        }

        /// <summary>
        /// Populates the _enemyClipsDictionary with the default enemy clip.
        /// </summary>
        private void InitializeEnemyAudio()
        {
            _enemyClipsDictionary = new Dictionary<AllAgentEvents, ShmupAudioCip>();

            for (int i = 0; i < EnemyClips.Length; i++)
            {
                if (!_enemyClipsDictionary.ContainsKey(EnemyClips[i].Event))
                {
                    _enemyClipsDictionary.Add(EnemyClips[i].Event, EnemyClips[i].ClipSettings);
                }
                else
                {
                    Debug.Log("you have two audio clip that use the same event :" + EnemyClips[i].Event.ToString());
                }
            }
        }

        /// <summary>
        /// Populates the _uiClipsDictionary with the UI clip.
        /// </summary>
        private void InitializeUIAudio()
        {
            _uiClipsDictionary = new Dictionary<LevelUIEvents, ShmupAudioCip>();

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

            LevelUIManager.OnUIEvent += PlayUISFX;
        }

        /// <summary>
        /// This is added to UI events, to play the designated clips in the UI events.
        /// </summary>
        private void PlayUISFX(ShmupEventArgs args)
        {
            LevelUIEvents uiEvent = ((LevelUIEventArg) args).UIEvent;

            if (_uiClipsDictionary.ContainsKey(uiEvent))
                PlayShmupClip(_uiClipsDictionary[uiEvent]);
        }

        /// <summary>
        /// Plays a default agent clip.
        /// </summary>
        /// <param name="agentType">The agent type : player , enemy , ..</param>
        /// <param name="agentEvent">The agent event</param>
        public override void PlayDefaultAgentClip(Type agentType, AllAgentEvents agentEvent)
        {
            if (agentType == typeof(Player))
            {
                if (_playerClipsDictionary.ContainsKey(agentEvent))
                {
                    PlayShmupClip(_playerClipsDictionary[agentEvent]);
                }
                else
                {
                    //Debug.Log("you haven't define a default clip in the audio manager for :"+ typeof(Player).ToString()+" "+agentEvent.ToString());
                    Debug.Log("no clip reference where found");
                }
            }

            if (agentType == typeof(Enemy))
            {
                if (_enemyClipsDictionary.ContainsKey(agentEvent))
                {
                    PlayShmupClip(_enemyClipsDictionary[agentEvent]);
                }
                else
                {
                    //Debug.Log("you haven't define a default clip in the audio manager for :" + typeof(Enemy).ToString() + " " + agentEvent.ToString());
                    Debug.Log("no clip reference where found");
                }
            }
        }

    }

}
