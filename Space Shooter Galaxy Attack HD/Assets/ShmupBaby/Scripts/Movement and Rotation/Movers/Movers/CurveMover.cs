using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Enemy Mover that move the enemy on a given curve.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/Curve Mover")]
    public class CurveMover : Mover, IRotate
    {
        [Header("Gizmos Settings")]
        [Space]
        [Tooltip("Draws lines between the samples")]
        public bool DrawLine = true;
        /// <summary>
        /// Only available in the Unity editor, draws a sphere in position of unity's samples.
        /// </summary>
        [Space]
        [Tooltip("Draws a sphere on the samples position")]
        public bool DrawPoints = false;


        /// <summary>
        /// Inputs speed by the inspector.
        /// </summary>
        [Space]
        [Tooltip("speed (World Unit/Sec).")]
        [HideInInspector]
        public float Speed ;

        /// <summary>
        /// Indicates if the enemy loops forward and backward on the curve until it's destroyed,
        /// if the enemy isn't set to loop the enemy will be destroyed when it reach the end of the curve.
        /// </summary>
		[Tooltip("Looping the enemy forward and backward on the curve until it's destroyed, " +
            "or it's destroyed when it reaches the end of the curve")]
        [HideInInspector]
        public bool Loop ;

        /// <summary>
        /// Rotates the enemy to follow the curve tangent.
        /// </summary>
        [Tooltip("Makes the rotation of the enemy follows the curve")]
        [HideInInspector]
        public bool FollowPath;

		/// <summary>
        /// The manager that contain the samples and 
        /// is responsible for sampling positions based on speed.
        /// </summary>
        public Curve2DSamplerManger SamplerManger
        {
            get;
            set;
        }

        /// <summary>
        /// Current speed for the mover in (World Unit/Sec).
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
        /// Tracks if the Enemy is going forward or backward on the curve.
        /// </summary>
        private bool _isForward = true ;

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


        void Start ()
        {
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }
                
            rotationManager.SetFaceAngleToEnemy();
            rotationManager.Subscribe(this, RotationAxis.Z);
        }

        /// <summary>
        /// UpdateDirection is called on every frame to update the mover direction.
        /// </summary>
        /// <returns>the current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
            //If the Mover doesn't have a reference for the Wave it shouldn't act.
            if (SamplerManger == null)
            {
                return Vector2.zero;
            }               

            //Increase the distance if the Object is moving forward, decrease it if it's going backward.
            if (_isForward)
            {
                //Update the Object position on the curve
                SamplerManger.SampleForward(Speed*Time.deltaTime);
            }
            else
            {
                //Update the Object position on the curve
                SamplerManger.SampleBackward(Speed * Time.deltaTime);
            }

            return SamplerManger.Tangent;
        }

        /// <summary>
        /// One of Unity's messages, called after all Update functions have been called.
        /// </summary>
        private void LateUpdate ()
        {
            if (SamplerManger == null)
            {
                return;
            }
                
            //Check if it passed the end of the curve
            if (SamplerManger.Distance >= SamplerManger.CurveLength)
            {
                //Destroy the Object if loop isn't checked.
                if (!Loop)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _isForward = false;
                }                    
            }

            //Check if it passed the start of the curve
            if (SamplerManger.Distance <= 0)
            {
                _isForward = true;
            }                
        }

        /// <summary>
        /// Edit the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of the rotation in degrees</param>
        /// <returns>the change on the rotation angle.</returns>
        public float EditRotation(float angle)
        {
            if (!FollowPath)
            {
                return angle;
            }
                
            return Math2D.VectorToDegree(Direction);
        }

#if UNITY_EDITOR

        void OnDrawGizmos ()
        {
		    if ( SamplerManger == null)
            {
                return;
            }                

            if ( DrawLine)
            {
                SamplerManger.DrawSampleCurve(Color.yellow, transform.position.z);
            }                

            if (DrawPoints)
            {
                SamplerManger.DrawSamplePoints(Color.red, 0.125f, transform.position.z);
            }               
        }

#endif

    }

}
