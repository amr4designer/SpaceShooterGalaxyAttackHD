using UnityEngine;
using System.Collections;

namespace ShmupBaby
{
    /// <summary>
    /// Defines what data to be passed when the player spawns.
    /// </summary>
    public class PlayerSpawnArgs : ShmupEventArgs
    {
        /// <summary>
        /// The player component for the new player instance.
        /// </summary>
        public Player PlayerComponent;
        /// <summary>
        /// Player's remaining lives.
        /// </summary>
        public int PlayerLives;

        /// <summary>
        /// PlayerSpawnArgs Constructor.
        /// </summary>
        /// <param name="playerComponent">The player component for the new player instance.</param>
        /// <param name="playerLives">Player remaining lives.</param>
        public PlayerSpawnArgs(Player playerComponent, int playerLives)
        {
            PlayerComponent = playerComponent;
            PlayerLives = playerLives;
        }
    }

    /// <summary>
    /// Defines what data to be passed when the player score changes.
    /// </summary>
    public class PlayerScoreArgs : ShmupEventArgs
    {
        /// <summary>
        /// The player's new score.
        /// </summary>
        public int CurrentScore;

        /// <summary>
        /// PlayerScoreArgs Constructor.
        /// </summary>
        /// <param name="currentScore">The player new score.</param>
        public PlayerScoreArgs(int currentScore)
        {
            CurrentScore = currentScore;
        }
    }

    /// <summary>
    /// The interface for level controller.
    /// </summary>
    public interface ILevelControl
    {
        /// <summary>
        /// This event should rise whenever a player spawns.
        /// </summary>
        event ShmupDelegate OnPlayerSpawn;
        /// <summary>
        /// This event should rise whenever a player get extra live.
        /// </summary>
        event ShmupDelegate OnPlayerGetExtraLife;
        /// <summary>
        /// This event should rise when the player is out of lives.
        /// </summary>
        event ShmupDelegate OnPlayerOutOfLives;
        /// <summary>
        /// This event should rise when the level start.
        /// </summary>
        event ShmupDelegate OnLevelStart;
        /// <summary>
        /// This event should rise when the player score changes.
        /// </summary>
        event ShmupDelegate OnScoreChange;

        /// <summary>
        /// The GameObject which all enemies spawn under.
        /// </summary>
        GameObject EnemyParent { get; }
        /// <summary>
        /// The GameObject which all PickUps spawn under.
        /// </summary>
        GameObject PickUpParent { get; }

        /// <summary>
        /// The player component for the current player.
        /// </summary>
        Player PlayerComponent { get; }

        /// <summary>
        /// The field that defines the enemy spawn area,also used by the
        /// DestroyByRegion to destroy any GameObject leaving it.
        /// </summary>
        Rect GameField { get; }
        /// <summary>
        /// The field where the player is allowed to move in.
        /// </summary>
        Rect PlayerBound { get; }
        /// <summary>
        /// The view type of the level.
        /// </summary>
        LevelViewType View { get; }       
    }

    /// <summary>
    /// Defines the wave spawn behavior.
    /// </summary>
	public interface ISpawnWave
    {
        /// <summary>
        /// This event should rise when wave creation starts.
        /// </summary>
        event ShmupDelegate OnWavesSpawnStart;
        /// <summary>
        /// This event should rise when a new wave has been spawned.
        /// </summary>
		event ShmupDelegate OnWaveSpawn;
	}

    /// <summary>
    /// Data structure of enemy wave creation.
    /// </summary>
    [System.Serializable]
    public class WaveCreationData
    {
        /// <summary>
        /// The index of this wave, represents the wave position in the z axis.
        /// </summary>
        [Tooltip("The index of this wave, represents the wave position in the z axis.")]
        [Range(0, 100)]
        public int LayerIndex;
        /// <summary>
        /// Reference to the wave prefab.
        /// </summary>
        [Tooltip("the wave prefab")]
        public Object Wave;
        /// <summary>
        /// Time in seconds for spawning the next wave.
        /// </summary>
		[Tooltip("Time in seconds for spawning the next wave.")]
        public float TimeForNextWave;
    }

