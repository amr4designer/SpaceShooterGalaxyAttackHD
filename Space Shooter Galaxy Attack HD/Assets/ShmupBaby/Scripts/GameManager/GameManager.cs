using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace  ShmupBaby
{

    /// <summary>
    /// Defines the data that's going to be saved on the disk for this package.
    /// </summary>
    [System.Serializable]
    public class ShmupSaveData
    {
        public enum LevelStatue
        {
            NotAvailable,
            Completed,
            OnGoing
        }

        [System.Serializable]
        public class SavedLevel
        {
            public LevelStatue Statue = LevelStatue.NotAvailable;
            public int HighScore;
        }

        /// <summary>
        /// Selected player prefab.
        /// </summary>
        public PlayerObject Ship;

        /// <summary>
        /// The statue of the game levels
        /// </summary>
        public SavedLevel[] Levels;

        public ShmupSaveData(int levelNumber)
        {
            if (levelNumber < 1)
                return;

            SavedLevel[] saveLevelData = new SavedLevel[levelNumber];

            for (int i = 0; i < levelNumber; i++)
            {
                saveLevelData[i] = new SavedLevel();
            }

            saveLevelData[0].Statue = LevelStatue.OnGoing;

            Levels = saveLevelData;
        }
    }

    /// <summary>
    /// Defines the data that's going to be saved on disk for the package settings.
    /// </summary>
    [System.Serializable]
    public class ShmupGameSettings
    {
        /// <summary>
        /// Master volume.
        /// </summary>
        public float MasterVolume = 1f;

        /// <summary>
        /// Music volume.
        /// </summary>
        public float MusicVolume = 1f;

        /// <summary>
        /// SFX volume.
        /// </summary>
        public float SFXVolume = 1f;
        
        /// <summary>
        /// Type of Input method.
        /// </summary>
        public InputMethod IMethod ;

        /// <summary>
        /// Enables auto fire for the player.
        /// </summary>
        public bool AutoFire = false;

    }

    /// <summary>
	/// Container for a selected player prefab for a different level view. (I.e. horizontal or vertical)
    /// </summary>
    [System.Serializable]
    public class PlayerObject
    {
        /// <summary>
        /// Player prefab for the vertical level.
        /// </summary>
        public Object Vertical;
        /// <summary>
        /// Player Prefab for the horizontal level.
        /// </summary>
        public Object Horizontal;
    }

    /// <summary>
    /// Manages the general settings for the game.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Main Menu/Game Manager")]
    public sealed class GameManager : PersistentSingleton<GameManager>
    {

        /// <summary>
        /// The build index for the first level.
        /// </summary>
		[Tooltip("The build index for the first level, the levels between should be " +
			"sorted in order in the build settings.")]
        public int MinLevelSceneIndex;
        /// <summary>
        /// The build index for the last level.
        /// </summary>
        [Tooltip("The build index for the last level, the levels between should be " +
                 "sorted in order in the build settings.")]
        public int MaxLevelSceneIndex;
        /// <summary>
        /// The build index for the Main Menu.
        /// </summary>
        [Space]
        [Tooltip("The build index for the Main Menu.")]
        public int MainSceneIndex;
        /// <summary>
        /// The build index for the player Selecting Menu.
        /// </summary>
        [Tooltip("The build index for the player selecting Menu.")]
        public int ShipSelectSceneIndex;
        
        /// <summary>
        /// The data saved on  disk, related to the player progress and player selecting choice.
        /// </summary>
        public ShmupSaveData SaveData
        {
            get { return _savedData; }
        }

        /// <summary>
        /// The data saved on disk, related to the game settings.
        /// </summary>
        public ShmupGameSettings GameSettings
        {
            get { return _gameSettings; }
        }

        /// <summary>
        /// Selected player prefab for the vertical view.
        /// </summary>
        public Object PlayerSelectedV
        {
            get {
                if (_selected == null)
                    return null;
                else
                    return _selected.Vertical;
            }
        }

        /// <summary>
        /// Selected player prefab the horizontal view.
        /// </summary>
        public Object PlayerSelectedH
        {
            get
            {
                if (_selected == null)
                    return null;
                else
                    return _selected.Horizontal;
            }
        }

        /// <summary>
        /// Number of levels in the game.
        /// </summary>
        public int LevelsNumber { get; set; }
        /// <summary>
        /// indicate if the user need to start a new game.
        /// </summary>
        public bool NewGame { get; set; }

        /// <summary>
        /// Back-end field for SaveData.
        /// </summary>
        private ShmupSaveData _savedData;
        /// <summary>
        /// Back-end field for GameSettings.
        /// </summary>
        private ShmupGameSettings _gameSettings;
        /// <summary>
        /// Reference to the player selected Avatar.
        /// </summary>
        private PlayerObject _selected;

        /// <summary>
        /// Name of the save file that contains the player progress and player select choice.
        /// </summary>
        private const string SaveFileName = "Save Data";
        /// <summary>
        /// Name of the save file that contains the game general settings.
        /// </summary>
        private const string GameSettingsFileName = "Game Settings";

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            LevelsNumber = MaxLevelSceneIndex - MinLevelSceneIndex  + 1;

            SaveLoadManager.CheckForDirectory();

            LoadSavedData();

            LoadGameSettings();
        }

        /// <summary>
        /// Load the save data using SaveFileName, assigns data if the file didn't exist.
        /// </summary>
        private void LoadSavedData()
        {
            _savedData = SaveLoadManager.Load<ShmupSaveData>(SaveFileName);

            if (_savedData == null)
            {
                _savedData = new ShmupSaveData(LevelsNumber);
                return;
            }

            if (_savedData.Levels.Length != LevelsNumber)
            {
                _savedData = new ShmupSaveData(LevelsNumber);
                return;
            }

            _selected = _savedData.Ship;
        }

        /// <summary>
        /// Loads game setting data, assigns settings if the save file didn't exists.
        /// </summary>
        private void LoadGameSettings()
        {
            _gameSettings = SaveLoadManager.Load<ShmupGameSettings>(GameSettingsFileName);

            if (_gameSettings == null)
            {
                _gameSettings = new ShmupGameSettings();
            }               
        }

        /// <summary>
        /// Saves the save data using SaveFileName.
        /// </summary>
        private void SaveSavedData()
        {
            SaveLoadManager.Save(SaveFileName, _savedData);
        }

        /// <summary>
        /// Saves the game settings using SaveFileName.
        /// </summary>
        public void SaveGameSettings()
        {
            SaveLoadManager.Save(GameSettingsFileName, _gameSettings);
        }

        /// <summary>
        /// Changes the selected player avatar and saves the changes to disk.
        /// </summary>
        /// <param name="player">New selected avatar.</param>
        public void AssignSelectedShip(PlayerObject player)
        {
            _selected = player;

            _savedData.Ship = player;

            SaveSavedData();
        }

        /// <summary>
		/// Checks if the given high score is greater than the saved one, if it is;
        /// it will replace the old one and save it on disk.
        /// </summary>
        /// <param name="highScore">The high score for the current level.</param>
        /// <returns>True if it's greater than the old high score for the current level.</returns>
        public bool CheckAssignHighScore(int highScore)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex >= MinLevelSceneIndex && sceneIndex <= MaxLevelSceneIndex)
            {
                int levelIndex = sceneIndex - MinLevelSceneIndex;
                _savedData.Levels[levelIndex].HighScore = highScore;
                _savedData.Levels[levelIndex].Statue = ShmupSaveData.LevelStatue.Completed;
                if (levelIndex+1 < MaxLevelSceneIndex)
                {
                    _savedData.Levels[levelIndex + 1].Statue = ShmupSaveData.LevelStatue.OnGoing;
                }
                SaveSavedData();
                return true;
            }

            return false;

        }

        /// <summary>
        /// return the statue for the current level.
        /// </summary>
        public ShmupSaveData.LevelStatue GetCurrentLevelStatue()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            int levelIndex = sceneIndex - MinLevelSceneIndex;
            if (levelIndex >= 0 && levelIndex <= MaxLevelSceneIndex - MinLevelSceneIndex)
            {
                return _savedData.Levels[levelIndex].Statue;
            }
            else
            {
                return ShmupSaveData.LevelStatue.NotAvailable;
            }                
        }

        /// <summary>
        /// Gets the saved high score for the current level.
        /// </summary>
        public int CurrentHighScore()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            int highScore = 0;

            if (sceneIndex >= MinLevelSceneIndex && sceneIndex <= MaxLevelSceneIndex)
                highScore = _savedData.Levels[sceneIndex - MinLevelSceneIndex].HighScore;

            return highScore;
        }

        /// <summary>
        /// Loads the Main Menu Scene.
        /// </summary>
        public void ToMainScene()
        {
            StartCoroutine(LoadLevel(MainSceneIndex));
        }
        /// <summary>
        /// Loads the Ship Select Scene.
        /// </summary>
        public void ToShipSelectScene()
        {
            StartCoroutine(LoadLevel(ShipSelectSceneIndex));
        }
        /// <summary>
        /// Loads the first level Scene.
        /// </summary>
        public void ToFirstLevel()
        {
            StartCoroutine(LoadLevel(MinLevelSceneIndex));
        }
        /// <summary>
        /// Loads the next level scene.
        /// </summary>
        public void ToNextLevel()
        {
            int nextLevelSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextLevelSceneIndex > MaxLevelSceneIndex )
                return;

            StartCoroutine(LoadLevel(nextLevelSceneIndex));
        }
        /// <summary>
        /// Reloads the active scene.
        /// </summary>
        public void ReloadLevel()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
        }

        /// <summary>
        /// Loads a level scene by the level index.
        /// </summary>
        /// <param name="levelIndex">The index of the level(build index).</param>
        /// <returns>True if the index is valid.</returns>
        public bool ToLevel(int levelIndex)
        {

            if (levelIndex >= MinLevelSceneIndex && levelIndex <= MaxLevelSceneIndex)
            {
                StartCoroutine(LoadLevel(levelIndex));
                return true;
            }
            else
                return false;

        }
                
        /// <summary>
        /// Loads a level scene by the level index, activate the scene when it's done loading.
        /// </summary>
        /// <param name="sceneIndex">The index of the level (build index).</param>
        private IEnumerator LoadLevel(int sceneIndex)
        {
            CreateBlackScreen();

            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneIndex);

            while (!asyncLoadLevel.isDone)
            {
                print("Loading the Scene");
                yield return null;
            }

        }

        /// <summary>
        /// return the index of the current level or -1 if the current scene isn't a level.
        /// </summary>
        public int GetCurrentLevelIndex ()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex >= MinLevelSceneIndex && currentSceneIndex <= MaxLevelSceneIndex)
                return currentSceneIndex - MinLevelSceneIndex + 1;
            else
                return -1;
        }

        /// <summary>
        /// create black image that cover all the screen.
        /// </summary>
        private void CreateBlackScreen()
        {
            GameObject blackScreen = new GameObject("black screen");

            Canvas blackScreenCanavas = blackScreen.AddComponent<Canvas>();
            blackScreenCanavas.renderMode = RenderMode.ScreenSpaceOverlay;

            RectTransform blackScreenRect = blackScreen.GetComponent<RectTransform>();
            blackScreenRect.offsetMin = Vector2.zero;
            blackScreenRect.offsetMax = Vector2.one;

            blackScreen.AddComponent<Image>().color = Color.black;
        }

    }

}
