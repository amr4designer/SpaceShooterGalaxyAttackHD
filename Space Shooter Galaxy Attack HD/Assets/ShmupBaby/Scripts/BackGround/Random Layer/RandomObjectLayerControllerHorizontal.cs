using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the Random Object Layer in horizontal view.
    /// </summary>
    [AddComponentMenu("")]
    public class RandomObjectLayerControllerHorizontal : RandomObjectLayerController
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
	    /// Positions the NextObject.
	    /// </summary>
	    /// <param name="target">Next Object transform</param>
        protected override void Position ( Transform Target ) {

			if ( Settings.RandomPosition )
				Target.position = new Vector3 (GameField.width+Settings.Offset, (Random.value-0.5f)*GameField.height, transform.position.z);
			else
				Target.position = new Vector3 (GameField.width + Settings.Offset, (Settings.Position-0.5f)* GameField.height, transform.position.z);

		}


	}

}