    /// <summary>
    /// The bass class for the level controller, the level controller connects all the main
	/// level elements and defines the general level settings.
    /// </summary>
    [AddComponentMenu("")]
	public class LevelController : Singleton<LevelController>, ISpawnWave, ILevelControl
    {
        /// <summary>
        /// Options for setting the player boundary.
        /// </summary>
	    public enum PlayerBoundOptions
	    {
	        OffsetGameField,
            ByPlayerMover	        
	    }

        /// <summary>
        /// Options for setting the GameField.
        /// </summary>
	    public enum GameFieldOptions
	    {
            OffsetCameraView ,
            Background,
            Input
	    }

        /// <summary>
        /// Options for setting the player input.
        /// </summary>
        public enum LevelInputOptions
        {
            ByGameManager,
            Controls,
            FollowTouch
        }

        /// <summary>
        /// Options for setting the player input.
        /// </summary>
        public enum LevelFireOptions
        {
            ByGameManager,
            AutoFire,
            ManualFire
        }

        /// <summary>
        /// Options for setting the PlayerPrefab.
        /// </summary>
        public enum PlayerPrefabOptions
        {
            Input,
            ByGameManager
        }

        /// <summary>
        /// The start time for wave creation, from the beginning of this level.
        /// </summary>
        [Space]
        [Tooltip("The start time for wave creation")]
		public float WaveStartTime;
        /// <summary>
        /// The start time for the player first spawn in the scene, from the beginning of this level.
        /// </summary>
        [Space]
        [Tooltip("The start time for the player first spawn")]
        public float PlayerFirstSpawnTime;

        /// <summary>
        /// The space between layers, background layers,enemy and player.
        /// </summary>
        [Space]
		[Space]
		[Tooltip("Space between each layer.")]
		public float SpaceBetween = 10 ;

        /// <summary>
        /// The Option to set the game field.
        /// </summary>
	    [Space]
	    [Header("Game Field")]
        public GameFieldOptions GameFieldOption;

        /// <summary>
        /// The GameField that's going to be used if GameFieldOption is set to Input.
        /// </summary>
        [Space]
        [Tooltip("This rectangle will be used if the Game Field Option is set to Input")]
        public Rect InputGameField;

        /// <summary>
        /// The offset value for the Game Field from the
        /// bottom and upwards of the Camera view if the GameFieldOption
        /// is set to CameraOffset, A value of 0 means there is no offset.
        /// </summary>
        [Space]
        [Range(0, 1f)]
        [Tooltip("Offsets the Game Field from the bottom upwards, 0 means there is no offset.")]
        public float OffsetCameraBottom = 0f;

        /// <summary>
        /// The offset value for the Game Field from the
        /// top and downwards of the Camera view if the GameFieldOption
        /// is set to CameraOffset, A value of 1 means there is no offset.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the Game Field from the top downwards, a value of 1 means there is no offset.")]
        public float OffsetCameraTop = 1f;

        /// <summary>
        /// The offset value for the Game Field from the
        /// left to the right of the Camera view if the GameFieldOption
        /// is set to CameraOffset, A value of 0 means there is no offset.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the Game Field from the left to the right, A value of 0 means there is no offset.")]
        public float OffsetCameraLeft = 0f;

        /// <summary>
        /// The offset value for the Game Field from the
        /// right to the left of the Camera view if the GameFieldOption
        /// is set to CameraOffset, A value of 1 means there is no offset.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the Game Field from the right to the left, a value 1 means there is no offset.")]
        public float OffsetCameraRight = 1f;

        /// <summary>
        /// Spawn position of the player, relative to the GameField width.
        /// </summary>
        [Header("Spawn Position")]
		[Space]
		[Range(0,1f)]
        [Tooltip("Spawn position for the player,relative to GameField width")]
        public float SpawnPosX = 0.5f;

        /// <summary>
        /// Spawn position for the player, relative to GameField height.
        /// </summary>
		[Range(0,1f)]
        [Tooltip("Spawn position for the player, relative to GameField height")]
		public float SpawnPosY = 0.5f;

        /// <summary>
        /// Option for the player prefab.
        /// </summary>
        [Header("Player")]
        [Space]
        public PlayerPrefabOptions PlayerReference;

