using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    ///  Mover that moves the object in a given direction,
    ///  and snaps back to the starting position after it passes 
    ///  a certain distance,
    /// </summary>
	[AddComponentMenu("Shmup Baby/Utilities/RepeaterMover")]
    [SelectionBase]
	public class RepeaterMover : Mover
	{

        /// <summary>
        /// Input speed by the inspector.
        /// </summary>
        [Tooltip("Speed in (World Unit/Seconds).")]
        public float Speed;
        /// <summary>
        /// The distance at which the mover will be reset to its original position,
        /// and start again.
        /// </summary>
		[Tooltip("After reaching this distance the mover will get back from where it begins")]
		public float  RepeatDistance ;
        /// <summary>
        /// Mover direction by inspector, describes the direction using
        /// EightDirection enumerator.
        /// </summary>
        [Tooltip("the direction of the mover")]
        public EightDirection MoverDirection;
        /// <summary>
        /// Rotates the mover to match the direction.
        /// </summary>
        [Space]
        [Tooltip("Uses the rotation on the Z axis instead")]
		public bool UseRotation = false;         

        /// <summary>
		/// Mover direction by inspector, describe the direction using vector 2.
        /// </summary>
        [HideInInspector]
        public Vector3 direction;

        /// <summary>
        /// trigger when the mover has reached it's repeat distance.
        /// </summary>
        public event ShmupDelegate OnRepeat;

        /// <summary>
        /// Current speed for the mover in (World Unit/Seconds).
        /// </summary>
        public override float speed
        {
            get
            {
                return Speed;
            }
            set
            {
                Speed = value;
            }
        }

        public float Distance
        {
            get
            {
                return _distance;
            }
        }

        /// <summary>
        /// The direction which will be used by the mover.
        /// </summary>
	    private Vector2 _direction;
        /// <summary>
        /// The start position of the mover.
        /// </summary>
        private Vector3 _startPosition;
        /// <summary>
        /// Distance passed by the mover.
        /// </summary>
        private float _distance;


        void Start () {

            // Normalizes the direction so it doesn't affect the speed.
            direction.Normalize();

            // If the MoverDirection is set to none the direction
            // represented by the vector will be used.
            if (MoverDirection != EightDirection.None)
            {
                _direction = Directions.EightDirectionToVector(MoverDirection);
            }
            else
            {
                _direction = direction;
            }
                
            // Saves the first position.
            _startPosition = transform.position;

            if (UseRotation)
            {
                transform.rotation = 
                    Quaternion.Euler(0, 0, Math2D.VectorToDegree(_direction) + 270f);
            }
                
		}

        /// <summary>
        /// UpdateDirection is called on every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
            return _direction;
        }

        /// <summary>
        /// Changes the mover direction.
        /// </summary>
        /// <param name="dir">New direction for the mover.</param>
		public void ChangeDirection(Vector3 dir)
        {
            _direction = dir;
        }


        void LateUpdate ()
        {
            //Resets the mover to its start position if it exceeds the repeat distance.
            _distance += DeltaDistance;

            if (_distance >= RepeatDistance)
            {
                transform.position = _startPosition;
                _distance = 0;

                //trigger OnRepeat
                if (OnRepeat != null)
                    OnRepeat(null);
            }            			
		}

	}

}