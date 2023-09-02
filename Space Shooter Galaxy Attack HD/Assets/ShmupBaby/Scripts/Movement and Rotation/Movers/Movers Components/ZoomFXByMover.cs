using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Controls the camera zooming using a mover object state.
    /// </summary>
	[RequireComponent(typeof(Mover))]
    [AddComponentMenu("Shmup Baby/Agent/Component/Zoom FX By Mover")]
    public sealed class ZoomFXByMover : MonoBehaviour
	{
        /// <summary>
        /// The mover attached on this gameObject.
        /// </summary>
		private Mover _mover;


		private void Start ()
		{
			_mover = GetComponent<Mover> ();
		}


	    private void Update ()
        {
			if (_mover.MoveState == MoverState.Moving)
            {
                ZoomOut();
            }
            else
            {
                ZoomIn();
            }                
		}

        /// <summary>
        /// Sets the camera on for zooming in.
        /// </summary>
	    private void ZoomIn ()
        {
			ZoomFX.Instance.Zooming = false;
		}

	    /// <summary>
	    /// Sets the camera on for zooming out.
	    /// </summary>
	    private void ZoomOut ()
        {
			ZoomFX.Instance.Zooming = true;
		}

	}

}