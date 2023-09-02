using System.Collections;
using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// plays the animator default clip once,
    /// with control over renderer.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(Animator))]
    [AddComponentMenu("Shmup Baby/Utilities/Animated Sprite FX")]
    public sealed class AnimatedSpriteFX : AnimatedFX {

        /// <summary>
        /// the SpriteRenderer component attached to this gameObject.
        /// </summary>
        private Renderer _myRenderer;

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        protected override void Awake () {

			if (_myRenderer == null)
				_myRenderer = gameObject.GetComponent<Renderer> ();

            _myRenderer.enabled = false;

            base.Awake();

		}

        /// <summary>
        /// play the clip and takes action after the clip is played once.
        /// </summary>
        /// <param name="delay">delay in seconds before the clip is played.</param>
        protected override IEnumerator PlayClip(float delay)
	    {
	        yield return base.PlayClip(delay);

            //enables the SpriteRenderer 
	        _myRenderer.enabled = true;

        }

        /// <summary>
        /// the OnDisable method is one of Unity's messages that gets called when this object is disabled.
        /// </summary>
	    protected override void OnDisable () {

	        base.OnDisable();

            //disable the SpriteRenderer to hide the first sprite from showing
            _myRenderer.enabled = false;

		}
        
	}

}