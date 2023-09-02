using System.Collections;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// The available Options for the AnimatedFX gameObject
    /// which will take place after the animation plays once.
    /// </summary>
    public enum FXAfterPlayOption
    {
        Destroy,
        Disable,
        None
    }

    /// <summary>
    /// play the animator default clip once.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Shmup Baby/Utilities/Animated FX")]
    public class AnimatedFX : MonoBehaviour
    {
        /// <summary>
        /// action to take place after the animator default clip play
        /// </summary>
        [Space]
        [Tooltip("Action to take place after the animator default clip is played")]
        public FXAfterPlayOption AfterPlayAction;
        /// <summary>
        /// time from when this Object was enabled to enable the animator Component.
        /// </summary>
        [Tooltip("Delay in Seconds to enable the Animator.")]
        public float EnableDelay;

        /// <summary>
        /// The animator component attached to this gameObject.
        /// </summary>
        private Animator _myAnimator;
        /// <summary>
        /// the length of the default clip in Seconds.
        /// </summary>
        private float _clipLength;

        /// <summary>
        /// one of Unity's messages which act the same way as start but gets called before start.
        /// </summary>
        protected virtual void Awake()
        {

            if (_myAnimator == null)
                _myAnimator = gameObject.GetComponent<Animator>();

            //disables the animator.
            _myAnimator.enabled = false;

            _clipLength = GetPlayTime();
            
        }

        /// <summary>
        /// the OnEnable method is one of Unity message's that gets called when this object is enabled.
        /// </summary>
        protected void OnEnable()
        {
            //enables the animator after a delay 
            StartCoroutine(PlayClip(EnableDelay));

        }
        
        /// <summary>
        /// plays the clip and takes action after the clip was played once.
        /// </summary>
        /// <param name="delay">delay in seconds before the clip plays.</param>
        protected virtual IEnumerator PlayClip(float delay)
        {
            yield return new WaitForSeconds(delay);

            //enables the Animator to start playing
            _myAnimator.enabled = true;

            if (AfterPlayAction == FXAfterPlayOption.Destroy)
                DestroyAfterPlay();

            if (AfterPlayAction == FXAfterPlayOption.Disable)
                StartCoroutine(DisableAfterPlay());

        }

        /// <summary>
        /// the OnDisable method is one of Unity's messages that gets called when this object is disabled.
        /// </summary>
        protected virtual void OnDisable()
        {

            //disable the Animator to stop it from playing
            _myAnimator.enabled = false;
            
        }

        /// <summary>
        /// destroys the gameObject after the _clipLength.
        /// </summary>
        public void DestroyAfterPlay()
        {

            Destroy(gameObject, _clipLength);

        }

        /// <summary>
        /// disables the gameObject after the _clipLength.
        /// </summary>
        IEnumerator DisableAfterPlay()
        {

            yield return new WaitForSeconds(_clipLength);

            gameObject.SetActive(false);

            yield return null;

        }

        /// <summary>
        /// returns the play time in seconds of the default clip in _myAnimator.
        /// </summary>
        float GetPlayTime()
        {

            return _myAnimator.GetCurrentAnimatorStateInfo(0).length;

        }

    }

}