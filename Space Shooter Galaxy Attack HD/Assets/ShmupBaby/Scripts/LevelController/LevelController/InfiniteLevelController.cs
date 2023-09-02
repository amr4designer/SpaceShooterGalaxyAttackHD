using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShmupBaby {

    /// <summary>
    /// The stream spawn state.
    /// </summary>
    public enum SteamState
    {
        NoMore ,
        NotYet ,
        GoAhead
    }

    /// <summary>
    /// define the upgrade parameter for the stream.
    /// </summary>
    [System.Serializable]
    public class StreamUpgrade
    {
        /// <summary>
        /// define the parameter for upgrade.
        /// </summary>
        [System.Serializable]
        public struct Upgrade
        {
            /// <summary>
            /// the amount that will be added every time the stream is spawn.
            /// </summary>
            [Tooltip("the amount that will be added every time the stream is spawn.")]
            public float Amount;
            /// <summary>
            /// the percentage of the original value that will be added when the stream is spawn.
            /// </summary>
            [Range(0,100)]
            [Tooltip("the percentage of the original value that will be added when the stream is spawn.")]
            public float Percentage;

            /// <summary>
            /// return the upgrade amount after passing the original value of the parameter.
            /// </summary>
            /// <param name="original">the original value of the parameter</param>
            public float UpgradeAmount(float original)
            {
                return original * Percentage*0.01f + Amount;
            }

            /// <summary>
            /// return the upgrade amount after passing the original value of the parameter.
            /// </summary>
            /// <param name="original">the original value of the parameter</param>
            public int UpgradeAmount(int original)
            {
                return (int)(original * Percentage*0.01f + Amount);
            }
        }

        /// <summary>
        /// the upgrade for enemy number.
        /// </summary>
        [Header("Wave")]
        [Tooltip("the amount of enemy will be added to every wave in the stream each time it's spawn.")]
        public Upgrade EnemyNumber;

        /// <summary>
        /// indicate if the Enemy upgrade is required.
        /// </summary>
        [Header("Enemy")]
        [Tooltip("indicate if the Enemy upgrade is required, if it's not it should be UnCheck to save performance.")]
        public bool EnableEnemyUpgrade;
        /// <summary>
        /// the upgrade for enemy health.
        /// </summary>
        [Tooltip("the amount of health will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade Health;
        /// <summary>
        /// the upgrade for enemy shield.
        /// </summary>
        [Tooltip("the amount of Shield will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade Shield;
        /// <summary>
        /// the upgrade for enemy reward points.
        /// </summary>
        [Tooltip("the amount of point will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade PointGiven;
        /// <summary>
        /// the upgrade for enemy collision damage.
        /// </summary>
        [Tooltip("the amount of collision damage will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade CollisionDamage;

        /// <summary>
        /// 
        /// </summary>
        [Header("Mover")]
        [Tooltip("indicate if the Mover upgrade is required, if it's not it should be UnCheck to save performance.")]
        public bool EnableMoverUpgrade;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("the amount of speed will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade Speed;

        /// <summary>
        /// 
        /// </summary>
        [Header("Weapon")]
        [Tooltip("indicate if the Weapon upgrade is required, if it's not it should be UnCheck to save performance.")]
        public bool EnableWeaponUpgrade;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("the amount of weapon shot speed will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade ShotSpeed;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("the amount of weapon damage will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade Damage;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("the amount of weapon fire rate will be added to every enemy in the stream each time it's spawn.")]
        public Upgrade Rate;

        /// <summary>
        /// indicate how many time this stream has been spawn.
        /// </summary>
        [HideInInspector]
        public int SpawnNumber;

        /// <summary>
        /// called this every time a stream is spawn.
        /// </summary>
        public void UpdateSpawnNumber()
        {
            SpawnNumber++;
        }

        /// <summary>
        /// return the upgrade amount after passing the original value of the parameter.
        /// </summary>
        /// <param name="upgrade">the property upgrade.</param>
        /// <param name="original">the original value of the parameter</param>
        public float GetPropertyValue (Upgrade upgrade , float original)
        {
            return original + upgrade.UpgradeAmount(original) * SpawnNumber;
        }

        /// <summary>
        /// return the upgrade amount after passing the original value of the parameter.
        /// </summary>
        /// <param name="upgrade">the property upgrade.</param>
        /// <param name="original">the original value of the parameter</param>
        public int GetPropertyValue(Upgrade upgrade, int original)
        {
            return original + upgrade.UpgradeAmount(original) * SpawnNumber;
        }
    }

    /// <summary>
    /// Stream data structure for the Infinite level controller.
    /// </summary>
	[System.Serializable]
	public class Stream : System.IComparable<Stream>
	{
        /// <summary>
        /// Stream Wave Creation Data.
        /// </summary>
	    [Space]
        [Tooltip("Waves to be created in order for this level and for this stream.")]
	    public WaveCreationData[] Waves;

        public StreamUpgrade WavesUpgrade;

        /// <summary>
        /// The level index this stream will start to spawn its waves at.
        /// </summary>
        [Space]
        [Tooltip("The index of the level for this stream to start spawning its waves")]
		public int StartLevel;
        /// <summary>
        /// The level index this stream will stop spawning.
        /// </summary>
		[Tooltip("The last level index for the stream, 0 means it will keep going forever")]
        public int EndLevel;
        /// <summary>
        /// Stream spawn level step from the StartIndex.
        /// </summary>
        [Tooltip("Indicates the step number for this stream, which defines how the 'nth' level in this stream should appear")]
		public int EveryNthLevel;

		/// <summary>
		/// The creation state of the stream.
		/// </summary>
		[HideInInspector]
		public SteamState State;

	    /// <summary>
	    /// Time left to spawn this stream.
	    /// </summary>
	    public int Uses { get { return (EndLevel - StartLevel) / EveryNthLevel + 1; } }

        /// <summary>
        /// Time left to spawn this stream.
        /// </summary>
		public int _leftUses ;

        /// <summary>
        /// Will this stream spawn forever.
        /// </summary>
		public bool _infiniteUse ;

        /// <summary>
        /// Initializes the stream.
        /// </summary>
        public void Initialize ()
        {

            if (EveryNthLevel <= 0)
                EveryNthLevel = 1;

			if (EndLevel == 0)
				_infiniteUse = true;
			else 
				_leftUses = Uses;
			
		}

        /// <summary>
        /// update the State of the stream.
        /// </summary>
        /// <param name="levelIndex">The level that this stream should be spawned in</param>
        /// <returns>The stream spawn state.</returns>
		public SteamState UpdateStreamState ( int levelIndex ) {

            //If the stream has infiniteUse we only check for EveryNthLevel.
            if (_infiniteUse && levelIndex >= StartLevel && (levelIndex-StartLevel)%EveryNthLevel == 0)
				return State = SteamState.GoAhead;

			if (_leftUses > 0)
			{
                
				if (levelIndex == StartLevel + EveryNthLevel * (Uses - _leftUses)) {
					return State = SteamState.GoAhead;
				}
				else
					return State = SteamState.NotYet;
			}else
				return State = SteamState.NoMore;

		}

        /// <summary>
        /// called when a stream is created to decrease it's uses.
        /// </summary>
        public void DecreaseStreamUse()
        {
            _leftUses--;
        }

        /// <summary>
        /// Compares the stream to another stream by the stream start index.
        /// </summary>
	    public int CompareTo(Stream other)
	    {
	        if (ReferenceEquals(this, other))
	            return 0;
	        if (ReferenceEquals(null, other))
	            return 1;
	        return StartLevel.CompareTo(other.StartLevel);
	    }

	}

    public class LevelArgs : ShmupEventArgs
    {
        public int LevelIndex;

        public LevelArgs(int levelIndex)
        {
            LevelIndex = levelIndex;
        }

    }

    /// <summary>
    /// Defines the behavior for spawning an unlimited waves. 
    /// </summary>
    public interface ISpawnInfiniteWave
    {
        /// <summary>
        /// This event is triggered when the level ends.
        /// </summary>
        event ShmupDelegate OnLevelEnd;
        /// <summary>
        /// This event is triggered when a stream start spawning.
        /// </summary>
        event ShmupDelegate OnStreamStart;
        /// <summary>
        /// This event should is triggered when a stream spawn ends.
        /// </summary>
        event ShmupDelegate OnStreamEnd;

    }

    /// <summary>
    /// Level controller that spawns an unlimited number of waves.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Level/Infinite Level Controller")]
    public sealed class InfiniteLevelController : LevelController , ISpawnInfiniteWave
	{
        /// <summary>
        /// Time in seconds between each level.
        /// </summary>
        [Header("Levels :")]
        [Space]
        [Tooltip("Time in seconds between each level")]
        public float TimeBetweenLevels;

        /// <summary>
        /// The streams that will form the infinite levels.
        /// </summary>
        [Space]
        [Tooltip("List of streams that will form the infinite levels")]
	    public Stream[] Streams;

        /// <summary>
        /// Is triggered when a level ends.
        /// </summary>
	    public event ShmupDelegate OnLevelEnd;

	    /// <summary>
	    /// Is triggered when a stream starts.
	    /// </summary>
	    public event ShmupDelegate OnStreamStart;

        /// <summary>
        /// Is triggered when a stream ends.
        /// </summary>
        public event ShmupDelegate OnStreamEnd;

        /// <summary>
        /// The enemies spawned until now.
        /// </summary>
	    public int EnemySpawn { get; protected set; }

        /// <summary>
        /// Lists for all active streams.
        /// </summary>
        public List<Stream> _streams;

        /// <summary>
        /// The index of the current level.
        /// </summary>
	    public int _levelIndex;

        /// <summary>
        /// Indicates if the scene is empty from waves.
        /// </summary>
	    public bool _waveClear ;

        /// <summary>
        /// A list of all wave prefabs spawned by this controller.
        /// </summary>
        protected override WaveCreationData[] waves
	    {
	        get
	        {
	            List<WaveCreationData> wavesData = new List<WaveCreationData>();

	            for (int i = 0; i < Streams.Length; i++)
	            {
	                wavesData.AddRange(Streams[i].Waves);
                }

	            return wavesData.ToArray();

	        }
	    }

	    /// <summary>
	    /// One of Unity's messages that act the same way as start but gets called before start.
	    /// </summary>
        protected override void Awake()
	    {

	        base.Awake();

	        LevelInitializer.Instance.SubscribeToStage(4, InitializeInfiniteLevel);

	    }

        /// <summary>
        /// Initializes the infinite level controller.
        /// </summary>
        void InitializeInfiniteLevel()
	    {
	        for (int i = 0; i < Streams.Length; i++)
	        {
	            Streams[i].Initialize();
	        }

	        _streams = new List<Stream>(Streams);
            
	        StartCoroutine(LevelCreation(WaveStartTime));
        }

        /// <summary>
        /// Starts the creation of the level with _levelIndex + 1.
        /// </summary>
        /// <param name="delay">Delay in seconds before the level creation starts</param>
	    IEnumerator LevelCreation(float delay)
	    {
            _levelIndex++;

            yield return null;

            RiseOnLevelStart(_levelIndex);

            yield return new WaitForSeconds(delay);

            //we run two checks to test if the level end.
            // 1- check if the scene is empty from wave.
            // 2- if we have a pass on the first check then we check if the 
            //    scene empty from enemy.
            Wave.AllWaveDestroyed -= CheckForWaveClear;
	        Enemy.OnAllEnemyDestroyed -= CheckForLevelEnd;
                        
            int lastStream = 0;

            for (int i = 0; i < _streams.Count; i++)
            {
                if (_streams[i].UpdateStreamState(_levelIndex) == SteamState.GoAhead)
                    lastStream = i;
            }

	        for (int i = 0; i < _streams.Count; i++)
            {
                //get the state of the stream if its ready to be created,
                //or its done its work and it should be removed, or if its
                //this is not it's turn to be created.
                SteamState state = _streams[i].UpdateStreamState(_levelIndex);
                
                //indicate if we reach the last stream for this level.
                bool isLastStream = i == lastStream;

                switch (state)
	            {
                    case SteamState.NoMore:
                        //set the stream to null,this will mark  the stream
                        //to be removed from the stream list at the end of the level.
                        Streams[i] = null;
                        break;

	                case SteamState.GoAhead:
	                {
                        RiseOnStreamStart();

                        if (isLastStream)
                            //for the last stream the stream creation should not be hold
                            //for its last wave time.
	                        yield return StreamCreation(_streams[i] , false);
                        else
                            yield return StreamCreation(_streams[i], true);

	                    RiseOnStreamEnd();

                        _streams[i].DecreaseStreamUse();

                        break;
	                }
	                case SteamState.NotYet:
	                
                        break;
	                
	            }
                
	        }

            //start the check for the end of the level.
	        if (PoolManager.GetList<Wave>().Count > 0)
	            Wave.AllWaveDestroyed += CheckForWaveClear;
	        else
	            CheckForWaveClear(null);

            Enemy.OnAllEnemyDestroyed += CheckForLevelEnd;


        }

        /// <summary>
        /// Creates a stream.
        /// </summary>
        /// <param name="stream">The stream that needs to be created</param>
        /// <param name="waitBetweenWaves">If the stream should wait for its last wave time.</param>
	    IEnumerator StreamCreation(Stream stream , bool waitBetweenWaves)
	    {

            for (int j = 0; j < stream.Waves.Length; j++)
	        {
	            bool lastWave = j == stream.Waves.Length - 1;

	            Object waveObject = stream.Waves[j].Wave;
                int layerIndex = stream.Waves[j].LayerIndex;

	            if (waveObject != null)
	            {
                    IWave wave = CreateWave(waveObject,layerIndex);
                    //set the wave to be upgraded.
                    wave.EnemyNumber = stream.WavesUpgrade.GetPropertyValue(stream.WavesUpgrade.EnemyNumber, wave.EnemyNumber);
                    wave.streamUpgrade = stream.WavesUpgrade;
                    wave.WaveSpawn += UpgradeWave;

                    if (wave != null)
	                    EnemySpawn += wave.EnemyNumber;

                    if (!lastWave || waitBetweenWaves)
                        yield return new WaitForSeconds(stream.Waves[j].TimeForNextWave);
                    

	            }

	        }

        }

        /// <summary>
        /// upgrade the enemy wave based on stream upgrade.
        /// </summary>
        public void UpgradeWave ( ShmupEventArgs args)
        {
            WaveSpawnArgs spawnArgs = args as WaveSpawnArgs;

            if (spawnArgs == null)
                return;

            StreamUpgrade upgrade = spawnArgs.streamUpgrade;

            if (upgrade == null)
                return;

            if (upgrade.EnableMoverUpgrade)
                UpgradeMover(spawnArgs.EnemyObject.GetComponent<Mover>(), upgrade);

            if (upgrade.EnableEnemyUpgrade)
                UpgradeEnemy(spawnArgs.EnemyObject.GetComponent<Enemy>(), upgrade);

            if (upgrade.EnableWeaponUpgrade)
                UpgradeWeapon(spawnArgs.EnemyObject.GetComponent<HandleWeapon>(), upgrade);
        }

        /// <summary>
        /// upgrade a mover based on a stream upgrade.
        /// </summary>
        /// <param name="mover">the mover that need to be upgraded.</param>
        /// <param name="upgrade">the upgrade data.</param>
        public static void UpgradeMover(Mover mover, StreamUpgrade upgrade)
        {
            if (mover != null)
                mover.speed = upgrade.GetPropertyValue(upgrade.Speed, mover.speed);
        }

        /// <summary>
        /// upgrade a enemy based on a stream upgrade.
        /// </summary>
        /// <param name="enemy">the enemy that need to be upgraded.</param>
        /// <param name="upgrade">the upgrade data.</param>
        public static void UpgradeEnemy(Enemy enemy, StreamUpgrade upgrade)
        {
            enemy.MaxHealth = upgrade.GetPropertyValue(upgrade.Health, enemy.MaxHealth);
            enemy.CurrentHealth = enemy.MaxHealth;
            enemy.MaxShield = upgrade.GetPropertyValue(upgrade.Shield, enemy.MaxShield);
            enemy.CurrentShield = enemy.MaxShield;
            enemy.PointGiven = upgrade.GetPropertyValue(upgrade.PointGiven, enemy.PointGiven);
            enemy.collisionDamage = upgrade.GetPropertyValue(upgrade.PointGiven, enemy.collisionDamage);
        }

        /// <summary>
        /// upgrade enemy weapons based on a stream upgrade.
        /// </summary>
        /// <param name="weaponHolder">the weapon handler</param>
        /// <param name="upgrade">the upgrade data.</param>
        public static void UpgradeWeapon(HandleWeapon weaponHolder, StreamUpgrade upgrade)
        {

            if (weaponHolder == null || weaponHolder.Weapons.Length < 1)
                return;

            NormalWeapon[] weapons = weaponHolder.Weapons;

            for (int i = 0; i < weapons.Length; i++)
            {
                WeaponStageData weaponData = weapons[i].CurrentStage;
                weaponData.Damage = upgrade.GetPropertyValue(upgrade.Damage, weaponData.Damage);
                weaponData.Speed = upgrade.GetPropertyValue(upgrade.ShotSpeed, weaponData.Speed);
                weaponData.Rate = upgrade.GetPropertyValue(upgrade.Rate, weaponData.Rate);
                weapons[i].SetToStage(weaponData);
            }
        }

        /// <summary>
        /// Called when the scene is empty from waves, declares
        /// that the scene is empty from waves.
        /// </summary>
        public void CheckForWaveClear( ShmupEventArgs args )
	    {
	        _waveClear = true;
            List<IPool> enemyInLevel = PoolManager.GetList<Enemy>();
            if (enemyInLevel != null)
            {
                if (enemyInLevel.Count <= 0)
                    CheckForLevelEnd(null);
            }
        }

	    /// <summary>
	    /// Called when the scene is empty from enemies, starts the
	    /// next level.
	    /// </summary>
	    void CheckForLevelEnd(ShmupEventArgs args)
	    {
            if (_waveClear)
	        {
	            RiseOnLevelEnd(_levelIndex);

	            CleanStreamsList();

                #if UNITY_EDITOR

                try
                {
                    StartCoroutine(LevelCreation(TimeBetweenLevels));
                }
                catch (System.Exception ex)
                {

                }

                #else

                StartCoroutine(LevelCreation(TimeBetweenLevels));

                #endif

                UpgradeStreams();

                _waveClear = false;
	        }

	    }

        /// <summary>
        /// update the spawn number for all streams.
        /// </summary>
        public void UpgradeStreams ()
        {
            for (int i = 0; i < _streams.Count; i++)
            {
				if (_streams[i].State == SteamState.GoAhead )
                	_streams[i].WavesUpgrade.UpdateSpawnNumber();
            }
        }

	    /// <summary>
	    /// Sort and clean the stream list from null members.
	    /// </summary>
	    public void CleanStreamsList()
	    {

	        _streams.Sort();

	        int emptyStreams = 0;

	        for (int i = 0; i < _streams.Count; i++)
	        {
	            if (_streams[i] == null)
	                emptyStreams++;
	            else
	                break;
	        }

	        for (int i = 0; i < emptyStreams; i++)
	        {
	            _streams.RemoveAt(0);
	        }
	    }

        /// <summary>
        /// Handles the rise of OnLevelEnd.
        /// </summary>
        /// <param name="index">the index of the next level.</param>
	    protected void RiseOnLevelEnd(int index)
	    {
	        if (OnLevelEnd != null)
	            OnLevelEnd(new LevelArgs(index));
	    }
        /// <summary>
        /// Handles the rise of OnStreamStart.
        /// </summary>
	    protected void RiseOnStreamStart()
	    {
	        if (OnStreamStart != null)
	            OnStreamStart(null);
	    }
        /// <summary>
        /// Handles the rise of OnStreamEnd.
        /// </summary>
	    protected void RiseOnStreamEnd()
	    {
	        if (OnStreamEnd != null)
	            OnStreamEnd(null);
	    }
        
	}
    
}