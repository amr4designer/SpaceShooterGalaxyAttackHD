using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShmupBaby
{
    /// <summary>
    /// simulate hold button behavior in the UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Shmup Baby/Input/Hold Button")]
    public class HoldButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// sprite that represent the button idle mode.
        /// </summary>
        [Tooltip("plug sprite that represent the button idle mode")]
        public Sprite Idle;
        /// <summary>
        /// sprite that represent the button hold mode.
        /// </summary>
        [Tooltip("plug sprite that represent the button hold mode")]
        public Sprite Hold;

        /// <summary>
        /// indicate if the button is on hold.
        /// </summary>
        public bool Holding { get; private set; }

        /// <summary>
        /// the image component for this button.
        /// </summary>
        private Image _image;

        /// <summary>
	    /// The Start method is one of Unity's messages that get called when a new object is instantiated.
	    /// </summary>
        void Start()
        {
            _image = GetComponent<Image>();
            _image.sprite = Idle;
        }

        /// <summary>
        /// when pointer is down this will be called.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            Holding = true;
            _image.sprite = Hold;
        }

        /// <summary>
        /// when pointer is up this will be called.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Holding = false;
            _image.sprite = Idle;
        }
    }

}
