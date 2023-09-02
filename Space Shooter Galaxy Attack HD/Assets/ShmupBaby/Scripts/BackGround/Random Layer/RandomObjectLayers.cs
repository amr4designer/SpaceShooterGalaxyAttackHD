using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Background Controller component, extendeds the background
    /// controller by adding Advance Object layers. 
    /// </summary>
	[RequireComponent(typeof(BackgroundController))]
    [AddComponentMenu("Shmup Baby/Background/Random Object Layers")]
    public class RandomObjectLayers : MonoBehaviour 
	{
	    /// <summary>
	    /// Layers that will be added to the background controller to be created.
	    /// </summary>
		[Header("List of Random Object Layer")]
		[Space]
		public RandomObjectLayerData[] Layers ;

	    /// <summary>
	    /// One of Unity's messages that act the same way as start but gets called before start.
	    /// </summary>
		void Awake ()
		{

		    LevelInitializer.Instance.SubscribeToStage(3, SubmitLayers);

        }

	    /// <summary>
	    /// Submits the layers and their index to the background controller.
	    /// </summary>
	    private void SubmitLayers ( ) {

			for (int i = 0; i < Layers.Length; i++) {
				BackgroundController.Instance.SubmitLayerIndex (Layers [i].Index);
			}

			BackgroundController.Instance.SubmitLayers (Layers);

		}
	}

}