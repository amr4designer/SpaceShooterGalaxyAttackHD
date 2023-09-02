using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Enemy Mover that moves the enemy depending on the level view 
	/// and add a simple tilt (A bend) to the move.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/Tilt Mover")]
    public class TiltMover : Mover , IRotate
	{
        /// <summary>
        /// Input speed by the inspector.
        /// </summary>
        [Space]
		[Tooltip("Speed in (World Unit/Seconds).")]
		public float Speed ;

        /// <summary>
        /// The rate at which the direction will change towards the X axis.
        /// </summary>
        [Space]
		[Tooltip("How much is the curve bending")]
		public float CurveCoefficient = 1 ;
        /// <summary>
        /// Indicate that the random coefficient will be used.
        /// </summary>
        [Space]
        [Tooltip("Check to use the max and min coefficients instead of the coefficient input above")]
		public bool RandomCurveCoefficient = false;
        /// <summary>
        /// The minimum coefficient in case the random coefficient is set to true.
        /// </summary>
        [Tooltip("The minimum value for the curve coefficient if the Random check box is ticked")]
        public float MiniCurveCoefficient = 1;
        /// <summary>
        /// The maximum coefficient in case the random coefficient is set to true.
        /// </summary>
        [Tooltip("The maximum value for the curve coefficient if the Random check box is ticked")]
        public float MaxCurveCoefficient = 1 ;
        

        /// <summary>
        /// If checked, this mover will rotate to follow the moving direction.
        /// </summary>
        [Space]
		[Tooltip("Make the Object rotation locked to the curve tangent")]
		public bool FollowCurve = true;


        [Header("Gizmo Settings")]
		[Space]
		[Tooltip("Draws the path that the Mover will take.")]
		public bool DrawPath;

		[Tooltip("The length of the drawing")]
		public float PathLength = 10 ;


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
        /// the view type of the level.
        /// </summary>
        private LevelViewType View
	    {
	        get { return LevelController.Instance.View; }
	    }
              
        /// <summary>
        /// Tracks the tilt amount.
        /// </summary>
        private float _tilit;


        private void Start ()
		{            
		    RotationManager rotationManager = GetComponent<RotationManager>();

		    if (rotationManager == null)
		        rotationManager = gameObject.AddComponent<RotationManager>();

            rotationManager.SetFaceAngleToEnemy();

            if (FollowCurve)
                rotationManager.Subscribe(this, RotationAxis.Z);
            
            if (RandomCurveCoefficient)
		        CurveCoefficient = Random.Range(MiniCurveCoefficient, MaxCurveCoefficient);               
        }

        /// <summary>
        /// UpdateDirection is called on every frame to update the mover direction.
        /// </summary>
        /// <returns>the current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
            //Increases the follow angle by CurveCoefficient over time.
            _tilit += CurveCoefficient * Time.deltaTime;
                        
            if (View == LevelViewType.Vertical)
                //Sets the direction to be between speed and follow angle in vertical view.
                return new Vector2(-_tilit, -speed).normalized;

            if (View == LevelViewType.Horizontal)
                //Sets the direction to be between speed and follow angle in horizontal view.
                return new Vector2(-speed, _tilit).normalized;

            return Vector2.zero;                       
        }

        /// <summary>
        /// edits the rotation angle provided by this rotator.
        /// </summary>
        /// <param name="angle">The current angle of rotation in degrees.</param>
        /// <returns>The change in rotation angle.</returns>
        public float EditRotation(float angle)
        {            
            if (!FollowCurve)
            {
                return angle;
            }
                                       
            return Math2D.VectorToDegree(Direction);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
	    {
            Gizmos.color = Color.yellow;

            //How accurate the path drawing is, smaller values means more accuracy.
            const float deltaLength = 0.1f;

	        if (!DrawPath || deltaLength <= 0)
	            return;

	        //Number of lines that need to be drawn for the curve.
	        int deltaNumber = Mathf.FloorToInt(PathLength / deltaLength) - 1;
            
            //Track the tilt in each line.
	        float tilt = 0;
                        
	        Vector2 prePoint = gameObject.transform.position;
	        Vector2 postPoint = gameObject.transform.position;

            //Gets the view type for this level.
	        OrthographicCamera orthographicCamera = FindObjectOfType<OrthographicCamera>();
            if (orthographicCamera == null)
	            return;

            //Tries to simulate what the mover does in order to be able to draw the curve
            for (int i = 0; i < deltaNumber; i++)
	        {
	            Vector2 gDirection = Vector2.zero;
                
                tilt += CurveCoefficient * deltaLength;
                
                //Defines the direction depending on the level view. 
	            if (orthographicCamera.ViewType == LevelViewType.Vertical)
                    gDirection = new Vector2(-tilt, -Speed);
	            if (orthographicCamera.ViewType == LevelViewType.Horizontal)
                    gDirection = new Vector2(-Speed, tilt);

                gDirection.Normalize();

	            gDirection *= deltaLength * Speed;

	            postPoint += gDirection;

	            Gizmos.DrawLine(Math2D.Vector2ToVector3(prePoint, transform.position.z),
	                Math2D.Vector2ToVector3(postPoint, transform.position.z));

	            prePoint = postPoint;
	        }
        }

#endif

    }

}