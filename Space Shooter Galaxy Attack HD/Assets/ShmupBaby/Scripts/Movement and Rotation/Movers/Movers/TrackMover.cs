using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Mover that moves the object toward a given target.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/Track Mover")]
    public class TrackMover : Mover , IRotate
	{

        /// <summary>
        /// A list of the track mover moving states.
        /// </summary>
        public enum TrackMoverStatue
        {
            Normal,
            Tracking
        }

        /// <summary>
        /// The option for the mover target.
        /// </summary>
        [Tooltip("The target that will be followed by this mover.")]
        public TargetOption targetOption;
        /// <summary>
        /// The input target is used when the targetOption is set to input.
        /// </summary>
        [Tooltip("This will be used if the target option is set to input.")]
        public Agent Target;
        /// <summary>
        /// The radius at which the mover will start moving towards the target.
        /// </summary>
        [Space]
        [Tooltip("The radius at which the mover will start moving towards the target")]
		public float DetectRadius;
        /// <summary>
        /// The mover speed when it doesn't detect the target.
        /// </summary>
		[Space]
        [Tooltip("The mover speed when it doesn't detect the target")]
		public float NormalSpeed;
        /// <summary>
        /// The speed when a target is detected.
        /// </summary>
        [Space]
        [Tooltip("the speed when another target is detected")]
		public float StartSpeed;
        /// <summary>
        /// The speed when the target is reached.
        /// </summary>
        [Tooltip("The speed when the target is reached")]
		public float EndSpeed;
        /// <summary>
        /// locks the rotation towards the target.
        /// </summary>
        [Space]
        [Tooltip("locks the rotation towards the target")]
		public bool TargetLock;


        [Tooltip("Draws the radius of detection")]
		public bool DrawGizmo;

        
        /// <summary>
        /// Tracker used by this mover to track the target.
        /// </summary>
        public TrackerDetector Tracker
        {
            set
            {
                _tracker = value;
            }
            get
            {
                return _tracker;
            }
        }

        /// <summary>
        /// The view type of the level.
        /// </summary>
        protected LevelViewType View
        {
            get
            {
                return LevelController.Instance.View;
            }
        }

        /// <summary>
        /// The current moving state for this mover. 
        /// </summary>
        private TrackMoverStatue _state;
        /// <summary>
        /// Tracker used by this mover to track the target.
        /// </summary>
        private TrackerDetector _tracker;


        private void Start ()
        {           
             _state = TrackMoverStatue.Normal;

            //Sets the rotation manager and the tracker.
            if (_tracker == null)
            {
                _tracker = gameObject.AddComponent<TrackerDetector>();
            }
                
            _tracker.DetectRadius = DetectRadius;
            _tracker.Target = targetOption;
            _tracker.InputTarget = Target;
            
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }
                
            rotationManager.Subscribe(this, RotationAxis.Z);
            rotationManager.SetFaceAngleToTarget(_tracker);
        }

        /// <summary>
        /// UpdateDirection is called on every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
            //Sets the direction to either towards the target or to its normal direction.
            switch (_tracker.TrackingState)
            {
                case TrackState.TargetReached:
                    return Vector3.zero;
                case TrackState.TargetDetected:
                    _state = TrackMoverStatue.Tracking;
                    return _tracker.Direction;
            }

            _state = TrackMoverStatue.Normal;
            return NormalDirection();
                        
        }

        /// <summary>
        /// Edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees.</param>
        /// <returns>the change of the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            if (_state == TrackMoverStatue.Tracking && TargetLock)
                return Math2D.VectorToDegree(_tracker.Direction);
            else
                return angle;
        }

        /// <summary>
        /// One of Unity's messages, called after all Update functions have been called.
        /// </summary>
        void LateUpdate()
        {
            //Sets the mover speed based on the moving state.
            switch (_state)
            {
                case TrackMoverStatue.Normal:
                    speed = NormalSpeed;
                    break;
                case TrackMoverStatue.Tracking:
                    speed = Math2D.Accp(StartSpeed, EndSpeed, 1 - _tracker.Distance / DetectRadius);
                    break;
            }

        }

        /// <summary>
        /// Returns the direction for the normal moving state.
        /// </summary>
        /// <returns>The direction for the normal moving state.</returns>
        private Vector2 NormalDirection ()
        {           
			if (View == LevelViewType.Vertical)
				return Vector3.down;

			if (View == LevelViewType.Horizontal)
				return Vector3.left;

            return Vector3.zero;            			
		}

#if UNITY_EDITOR

        void OnDrawGizmos ()
        {

            //Draws the detect radius for this mover.

			Gizmos.color = Color.yellow;

		    if (DrawGizmo)
				GizmosExtension.DrawCircle (transform.position, DetectRadius , transform.position.z);
		}

#endif

    }
		

}