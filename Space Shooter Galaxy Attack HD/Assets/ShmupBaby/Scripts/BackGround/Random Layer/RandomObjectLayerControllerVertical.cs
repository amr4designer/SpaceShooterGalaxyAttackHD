using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the Random Objects Layer in vertical view.
    /// </summary>
    [AddComponentMenu("")]
    public class RandomObjectLayerControllerVertical : RandomObjectLayerController
	{

	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        protected override void Start () {

            base.Start();

            mover.ChangeDirection (Vector3.down);

        }

	    /// <summary>
	    /// Positions the Next Object.
	    /// </summary>
	    /// <param name="target">Next Object transform</param>
		protected override void Position ( Transform Target ) {

		    if (Settings.RandomPosition)
		        Target.position = new Vector3((Random.value - 0.5f) * GameField.width, GameField.height + Settings.Offset, transform.position.z);
		    else
		        Target.position = new Vector3((Settings.Position - 0.5f) * GameField.width, GameField.height + Settings.Offset, transform.position.z);

        }


	}

}