using UnityEngine;
using UnityEngine.EventSystems;

namespace ShmupBaby
{
    /// <summary>
    /// simulate JoyStick behavior in the UI.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Input/JoyStick")]
    public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// the stick moving limit relative to it's background.
        /// </summary>
        [Range(0f, 1f)]
        [Tooltip("the stick moving limit relative to it's background.")]
        public float StickLimit = 1f;

        /// <summary>
        /// the background transform for joystick.
        /// </summary>
        [Space]
        [Tooltip("plug the background transform for this stick")]
        public RectTransform Background;
        /// <summary>
        /// the stick transform of the joystick.
        /// </summary>
        [Tooltip("plug the stick transform of the joystick.")]
        public RectTransform Stick;

        /// <summary>
        /// the input direction by the joystick.
        /// </summary>
        public Vector2 Direction { get; private set; }

        /// <summary>
        /// the input of the player from moving the stick.
        /// </summary>
        private Vector2 _inputVector = Vector2.zero;
        /// <summary>
        /// the start background position
        /// </summary>
        private Vector2 _backgroundPosition = Vector2.zero;
        /// <summary>
        /// the start background radius.
        /// </summary>
        private float _backgroundRadius;
        /// <summary>
        /// reference to the camera that will manage the joystick input.
        /// </summary>
        private Camera _cam  = new Camera();

        /// <summary>
	    /// The Start method is one of Unity's messages that get called when a new object is instantiated.
	    /// </summary>
        void Start()
        {
            //cache the background radius and position.
            _backgroundPosition = RectTransformUtility.WorldToScreenPoint(_cam, Background.position);
            _backgroundRadius = Background.sizeDelta.x * 0.5f;
        }

        /// <summary>
        /// When dragging is occurring this will be called every time the cursor is moved.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - _backgroundPosition;

            //remap the input vector to be between it's starting point and it's limit.
            _inputVector = (direction.magnitude > _backgroundRadius) ? direction.normalized : direction / _backgroundRadius;
            //move the stick to match the input
            Stick.anchoredPosition = _inputVector * _backgroundRadius * StickLimit;

            Direction = _inputVector;
        }

        /// <summary>
        /// when pointer is down this will be called.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        /// <summary>
        /// when pointer is up this will be called.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            //reset the stick position when the pointer is up
            Stick.anchoredPosition = Vector2.zero;
        }

    }

}
