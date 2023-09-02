using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Destroy the gameObjects leaving the collider area.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Shmup Baby/Utilities/Destroy By Region")]
    public class DestroyByRegion : Singleton<DestroyByRegion>
    {
        /// <summary>
        /// the available region Options for DestroyByRegion.
        /// </summary>
        public enum RegionOption
        {
            GameField,
            Input
        }

        /// <summary>
        /// the region Option to be used as the safe region.
        /// </summary>
        [Space]
        [Tooltip("What is the region to use as a safe region.")]
        public RegionOption RegionToUse;

        /// <summary>
        /// the safe region if RegionToUse is set to Input.
        /// </summary>
        [Space]
        [Tooltip("This region will be used if RegionToUse is set to Input.")]
        public Rect InputRegion;
        /// <summary>
        /// offset for the safe region from the selected region.
        /// </summary>
        [Space]
        [Tooltip("Offsets for the safe region from the selected region.")]
        public float Offset = 2;
        /// <summary>
        /// The layer that will be affected by the DestroyByRegion.
        /// </summary>
        [Space]
        [Tooltip("The layer that will be affected")]
        public LayerMask TargetLayers = -1;

        #if UNITY_EDITOR

        /// <summary>
        /// Available only for Editor. 
        /// </summary>
        [Space]
        [Tooltip("Check to draw a gizmo for the region")]
        public bool DrawRegion = true;

        #endif

        /// <summary>
        /// the BoxCollider2D component attached to this gameObject.
        /// </summary>
        private BoxCollider2D _collider;
        /// <summary>
        /// the Rigidbody2D component attached to this gameObject.
        /// </summary>
        private Rigidbody2D _rigidbody2D;

        /// <summary>
        /// One of Unity's messages which act the same way as start but gets called before start.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            LevelInitializer.Instance.SubscribeToStage(4, InitializeRegion);

        }

        /// <summary>
        /// Initializes the destroy by region.
        /// </summary>
        void InitializeRegion()
        {
            //resets transformation
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            //adjusts the Rigidbody2D component.
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.useFullKinematicContacts = true;

            //adjusst the BoxCollider2D component.
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = true;

            //adjusts the collider to the gameField.
            if (RegionToUse == RegionOption.GameField)
            {
                _collider.size = LevelController.Instance.GameField.size + Vector2.one * Offset * 2;
                _collider.offset = LevelController.Instance.GameField.center;
            }

            //adjusts the collider to the InputRegion.
            if (RegionToUse == RegionOption.Input)
            {
                _collider.size = InputRegion.size + Vector2.one * Offset * 2;
                _collider.offset = InputRegion.center;
            }
        }

        /// <summary>
        /// One of Unity's messages sent when another object leaves a trigger collider attached to this object (2D physics only).
        /// </summary>
        /// <param name="exitCollider2D">The other Collider2D involved in this collision.</param>
        void OnTriggerExit2D(Collider2D exitCollider2D)
        {
            //check if the Object layer is effected by this component.
            int objectLayer = 1;
            objectLayer <<= exitCollider2D.gameObject.layer;
            int isTargeted = objectLayer & TargetLayers.value; 

			if (isTargeted > 1) {

                //if the Object is enemy we need to destroy the weapons safely.
				Enemy enemy = exitCollider2D.gameObject.GetComponent<Enemy> ();

				if (enemy != null)
					enemy.DestroyWeapon ();

				Destroy (exitCollider2D.gameObject);

			}
        }

        #if UNITY_EDITOR

        void OnDrawGizmos()
        {

            if ( !DrawRegion )
                return;

            Rect regionRect = new Rect();

            Gizmos.color = Color.red;

            if (RegionToUse == RegionOption.GameField)
            {

                BackgroundController BgController = FindObjectOfType<BackgroundController>();

                if (BgController == null)
                    return;

                regionRect = BgController.BackgroundForGizmo;

                regionRect.size = regionRect.size + Vector2.one * Offset * 2;

                regionRect.center = BgController.BackgroundForGizmo.center;

            }

            if (RegionToUse == RegionOption.Input)
            {
                regionRect = InputRegion;

                regionRect.size = regionRect.size + Vector2.one * Offset * 2;

                regionRect.center = InputRegion.center;
            }
            
            GizmosExtension.DrawRect(regionRect);

        }

        #endif

    }

}
