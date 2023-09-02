using UnityEngine;


namespace ShmupBaby {

    /// <summary>
    /// Defines the behavior for spawning a limited number of waves. 
    /// </summary>
	public interface ISpawnLimitWave {

        /// <summary>
        /// The number for waves spawning.
        /// </summary>
	    int WavesNumber { get; }
        /// <summary>
        /// Time in seconds to spawn the next wave.
        /// </summary>
	    float TimeToNextWave { get; }

        /// <summary>
        /// This event should be a trigger when the level spawn ends.
        /// </summary>
        event ShmupDelegate OnWavesSpawnEnd;
        /// <summary>
        /// This event should be a trigger when the level is completed by the player.
        /// </summary>
        event ShmupDelegate OnLevelComplete;

        /// <summary>
        /// Number of enemies in a wave.
        /// </summary>
        /// <param name="waveIndex">Wave index.</param>
        /// <returns>Number of enemies in the wave with waveIndex</returns>
	    int GetEnemyInWave(int waveIndex);

        /// <summary>
        /// Change the time for the next wave spawn.
        /// </summary>
        /// <param name="time">New time in seconds.</param>
	    void SetTimeToNextWave(float time);
    }

    /// <summary>
    /// Level controller that spawns a limited number of waves.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Level/Finite Level Controller")]
    public sealed class FiniteLevelController : LevelController , ISpawnLimitWave {

        /// <summary>
        /// Wave Creation Data defines the waves that will be created in this level in order.
        /// </summary>
		[Header("Waves :")]
		[Space]
        [Tooltip("Waves to be created in this level in order.")]
		public WaveCreationData[] Waves ;

        /// <summary>
        /// The number of enemies that will be spawned by the waves.
        /// </summary>
        public static int EnemyInMission;
        
        /// <summary>
        /// Triggers when wave spawn end.
        /// </summary>
        public event ShmupDelegate OnWavesSpawnEnd;

        /// <summary>
        /// Triggers when the level is completed by the player.
        /// </summary>
        public event ShmupDelegate OnLevelComplete;

        /// <summary>
        /// The number for waves to spawn.
        /// </summary>
        public int WavesNumber { get { return Waves.Length; } }

        /// <summary>
        /// Time in seconds to spawn the next wave.
        /// </summary>
		public float TimeToNextWave { get; private set;}

        /// <summary>
        /// All Wave Creation Data used by this level.
        /// </summary>
		protected override WaveCreationData[] waves { get { return Waves; } }

        /// <summary>
        /// Indicates if the wave spawn creation ends.
        /// </summary>
	    private bool _waveCreationEnd ;

        /// <summary>
        /// Indicates if wave spawn creation end and every wave in the scene is destroyed.
        /// </summary>
        private bool _allWaveDestroyed ;

        /// <summary>
        /// The index for the next wave to be spawned.
        /// </summary>
        private int _waveIndex ;

        /// <summary>
        /// One of Unity's messages that act in the same way as start but gets called before start.
        /// </summary>
        protected override void Awake()
	    {

	        base.Awake();

	        LevelInitializer.Instance.SubscribeToStage(4, InitializeLimitedLevel);
	        
	    }

        /// <summary>
        /// Initializes a limited level controller. 
        /// </summary>
        void InitializeLimitedLevel()
	    {
	        TimeToNextWave = WaveStartTime + Time.time;

	    }

        /// <summary>
        /// One of Unity's messages that gets called every frame.
        /// </summary>
		private void Update () {

			if (Waves.Length == 0 || _waveCreationEnd)
				return;

			if (Time.time >= TimeToNextWave ) {

			    RiseOnWaveSpawn ();

				Object waveObject = Waves [_waveIndex].Wave;
                int waveIndex = Waves[_waveIndex].LayerIndex;

                if (waveObject != null)
				{
				    IWave wave = CreateWave(waveObject, waveIndex);

				    if (wave != null)
				        EnemyInMission += wave.EnemyNumber;

                    if (_waveIndex == Waves.Length - 1)
				    {
				        RiseOnWavesSpawnEnd();

                        //after the creation for the final wave, the check start when the 
                        //scene in empty from wave and enemy.
						Wave.AllWaveDestroyed += CheckForAllWaveEnd;

				        Enemy.OnAllEnemyDestroyed += CheckForLastEnemy;

                        _waveCreationEnd = true;
				    }
				}

				TimeToNextWave += Waves[_waveIndex].TimeForNextWave;

				_waveIndex++;
                
			}

		}
        

        /// <summary>
        /// Called when all enemies in the scene get destroyed, when the last enemy
        /// is destroyed this method will declare the level completed.
        /// </summary>
		private void CheckForLastEnemy (ShmupEventArgs args)
        {
		    if (!_allWaveDestroyed)
                return;

            if (!GameManager.IsInitialize)
                return;

		    int highScore = GameManager.Instance.CurrentHighScore();

		    if (playerScore > highScore || GameManager.Instance.GetCurrentLevelStatue() == ShmupSaveData.LevelStatue.OnGoing)
		    {
		        highScore = playerScore;
		        GameManager.Instance.CheckAssignHighScore(playerScore);
		    }

			Enemy.OnAllEnemyDestroyed -= CheckForLastEnemy;

            if (!LevelUIManager.IsInitialize)
                return;

            RiseOnLevelComplete();            
        }

        /// <summary>
        /// Called from the last wave in the scene, when it gets destroyed, declare that 
		/// all wave creation has ended and all waves in the scene are destroyed.
        /// </summary>
        private void CheckForAllWaveEnd(ShmupEventArgs args)
	    {
            _allWaveDestroyed = true;
        }

        /// <summary>
        /// Handles the rise of OnWavesSpawnEnd.
        /// </summary>
        private void RiseOnWavesSpawnEnd () {
			if (OnWavesSpawnEnd != null)
				OnWavesSpawnEnd (null);
		}

        /// <summary>
        /// Handles the rise of OnLevelComplete.
        /// </summary>
        private void RiseOnLevelComplete()
        {
            if (OnLevelComplete != null)
                OnLevelComplete(null);
        }

        /// <summary>
        /// Number of enemies in a wave.
        /// </summary>
        /// <param name="waveIndex">Wave index.</param>
        /// <returns>Number of enemies in the wave with waveIndex</returns>
        public int GetEnemyInWave ( int waveIndex ) {

			if ( waveIndex < 0 || waveIndex >= Waves.Length )
				return 0;

			GameObject wave = Waves [waveIndex].Wave as GameObject;

			if (wave == null)
				return 0;

			IWave waveHandle = wave.GetComponent<IWave> ();

			if (waveHandle == null)
				return 0;
			else
				return waveHandle.EnemyNumber;

		}

        /// <summary>
        /// Changes the time for the next wave spawn.
        /// </summary>
        /// <param name="time">New time in seconds.</param>
		public void SetTimeToNextWave ( float time ) {
			TimeToNextWave = Time.time + time;
		}

	}

}