using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Rotates the object towards a given target using rotation speed.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Utilities/Focus Rotator")]
    public class FocusRotator : MonoBehaviour ,IRotate
    {
        /// <summary>
        /// The option for the Rotator target.
        /// </summary>
        [Tooltip("The target that will be Focused on by this rotator.")]
        [Space]
        public TargetOption targetOption;
        /// <summary>
        /// The input target used when the targetOption is set to input.
        /// </summary>
        [Tooltip("This will be used if the target option is set to input.")]
        public Agent Target;
        /// <summary>
        /// Rotation speed to focus on target.
        /// </summary>
        [Space]
        [Tooltip("Rotation speed in (Degree/Seconds)")]
        public float RotationSpeed;

        /// <summary>
        /// Tracker used to track the target.
        /// </summary>
        private Tracker _tracker;
        /// <summary>
        /// The angle on the Z-Axis when the object starts.
        /// </summary>
        private float _startAngle;
        /// <summary>
		/// The angle that this rotator will try to match using rotation speed.
        /// </summary>
        private float _desireAngle;


        void Start()
        {
            //Sets the rotation manager and the tracker.
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
                rotationManager = gameObject.AddComponent<RotationManager>();
                        
            if (_tracker == null)
                _tracker = gameObject.AddComponent<Tracker>();

            _tracker.Target = targetOption;
            _tracker.InputTarget = Target;

            rotationManager.SetFaceAngleToTarget(_tracker);
            rotationManager.Subscribe(this, RotationAxis.Z);

            _startAngle = rotationManager.FacingAngle + transform.eulerAngles.z;
        }

        /// <summary>
        /// Edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees.</param>
        /// <returns>The change in the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            //Sets the rotation to start rotation if the target is lost.
            if (_tracker.TrackingState == TrackState.TargetLost)
                _desireAngle = _startAngle;
            else
                _desireAngle = Math2D.VectorToDegree(_tracker.Direction);

            //The angle between the current rotation and the rotation towards the target.
            float angleBetween = angle - _desireAngle;

            //Moves the current angle towards the target using rotation speed. 
            if (Mathf.Abs(angleBetween) > 1)
            {
                if (angleBetween > 0)
                    angle -= RotationSpeed * Time.deltaTime;
                else
                    angle += RotationSpeed * Time.deltaTime;
            }

            return angle;
        }       

    }

}
