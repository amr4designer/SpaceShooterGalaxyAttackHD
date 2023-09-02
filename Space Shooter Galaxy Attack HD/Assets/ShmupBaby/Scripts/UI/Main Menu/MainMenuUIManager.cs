using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShmupBaby
{
    /// <summary>
    /// a list of Options for player control.
    /// </summary>
    public enum InputMethod
    {
		Controls,
        FollowTouch
    }

    /// <summary>
    /// a list of all Main Menu UI Events.
    /// </summary>
    public enum MainUIEvents
    {
        ButtonPressed,
        ToggleChange
    }

    /// <summary>
    /// Data structure which defines what to be passed when MainUIEvent is triggered.
    /// </summary>
    public class MainUIEventArg : ShmupEventArgs
    {
        public MainUIEvents UIEvent;

        public MainUIEventArg(MainUIEvents uIEvent)
        {
            UIEvent = uIEvent;
        }
    }

    /// <summary>
    /// Controls all the UI Canvas in the Main Menu.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Main Menu/Main Menu UI Manager")]
    public class MainMenuUIManager : Singleton<MainMenuUIManager>
    {

        /// <summary>
        /// transform component for Main Menu UI Canvas.
        /// </summary>
        [Header("Main Menu")]
        [Space]
        [Tooltip("Plug the Main Menu canvas that contains all the Main Menu UI Element.")]
        public RectTransform MainMenuCanvas;
        /// <summary>
        /// New Game Button for the Main Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the New Game Button for the Main Menu UI")]
        public Button NewGameButton;
        /// <summary>
        /// Load Game Button for the Main Menu UI.
        /// </summary>
        [Tooltip("Plug the LoadGame Button for the Main Menu UI")]
        public Button LoadGameButton;
        /// <summary>
        /// Player Select Button for the Main Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the Player Select Button for the Main Menu UI")]
        public Button ShipSelectButton;
        /// <summary>
        /// Settings Button for the Main Menu UI.
        /// </summary>
        [Tooltip("Plug the Settings Button for the Main Menu UI")]
        public Button SettingsButton;
        /// <summary>
        /// Quit Game Button for the Main Menu UI.
        /// </summary>
        [Tooltip("Plug the QuitGame Button for the Main Menu UI")]
        public Button QuitGameButtons;


        /// <summary>
        /// Transform component to Load Menu UI Canvas.
        /// </summary>
        [Header("Load Menu")]
        [Space]
        [Tooltip("Plug the Load Menu canvas that contains the Load Menu UI Elements")]
        public RectTransform LoadGameCanvas;
        /// <summary>
        /// the template button that represents the levels.
        /// </summary>
        [Space]
        [Tooltip("Plug the Button to be used to present the game stages")]
        public RectTransform LevelButton;
        /// <summary>
        /// Back Button for the Load Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the Back Button for the Load Menu UI")]
        public Button LoadGameBackButton;
        /// <summary>
        /// the canvas that will contain the Button for game levels for the Load Menu UI.
        /// </summary>
        [Space]
        [Tooltip("The canvas that will contain the Button for game levels")]
        public RectTransform Content;
        /// <summary>
        /// space between the levels buttons of the load Menu UI.
        /// </summary>
        [Space]
        [Tooltip("space between the levels buttons")]
        public float SpaceBetween;

        /// <summary>
        /// transform component to Settings Menu UI Canvas.
        /// </summary>
        [Header("Settings Menu")]
        [Space]
        [Tooltip("plug the Settings Menu canvas that contain all the Settings Menu UI Element")]
        public RectTransform SettingsCanvas;
        /// <summary>
        /// the Music Slider for the Settings Menu UI.
        /// </summary>
        [Space]
        [Tooltip("Plug the Music Slider for the Settings Menu UI")]
        public Slider MusicSlider;
        /// <summary>
        /// The SFX Slider for the Settings Menu UI.
        /// </summary>
        [Tooltip("Plug the SFX Slider for the Settings Menu UI")]
        public Slider SoundFXSlider;
        /// <summary>
        /// The Next Control Button for the Settings Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the next Input Control Button for the Settings Menu UI")]
        public Button NextControlButton;
        /// <summary>
        /// the Previous Control Button for the Settings Menu UI.
        /// </summary>
        [Tooltip("plug the Previous Input Control Button for the Settings Menu UI")]
        public Button PreviousControlButton;
        /// <summary>
        /// the text that will represent the selected Input Method for the Settings Menu UI.
        /// </summary>
        [Tooltip("plug the text that will represent the selected Input Method for the Settings Menu UI")]
        public Text ControlMethodText;
        /// <summary>
        /// the Auto Fire Toggle for the Settings UI Menu.
        /// </summary>
        [Space]
        [Tooltip("plug the Auto Fire Toggle for the Settings Menu UI")]
        public Toggle AutoFireToggle;
        /// <summary>
        /// Back Button for the Settings Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the Back Button for the Settings Menu UI")]
        public Button SettingsBackButton;

        /// <summary>
        /// transform component for the Quit Menu UI Canvas.
        /// </summary>
        [Header("Quit Menu")]
        [Space]
        [Tooltip("plug the Quit Menu canvas that contains all the Quit Menu UI Element")]
        public RectTransform QuitCanvas;
        /// <summary>
        /// Accept Button for the Quit Menu UI.
        /// </summary>
        [Space]
        [Tooltip("plug the Accept Button for the Quit Menu UI")]
        public Button AcceptButton;
        /// <summary>
        /// Decline Button for the Quit Menu UI.
        /// </summary>
        [Tooltip("plug the Decline Button for the Quit Menu UI")]
        public Button DeclineButton;

        /// <summary>
        /// triggered when one of the UI events Occur.
        /// </summary>
        public static event ShmupDelegate OnUIEvent;

        /// <summary>
        /// the index of the selected Input method.
        /// </summary>
        private int _availableMethodsIndex;
        /// <summary>
        /// the button of the first level.
        /// </summary>
        private LevelSelectButton FirstLevelLoadButton;
        /// <summary>
        /// the available Input method for this game. 
        /// </summary>
        private readonly InputMethod[] _availableMethods = new InputMethod[]
        {
            InputMethod.Controls,
            InputMethod.FollowTouch
        };
        /// <summary>
        /// selected input method for this game.
        /// </summary>
        private InputMethod SelectedInputMethod
        {
            get { return _availableMethods[_availableMethodsIndex]; }
            set
            {
                for (int i = 0; i < _availableMethods.Length; i++)
                {
                    if (value == _availableMethods[i])
                    {
                        _availableMethodsIndex = i;
                        if (ControlMethodText.text != null)
                            ControlMethodText.text = SelectedInputMethod.ToString();
                        break;
                    }
                }
            }
        }
                
        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
			// Sets the Canvases of the main menu and sets the navigation to the main menu canvas
            SetupMainMenuCanvas();

            SetupLoadGameCanvas();

            SetupSettingsCanvas();

            SetupQuitCanvas();

            MainMenu();

        }

        /// <summary>
        /// Initializes the UI element for the Main Menu.
        /// </summary>
        void SetupMainMenuCanvas()
        {
            if (NewGameButton != null)
                NewGameButton.onClick.AddListener(NewGame);

            if (ShipSelectButton != null)
                ShipSelectButton.onClick.AddListener(ShipSelect);

            if (LoadGameButton != null)
                LoadGameButton.onClick.AddListener(LoadGame);

            if (SettingsButton != null)
                SettingsButton.onClick.AddListener(Settings);

            if (QuitGameButtons != null)
                QuitGameButtons.onClick.AddListener(Quit);
        }

        /// <summary>
        /// Initializes the UI element for the Settings.
        /// </summary>
        void SetupSettingsCanvas()
        {

            if (SettingsBackButton != null)
            {
                SettingsBackButton.onClick.AddListener(MainMenu);
                SettingsBackButton.onClick.AddListener(SaveSettings);
            }

            if ( NextControlButton != null)
                NextControlButton.onClick.AddListener(NextControlOption);

            if (PreviousControlButton != null)
                PreviousControlButton.onClick.AddListener(PreviousControlOption);

            if (AutoFireToggle != null)
                AutoFireToggle.onValueChanged.AddListener(AutoFireChange);

            if (MusicSlider != null)
                MusicSlider.onValueChanged.AddListener(UpdateMusicVolume);

            if (SoundFXSlider != null)
                SoundFXSlider.onValueChanged.AddListener(UpdateSFXVolume);
        }

        /// <summary>
        /// Initializes the UI element for the Quit button.
        /// </summary>
        void SetupQuitCanvas()
        {
            if (AcceptButton != null)
                AcceptButton.onClick.AddListener(AcceptQuit);

            if (DeclineButton != null)
                DeclineButton.onClick.AddListener(MainMenu);
        }

        /// <summary>
        /// Initializes the UI element for the Load Game.
        /// </summary>
        void SetupLoadGameCanvas()
        {
            SetupLevelSelectButtons();

            if (LoadGameBackButton != null)
                LoadGameBackButton.onClick.AddListener(MainMenu);
        }

        /// <summary>
        /// Setup the level select buttons.
        /// </summary>
        private void SetupLevelSelectButtons()
        {
            int levelsNumber = GameManager.Instance.LevelsNumber;

            ShmupSaveData.SavedLevel[] levels = GameManager.Instance.SaveData.Levels;

            MoveRectBotDown( Content ,0.15f * (levelsNumber - 1));

            //indicate this is the first time to play the game.
            if (GameManager.Instance.PlayerSelectedV == null)
            {
                GameManager.Instance.NewGame = true;
            }

            for (int i = 0; i < levelsNumber; i++)
            {
                if (levels[i].Statue != ShmupSaveData.LevelStatue.NotAvailable)
                    if (i==0)
                        FirstLevelLoadButton = CreateLevelsButton(levels[i].HighScore, i+1,true);
                    else
                        CreateLevelsButton(levels[i].HighScore, i + 1, true);
                else
                    CreateLevelsButton(levels[i].HighScore, i+1, false);
            }

            LevelButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Creates a level button.
        /// </summary>
        /// <param name="levelScore">the highest score for the button level.</param>
        /// <param name="levelIndex">the index of the button level.</param>
        /// <param name="interactable">set the button interact able</param>
        private LevelSelectButton CreateLevelsButton(int levelScore, int levelIndex,bool interactable)
        {
            RectTransform levelButton = Instantiate<RectTransform>(LevelButton, LevelButton.transform.parent);

            Vector2 offsetMax = levelButton.offsetMax;
            Vector2 offsetMin = levelButton.offsetMin;

            float displacementOnY = (levelIndex - 1) *( SpaceBetween + levelButton.rect.size.y);

            offsetMin.y -= displacementOnY;
            offsetMax.y -= displacementOnY;

            levelButton.offsetMax = offsetMax;
            levelButton.offsetMin = offsetMin;

            levelButton.name = "Level " + levelIndex.ToString();

            LevelSelectButton buttonScript = levelButton.GetComponent<LevelSelectButton>();

            if (buttonScript != null)
                buttonScript.LevelIndex = levelIndex - 1;

                Button button = levelButton.GetComponent<Button>();
                button.interactable = interactable;

            Text buttonText = levelButton.GetComponentInChildren<Text>();
            
            if (buttonText != null)
                buttonText.text = "Level " + (levelIndex).ToString() + " " + "Score :" + levelScore.ToString();

            levelButton.parent = Content.transform;

            return buttonScript;
        }

        /// <summary>
        /// Moves the bottom edge down for a rectangular transform.
        /// </summary>
        /// <param name="rect">the rectangle which will be scaled.</param>
        /// <param name="value">the value in world units for moving the edge.</param>
        private void MoveRectBotDown(RectTransform rect, float value)
        {
            Vector2 contentAnchorMin = rect.anchorMin;

            contentAnchorMin.y -= value;

            rect.anchorMin = contentAnchorMin;
        }

        /// <summary>
        /// gathers the settings from the UI and saves them.
        /// </summary>
        public void SaveSettings()
        {
            ShmupGameSettings settings = GameManager.Instance.GameSettings;

            if (MusicSlider!= null)
                settings.MusicVolume = MusicSlider.value;
            if (SoundFXSlider != null)
                settings.SFXVolume = SoundFXSlider.value; 
            if (AutoFireToggle != null)
                settings.AutoFire = AutoFireToggle.isOn;
            settings.IMethod = SelectedInputMethod;

            GameManager.Instance.SaveGameSettings();
        }

        /// <summary>
        /// adjusts the UI settings to match the saved one.
        /// </summary>
        public void LoadSettings()
        {
            ShmupGameSettings settings = GameManager.Instance.GameSettings;

            if (MusicSlider != null)
                MusicSlider.value = settings.MusicVolume;
            if (SoundFXSlider != null)
                SoundFXSlider.value = settings.SFXVolume;
            if (AutoFireToggle != null)
                AutoFireToggle.isOn = settings.AutoFire;
            SelectedInputMethod = settings.IMethod;
            InputManager.SetInputMethod(SelectedInputMethod);
            InputManager.AutoFire = settings.AutoFire;
        }

        /// <summary>
        /// update the master in the audio manager.
        /// </summary>
        private void UpdateMasterVolume(float value)
        {
            AudioManager.Instance.MasterVolume = value;
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
        /// called when Auto Fire Toggle is changed, raises the Toggle change event.
        /// </summary>
        /// <param name="value">the value of the change slider</param>
        public void AutoFireChange(bool value)
        {
            RiseOnUIEvent(MainUIEvents.ToggleChange);
            InputManager.AutoFire = value;
        }

        /// <summary>
        /// change the SelectedInputMethod to the next one.
        /// </summary>
        public void NextControlOption()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);
            SelectedInputMethod++;
            InputManager.SetInputMethod(SelectedInputMethod);
        }

        /// <summary>
        /// changes the SelectedInputMethod to the previous one.
        /// </summary>
        public void PreviousControlOption()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);
            SelectedInputMethod--;
            InputManager.SetInputMethod(SelectedInputMethod);
        }

        /// <summary>
        /// sets the UI to the MainMenu.
        /// </summary>
        public void MainMenu()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            MainMenuCanvas.gameObject.SetActive(true);
            LoadGameCanvas.gameObject.SetActive(false);
            SettingsCanvas.gameObject.SetActive(false);
            QuitCanvas.gameObject.SetActive(false);

            InputManager.SetSelectedUI(NewGameButton.gameObject);
        }

        /// <summary>
        /// start a new Game.
        /// </summary>
        public void NewGame()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            GameManager.Instance.NewGame = true;
            GameManager.Instance.ToShipSelectScene();
        }

        /// <summary>
        /// goes to the player select scene.
        /// </summary>
        public void ShipSelect()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            GameManager.Instance.ToShipSelectScene();
        }

        /// <summary>
        /// sets the UI to the Load Game menu.
        /// </summary>
        public void LoadGame()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            LoadSettings();

            MainMenuCanvas.gameObject.SetActive(false);
            LoadGameCanvas.gameObject.SetActive(true);
            SettingsCanvas.gameObject.SetActive(false);
            QuitCanvas.gameObject.SetActive(false);

            InputManager.SetSelectedUI(FirstLevelLoadButton.gameObject);
        }

        /// <summary>
        /// sets the UI to the Settings menu.
        /// </summary>
        public void Settings()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            MainMenuCanvas.gameObject.SetActive(false);
            LoadGameCanvas.gameObject.SetActive(false);
            SettingsCanvas.gameObject.SetActive(true);
            QuitCanvas.gameObject.SetActive(false);

            InputManager.SetSelectedUI(MusicSlider.gameObject);

            LoadSettings();
        }

        /// <summary>
        /// sets the UI to the Quit menu.
        /// </summary>
        public void Quit()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            MainMenuCanvas.gameObject.SetActive(false);
            LoadGameCanvas.gameObject.SetActive(false);
            SettingsCanvas.gameObject.SetActive(false);
            QuitCanvas.gameObject.SetActive(true);

            InputManager.SetSelectedUI(AcceptButton.gameObject);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void AcceptQuit()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);

            Application.Quit();
        }

        /// <summary>
        /// go to the credits scene.
        /// </summary>
        void Credit()
        {
            RiseOnUIEvent(MainUIEvents.ButtonPressed);
        }
        
        /// <summary>
        /// handles the rise of the OnUIEvent.
        /// </summary>
        /// <param name="uiEvent">The type of UI event that raises this event.</param>
        private void RiseOnUIEvent(MainUIEvents uiEvent)
        {
            if (OnUIEvent != null)
                OnUIEvent(new MainUIEventArg(uiEvent));
        }

    }

}