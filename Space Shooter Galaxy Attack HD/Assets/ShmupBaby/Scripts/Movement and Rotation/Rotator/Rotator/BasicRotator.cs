using UnityEngine;
using System.Collections;

namespace ShmupBaby
{
    /// <summary>
    /// Rotates the GameObject on one axis with a constant speed.
    /// </summary>
    public class Rotator : IRotate
    {
        /// <summary>
        /// Rotator constructor.
        /// </summary>
		/// <param name="rotationSpeed">The speed of the rotation in (Degrees/sec)</param>
        public Rotator (float rotationSpeed)
        {
            RotationSpeed = rotationSpeed;
        }

        /// <summary>
		/// The speed of the rotation in (Degrees/Seconds)
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        /// Edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees</param>
        /// <returns>The change in the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            return (angle + (RotationSpeed * Time.deltaTime));
        }
    }

    /// <summary>
    /// Rotates the gameObject at a constant speed for any given axis.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Utilities/Basic Rotator")]
    public class BasicRotator : MonoBehaviour 
    {
		[Tooltip("The rotation speed on the X-Axis in (Degrees/Seconds)")]
        public float SpeedOnX;
		[Tooltip("The rotation speed on the Y-Axis in (Degrees/Seconds)")]
        public float SpeedOnY;
		[Tooltip("The rotation speed on the Z-Axis in (Degrees/Seconds)")]
        public float SpeedOnZ;

        /// <summary>
        /// The object that rotates on the X-Axis.
        /// </summary>
        private Rotator _rotatorOnX;
        /// <summary>
        /// The object that rotates on the Y-Axis.
        /// </summary>
        private Rotator _rotatorOnY;
        /// <summary>
        /// The object that rotates on the Z-Axis.
        /// </summary>
        private Rotator _rotatorOnZ;


        void Start()
        {
            //Sets the rotation manager and the tracker.
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
                rotationManager = gameObject.AddComponent<RotationManager>();

            _rotatorOnX = new Rotator(SpeedOnX);
            _rotatorOnY = new Rotator(SpeedOnY);
            _rotatorOnZ = new Rotator(SpeedOnZ);

            rotationManager.Subscribe(_rotatorOnX, RotationAxis.X);
            rotationManager.Subscribe(_rotatorOnY, RotationAxis.Y);
            rotationManager.Subscribe(_rotatorOnZ, RotationAxis.Z);

        }


        private void Update()
        {
            _rotatorOnX.RotationSpeed = SpeedOnX;
            _rotatorOnY.RotationSpeed = SpeedOnY;
            _rotatorOnZ.RotationSpeed = SpeedOnZ;
        }
    }

}