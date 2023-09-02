using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the Advance Object Layers in the vertical view.
    /// </summary>
    [AddComponentMenu("")]
    public class AdvanceObjectLayerControllerVertical : AdvanceObjectLayerController
	{
	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
		protected override void Start () {

			base.Start();

			mover.ChangeDirection (Vector3.down);
            
		}

	    /// <summary>
	    /// Position the NextObject to its location.
	    /// </summary>
	    /// <param name="target">NextObject transform</param>
		protected override void Position ( Transform target ) {

			target.position = new Vector3 (GameField.width * (NextObject.Position - 0.5f), GameField.height + Settings.Offset, transform.position.z);

		}


	}

}