        /// <summary>
        /// the player input option for this level.
        /// </summary>
        [Tooltip("the player input option for this level")]
        public LevelInputOptions PlayerInput;

        /// <summary>
        /// the player fire option for this level.
        /// </summary>
        [Tooltip("the player fire option for this level.")]
        public LevelFireOptions PlayerFire;

        /// <summary>
        /// Player prefab reference if the Player Reference set to input.
        /// </summary>
        [Space]
        [Tooltip("Player prefab reference if the PlayerReference set to input")]
		public GameObject Player;

        /// <summary>
        /// Player start lives for this level.
        /// </summary>
        [Space]
        [Tooltip("Player start lives for this level")]
        public int PlayerLives = 3;

        /// <summary>
        /// Delay in seconds for the player to spawn when it gets destroyed.
        /// </summary>
        [Space]
        [Tooltip("Delay in seconds for the player to spawn when it gets destroyed")]
        public float PlayerSpawnDelay;

        /// <summary>
        /// Represents player position on the Z axis.
        /// </summary>
        [Space]
		[Range(0,100)]
        [Tooltip("Represent player position on the Z axis.")]
		public int PlayerIndex = 1;

        /// <summary>
        /// Option for player boundary.
        /// </summary>
		[Space]
        [Tooltip("Option for the player boundary")]
		public PlayerBoundOptions PlayerBoundOption;

        /// <summary>
        /// The offset value for the Player Boundary from the
        /// bottom and upwards of the Game Field. if the PlayerBoundOption
        /// is set to OffsetGameField, 1 means there is no offset.
        /// </summary>
        [Space]
        [Range(0, 1f)]
		[Tooltip("Offsets the Player Boundary from the bottom and upwards, 0 means there is no offset.")]
        public float PlayerFieldBottom = 0f;

        /// <summary>
        /// The offset value for the Player Boundary from the
        /// top and downward of the Game Field if the PlayerBoundOption
        /// is set to OffsetGameField, a value of 1 means there is no offset.
        /// </summary>
        [Range(0, 1f)]
		[Tooltip("offset the Player Boundary from the top and downwards, 1 means there is no offset.")]
        public float PlayerFieldTop = 1f;

        /// <summary>
        /// The offset value for the Player Boundary from the
        /// left to the right of the Game Field if the PlayerBoundOption
        /// is set to OffsetGameField, 1 means there is no offset.
        /// </summary>
        [Range(0,1f)]
		[Tooltip("Offsets the Player Boundary from the left to the right, 1 means there is no offset.")]
        public float PlayerFieldLeft = 0f;
	
        /// <summary>
        /// The offset value for the Player Boundary from the
        /// right to the left of the Game Field, if the PlayerBoundOption
        /// is set to OffsetGameField, 1 means there is no offset.
        /// </summary>
        [Range(0,1f)]
		[Tooltip("Offsets the Player Boundary from the right to the left, 1 means there is no offset.")]
        public float PlayerFieldRight = 1f;

        /// <summary>
        /// Triggers when a player has been spawned.
        /// </summary>
        public event ShmupDelegate OnPlayerSpawn;
        /// <summary>
        /// Triggers when a player is out of lives.
        /// </summary>
        public event ShmupDelegate OnPlayerOutOfLives;
        /// <summary>
        /// Triggers when the level starts.
        /// </summary>
        public event ShmupDelegate OnLevelStart;
        /// <summary>
        /// Triggers when the player score changes.
        /// </summary>
        public event ShmupDelegate OnScoreChange;
        /// <summary>
        /// Triggers when a wave spawn start.
        /// </summary>
        public event ShmupDelegate OnWavesSpawnStart;
        /// <summary>
        /// Triggers when a wave is spawned.
        /// </summary>
        public event ShmupDelegate OnWaveSpawn;
        /// <summary>
        /// Trigger every time the player get extra live.
        /// </summary>
        public event ShmupDelegate OnPlayerGetExtraLife;

        /// <summary>
        /// The field that defines the play region, where the player and enemy spawn and where
        /// the DestroyByRegion destroys any game objects leaving this region.
        /// </summary>
        public Rect GameField { get; protected set; }

