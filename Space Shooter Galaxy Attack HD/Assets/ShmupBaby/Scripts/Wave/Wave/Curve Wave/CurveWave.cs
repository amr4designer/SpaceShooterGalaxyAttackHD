using UnityEngine;
using System.Collections;

namespace ShmupBaby {

    /// <summary>
    /// Enemy wave that spawns an enemy on a defined curve.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Wave/Curve Wave")]
    public sealed class CurveWave : Wave
	{

        #if UNITY_EDITOR

        /// <summary>
        /// only available in the editor, shows the index of the controlling point.  
        /// </summary>
        [Header("Handle Settings")]
		[Space]
        [Tooltip("Shows the index of the controlling point")]
		public bool ShowLabel = true;
        /// <summary>
        /// Only available in the editor, shows a mover handle to control points similar to a position handle.  
        /// </summary>
        [Tooltip("Shows a mover handle to control points similar to a position handle")]
        public bool UsePositionHandle = false;
        /// <summary>
        /// only available in the editor, shows a mover handle to controlpoints with circle shapes.  
        /// </summary>
        [Tooltip("show a mover handle to control points with a circle shape")]
        public bool UseCircleHandle = true;
        /// <summary>
        /// only available in the editor, controls the size of the circle handle.  
        /// </summary>
        [Tooltip("Controls the size of the circle handle.")]
        public float HandleSize = 0.5f;

        #endif

	    /// <summary>
	    /// Number of enemies spawn by this wave.
	    /// </summary>
        [Header("Wave Settings")]
		[Space]
	    [Tooltip("Number of enemies spawned by this wave")]
        public int Number;
	    /// <summary>
	    /// The enemy that is going to be spawned by the wave.
	    /// </summary>
	    [Tooltip("The enemy that is going to be spawned by the wave")]
        public GameObject enemy ;
	    /// <summary>
	    /// Time between each enemy spawned.
	    /// </summary>
	    [Space]
	    [Tooltip("Time between each enemy spawned")]
        public float TimeBetween ;
        /// <summary>
        /// Speed of the enemy spawned by the wave
        /// </summary>
		[Tooltip("Speed of the enemy on the curve")]
		public float Speed ;
        /// <summary>
        /// indicates if the enemy loops forward and backward on the curve until it's destroyed,
        /// if the enemy isn't set to loop the enemy will be destroyed when it reaches the end of the curve.
        /// </summary>
		[Tooltip("looping the enemy forward and backward on the curve until it's destroyed, " +
			"or it's destroyed when it reaches the end of the curve")]
		public bool Loop;
        /// <summary>
        /// rotates the enemy to follow the curve tangent.
        /// </summary>
		[Tooltip("Make the rotation of the enemy follow the curve")]
		public bool FollowPath ;

        /// <summary>
        /// controlling points of the curve, position is in world space
        /// </summary>
		[Header("Curve Settings")]
		[Space]
        [Tooltip("Controlling the points of the curve, position is in world space")]
		public Vector2[] Points ;
        /// <summary>
        /// smooth value for the curve path.
        /// </summary>
        [Tooltip("A high value will make the curve smoother")]
		public float Smooth;
        /// <summary>
        /// Snap option for the first control points to the game field rectangle.
        /// </summary>
        [Tooltip("An option to snap the first point on the curve to the game field rectangle")]
		public PointSnapOptions FirstPointSnap;
	    /// <summary>
	    /// snap option for the last control point to the game field rectangle.
	    /// </summary>
	    [Tooltip("option to snap the last point on the curve to the game field rectangle")]
        public PointSnapOptions LastPointSnap;
	    /// <summary>
	    /// offset for the enemy spawn position from the edge of the game field.
	    /// </summary>
	    [Space]
	    [Tooltip("To prevent an enemy from spawning at the edge of the game field")]
        public float Offset;

        /// <summary>
        /// Number of samples taken from the curve before passing it to the spawned enemy,
        /// higher value mean more computation and a smoother curve.
        /// </summary>
	    [Header("Curve Sampler")]
	    [Space]
        [Tooltip("Higher values mean more computation and a smoother curve")]
	    public int SampleNumber = 30;
        /// <summary>
        /// Sampler object for the curve initialized on start.
        /// </summary>
        [HideInInspector]
	    public Curve2DSampler Sampler;

		#if UNITY_EDITOR

        /// <summary>
        /// only available in the editor, draws the curve in the scene. 
        /// </summary>
		[Header("Gizmos Settings")]
		[Space]
        [Tooltip("draw the curve in the scene")]
		public bool DrawPath;

        #endif

	    /// <summary>
	    /// Number of the enemy agents that the wave will create.
	    /// </summary>
        public override int EnemyNumber { get { return Number; } set { Number = value; } }

        /// <summary>
        /// curve object.
        /// </summary>
		private Curve2D _curve ;

	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        protected override void Start ()
		{
            //Snaps the point to position.
			Points[0] = SnapPoint ( Points[0] , GameField , Offset , FirstPointSnap );
			Points[Points.Length-1] = SnapPoint ( Points[Points.Length-1] , GameField , Offset , LastPointSnap );

            //creates the curve object.
		    _curve = new Curve2D(Points)
		        { Smooth = Smooth};

            //samples the curve and save the sample in the sampler object.
            Sampler = new Curve2DSampler(_curve, SampleNumber);

            base.Start ();
		}

		/// <summary>
		/// Creates the enemy wave on the curve
		/// </summary>
		protected override IEnumerator WaveCreation () {

			for (int i = 0; i < Number; i++) {
                

				//Instantiates the enemy 
				GameObject currentEnemy = (GameObject)Instantiate (enemy , EnemyParent);
			    currentEnemy.SetActive(true);

                RiseWaveSpawn(currentEnemy);

                //Positions the enemy to the first point
                currentEnemy.transform.position = Math2D.Vector2ToVector3 ( Points[0] , transform.position.z ) ;

				//gets the curve mover component from the enemy.

				CurveMover mover = currentEnemy.GetComponent<CurveMover> ();
				
			    if (mover == null)
					mover = currentEnemy.AddComponent<CurveMover>();

				//passes the move settings to the mover
				mover.SamplerManger = new Curve2DSamplerManger(Sampler);
				mover.Speed = Speed;
				mover.Loop = Loop;
				mover.FollowPath = FollowPath;

			    //stops waiting if this is the last enemy
                if (i + 1 != Number)
			    {
			        yield return new WaitForSeconds(TimeBetween);
			    }
			}
            
			RiseWaveCreationEnd ();

		    //destrosy the wave after the spawning end.
            DestroyWave();

		    yield return null;

        }

        #if UNITY_EDITOR

        void OnDrawGizmos ()
        {

            //higher value means more computation and a smoother curve drawn.
            float Delta = 0.01f;

            if (!DrawPath)
				return;

	        Curve2D GizmoCurve = new Curve2D(Points)
	        {
	            Smooth = Smooth
	        };

	        Gizmos.color = Color.yellow;

			GizmoCurve.Draw ( Delta , transform.position.z );
		}

        #endif

    }

}