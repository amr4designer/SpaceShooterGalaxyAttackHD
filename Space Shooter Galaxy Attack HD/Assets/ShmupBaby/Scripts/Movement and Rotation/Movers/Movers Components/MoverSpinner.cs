using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Component which gives the mover the ability to spin.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Mover Spinner")]
    public class MoverSpinner : MonoBehaviour , IRotate
    {
        /// <summary>
        /// A list of the available spin options.
        /// </summary>
        public enum SpinMode
        {
            Normal,
            Loop,
            SpinByDirection
        }

        /// <summary>
        /// The mover is attached to this component.
        /// </summary>
        [Tooltip("Reference to the spinner mover, this should be assigned if the mover" +
            "exists on other GameObject.")]
        public Mover mover;

        /// <summary>
        /// Spin speed (Degrees/Seconds).
        /// </summary>
        [Header("Spin Settings")]
		[Space]
        [Tooltip("How fast the spin will be (Degree/Seconds)")]
		public float SpinSpeed;
        /// <summary>
        /// An angle which will limit the spin from exceeding it.
        /// </summary>
        [Tooltip("Limit the spin from exceeding this angle.")]
		public float LimitAngle;
        /// <summary>
        /// The spin mode.
        /// </summary>
        public SpinMode Mode;
        
        /// <summary>
        /// The view type for this level.
        /// </summary>
        private LevelViewType View
        {
            get
            {
                return LevelController.Instance.View;
            }
        }
                
        /// <summary>
        /// The mover direction in the previous frame. 
		/// This is the direction which is perpendicular
		/// to the spin axis.
        /// </summary>
        private float _preDirection;


        private void Start()
        {
            if (mover == null)
            {
                mover = GetComponent<Mover>();
            }                

            RotationManager rotationManager = GetComponent<RotationManager>();
            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }               

            if (mover is PlayerMover)
            {
                rotationManager.SetFaceAngleToPlayer();
            }
            else
            {
                rotationManager.SetFaceAngleToEnemy();
            }
                
            if (View == LevelViewType.Vertical)
            {
                rotationManager.Subscribe(this, RotationAxis.Y);
            }               

            if (View == LevelViewType.Horizontal)
            {
                rotationManager.Subscribe(this, RotationAxis.X);
            }                
        }

        /// <summary>
        /// Edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees.</param>
        /// <returns>The change in the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            //Spins the object depending on the spinner mode.   
            switch (Mode)
            {
                case SpinMode.Normal:
                    return NormalSpin(angle);

                case SpinMode.Loop:
                    return LoopSpin(angle);

                case SpinMode.SpinByDirection:
                    return DirectionSpin(angle);
            }

            return angle;
        }

        /// <summary>
        /// Edits the spin angle for the normal mode.
        /// </summary>
        /// <param name="angle">The spin angle</param>
        /// <returns>The spin angle after spinning in normal mode.</returns>
        protected float NormalSpin ( float angle )
        {
            //Makes sure that we don't exceed the limit angle.
            if (!ReachLimitAngle(angle))
		    {
                //Increases the spin angle by the spin speed.
                angle += SpinSpeed * Time.deltaTime;

		    }

            return angle;
        }

        /// <summary>
        /// Edits the spin angle in the loop spin mode.
        /// </summary>
        /// <param name="angle">The spin angle</param>
        /// <returns>The spin angle after spinning in the loop mode.</returns>
		protected float LoopSpin ( float angle ) {

            //Gives permission to spin if:
            // - The spin angle didn't exceed the spin angle.
            // - The spin speed will move the spin angle away from the limit angle.
            if (!ReachLimitAngle(angle) ||
                SpinSpeed < 0 && angle > 0 ||
		        SpinSpeed > 0 && angle < 0 )
		    {

                angle += SpinSpeed * Time.deltaTime;

		    }
		    else
                //Inverses the spin speed in case the limit angle is reached.
		        SpinSpeed = -SpinSpeed;

            return angle;           
        }
                
        /// <summary>
        /// Edits the spin angle for the Directional spin mode.
        /// </summary>
        /// <param name="angle">The spin angle</param>
        /// <returns>The spin angle after spinning in the Directional mode.</returns>
		protected float DirectionSpin ( float angle ) {

		    const float angleTolrance = 0.1f;

            //Getting the direction of the target mover in the axis perpendicular to the spin axis.
            float targetDirection = 0;

		    if (View == LevelViewType.Horizontal)
		        targetDirection = mover.Direction.y;

		    if (View == LevelViewType.Vertical)
		        targetDirection = mover.Direction.x;

			//If the target isn't moving; the spin angle will return to zero.
            if (targetDirection == 0)
		    {
		        if (angle > angleTolrance)
                    angle -= Mathf.Abs(SpinSpeed) * Time.deltaTime;
		        if (angle < -angleTolrance)
                    angle += Mathf.Abs(SpinSpeed) * Time.deltaTime;
		    }


		    if (ReachLimitAngle(angle))
		    {
				//If the limit angle is reached and the mover didn't change direction
                //from the last frame then the spin angle won't be edited.
		        if (_preDirection > 0 && targetDirection > 0 ||
		            _preDirection < 0 && targetDirection < 0)
		            return angle;
		    }

            //Spins the target depending on the spin direction.
            if (targetDirection > 0)
                angle += SpinSpeed * Time.deltaTime;
            if (targetDirection < 0)
		        angle += -SpinSpeed * Time.deltaTime;
		    
            //Caches the mover direction of the past frame.
		    _preDirection = targetDirection;
                        
            return Mathf.Clamp(angle, -LimitAngle, LimitAngle); ;
        }

        /// <summary>
        /// Checks if the angle reached the limit angle.
        /// </summary>
        /// <param name="angle">The angle to check.</param>
        /// <returns>True if the limit angle has been reached.</returns>
        private bool ReachLimitAngle(float angle)
        {
            if (Mathf.Abs(angle) < LimitAngle || LimitAngle == 0)
            {
                return false;
            }
            else
            {
                return true;
            }               
        }
                                
    }

}