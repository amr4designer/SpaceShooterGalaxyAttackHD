using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Axis of rotation.
    /// </summary>
    public enum RotationAxis
    {
        X,
        Y,
        Z,
    }

    /// <summary>
    /// Defines the behavior for the components that's responsible for the rotation of the Object.
    /// </summary>
    public interface IRotate
    {
        float EditRotation(float angle);
    }

    /// <summary>
    /// Handles the rotation order for this package, and
    /// ensures that only one component is responsible for 
    /// each rotation axis.
    /// </summary>
    [AddComponentMenu("")]
    public class RotationManager : MonoBehaviour
    {
        /// <summary>
        /// The change in rotation on the X-Axis from the start rotation.
        /// </summary>
        public float RotationOnX { get; private set; }
        /// <summary>
        /// The change in rotation on the Y-Axis from the start rotation.
        /// </summary>
        public float RotationOnY { get; private set; }
        /// <summary>
        /// The change in rotation on the Z-Axis from the start rotation.
        /// </summary>
        public float RotationOnZ { get; private set; }

        /// <summary>
        /// The angle that this object visual is facing,
        /// The angle at which the rotation will start from 
        /// on the Z-Axis.
        /// </summary>
        public float FacingAngle { get; set; }
        /// <summary>
        /// Snaps the FacingAngle to one of the four directional angles.
        /// </summary>
        public FourDirection FacingDirection
        {
            set
            {
                if (value != FourDirection.None)
                {
                    FacingAngle = Directions.FourDirectionToAngle(value);
                }
            }
        }

        protected LevelViewType View
        {
            get { return LevelController.Instance.View; }
        }

        /// <summary>
        /// The angle on the Z-Axis when the object starts.
        /// </summary>
        private Quaternion _startRotation;

        /// <summary>
        /// Handle for the object that is responsible of the rotation on the X-Axis.
        /// </summary>
        private IRotate _rotatorOnX;
        /// <summary>
		/// Handle for the object that is responsible of the rotation on the Y-Axis.
        /// </summary>
        private IRotate _rotatorOnY;
        /// <summary>
		/// Handle for the object that is responsible of the rotation on the Z-Axis.
        /// </summary>
        private IRotate _rotatorOnZ;


        void Start()
        {
            _startRotation = transform.rotation;
        }


        void Update()
        {
            transform.rotation = _startRotation;

            if (View == LevelViewType.Vertical)
            {
                RotateOnX();
                RotateOnY();
            }
            if (View == LevelViewType.Horizontal)
            {
                RotateOnY();
                RotateOnX();
            }
            RotateOnZ();
        }

        /// <summary>
        /// Subscribes an object to edit the rotation on a given axis.
        /// </summary>
        /// <param name="rotator">Handle for the object responsible for the rotation</param>
        /// <param name="axis">The axis which the object will handle the rotation on.</param>
        /// <returns>False if there is already an object to handle the rotation on that axis.</returns>
        public bool Subscribe (IRotate rotator , RotationAxis axis)
        {
            switch (axis)
            {
                case RotationAxis.X:

                    _rotatorOnX = rotator;
                    if (_rotatorOnX == null)
                        return true;
                    else
                        return false;
                
                case RotationAxis.Y:

                    _rotatorOnY = rotator;
                    if (_rotatorOnY == null)
                        return true;
                    else
                        return false;
                    
                case RotationAxis.Z:

                    _rotatorOnZ = rotator;
                    RotationOnZ = FacingAngle;
                    if (_rotatorOnZ == null)
                        return true;
                    else
                        return false;                    
            }

            return true;
        }

        /// <summary>
        /// Sets the face angle in a smart way according to this object tracker's target.
        /// </summary>
        /// <param name="tracker"></param>
        public void SetFaceAngleToTarget (Tracker tracker)
        {
            switch (tracker.Target)
            {
                case TargetOption.InputAgent:
                    if (tracker.InputTarget != null && tracker.InputTarget is Player)
                        SetFaceAngleToPlayer();
                    else
                        SetFaceAngleToEnemy();
                    break;
                case TargetOption.InputPosition:
                    SetFaceAngleToEnemy();
                    break;
                case TargetOption.Player:
                    SetFaceAngleToEnemy();
                    break;
                case TargetOption.RandomEnemy:
                    SetFaceAngleToPlayer();
                    break;
            }
        }

        /// <summary>
        /// Sets the facing angle to match an enemy's agent.
        /// </summary>
        public void SetFaceAngleToEnemy()
        {
            if (View == LevelViewType.Vertical)
                FacingDirection = FourDirection.Down;

            if (View == LevelViewType.Horizontal)
                FacingDirection = FourDirection.Left;
        }

        /// <summary>
        /// Sets the facing angle to match the player's agent.
        /// </summary>
        public void SetFaceAngleToPlayer()
        {
            if (View == LevelViewType.Vertical)
                FacingDirection = FourDirection.Up;

            if (View == LevelViewType.Horizontal)
                FacingDirection = FourDirection.Right;

        }

        /// <summary>
        /// Rotates the object on the X-Axis.
        /// </summary>
        private void RotateOnX ()
        {
            if (_rotatorOnX != null)
            {
                RotationOnX = _rotatorOnX.EditRotation(RotationOnX);
                transform.RotateAround(transform.position, Vector3.right, RotationOnX);
            }
        }

        /// <summary>
        /// Rotates the object on the Y-Axis.
        /// </summary>
        private void RotateOnY()
        {
            if (_rotatorOnY != null)
            {
                RotationOnY = _rotatorOnY.EditRotation(RotationOnY);
                transform.RotateAround(transform.position, Vector3.up, RotationOnY);
            }
        }

        /// <summary>
        /// Rotates the object on the Z-Axis.
        /// </summary>
        private void RotateOnZ()
        {
            //Subtracts the FacingAngle from the rotation to act as the start angle for the Z-Axis
            if (_rotatorOnZ != null)
            {
                RotationOnZ = _rotatorOnZ.EditRotation(RotationOnZ);
                transform.RotateAround(transform.position, Vector3.forward, RotationOnZ - FacingAngle);
            }
        }

    }

}
