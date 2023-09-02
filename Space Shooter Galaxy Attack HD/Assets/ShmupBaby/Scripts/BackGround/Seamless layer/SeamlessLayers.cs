using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Background Controller component, extended the background
    /// controller by adding seamless layers. 
    /// </summary>
	[RequireComponent(typeof(BackgroundController))]
    [AddComponentMenu("Shmup Baby/Background/Seamless Layers")]
    public class SeamlessLayers : MonoBehaviour 
	{
        /// <summary>
        /// layers that will be added to the background controller to be created.
        /// </summary>
		[Header("List of Seamless Layer")]
		[Space]
		public SeamlessLayerData[] Layers ;

	    /// <summary>
	    /// one of unity messages act the same way as start but it get called before start.
	    /// </summary>
        private void Awake ()
		{

		    LevelInitializer.Instance.SubscribeToStage(3, SubmitLayers);

        }

        /// <summary>
        /// Submits the layers and their index to the background controller.
        /// </summary>
	    private void SubmitLayers ( ) {

			for (var i = 0; i < Layers.Length; i++)
			    BackgroundController.Instance.SubmitLayerIndex (Layers [i].Index);

	        BackgroundController.Instance.SubmitLayers (Layers);

		}
	}

}