        /// <summary>
        /// The view type of the level.
        /// </summary>
        public LevelViewType View { get { return OrthographicCamera.Instance.ViewType; } }

        /// <summary>
        /// The GameObject which all enemies spawn under.
        /// </summary>
        public GameObject EnemyParent { get; protected set; }
        /// <summary>
        /// The GameObject which all the PickUps spawn under.
        /// </summary>
		public GameObject PickUpParent { get; protected set; }
        /// <summary>
        /// The player component for the current player agent.
        /// </summary>
        public Player PlayerComponent { get; protected set; }
        /// <summary>
        /// The boundary of the player.
        /// </summary>
        public Rect PlayerBound { get { return PlayerComponent.mover.Boundary; } }
        
        public int PlayerScore
        {
            get
            {
                return playerScore;
            }
        }

        /// <summary>
        /// The current score of the player. You could also do the same thing using the hide in inspector.
        /// </summary>
        protected int playerScore ;

        /// <summary>
        /// A list of all wave prefab spawn by this controller.
        /// </summary>
        protected virtual WaveCreationData[] waves { get { return null; } }

        /// <summary>
        /// Player remaining lives.
        /// </summary>
        private int _playerLives;

        /// <summary>
        /// Indicate if the level controller is spawning the player.
        /// </summary>
        private bool _spawnPlayer ;


        protected override void Awake ()
        {
			base.Awake();

	        LevelInitializer.Instance.SubscribeToStage(3, InitializeLevel);
	        LevelInitializer.Instance.SubscribeToStage(3, SubmitLayers);
	        LevelInitializer.Instance.SubscribeToStage(4, RequestPlayer);           
        }

        /// <summary>
        /// Initializes the Level
        /// </summary>
        private void InitializeLevel()
        {
            EnemyParent = new GameObject("Enemy");
            PickUpParent = new GameObject("PickUp");

            _playerLives = PlayerLives;

            if (Player == null)
            {
                return;
            }
                
            switch (GameFieldOption)
            {
                case GameFieldOptions.OffsetCameraView:
                    GameField = OffsetRect(OrthographicCamera.Instance.CameraRect, OffsetCameraTop, OffsetCameraBottom,
                        OffsetCameraRight, OffsetCameraLeft);
                    break;
                case GameFieldOptions.Input:
                    GameField = InputGameField;
                    break;
                case GameFieldOptions.Background:
                    GameField = BackgroundController.Instance.BackgroundRect;
                    break;
            }

            switch (PlayerFire)
            {
                case LevelFireOptions.AutoFire:
                    InputManager.AutoFire = true;
                    break;
                case LevelFireOptions.ManualFire:
                    InputManager.AutoFire = false;
                    break;
                case LevelFireOptions.ByGameManager:
                    InputManager.AutoFire = GameManager.Instance.GameSettings.AutoFire;
                    break;
            }

            switch (PlayerInput)
            {
                case LevelInputOptions.Controls:
                    InputManager.SetInputMethod(InputMethod.Controls);
                    break;
                case LevelInputOptions.FollowTouch:
                    InputManager.SetInputMethod(InputMethod.FollowTouch);
                    break;
                case LevelInputOptions.ByGameManager:
                    InputManager.SetInputMethod(GameManager.Instance.GameSettings.IMethod);
                    break;
            }

            

        }

        /// <summary>
        /// Submits the player index and the wave index to the background controller.
        /// </summary>
	    private void SubmitLayers ()
        {
			BackgroundController.Instance.SubmitLayerIndex ( PlayerIndex );

			if (waves == null)
				return;

			for (int i = 0; i < waves.Length; i++) {

				BackgroundController.Instance.SubmitLayerIndex ( waves[i].LayerIndex );

			}
		}

        /// <summary>
        /// Check if there are enough lives to spawn a player, called by the current player in the scene when it get destroyed.
        /// </summary>
	    public void RequestPlayer(ShmupEventArgs args)
	    {
	        RequestPlayer(PlayerSpawnDelay);
	    }

        /// <summary>
        /// checks if there is enough lives to spawn a player with PlayerFirstSpawnTime delay.
        /// </summary>
        public void RequestPlayer ()
	    {
            RequestPlayer(PlayerFirstSpawnTime);
	    }

