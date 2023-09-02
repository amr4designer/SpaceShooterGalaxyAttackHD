using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShmupBaby
{
    /// <summary>
    /// a list of all Level UI Events.
    /// </summary>
    public enum LevelUIEvents
    {
        PauseButtonPressed,
        ResumeButtonPressed,
        MainMenuButtonPressed,
        RetryButtonPressed,
        NextLevelButtonPressed,
        GameOverCanvasAppear,
        LevelCompleteAppear
    }

    /// <summary>
    /// Data structure to define what to be passed when LevelUIEvent is triggered.
    /// </summary>
    public class LevelUIEventArg : ShmupEventArgs
    {
        public LevelUIEvents UIEvent;

        public LevelUIEventArg(LevelUIEvents uIEvent)
        {
            UIEvent = uIEvent;
        }
    }

    /// <summary>
    /// Controls all the UI Canvas in the level.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Level/Level UI Manager")]
    public sealed class LevelUIManager : Singleton<LevelUIManager>
    {
        /// <summary>
        /// transforms component to HUD UI Canvas.
        /// </summary>
        [Header("HUD")]
        [Space]
        [Tooltip("Plug the HUD canvas that contain all the HUD UI Element.")]
        public RectTransform HUDCanvas;
        /// <summary>
        /// the Pause Button for the HUD UI.
        /// </summary>
        [Tooltip("Plug the Pause Button for the HUD UI")]
        public Button HUDPauseButton;
        /// <summary>
        /// UI Text Component that represents the player remaining lives in the HUD.
        /// </summary>
        [Space]
        [Tooltip(" UI Text Component that represents the player remaining lives in the HUD.")]
        public Text PlayerLives;
        /// <summary>
        /// UI Text Component that represents the player score in the HUD.
        /// </summary>
        [Tooltip("UI Text Component that represents the player score in the HUD")]
        public Text PlayerScore;
        /// <summary>
        /// UI Text Component that represents the high score in the HUD.
        /// </summary>
        [Tooltip("UI Text Component that represents the high score in the HUD")]
        public Text HighScore;
        /// <summary>
        /// BarControl component that represents the player remaining health in the HUD.
        /// </summary>
        [Space]
        [Tooltip("BarControl component that represents the player remaining health in the HUD")]
        public BarControl HealthBar;
        /// <summary>
        /// BarControl component that represents the player remaining shield in the HUD.
        /// </summary>
        [Tooltip("BarControl component that represents the player remaining shield in the HUD")]
        public BarControl ShieldBar;

        /// <summary>
        /// Transforms component to PauseGame UI Canvas.
        /// </summary>
        [Header("PauseGame")]
        [Space]
        [Tooltip("Plug the PauseGame canvas that contains all the PauseGame panel and its UI Element.")]
        public RectTransform PauseGameCanvas;
        /// <summary>
        /// Resume Button for the PauseGame UI.
        /// </summary>
        [Space]
        [Tooltip("Plug the Resume Button for the PauseGame UI")]
        public Button ResumeButton;
        /// <summary>
        /// Main Menu Button for the PauseGame UI.
        /// </summary>
        [Tooltip("plug the Main Menu Button for the PauseGame UI")]
        public Button PauseGameMainButton;
        /// <summary>
        /// Retry Button for the PauseGame UI.
        /// </summary>
        [Tooltip("Plug the Retry Button for the PauseGame UI")]
        public Button PauseGameRetryButton;
        /// <summary>
        /// the Music Slider for the PauseGame Menu UI.
        /// </summary>
        [Space]
        [Tooltip("Plug the Music Slider for the PauseGame Menu UI")]
        public Slider PauseMusicSlider;
        /// <summary>
        /// The SFX Slider for the PauseGame Menu UI.
        /// </summary>
        [Tooltip("Plug the SFX Slider for the PauseGame Menu UI")]
        public Slider PauseSoundFXSlider;

        /// <summary>
        /// Transform component for the GameOver UI Canvas.
        /// </summary>
        [Header("GameOver")]
        [Space]
        [Tooltip("Plug the GameOver canvas that contain all the GameOver panel and its UI Element.")]
        public RectTransform GameOverCanvas;
        /// <summary>
        /// UI Text Component that represents the High Score for this level in the GameOver UI.
        /// </summary>
        [Tooltip("UI Text Component that represent the High Score for this level in the GameOver UI")]
        [Space]
        public Text GameOverHighScore;
        /// <summary>
        /// UI Text Component that represents the score for this level in the GameOver UI.
        /// </summary>
        [Tooltip("UI Text Component that represents score for this level in the GameOver UI")]
        public Text GameOverCurrentScore;
        /// <summary>
        /// Main Menu Button for the GameOver UI.
        /// </summary>
        [Tooltip("Plug the Main Menu Button for the GameOver UI")]
        [Space]
        public Button GameOverMainButton;
        /// <summary>
        /// Retry Button for the GameOver UI.
        /// </summary>
        [Tooltip("plug the Retry Button for the GameOver UI")]
        public Button GameOverRetryButton;
        /// <summary>
        /// Transform component for the LevelComplete UI Canvas.
        /// </summary>
        /// 
        [Header("LevelComplete")]
        [Space]
        [Tooltip("Plug the LevelComplete canvas that contains all the LevelComplete panel and its UI Element.")]
        public RectTransform LevelCompleteCanvas;
        /// <summary>
        /// UI Text Component that represent the High Score for this level in the LevelComplete UI.
        /// </summary>
        [Tooltip("UI Text Component that represent the High Score for this level in the LevelComplete UI")]
        [Space]
        public Text LevelCompleteHighScore;
        /// <summary>
        /// UI Text Component that represent the High Score for this level in the LevelComplete UI.
        /// </summary>
        [Tooltip("UI Text Component that represents the High Score for this level in the LevelComplete UI")]
        public Text LevelCompleteCurrentScore;

        /// <summary>
        /// Next Level Button for the LevelComplete UI.
        /// </summary>
        [Tooltip("Plug the Next Level Button for the LevelComplete UI")]
        [Space]
        public Button NextLevelButton;
        /// <summary>
        /// Main Menu Button for the LevelComplete UI.
        /// </summary>
        [Tooltip("Plug the Main Menu Button for the LevelComplete UI")]
        public Button LevelCompleteMainButton;
        /// <summary>
        /// Retry Button for the LevelComplete UI.
        /// </summary>
        [Tooltip("Plug the Retry Button for the LevelComplete UI")]
        public Button LevelCompleteRetryButton;
        /// <summary>
        /// the phone control canvas.
        /// </summary>
        [Header("PhoneControls")]
        [Tooltip("Plug the phone controls canvas.")]
        public Canvas PhoneControlsCanvas;
        /// <summary>
        /// joystick of the phone controls.
        /// </summary>
        [Tooltip("plug the UI joystick of the phone controls.")]
        public JoyStick PhoneJoyStick;
        /// <summary>
        /// fire button of the phone controls
        /// </summary>
        [Tooltip("plug in the fire button of the phone controls")]
        public HoldButton PhoneFireButton;

        [Header("Infinite Level")]
        [Tooltip("the text that will show the level number.")]
        public Text LevelDeclare;
        [Tooltip("the delay before show the text in sec.")]
        public float LevelDeclareDelay;
        [Tooltip("how much the text will stay on the screen.")]
        public float LevelDeclareStay;

        /// <summary>
        /// triggered when one of the UI event Occur.
        /// </summary>
        public static event ShmupDelegate OnUIEvent;

        /// <summary>
        /// indicate if pause is allow.
        /// </summary>
        private bool _allowPause = true;
        /// <summary>
        /// indicate if the PoneControls is needed.
        /// </summary>
        private bool _phoneControlsNeeded = false;


        protected override void Awake()
        {
            base.Awake();
            LevelInitializer.Instance.SubscribeToStage(4, InitializeUIManager);
        }

        private void Update()
        {
            if (InputManager.Instance.GetInput(PlayerID.Player1).Pause)
            {
                Pause();
            }               
        }

        /// <summary>
        /// Initializes the Level UI Manager.
        /// </summary>
        private void InitializeUIManager()
        {
            //subscribes to the LevelController events:
            LevelController.Instance.OnPlayerSpawn += UpdateHUD;
            LevelController.Instance.OnPlayerGetExtraLife += UpdatePlayerLive;
            LevelController.Instance.OnScoreChange += UpdateScore;
            LevelController.Instance.OnPlayerOutOfLives += GameOver;

            FiniteLevelController finiteLevelController = LevelController.Instance as FiniteLevelController;

            if (finiteLevelController != null)
                finiteLevelController.OnLevelComplete += LevelComplete;


            InfiniteLevelController infiniteLevelController = LevelController.Instance as InfiniteLevelController;

            if (infiniteLevelController != null && LevelDeclare != null)
            {
                LevelDeclare.gameObject.SetActive(false);
                infiniteLevelController.OnLevelStart += ShowLevelDeclareUI;
            }

            InitializeHUDHighScore();
            InitializePauseUI();
            InitializeGameOverUI();
            InitializeLevelCompleteUI();

            AdjustUIToForceAspect();
        }
        
        /// <summary>
        /// show a text that indicate the level number.
        /// </summary>
        /// <param name="args"></param>
        private void ShowLevelDeclareUI(ShmupEventArgs args)
        {
            LevelArgs levelArgs = args as LevelArgs;

#if UNITY_EDITOR

            try
            {
                if (levelArgs != null)
                    StartCoroutine(LevelDeclareRoutine(levelArgs.LevelIndex));
            }catch (System.Exception)
            { }

#else
            if (levelArgs != null )
                StartCoroutine(LevelDeclareRoutine(levelArgs.LevelIndex));
#endif
        }

        /// <summary>
        /// show a text that indicate the level number.
        /// </summary>
        /// <param name="levelIndex">the level number.</param>
        /// <returns></returns>
        private IEnumerator LevelDeclareRoutine (int levelIndex )
        {
            yield return new WaitForSeconds(LevelDeclareDelay);
            LevelDeclare.gameObject.SetActive(true);
            LevelDeclare.text = "Level " + levelIndex.ToString();
            yield return new WaitForSeconds(LevelDeclareStay);
            LevelDeclare.gameObject.SetActive(false);
        }

        /// <summary>
        /// rescale the UI depending on the camera force aspect.
        /// </summary>
        private void AdjustUIToForceAspect ()
        {
            if (!OrthographicCamera.IsInitialize)
                return;

            OrthographicCamera Camera = OrthographicCamera.Instance;

            if (Camera.Aspect == OrthographicCamera.VerticalAspect.None)
                return;

            float UnUsedSpace = Camera.GetUnusedSpace(HUDCanvas);

            if (HUDCanvas != null)
                Camera.AdujstCanvasToForceAspect(HUDCanvas, UnUsedSpace);
            if (PauseGameCanvas != null)
                Camera.AdujstCanvasToForceAspect(PauseGameCanvas, UnUsedSpace);
            if (GameOverCanvas != null)
                Camera.AdujstCanvasToForceAspect(GameOverCanvas, UnUsedSpace);
            if (LevelCompleteCanvas != null)
                Camera.AdujstCanvasToForceAspect(LevelCompleteCanvas, UnUsedSpace);
            if (PhoneControlsCanvas != null)
                Camera.AdujstCanvasToForceAspect(PhoneControlsCanvas.gameObject.GetComponent<RectTransform>(), UnUsedSpace);
        }

        /// <summary>
        /// set the visibility of the phone UI controls.
        /// </summary>
        /// <param name="activePhoneCanvas">active the phone UI canvas</param>
        /// <param name="activeFireButton">active the fire button.</param>
        public void SetActivePhoneControls (bool activePhoneCanvas,bool activeFireButton)
        {
            PhoneControlsCanvas.enabled = activePhoneCanvas;
            _phoneControlsNeeded = activePhoneCanvas;

            if (activePhoneCanvas)
                PhoneFireButton.gameObject.SetActive(activeFireButton);
            else
                PhoneFireButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// hide the phone controls.
        /// </summary>
        private void HidePhoneControls ()
        {
            if (PhoneControlsCanvas != null)
                PhoneControlsCanvas.enabled = false;
        }

        /// <summary>
        /// show the phone controls.
        /// </summary>
        private void ShowPhoneControls()
        {
            if (PhoneControlsCanvas != null && _phoneControlsNeeded)
                PhoneControlsCanvas.enabled = true;
        }

        /// <summary>
        /// return the Joystick of the phone controls UI.
        /// </summary>
        public JoyStick GetJoyStick ()
        {
            return PhoneJoyStick;
        }

        /// <summary>
        /// return the fire button of the phone controls UI.
        /// </summary>
        public HoldButton GetFireButton ()
        {
            return PhoneFireButton;
        }


        /// <summary>
        /// Sets the HUD high score value.
        /// </summary>
        private void InitializeHUDHighScore()
        {
            if (HighScore != null)
            {
                HighScore.text = "High Score : " + (GameManager.Instance.CurrentHighScore().ToString());
            }           
        }


        /// <summary>
        /// Initialize the UI element for the Level Complete.
        /// </summary>
        private void InitializeLevelCompleteUI()
        {
            if (LevelCompleteCanvas != null)
                LevelCompleteCanvas.gameObject.SetActive(false);
            
            if (LevelCompleteCurrentScore != null)
                LevelCompleteCurrentScore.text = "";

            if (LevelCompleteHighScore != null)
                LevelCompleteHighScore.text = "";

            if (LevelCompleteMainButton != null)
                LevelCompleteMainButton.onClick.AddListener(MainMenu);

            if (LevelCompleteRetryButton != null)
                LevelCompleteRetryButton.onClick.AddListener(Retry);

            if (NextLevelButton != null)
                NextLevelButton.onClick.AddListener(NextLevel);
            
        }

        /// <summary>
        /// Initializes the UI element for Game Over.
        /// </summary>
        private void InitializeGameOverUI()
        {
            if (GameOverCanvas != null)
                GameOverCanvas.gameObject.SetActive(false);

            if (GameOverCurrentScore != null)
                GameOverCurrentScore.text = "";

            if (GameOverHighScore != null)
                GameOverHighScore.text = "";

            if (GameOverRetryButton != null)
                GameOverRetryButton.onClick.AddListener(Retry);

            if (GameOverMainButton != null)
                GameOverMainButton.onClick.AddListener(MainMenu);

        }

        /// <summary>
        /// Initializes the UI element for the Pause screen.
        /// </summary>
        private void InitializePauseUI()
        {
            if (PauseGameCanvas != null)
                PauseGameCanvas.gameObject.SetActive(false);

            if (ResumeButton != null)
                ResumeButton.onClick.AddListener(Resume);
            
            if (PauseGameMainButton != null)
                PauseGameMainButton.onClick.AddListener(MainMenu);

            if (PauseGameRetryButton != null)
                PauseGameRetryButton.onClick.AddListener(Retry);

            if (PauseMusicSlider != null)
                PauseMusicSlider.onValueChanged.AddListener(UpdateMusicVolume);

            if (PauseSoundFXSlider != null)
                PauseSoundFXSlider.onValueChanged.AddListener(UpdateSFXVolume);
        }

        /// <summary>
        /// update the music in the audio manager
        /// </summary>
        public void UpdateMusicVolume(float value)
        {
            AudioManager.Instance.MusicVolume = value;
        }

        /// <summary>
        /// update the SFX in the audio manager
        /// </summary>
        public void UpdateSFXVolume(float value)
        {
            AudioManager.Instance.SFXVolume = value;
        }

        /// <summary>
        /// Called when a player gets destroyed and a new player instance
        /// is created, reconnects the HUD UI to the new Instance.
        /// </summary>
        private void UpdateHUD(ShmupEventArgs args)
        {
            PlayerSpawnArgs playerSpawnArgs = args as PlayerSpawnArgs;

            if (playerSpawnArgs == null)
                return;

            if (HealthBar != null)
            {
                HealthBar.Target = playerSpawnArgs.PlayerComponent;
                HealthBar.Initialize();
            }

            if (ShieldBar != null)
            {
                ShieldBar.Target = playerSpawnArgs.PlayerComponent;
                ShieldBar.Initialize();
            }

            UpdatePlayerLive(null);
        }

        /// <summary>
        /// update the player live in the UI.
        /// </summary>
        private void UpdatePlayerLive (ShmupEventArgs args)
        {
            if (PlayerLives != null)
                PlayerLives.text = "X" + LevelController.Instance.GetPlayerLive();
        }

        /// <summary>
        /// Called when a player score is changed and updates the score in the HUD UI
        /// </summary>
        private void UpdateScore(ShmupEventArgs args)
        {
            PlayerScoreArgs playerScoreArgs = args as PlayerScoreArgs;

            if (playerScoreArgs == null)
                return;

            if (PlayerScore != null)
                PlayerScore.text = "Score : " + playerScoreArgs.CurrentScore.ToString();
        }
       
        /// <summary>
        /// Called when a player is out of lives, enables the GameOver UI.
        /// </summary>
        private void GameOver(ShmupEventArgs args)
        {
            int highScore = GameManager.Instance.CurrentHighScore();
            int currentPlayerScore = LevelController.Instance.PlayerScore;

            // To check and save the high score when the game is over.
            if (currentPlayerScore > highScore)
            {
                highScore = currentPlayerScore;
                GameManager.Instance.CheckAssignHighScore(currentPlayerScore);
            }

            // Enables the game over UI and loads it with the high score and current score.
            StartCoroutine(EnableGameOverUI(GameManager.Instance.CurrentHighScore(), LevelController.Instance.PlayerScore, 2f));
        }

        /// <summary>
        /// called when a player completes the level, enables the LevelComplete UI.
        /// </summary>
        private void LevelComplete(ShmupEventArgs args)
        {
            if(LevelController.IsInitialize)
                StartCoroutine(EnableLevelCompleteUI( GameManager.Instance.CurrentHighScore(), LevelController.Instance.PlayerScore, 2f));
        }

        /// <summary>
        /// Pauses time and enables the pause game UI.
        /// </summary>
        public void Pause()
        {
            if (!_allowPause)
                return;

            if (PauseGameCanvas != null)
            {
                PauseGameCanvas.gameObject.SetActive(true);
                InputManager.SetSelectedUI(ResumeButton.gameObject);
            }

            TimeManager.Pause();

            if (AudioManager.IsInitialize)
                AudioManager.Instance.PauseMusic();

            HidePhoneControls();

            RiseOnUIEvent ( new LevelUIEventArg( LevelUIEvents.PauseButtonPressed ) );
        }

        /// <summary>
        /// Resumes the time and disables the pause game UI.
        /// </summary>
        public void Resume()
        {
            TimeManager.UnPause();

            if (AudioManager.IsInitialize)
                AudioManager.Instance.UnPauseMusic();

            ShowPhoneControls();

            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.ResumeButtonPressed));

            PauseGameCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables the GameOver UI.
        /// </summary>
        /// <param name="highScore">the high score for this level</param>
        /// <param name="currentScore">the current score for the player.</param>
        /// <param name="delay">delay in seconds before enabling this method</param>
        private IEnumerator EnableGameOverUI( float highScore , float currentScore, float delay)
        {
            Resume();
            _allowPause = false;

            HidePhoneControls();

            yield return new WaitForSeconds(delay);

            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.GameOverCanvasAppear));

            if (GameOverCanvas != null)
            {
                GameOverCanvas.gameObject.SetActive(true);
                InputManager.SetSelectedUI(GameOverRetryButton.gameObject);
            }

            if (GameOverCurrentScore != null)
                GameOverCurrentScore.text = currentScore.ToString();
            if (GameOverHighScore != null)
                GameOverHighScore.text = highScore.ToString();
        }

        /// <summary>
        /// Enable the LevelComplete UI.
        /// </summary>
        /// <param name="highScore">the high score for this level</param>
        /// <param name="currentScore">the current score for the player.</param>
        /// <param name="delay">delay in seconds before enable this method</param>
        private IEnumerator EnableLevelCompleteUI(float highScore, float currentScore , float delay)
        {
            Resume();
            _allowPause = false;

            HidePhoneControls();

            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.LevelCompleteAppear));

            yield return new WaitForSeconds(delay);

            if (LevelCompleteCanvas != null)
            {
                LevelCompleteCanvas.gameObject.SetActive(true);
                InputManager.SetSelectedUI(NextLevelButton.gameObject);
            }

            LevelCompleteCurrentScore.text = currentScore.ToString();
            LevelCompleteHighScore.text = highScore.ToString();
        }

        /// <summary>
        /// go to the main menu scene.
        /// </summary>
        public void MainMenu()
        {
            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.MainMenuButtonPressed));
            GameManager.Instance.ToMainScene();
            TimeManager.UnPause();
        }

        /// <summary>
        /// reload the current level.
        /// </summary>
        public void Retry()
        {
            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.RetryButtonPressed));
            GameManager.Instance.ReloadLevel();
            TimeManager.UnPause();
        }

        /// <summary>
        /// load the next level if possible.
        /// </summary>
        public void NextLevel()
        {
            RiseOnUIEvent(new LevelUIEventArg(LevelUIEvents.NextLevelButtonPressed));
            GameManager.Instance.ToNextLevel();
        }

        /// <summary>
        /// handles the rise of the OnUIEvent.
        /// </summary>
        /// <param name="uiEvent">the type of UI event that rise this event.</param>
        private void RiseOnUIEvent(LevelUIEventArg uiEvent)
        {
            if (OnUIEvent != null)
                OnUIEvent(uiEvent);
        }

    }

}
