using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Mover that moves the object in a given straight direction.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/Directional Mover")]
    public class DirectionalMover : Mover
	{
        [Tooltip("Draw the path that the Mover will take")]
		public bool DrawPath;


        /// <summary>
        /// Input speed by the inspector.
        /// </summary>
        [Tooltip("speed (World Unit/Sec).")]
		public float Speed ;
        /// <summary>
        /// Mover direction by inspector, describes the direction using
        /// EightDirection enumerator.
        /// </summary>
	    [Space]
        [Tooltip("The direction of the mover")]
        public EightDirection MoverDirection;

        //This only gives the direction. The magnitude doesn't matter.

        /// <summary>
        /// Mover direction by inspector, describes the direction using
        /// a vector 2.
        /// </summary>
        [HideInInspector]
	    public Vector3 direction;

	    /// <summary>
	    /// Current speed for the mover in (World Unit/Sec).
	    /// </summary>
	    public override float speed
	    {
	        get {return Speed;}
	        set {Speed = value;}
	    }

        /// <summary>
        /// The direction which will be used by the mover.
        /// </summary>
	    private Vector2 _direction;


        void Start ()
        {
			//Normalizes the direction so it doesn't affect the speed
			direction.Normalize ();

			//If the MoverDirection is set to none the direction;
            //represented by the vector will be used.

            if (MoverDirection != EightDirection.None)
		        _direction = Directions.EightDirectionToVector(MoverDirection);
            else
		        _direction = direction;           
		}

        /// <summary>
        /// UpdateDirection is called on every frame and updates the mover direction.
        /// </summary>
        /// <returns>the current mover direction.</returns>
	    protected override Vector2 UpdateDirection()
	    {
	        return _direction;
	    }

        /// <summary>
        /// Changes the mover direction
        /// </summary>
        /// <param name="dir">New direction for the mover.</param>
		public void ChangeDirection ( Vector3 dir )
        {
			_direction = dir;
		}

#if UNITY_EDITOR

        void OnDrawGizmos ()
        {
		    if (!DrawPath)
                return;

            Gizmos.color = Color.yellow;

            if ( MoverDirection == EightDirection.None )
			    //Draws the direction of the mover (the length of the line present the speed)
			    Gizmos.DrawLine (transform.position, transform.position + direction.normalized * Speed);
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)Directions.EightDirectionToVector(MoverDirection) * Speed);
            }        
		}

#endif

    }

}