        /// <summary>
        /// Checks if there is enough lives to spawn a player with delay.
        /// </summary>
        /// <param name="timeBeforeSpawn">Delay in seconds.</param>
	    public void RequestPlayer (float timeBeforeSpawn)
        {
			if (_playerLives > 0)
            {
                if (!_spawnPlayer)
                {
                    StartCoroutine(SpawnPlayer(timeBeforeSpawn));
                }			                       
            }
            else
			{
                RiseOnPlayerOutOfLives();
			}
		}
        
        /// <summary>
        /// Spawns the player.
        /// </summary>
        /// <param name="timeBeforeSpawn">Delay in seconds before spawning.</param>
        private IEnumerator SpawnPlayer ( float timeBeforeSpawn )
		{
            //declare that there is a player spawning.
            _spawnPlayer = true;
            yield return new WaitForSeconds(timeBeforeSpawn);

            //create the player.
            CreatePlayer();

		    _playerLives--;

            //rise the OnPlayerSpawn event.
            RiseOnPlayerSpawn(new PlayerSpawnArgs(PlayerComponent, _playerLives));

		    //declare that there is no player spawning.
            _spawnPlayer = false;
            yield return null;
		}

        /// <summary>
        /// Creates a player instance and sets it to the current level.
        /// </summary>
        private void CreatePlayer()
        {
            GameObject playerObject = GetPlayerInstance();

            PlayerComponent = playerObject.GetComponent<Player>();
            PlayerComponent.OnDestroyStart += RequestPlayer;
            PlayerComponent.mover = PlayerComponent.GetComponent<PlayerMover>();

            playerObject.transform.position = GetPlayerSpawnPosition();

            SetPlayerBound(PlayerComponent);

            playerObject.SetActive(true);
        }

        /// <summary>
        /// Creates an instance from the player prefab.
        /// </summary>
        /// <returns>the player instance created by this method.</returns>
        private GameObject GetPlayerInstance()
        {
            GameObject playerObject = null;

            switch (PlayerReference)
            {
                case PlayerPrefabOptions.ByGameManager:

                    if (View == LevelViewType.Vertical)
                        playerObject = (GameObject)Instantiate(GameManager.Instance.PlayerSelectedV);

                    if (View == LevelViewType.Horizontal)
                        playerObject = (GameObject)Instantiate(GameManager.Instance.PlayerSelectedH);

                    break;
                case PlayerPrefabOptions.Input:
                    playerObject = (GameObject)Instantiate(Player);
                    break;
            }

            return playerObject;
        }

        /// <summary>
        /// Return the player spawn position.
        /// </summary>
        /// <returns>Player spawn position</returns>
        private Vector3 GetPlayerSpawnPosition()
        {
            return new Vector3(
                Mathf.Lerp(GameField.xMin, GameField.xMax, SpawnPosX),
                Mathf.Lerp(GameField.yMin, GameField.yMax, SpawnPosY),
                SpaceBetween * PlayerIndex);
        }

        /// <summary>
        /// Sets the boundary for the player.
        /// </summary>
        /// <param name="player">The current player instance.</param>
        private void SetPlayerBound(Player player)
        {
            if (PlayerBoundOption == PlayerBoundOptions.OffsetGameField)
            {
                Rect PlayerField = OffsetRect(GameField, PlayerFieldTop, PlayerFieldBottom, PlayerFieldRight,
                    PlayerFieldLeft);

                player.mover.Boundary = GameField;
            }
        }


        /// <summary>
        /// Handles wave creation.
        /// </summary>
        /// <param name="waveObject">The wave prefab.</param>
        /// <returns>Handler to the wave component</returns>
        protected static IWave CreateWave(Object waveObject, int index)
        {
            GameObject waveGo = (GameObject)GameObject.Instantiate(waveObject);

            waveGo.transform.position = new Vector3(0, 0, (LevelController.Instance.SpaceBetween * index));

            waveGo.SetActive(true);

            IWave wave = waveGo.GetComponent<IWave>();
            return wave;          
        }

