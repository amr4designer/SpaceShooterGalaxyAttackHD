using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Controls the mover speed over time.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Utilities/Mover Speed Control")]
    [RequireComponent(typeof(Mover))]
    public class MoverSpeedControl : MonoBehaviour
    {
        /// <summary>
        /// The curve that controls the mover speed over time.
        /// </summary>
        [Tooltip("The curve that multiplies the mover's speed over time.")]
        public AnimationCurve Curve;

        /// <summary>
        /// The mover to be controlled.
        /// </summary>
        private Mover _mover;
        /// <summary>
        /// The time when this script is awake.
        /// </summary>
        private float _awakeTime;
        /// <summary>
        /// The mover starting speed.
        /// </summary>
        private float _speed;


        private void Start()
        {
            _awakeTime = Time.time;
            _mover = GetComponent<Mover>();
            _speed = _mover.speed;
        }


        private void Update()
        {
            float t = Time.time - _awakeTime;

            _mover.speed = Curve.Evaluate(t) * _speed;
        }

    }

}