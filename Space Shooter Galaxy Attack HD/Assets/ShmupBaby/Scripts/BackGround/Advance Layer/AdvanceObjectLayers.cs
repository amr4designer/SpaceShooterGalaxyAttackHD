using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Background Controller component, extends the background
    /// controller by adding Advance Object layers. 
    /// </summary>
	[RequireComponent(typeof(BackgroundController))]
	[AddComponentMenu("Shmup Baby/Background/Advance Object Layers")]
	public class AdvanceObjectLayers : MonoBehaviour 
	{
	    /// <summary>
	    /// Layers that will be added to the background controller to be created.
	    /// </summary>
		[Header("List of Advance Object Layer")]
		[Space]
		public AdvanceObjectLayerData[] Layers ;

	    /// <summary>
	    /// One of Unity's messages that gets called before start.
	    /// </summary>
	    private void Awake()
	    {

	        LevelInitializer.Instance.SubscribeToStage(3, SubmitLayers);

        }

	    /// <summary>
	    /// Submits the layers and their index to the background controller.
	    /// </summary>
	    private void SubmitLayers () {

			for (int i = 0; i < Layers.Length; i++) {
				BackgroundController.Instance.SubmitLayerIndex (Layers [i].Index);
			}

			BackgroundController.Instance.SubmitLayers (Layers);

		}
	}

}