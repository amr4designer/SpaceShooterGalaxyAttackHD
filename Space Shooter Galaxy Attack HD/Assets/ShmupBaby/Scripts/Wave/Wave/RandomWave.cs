using System.Collections;
using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// An enemy wave that spawn a random enemy in random time and area.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Wave/Random Wave")]
    public sealed class RandomWave : Wave {

        /// <summary>
        /// The enemy that is going to be spawned by the wave
        /// </summary>
		[Space]
        [Tooltip("List of enemies to spawn randomly")]
		public Object[] enemy;

        /// <summary>
        /// Number of enemies spawned by this wave.
        /// </summary>
		[Space]
        [Tooltip("Number of enemies spawned by this wave")]
		public int Number;
       
        /// <summary>
        /// game flied side which the wave will spawn the enemy from.
        /// </summary>
		[Space]
        [Tooltip("The side of the game field which the enemy will spawn from")]
		public RectSides SpawnSide;

        /// <summary>
        /// time between each enemy spawn.
        /// </summary>
        [Space]
        [Tooltip("Time between each enemy spawn")]
		public float TimeBetween;
        /// <summary>
        /// Indicates if the wave is spawning an enemy with a random wait time in between.
        /// </summary>
        [Tooltip("Checks if you want to randomize the time between each spawn")]
        public bool RandomTime ;
        /// <summary>
        /// The minimum wait time between enemies spawned, this is used if RandomTime set to true.
        /// </summary>
        [Tooltip("This time will be used if RandomTime is checked")]
        public float MinTime;
        /// <summary>
        /// The maximum wait time between enemyies spawned, used if RandomTime is set to true.
        /// </summary>
        [Tooltip("This time will be used if RandomTime is checked")]
        public float MaxTime;

        /// <summary>
        /// Spawn an enemy with random rotation
        /// </summary>
        [Space]
        [Tooltip("Spawn the enemy with a random rotation")]
	    public bool RandomRotation;


        /// <summary>
        /// Offsets for the enemy spawn position from the edge of the game field.
        /// </summary>
        [Space]
        [Tooltip("To prevent an enemy from spawning at the edge of the game field")]
        public float Offset = 2 ;

        /// <summary>
        /// Number of enemy agents the wave will create.
        /// </summary>
		public override int EnemyNumber { get { return Number; } set { Number = value; } }

        /// <summary>
        /// handles the wave creation.
        /// </summary>
		protected override IEnumerator WaveCreation () {

			if (enemy.Length > 0) {

				for (int i = 0; i < Number; i++) {
					
                    //picks a random enemy to spawn.
				    GameObject currentEnemy = (GameObject)Instantiate (enemy[Random.Range(0,enemy.Length)] , EnemyParent);
				    currentEnemy.SetActive(true);

                    RiseWaveSpawn(currentEnemy);

                    //positions the enemy to the spawn side.
                    switch (SpawnSide) {

					case RectSides.Top:
					    currentEnemy.transform.position = new Vector3 ( Mathf.Lerp( GameField.xMin , GameField.xMax , Random.value ) , GameField.yMax + Offset , transform.position.z);
						break;
					case RectSides.Right:
					    currentEnemy.transform.position = new Vector3 (  GameField.xMax + Offset ,  Mathf.Lerp( GameField.yMin , GameField.yMax , Random.value ) , transform.position.z);
						break;
					case RectSides.Left:
					    currentEnemy.transform.position = new Vector3 (  GameField.xMin - Offset ,  Mathf.Lerp( GameField.yMin , GameField.yMax , Random.value ) , transform.position.z);
						break;
					case RectSides.Bottom:
					    currentEnemy.transform.position = new Vector3 ( Mathf.Lerp( GameField.xMin , GameField.xMax , Random.value ) , GameField.yMin - Offset , transform.position.z);
						break;
					}
					
                    if (RandomRotation)
				    {
                        Vector3 enemyRotation = currentEnemy.transform.eulerAngles;

                        if (ViewType == LevelViewType.Vertical)
				            enemyRotation.z = Random.Range(0,180);
				        if (ViewType == LevelViewType.Horizontal)
				            enemyRotation.z = Random.Range(-90, 90);

				        currentEnemy.transform.eulerAngles = enemyRotation;

				    }

                    //Stops waiting if this is the last enemy
					if (i + 1 != Number) {
						
						if (RandomTime)
							yield return new WaitForSeconds (Random.Range (MinTime, MaxTime));
						else
							yield return new WaitForSeconds (TimeBetween);
						
					}
				}
			}
				
			RiseWaveCreationEnd ();

            //destroys the wave after the spawning ends.
			DestroyWave ();

			yield return null;

		}

	}
}