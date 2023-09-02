using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// The types of background for this package.
    /// </summary>
    public enum LayerControllerType
    {
        Seamless,
        RandomObject,
        AdvanceObject
    }

    /// <summary>
    /// A list of rotation options with 90 degrees in-between
    /// </summary>
    public enum OrthographicRotate
    {
        None,
        Rotate90,
        Rotate270,
        Rotate180
    }

    /// <summary>
    /// Interface for the layer control handle.
    /// </summary>
    public interface ILayerController
    {
        /// <summary>
        /// The index of the layer.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// The Current speed of the layer.
        /// </summary>
        float Speed { get; }
        /// <summary>
        /// The type of layer that this controller controls.
        /// </summary>
        LayerControllerType Type { get; }

        /// <summary>
        /// Change the layer speed.
        /// </summary>
        /// <param name="value">New Speed value</param>
        void ChangeSpeed(float value);

        /// <summary>
        /// Changes the layer settings.
        /// </summary>
        /// <param name="settings">A new setting that will replace the old ones</param>
        /// <param name="layerMover">The new mover that will replace the old one</param>
        /// <param name="index">The new index for the layer</param>
        void ChangeSettings(LayerCreator settings, Mover layerMover, int index);

    }

    /// <summary>
    /// To Implement the layer creation behavior.
    /// </summary>
    public interface ILayerCreation
    {
        /// <summary>
        /// The layer index (represents the layer position in the z axis.
        /// </summary>
        int LayerIndex { get; }

        /// <summary>
        /// Creates the layer and overrides the LayerIndex
        /// </summary>
        /// <param name="index">The index of the layer.</param>
        /// <param name="parent">The layer parent</param>
        /// <param name="ViewType">the view type for the layer</param>
        /// <returns>Handle to control the layer</returns>
        ILayerController CreateLayer(int index,
            Transform parent,
            LevelViewType viewType);
        /// <summary>
        /// Creates the layer.
        /// </summary>
        /// <param name="parent">The layer parent</param>
        /// <param name="viewType">The view type for the layer</param>
        /// <returns>Handle to control the layer</returns>
        ILayerController CreateLayer(Transform parent,
            LevelViewType viewType);
    }

    /// <summary>
    /// Base class to every background layer, that handles the layer creation. 
    /// </summary>
    [System.Serializable]
    public class LayerCreator : ILayerCreation
    {
        /// <summary>
        /// The layer index (it will keep returning the value of one unless it's overriden).
        /// </summary>
        public virtual int LayerIndex { get { return 1; } }

        /// <summary>
        /// The space between the layers in the Z axis.
        /// </summary>
        protected static float SpaceBetween
        {
            get { return LevelController.Instance.SpaceBetween; }
        }

        /// <summary>
        /// Create the layers.
        /// </summary>
        /// <param name="parent">The layer parent</param>
        /// <param name="viewType">The view type for the layer</param>
        /// <returns>Handle to control the layer</returns>
        public ILayerController CreateLayer(Transform parent, LevelViewType viewType)
        {
            return CreateLayer(LayerIndex, parent, viewType);
        }
        /// <summary>
        /// Creates the layer and overrides the LayerIndex
        /// </summary>
        /// <param name="index">The index of the layer.</param>
        /// <param name="parent">The layer parent</param>
        /// <param name="viewType">The view type for the layer</param>
        /// <returns>Handle to control the layer</returns>
        public virtual ILayerController CreateLayer(int index, Transform parent, LevelViewType viewType)
        {

            return null;

        }

        /// <summary>
        /// Handles the layer creation internally.
        /// </summary>
        /// <typeparam name="V">The component that will handle the layer if the level is set to vertical view</typeparam>
        /// <typeparam name="H">The component that will handle the layer if the level is set to Horizontal view</typeparam>
        /// <typeparam name="M">The Mover component that's responsible for moving the layers</typeparam>
        /// <param name="layerName">The name of the game object that will hold the layer</param>
        /// <param name="index">The index of the layer</param>
        /// <param name="parent">The parent for the gameObject that holds the layer</param>
        /// <param name="viewType">The view that the layer should be created to.</param>
        /// <returns></returns>
        protected ILayerController CreateLayer<V, H, M>(string layerName, int index, Transform parent, LevelViewType viewType)
            where V : MonoBehaviour
            where H : MonoBehaviour
            where M : Mover
        {

            //Game object that will hold the layer.
            GameObject myLayer = CreateLayerObject(layerName, parent, index);
            //Adding the mover for the layer.
            Mover myMover = myLayer.AddComponent<M>();

            ILayerController myLayerController = null;

            //Adds the vertical or the horizontal component depending on the level view. 
            switch (viewType)
            {
                case LevelViewType.Vertical:
                    myLayerController = myLayer.AddComponent<V>() as ILayerController;
                    break;
                case LevelViewType.Horizontal:
                    myLayerController = myLayer.AddComponent<H>() as ILayerController;
                    break;
            }

            //Adds the start settings for layer controller
            if (myLayerController != null)
                myLayerController.ChangeSettings(this, myMover, index);

            return myLayerController;

        }

        /// <summary>
        /// Handles the creation for the game object that will hold the layer.
        /// </summary>
        /// <param name="name">The name of the game object</param>
        /// <param name="parent">The parent for the game object</param>
        /// <param name="index">This number represent the game object position in the z axis</param>
        private static GameObject CreateLayerObject(string name, Transform parent, int index)
        {

            GameObject myLayer = new GameObject(name);
            myLayer.transform.parent = parent;
            myLayer.transform.position = new Vector3(0, 0, index * SpaceBetween);

            return myLayer;
        }

    }
}