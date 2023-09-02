using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Mine that can bounce to the player when it gets hit by the same type of mines.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Enemy Chain Mine")]
    public sealed class EnemyChainMine : EnemyMine
    {
        /// <summary>
        /// The probability of the mine to behave as an active mine or as inactive mine,
        /// the active mine will impulse itself to the target when it detects it,
        /// the inactive mine will only impulse itself when it's hit by a mine of the same type.
        /// </summary>
        [Header("Chain Mine Settings")]
        [Space]
        [Range(0, 100)]
        [Tooltip("If the mine is active it will be able to impulse to the player, when the player is detected," +
                 "if its not the mine will only Impulse to the player when it hits a mine of the same type.")]
        public int ActiveChance = 0;

        /// <summary>
        /// The radius around the mine,which if the target enters it the mine will detect it.
        /// </summary>
        [Space]
        [Tooltip("The radius around the mine,which if the target enters it, the mine will detect it.")]
        public float DetectRadius;

        /// <summary>
        /// The force that will be used when the mine impulses itself toward the target in a normal way.
        /// </summary>
        [Space]
        [Tooltip("The force that will be used when the mine impulses itself toward the target in a normal way.")]
        public float NormalForce;
        /// <summary>
        /// The force that will be used when the mine impulse itself toward the target ,when the mine gets hit by another mine of the same type.
        /// </summary>
        [Tooltip("The force that will be used when the mine impulses itself toward the target, when the mine gets hit by another mine of the same type")]
        public float ChainForce;

        /// <summary>
        /// Rotation speed for the mine, doesn't have to do with the mine behavior only with its look.
        /// </summary>
        [Space]
        [Tooltip("Rotation speed for the mine, doesn't have to do with the mine behavior only with its look.")]
        public float RotationSpeed;
        /// <summary>
        /// Mine speed moving towards the end of the level when the mine cannott detect the target.
        /// </summary>
        [Tooltip("Mine speed moving towards the end of the level when the mine cannot detect the player.")]
        public float NormalSpeed;
        /// <summary>
		/// The minimum mine speed for the mine to be impulsed to (as in pushed towards) the player.
        /// </summary>
		[Tooltip("The minimum mine speed for the mine to be impulsed to (as in pushed towards) the player.")]
        public float SpeedThreshold;
        
        /// <summary>
        /// level view type.
        /// </summary>
        private LevelViewType View
        {
            get { return LevelController.Instance.View; }
        }

        //The rigid body of the mine.
        private Rigidbody2D _myRb;
        //Indicates if this is an active mine or an inactive mine.
        private bool _active ;

        /// <summary>
        /// One of Unity's messages, it acts the same way as start but gets called before start.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            //Sets the mine to be active or inactive depending on ActiveChance.
            if (ActiveChance >= Random.Range(0, 100))
                _active = true;

            //Gets the rigid body and sets it to dynamic and the gravity to zero.
            _myRb = GetComponent<Rigidbody2D>();

            _myRb.bodyType = RigidbodyType2D.Dynamic;
            _myRb.gravityScale = 0;

            //Finding the mine normal direction depending on the level view type.
            if (View == LevelViewType.Vertical)
                _myRb.velocity = Vector3.down * NormalSpeed;
            if (View == LevelViewType.Horizontal)
                _myRb.velocity = Vector3.left * NormalSpeed;

            //Sets the mine rotation speed.
            _myRb.angularVelocity = RotationSpeed * 10;
            //Adds the normal speed to the Threshold so it doesn't effect the calculation.
            SpeedThreshold += NormalSpeed;

        }

        /// <summary>
        /// One of Unity's messages, called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
			// -The mine velocity should be less than the SpeedThreshold for the mine to impulse (push itself).
            // -The mine should be active.
            // -The mine should be inside the detection area.
            if ( _active && tracker.Distance <= DetectRadius && _myRb.velocity.magnitude <= SpeedThreshold)
            {
                _myRb.AddForce(tracker.Direction * NormalForce, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's collider (2D physics only).
        /// </summary>
        protected override void OnCollisionEnter2D(Collision2D hitObject)
        {
            base.OnCollisionEnter2D(hitObject);

            //Check to see if this mine is by hit another mine.
            if (hitObject.gameObject.tag != "enemy mine" || tracker.TrackingState != TrackState.TargetDetected)
                return;

            //Sends the other mine to the target with the chain force value.
            Rigidbody2D otherMine = hitObject.gameObject.GetComponent<Rigidbody2D>();
            otherMine.AddForce(tracker.Direction * ChainForce, ForceMode2D.Impulse);
            otherMine.AddTorque(ChainForce);

        }

        #if UNITY_EDITOR

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!DrawGizmos)
                return;

            //Draw a circle gizmo that represents the DetectRadius.
            Gizmos.color = Color.yellow;
            GizmosExtension.DrawCircle(transform.position, DetectRadius);
        }

        #endif

    }
}
