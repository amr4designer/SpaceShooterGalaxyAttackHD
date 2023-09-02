using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Mover that is specific to the missile, follows the target with 
    /// a "lazy" rotation towards the target.
    /// </summary>
    [AddComponentMenu("")]
    public class MissileMover : Mover , IRotate
    {
        /// <summary>
        /// The option for the mover target.
        /// </summary>
        [Tooltip("The target that will be followed by this mover.")]
        public TargetOption targetOption;
        /// <summary>
        /// The input target that is used when the targetOption is set to input.
        /// </summary>
        [Tooltip("This will be used if the target option is set to input.")]
        public Agent Target;
        /// <summary>
        /// Input speed by the inspector.
        /// </summary>
        [Tooltip("Speed in (World Unit/Seconds).")]
        public float Speed;
        /// <summary>
        /// The turning speed (Degree/Seconds).
        /// </summary>
        [Tooltip("turning speed (Degree/Seconds)")]
        public float TurnSpeed;
              
        /// <summary>
	    /// Current speed for the mover (World Unit/Seconds).
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
        
        /// <summary>
        /// The angle towards the target.
        /// </summary>
        private float _desireAngle;
        /// <summary>
        /// The angle that is considered as the origin to start the Z-Axis rotation from it.
        /// </summary>
        private float _faceAngle;
        
        /// <summary>
        /// The current rotation on the Z-Axis.
        /// </summary>
        [HideInInspector]
        public float FollowAngle;

        /// <summary>
        /// Tracker used by this component.
        /// </summary>
        public Tracker tracker
        {
            get;
            set;
        }


        void Start()
        {
            //Set the rotation manager and the tracker.
            RotationManager rotationManager = GetComponent<RotationManager>();
            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }               
            rotationManager.Subscribe(this, RotationAxis.Z);           
                       
            if ( tracker == null)
            {
                tracker = gameObject.AddComponent<Tracker>();
            }
                

            tracker.Target = targetOption;
            tracker.InputTarget = Target;

            rotationManager.SetFaceAngleToTarget(tracker);

            _faceAngle = rotationManager.FacingAngle;
            FollowAngle += _faceAngle;
        }

        /// <summary>
        /// UpdateDirection called on every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
        
            _desireAngle = Math2D.VectorToDegree(tracker.Direction);
            
            //The angle between the current rotation and the rotation towards the target.
            float angleBetween = FollowAngle - _desireAngle;
            
            //Move the direction towards the angle and towards the target using the turn speed. 
            if (Mathf.Abs(angleBetween) > 1)
            {
                if (angleBetween > 0)
                    FollowAngle -= TurnSpeed * Time.deltaTime;
                else
                    FollowAngle += TurnSpeed * Time.deltaTime;
            }

            return Math2D.DegreeToVector2(FollowAngle);

        }

        /// <summary>
        /// Edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees.</param>
        /// <returns>The change in the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            return FollowAngle;
        }
    }

}