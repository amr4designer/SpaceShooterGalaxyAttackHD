using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the Advance Object Layers in the horizontal view.
    /// </summary>
    [AddComponentMenu("")]
	public class AdvanceObjectLayerControllerHorizontal : AdvanceObjectLayerController
	{

	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
	    protected override void Start()
	    {

	        base.Start();

	        mover.ChangeDirection(Vector3.left);

	    }

	    /// <summary>
	    /// Position the NextObject to its location.
	    /// </summary>
	    /// <param name="target">NextObject transform</param>
        protected override void Position ( Transform target ) {

			target.position = new Vector3 (GameField.width + Settings.Offset , GameField.height* (NextObject.Position - 0.5f) , transform.position.z);

		}


	}

}