using UnityEngine;
using System.Collections;

namespace ShmupBaby {

    /// <summary>
    /// An enemy wave that spawns enemies with a fixed time and distance in-between.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Wave/Organized Wave")]
    public sealed class OrganizedWave : Wave
	{
	    /// <summary>
	    /// the enemy that's going to be spawned by the wave.
	    /// </summary>
	    [Space]
	    [Tooltip("The enemy that's going to be spawned by the wave")]
        public Object enemy;

	    /// <summary>
	    /// Number of enemies spawned by this wave.
	    /// </summary>
	    [Space]
	    [Tooltip("Number of enemies spawned by this wave")]
        public int Number;
        /// <summary>
        /// The first enemy spawn position depending
        /// on the SpawnSide.
        /// </summary>
        [Space]
		[Range(0,1)]
        [Tooltip("The first enemy spawn position depending on the SpawnSide.")]
		public float StartPosition;
        /// <summary>
        /// Displacement between each enemy spawned.
        /// </summary>
	    [Range(-1,1)]
        [Tooltip("Displacement between each enemy spawned.")]
		public float SpaceBetween = 0 ;

	    /// <summary>
	    /// game flied side which the wave will spawn the enemy from.
	    /// </summary>
	    [Space]
	    [Tooltip("the side of the game field which the enemy will spawn from")]
        public RectSides SpawnSide;

	    /// <summary>
	    /// Time between each enemy spawned.
	    /// </summary>
	    [Space]
	    [Tooltip("Time between each enemy spawn")]
        public float TimeBetween;
	    /// <summary>
	    /// Offset for the enemy spawn position from the edge of the game field.
	    /// </summary>
	    [Space]
	    [Tooltip("To prevent an enemy from spawning at the edge of the game field")]
        public float Offset = 2;

	    /// <summary>
	    /// Number of enemy agents the wave will create.
	    /// </summary>
		public override int EnemyNumber { get { return Number; } set { Number = value; } }

		protected override void Start ()
        {
			if (enemy == null)
				return;

			base.Start ();
		}

	    /// <summary>
	    /// Handles the wave creation.
	    /// </summary>
		protected override IEnumerator WaveCreation ()
        {
			GameObject currentEnemy;

			float spawnPosition = StartPosition;

			for (int i = 0; i < EnemyNumber; i++)
            {	
				currentEnemy = (GameObject)Instantiate (enemy , EnemyParent);
			    currentEnemy.SetActive(true);

                RiseWaveSpawn(currentEnemy);

			    //Positions the enemy to the spawn side.
                switch (SpawnSide)
                {
				case RectSides.Top:
					currentEnemy.transform.position = new Vector3 (Mathf.Lerp(GameField.xMin, GameField.xMax, spawnPosition), (GameField.yMax + Offset), transform.position.z);
					break;
				case RectSides.Right:
					currentEnemy.transform.position = new Vector3 (GameField.xMax + Offset,  Mathf.Lerp(GameField.yMax, GameField.yMin, spawnPosition), transform.position.z);
					break;
				case RectSides.Left:
					currentEnemy.transform.position = new Vector3 (GameField.xMin - Offset,  Mathf.Lerp(GameField.yMax, GameField.yMin, spawnPosition), transform.position.z);
					break;
				case RectSides.Bottom:
					currentEnemy.transform.position = new Vector3 (Mathf.Lerp(GameField.xMin, GameField.xMax, spawnPosition), (GameField.yMin - Offset), transform.position.z);
					break;
				}

                //move the spawn position by space between, and clamp it between 0 and 1
				spawnPosition += SpaceBetween;
                spawnPosition = Mathf.Clamp (spawnPosition, 0f, 1f);

			    //stops waiting if this is the last enemy
                if (i + 1 != Number)
				{
					yield return new WaitForSeconds(TimeBetween);
				}
			}

			RiseWaveCreationEnd();

	        //Destroys the wave after the creation end.
            DestroyWave();

			yield return null;
		}

	}

}

