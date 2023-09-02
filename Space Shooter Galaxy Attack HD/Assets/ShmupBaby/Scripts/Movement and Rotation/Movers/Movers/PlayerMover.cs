using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Mover that responds to the Input control to move.
    /// </summary>
	[AddComponentMenu("Shmup Baby/Agent/Player/Player Mover")]
    [RequireComponent(typeof(Rigidbody2D))]
	[SelectionBase]
	public class PlayerMover : Mover
	{
	    /// <summary>
	    /// Input speed by the inspector.
	    /// </summary>
	    [Space]
	    [Tooltip("Speed (World Unit/Seconds).")]
        public float Speed = 5;
		

        /// <summary>
        /// The boundary for this mover in the XY plane.
        /// </summary>
		[Header("Boundary")]
        [RectHandle("Player Moving Field" , 0.5f , BasicColors.Cyan , BasicColors.Green)]
        [Tooltip("The boundary that will prevent the player from passing it")]
		public Rect Boundary;

        [Tooltip("Draw the boundary region in the scene view.")]
        public bool DrawBoundary;
               
	    /// <summary>
	    /// Current speed for the mover in (World Unit/Seconds).
	    /// </summary>
	    public override float speed
	    {
	        get { return Speed; }
	        set { Speed = value; }
	    }


        void Start ()
        {
			rigidbody = GetComponent<Rigidbody2D> ();            
		}


        void LateUpdate()
        {
            //Clamps the player position to its boundary.
            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, Boundary.xMin, Boundary.xMax),
                Mathf.Clamp(transform.position.y, Boundary.yMin, Boundary.yMax),
                transform.position.z
            );
        }


        /// <summary>
        /// UpdateDirection is called on every frame to update the mover direction.
        /// </summary>
        /// <returns>the current mover direction.</returns>
        protected override Vector2 UpdateDirection()
	    {
            return InputManager.Instance.GetInput(PlayerID.Player1).Direction;
	    }

#if UNITY_EDITOR

        void OnDrawGizmos ()
        {
            //Draws player boundary
			if (!DrawBoundary)
            {
                return;
            }
				
			GizmosExtension.DrawRect (Boundary);
		}

#endif

    }

}