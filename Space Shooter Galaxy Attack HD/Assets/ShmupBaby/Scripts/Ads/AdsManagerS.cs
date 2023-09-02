using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace ShmupBaby
{

    /// <summary>
    /// This script manages all ads inside Shmup Baby
    /// </summary>
    [AddComponentMenu("Shmup Baby/Ads/Ads Manager")]
    public class AdsManager : PersistentSingleton<AdsManager>
    {
        #if (UNITY_ADS && (UNITY_ANDROID || UNITY_IOS))
        #region Declaring Variables
        [Tooltip("This is required to integrate the ads into the game.")]
        public string GameID;
        public string placmentID = "video";

        [Space]
        [Tooltip("Checks to add ads every time the game starts.")]
        public bool StartUpAds;

        [Space]
        [Tooltip("Checks to add ads associated with a timer.")]
        public bool TimerAds;
        [Tooltip("Time in seconds between ads.")]
        public float TimeBetweenAds = 300f;

        [Space]
        [Tooltip("Checks to add ads everytime the player used a life.")]
        public bool RetryAds;
        [Tooltip("How many times the player should use a life before showing ads.")]
        public int Retries = 1;

        private float _timer;
        private int _retryCounter;
        #endregion

        #region Methods & Coroutines
        //Intializing Ads, setting up the startUP & retry ads
        void Start()
        {           
			//Starts a function to initializes Unity Ads with the GameID
			InitializeAds();
            if (StartUpAds) StartCoroutine(FirstAds(1.0f));              
			//Adding an ads delegate function to loading a scene if retry ads is enabled 
			if (RetryAds) SceneManager.sceneLoaded += OnSceneLoad;               
            _retryCounter = Retries;
        }

        //In Game Timer Ads
        void Update()
        {
            if (!TimerAds) return;           
           
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                StartCoroutine(loadAndShow());
                _timer = TimeBetweenAds;
            }           
        }

        /// <summary>
        /// Initializes Unity ads with a game ID.
        /// </summary>
        void InitializeAds()
        {
            if (!Advertisement.isInitialized) Advertisement.Initialize(GameID); 
        }

        /// <summary>
        /// Showing ads & pausing the game when ads are on.
        /// </summary>
        IEnumerator loadAndShow()
        {
			// To avoid showing ads when Unity ads has not yet been loaded.    
			if (!Advertisement.IsReady(placmentID))
            {
                yield break;
            }
            
			// Pause game.
            TimeManager.Pause();
            AudioManager.Instance.PauseMusic();
            
            InitializeAds();

			// To be able to unpause the game & go back to update  
            ShowOptions AdsOptions = new ShowOptions();
            AdsOptions.resultCallback += UnPause;
            Advertisement.Show(placmentID, AdsOptions);                    
        }       

        /// <summary>
        /// Stacks the CheckForRetryAds to the spawn player event.
        /// </summary>
        void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (GameManager.Instance.GetCurrentLevelIndex() == -1) return;
            LevelController.Instance.OnPlayerSpawn += CheckForRetryAds;
        }

        /// <summary>
        /// Checks if it's time to show retry ads and then shows it
        /// </summary>
        void CheckForRetryAds (ShmupEventArgs args)
        {
            _retryCounter--;

            if (_retryCounter < 0)
            {
                StartCoroutine(loadAndShow());
                _retryCounter = Retries;
            }
            
        }

        /// <summary>
        /// UnPause the game
        /// </summary>
        void UnPause(ShowResult obj)
        {
            AudioManager.Instance.UnPauseMusic();
            TimeManager.UnPause();
        }

        /// <summary>
        /// Show ads after a set time
        /// </summary>
        /// <param name="time">Time in sec.</param>
        IEnumerator FirstAds (float time)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(loadAndShow());
        }
        #endregion
        #endif
    }
}