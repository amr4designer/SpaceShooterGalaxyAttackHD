using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// follow the position of another transform.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Utilities/Point Constrain")]
    public class PointConstrain : MonoBehaviour
    {
        /// <summary>
        /// the target that will be followed.
        /// </summary>
        [Tooltip("target for this gameObject to follow.")]
        public Transform Target;
        /// <summary>
        /// indicates if the target should be followed without offset.
        /// </summary>
        [Tooltip("snap this gameObject to the target position on start")]
        public bool SnapToTargetPosition;

        /// <summary>
        /// displacement between the target and this gameObject.
        /// </summary>
        private Vector3 _displacementFromTarget;

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
            if (SnapToTargetPosition)
                _displacementFromTarget = Vector3.zero;
            else
                _displacementFromTarget = transform.position - Target.position;
        }

        /// <summary>
        /// One of Unit'sy messages that gets called on every frame.
        /// </summary>
        void Update()
        {
            if (Target != null)
                transform.position = _displacementFromTarget + Target.position;
        }

    }

}