        /// <summary>
        /// Creates an offset rectangle with control over every edge.
        /// </summary>
        /// <param name="rect">The original rect</param>
        /// <param name="topOffset">Offset from the top edge inward.</param>
        /// <param name="bottomOffset">Offset from the bottom edge inward.</param>
        /// <param name="rightOffset">Offset from the right edge inward.</param>
        /// <param name="leftOffset">Offset from the left edge inward.</param>
        /// <returns>the Offset rect.</returns>
        private Rect OffsetRect(Rect rect, float topOffset, float bottomOffset, float rightOffset, float leftOffset)
        {
            Rect newRect =  new Rect
            {
                xMin = Mathf.Lerp(rect.xMin, rect.xMax, leftOffset),
                xMax = Mathf.Lerp(rect.xMin, rect.xMax, rightOffset),
                yMin = Mathf.Lerp(rect.yMin, rect.yMax, bottomOffset),
                yMax = Mathf.Lerp(rect.yMin, rect.yMax, topOffset)
            };

            return newRect;
        }

        /// <summary>
        /// Adds points to the player score.
        /// </summary>
        /// <param name="point">Amount of points to add</param>
        public void AddScore(int point)
        {
            if (point >= 0)
            {
                playerScore += point;
                RiseOnScoreChange(new PlayerScoreArgs(playerScore));
            }
        }

        public void AddPlayerLife()
        {
            _playerLives++;
            RiseOnPlayerGetExtraLife();
        }

        public int GetPlayerLive()
        {
            return _playerLives;
        }

        /// <summary>
        /// Handles the rise of OnPlayerSpawn event.
        /// </summary>
        protected void RiseOnPlayerSpawn(PlayerSpawnArgs args)
        {
            if (OnPlayerSpawn != null)
                OnPlayerSpawn(args);
        }
        /// <summary>
        /// Handles the rise of OnPlayerGetExtraLive event.
        /// </summary>
        protected void RiseOnPlayerGetExtraLife()
        {
            if (OnPlayerGetExtraLife != null)
                OnPlayerGetExtraLife(null);
        }
        /// <summary>
        /// Handles the rise of OnPlayerOutOfLives event.
        /// </summary>
        protected void RiseOnPlayerOutOfLives()
        {
            if (OnPlayerOutOfLives != null)
                OnPlayerOutOfLives(null);
        }
        /// <summary>
        /// Handles the rise of OnScoreChange event.
        /// </summary>
        protected void RiseOnScoreChange(PlayerScoreArgs args)
        {
            if (OnScoreChange != null)
                OnScoreChange(args);
        }
        /// <summary>
        /// Handles the rise of OnLevelStart event.
        /// </summary>
        protected void RiseOnLevelStart(int levelIndex)
        {
            if (OnLevelStart != null)
                OnLevelStart(new LevelArgs(levelIndex));
        }
        /// <summary>
        /// Handles the rise of OnLevelStart event.
        /// </summary>
        protected void RiseOnLevelStart()
        {
            if (OnLevelStart != null)
                OnLevelStart(null);
        }
        /// <summary>
        /// Handles the rise of OnWavesSpawnStart event.
        /// </summary>
        protected void RiseOnWavesSpawnStart()
        {
			if (OnWavesSpawnStart != null)
				OnWavesSpawnStart (null);
		}
        /// <summary>
        /// Handles the rise of OnWaveSpawn event.
        /// </summary>
		protected void RiseOnWaveSpawn()
        {
			if (OnWaveSpawn != null)
				OnWaveSpawn (null);
		}

#if UNITY_EDITOR

        private void OnDrawGizmos()
	    {
            //get the rect by the background to define the player spawn position.
	        BackgroundController bgController = FindObjectOfType<BackgroundController>();

            if ( bgController == null )
                return;

	        Rect backgroundRect = bgController.BackgroundForGizmo;

            //set the spawn position relative to background rect.
	        Vector2 spawnPosition = new Vector2(Mathf.Lerp(backgroundRect.xMin, backgroundRect.xMax, SpawnPosX),
	            Mathf.Lerp(backgroundRect.yMin, backgroundRect.yMax, SpawnPosY));

            Gizmos.color = Color.yellow;

	        GizmosExtension.DrawCircle(spawnPosition, 0.5f);
	    }

#endif

    }

}