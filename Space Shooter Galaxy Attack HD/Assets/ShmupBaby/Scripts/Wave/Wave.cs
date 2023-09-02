using UnityEngine;
using System.Collections;

namespace ShmupBaby
{
    /// <summary>
    /// Position the snap options to a rectangle.
    /// </summary>
    public enum PointSnapOptions
    {
        None,
        Left,
        Right,
        Up,
        Down,
        UpperLeft,
        UpperRight,
        DownLeft,
        DownRight
    }

    /// <summary>
    /// rectangle sides.
    /// </summary>
	public enum RectSides
    {
		Top ,
		Bottom ,
		Left ,
		Right 
	}

    /// <summary>
    /// defines the enemy wave.
    /// </summary>
    public interface IWave
    {
        /// <summary>
        /// this event should be raised when the wave starts creating its agents.
        /// </summary>
		event ShmupDelegate WaveCreationStart;
        /// <summary>
        /// this event should be raise when the wave ends creating its agents.
        /// </summary>
        event ShmupDelegate WaveCreationEnd;
        /// <summary>
        /// this event should be raised when the wave spawns an agent.
        /// </summary>
        event ShmupDelegate WaveSpawn;

        /// <summary>
        /// number of the enemy agents the wave will create.
        /// </summary>
		int EnemyNumber { get; set; }

        /// <summary>
        /// the upgrade data associate with the stream that spawn the wave (used for infinite level controller).
        /// </summary>
        StreamUpgrade streamUpgrade { set; }
	}

    /// <summary>
    /// define the data passed by WaveSpawn event.
    /// </summary>
    public class WaveSpawnArgs : ShmupEventArgs
    {
        public GameObject EnemyObject;
        public StreamUpgrade streamUpgrade;

        public WaveSpawnArgs(GameObject enemyObject, StreamUpgrade upgrade)
        {
            EnemyObject = enemyObject;
            streamUpgrade = upgrade;
        }
    }

    /// <summary>
    /// base class for enemy waves.
    /// </summary>
	[AddComponentMenu("")]
	public class Wave : MonoBehaviour , IWave , IPool
	{
	    /// <summary>
	    /// Is triggered when there are no enemy wave in the scene.
	    /// </summary>
        public static event ShmupDelegate AllWaveDestroyed;
        /// <summary>
        /// wave event is a trigger when the wave start create its agent.
        /// </summary>
        public event ShmupDelegate WaveCreationStart;
        /// <summary>
        /// wave event is triggered when the wave ends with the creation of its agent.
        /// </summary>
        public event ShmupDelegate WaveCreationEnd;
        /// <summary>
        /// wave event is triggered when the wave spawn an agent.
        /// </summary>
        public event ShmupDelegate WaveSpawn;
        /// <summary>
        /// pool event is triggered when this wave is added to the PoolManager.
        /// </summary>
        public event ShmupDelegate OnAddToPool;
	    /// <summary>
	    /// pool event is triggered when this wave is removed from the PoolManager.
	    /// </summary>
		public event ShmupDelegate OnRemoveFromPool;

	    /// <summary>
	    /// number of enemy agents the wave will create.
	    /// </summary>
		public virtual int EnemyNumber { get { return 0; } set { } }

        /// <summary>
        /// current level game field.
        /// </summary>
	    protected Rect GameField
	    {
	        get { return LevelController.Instance.GameField; }
	    }
	    /// <summary>
	    /// current level view.
	    /// </summary>
	    protected LevelViewType ViewType
	    {
	        get { return LevelController.Instance.View; }
	    }
	    /// <summary>
	    /// the parent that contains the enemy for the level.
	    /// </summary>
	    protected Transform EnemyParent
	    {
	        get { return LevelController.Instance.EnemyParent.transform; }
	    }

        /// <summary>
        /// the upgrade data associated with the stream that spawn the wave (used for infinite level controller).
        /// </summary>
        public StreamUpgrade streamUpgrade { set; private get; }


        protected virtual void Start()
        {
			RiseWaveCreationStart();
			StartCoroutine (WaveCreation());
		}

        void OnEnable()
        {
			this.AddToPool <Wave>();
		}

        void OnDisable()
        {			
			if (!this.RemoveFromPool <Wave>())
				return;

			if (AllWaveDestroyed != null)
				AllWaveDestroyed (null);
		}

        /// <summary>
        /// handles the wave creation.
        /// </summary>
        protected virtual IEnumerator WaveCreation()
        {
			yield return null;
		}

        /// <summary>
        /// handles the rise of WaveCreationStart event.
        /// </summary>
        void RiseWaveCreationStart()
        {
			if (WaveCreationStart != null)
				WaveCreationStart (null);
		}
        /// <summary>
        /// Handles the rise of WaveCreationEnd event.
        /// </summary>
        protected void RiseWaveCreationEnd()
        {
			if (WaveCreationEnd != null)
				WaveCreationEnd (null);
		}
        /// <summary>
        /// Handles the rise of WaveSpawn event.
        /// </summary>
        protected void RiseWaveSpawn(GameObject enemy)
        {
            if (WaveSpawn != null)
                WaveSpawn(new WaveSpawnArgs(enemy , streamUpgrade));
        }

        /// <summary>
        /// Destroys this wave.
        /// </summary>
		protected void DestroyWave()
        {
			StopAllCoroutines ();
			Destroy (gameObject);
		}

        /// <summary>
        /// Snaps a position to a rectangle.
        /// </summary>
        /// <param name="point">a position to snap to.</param>
        /// <param name="area">rectangle for the point to be snapped to</param>
        /// <param name="offset">point offsets outside of the area after it has been snapped.</param>
        /// <param name="pointSnap">the side or the corner of the rectangle for this point to be snapped to.</param>
        /// <returns>the position after snap</returns>
	    public static Vector2 SnapPoint(Vector2 point, Rect area, float offset, PointSnapOptions pointSnap)
	    {
	        switch (pointSnap)
	        {
	            case PointSnapOptions.Up:
	                return new Vector2(point.x, area.yMax + offset);

	            case PointSnapOptions.Down:
	                return new Vector2(point.x, area.yMin - offset);

	            case PointSnapOptions.Left:
	                return new Vector2(area.xMin - offset, point.y);

	            case PointSnapOptions.Right:
	                return new Vector2(area.xMax + offset, point.y);

	            case PointSnapOptions.UpperRight:
	                return new Vector2(area.xMax + offset, area.yMax + offset);

	            case PointSnapOptions.UpperLeft:
	                return new Vector2(area.xMin - offset, area.yMax + offset);

	            case PointSnapOptions.DownLeft:
	                return new Vector2(area.xMin - offset, area.yMin - offset);

	            case PointSnapOptions.DownRight:
	                return new Vector2(area.xMax + offset, area.yMin - offset);
                default:
                    return point;
            }
	    }

    }

}