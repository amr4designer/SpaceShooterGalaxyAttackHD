using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{
    
    /// <summary>
	/// The structure of the main layer data, this will also handle
    /// the creation of the layer based on the data.
    /// </summary>
    [System.Serializable]
    public class MainLayerData : LayerCreator
    {

        /// <summary>
		/// backgroundObject for the MainLayer, contain the reference to the
        /// background prefab with additional options for it.
        /// </summary>
        [System.Serializable]
        public class BackgroundObject
        {
			/// <summary>
			/// Reference for the background Game Object
            /// </summary>
            [Space]
            [Tooltip("the prefab for the background GameObject")]
            public Object Background;

            /// <summary>
			/// Indicate how many times the background will be repeated before
            /// its turn ends.
            /// </summary>
            [Space]
            [Tooltip("how many time should the background be repeated before" +
                     "moving to the next one")]
            public int Repeat ;

            public BackgroundObject(int repeat)
            {
                Repeat = repeat;
            }

            /// <summary>
			/// Examining if the background matches this package background standards.
            /// </summary>
            public bool IsValid { get; set; }

        }

        /// <summary>
		/// Background Objects that will be looping in order.
        /// </summary>
        [Tooltip("list of Backgrounds object to be looping in order")]
        public BackgroundObject[] Backgrounds;

        /// <summary>
		/// The speed of the scrolling background.
        /// </summary>
        [Space]
        [Tooltip("main background speed")]
        public float Speed = 1;

        /// <summary>
		/// Indicates if the prefab scale should be used or the Scale field of the background.
        /// </summary>
        [Space]
        [Tooltip("override the prefab scale for the backgrounds prefab with the scale")]
        public bool OverrideScale = false;
        /// <summary>
		/// The scale that will be used when OverrideScale is set to true.
        /// </summary>
        [Tooltip("the scale that will be  used when OverrideScale is checked")]
        public Vector3 Scale = Vector3.one;

        /// <summary>
		/// The option to override the background's rotation.
        /// </summary>
        [Space]
        [Tooltip("options to override the background rotations")]
        public OrthographicRotate OverrideRotation = OrthographicRotate.None;

        /// <summary>
		/// This value indicates how many of the backgrounds will be
		/// loaded in the scene at the same time for this layer.
        /// </summary>
        [Space]
        [Range(1, 10)]
        [Tooltip("this value indicate how many of the backgrounds will be loaded in the scene" +
                 "increase this value only if it's necessary (if you see the sudden appear of the background during the game play)")]
        public int BackgroundRepetition;

        /// <summary>
		/// The dimensions of the first background.
        /// </summary>
        [HideInInspector]
        public Vector2 Dimension;

        public MainLayerData(BackgroundObject[] backgrounds, float speed, Vector3 scale, int backgroundRepetition)
        {
            Backgrounds = backgrounds;
            Speed = speed;
            Scale = scale;
            BackgroundRepetition = backgroundRepetition;
        }

        /// <summary>
		/// Indicates if this layer is Initialized,
		/// the dimension is ready to be used. 
        /// </summary>
        public bool IsInitialize { get; protected set; }

        /// <summary>
		/// Creates the layer and overrides the LayerIndex.
        /// </summary>
        /// <param name="index">The index of the layer.</param>
        /// <param name="parent">The layer parent</param>
        /// <param name="viewType">The view type for the layer</param>
        /// <returns>Handle to control the layer</returns>
        public override ILayerController CreateLayer(int index, Transform parent, LevelViewType viewType)
        {

            return CreateLayer<SeamlessLayerControllerVertical, SeamlessLayerControllerHorizontal, RepeaterMover>("Main Layer", index, parent, viewType);

        }

        /// <summary>
		/// Checks if the backgrounds is valid,
        /// and gets the first background dimensions.
        /// </summary>
        public void Initialize()
        {

            if (Backgrounds == null || Backgrounds.Length == 0)
                return;

            //run check for the backgrounds if they have a sprite renderer
            ValidateBackgrounds<SpriteRenderer>();

            //get the first background with sprite renderer
            GameObject firstValidObject = null;
            Sprite firstSprite = null;

            for (int i = 0; i < Backgrounds.Length; i++)
            {
                if (!Backgrounds[i].IsValid)
                    continue;

                firstValidObject = Backgrounds[i].Background as GameObject;

                firstSprite = firstValidObject.GetComponent<SpriteRenderer>().sprite;

            }

            if (firstSprite == null)
                return;

            //get the scale for the first background
            Vector2 scale = GetScale(firstValidObject);

            //get the dimension for the background taking in count the Scale and the rotation.
            Dimension = GetDimension(firstSprite, scale, OverrideRotation);

            IsInitialize = true;

        }

        /// <summary>
		/// Returns the scale for the background taking into account the scale override.
        /// </summary>
        /// <param name="backGround">The background GameObject</param>
        protected Vector2 GetScale(GameObject backGround)
        {
            return OverrideScale ? (Vector2)Scale : new Vector2(backGround.transform.localScale.x, backGround.transform.localScale.y);
        }

        /// <summary>
		/// Returns the dimensions for the background taking into account the scale and the rotation.
        /// </summary>
        /// <param name="sprite">The background sprite</param>
        /// <param name="scale">The scale for the Object that hold the sprite</param>
        /// <param name="rotation">The rotation for the Object the hold the sprite</param>
        protected Vector2 GetDimension(Sprite sprite, Vector2 scale, OrthographicRotate rotation)
        {

            if (rotation == OrthographicRotate.None || rotation == OrthographicRotate.Rotate180)
                return new Vector2(sprite.bounds.size.x * scale.x,
                    sprite.bounds.size.y * scale.y);
            else
                return new Vector2(sprite.bounds.size.y * scale.y,
                    sprite.bounds.size.x * scale.x);

        }

        /// <summary>
		/// Checks if the background has an object and isn't null,
		/// then proceeds to check if it has a component.
        /// </summary>
       protected void ValidateBackgrounds<T>()
        {
           
            for (int i = 0; i < Backgrounds.Length; i++)
            {
                //check for null
                if (Backgrounds[i].Background == null)
                {
                    Backgrounds[i].IsValid = false;
                    continue;
                }

                GameObject background = Backgrounds[i].Background as GameObject;

                //check if the prefab is for a game object
                if(background == null)
                {
                    Backgrounds[i].IsValid = false;
                    continue;
                }

                T objectWithComponent = background.GetComponent<T>();

                //check for the component
                if (objectWithComponent != null)
                    Backgrounds[i].IsValid = true;

            }

        }

    }

    /// <summary>
	/// One of the main components for this package, this is responsible for creating the background 
	/// and the background layers ,it works with the OrthographicCamera to define the game boundary.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Background/Background Controller")]
    public sealed class BackgroundController : Singleton<BackgroundController>
    {

        /// <summary>
        /// The data for the main background of the level.
        /// </summary>
        [Space]
        [Tooltip("The main background for the game, the main background defines the Background field")]
        public MainLayerData MainBackground = new MainLayerData(new MainLayerData.BackgroundObject[] { new MainLayerData.BackgroundObject(1)}, 1f , Vector3.one, 1 );

        #if UNITY_EDITOR

        /// <summary>
        /// Only available in the editor.
        /// </summary>
		[HelpBox("Please use only prefabs with the Background slot and not sprites.\n" +
			"Background pixel resolution and size determines the game field size.")]
		[Space]
        [Tooltip("draw gizmo represent the boundary for the main background")]
        public bool DrawMainBGBound = true;
        /// <summary>
        /// The rectangle that represents the main background boundary.
        /// </summary>
        [HideInInspector]
        public Rect BackgroundForGizmo = new Rect();

        #endif

        /// <summary>
		/// Represent the main background position and dimension in the scene.
        /// </summary>
        public Rect BackgroundRect { get; protected set; }
        
        /// <summary>
		/// Index for all of the background layers, enemy waves and the player.
        /// </summary>
        public List<int> _allIndex;
        /// <summary>
		/// All data for all of the layers in the scenes.
        /// </summary>
        public List<ILayerCreation> _layersData;
        /// <summary>
        /// A Dictionary for all the scenes background layers by their index.
        /// </summary>
        public Dictionary<int, ILayerController> _activeLayers;

        /// <summary>
        /// The parent transform for all background layers.
        /// </summary>
        public Transform LayersParent { get { return transform; } }
        /// <summary>
        /// The view type of the level.
        /// </summary>
        public LevelViewType ViewType { get { return LevelController.Instance.View; } }

        /// <summary>
        /// One of Unity's messages, it act the same way as start but it gets called before start.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            LevelInitializer.Instance.SubscribeToStage(1, InitializeMainBackground);
            LevelInitializer.Instance.SubscribeToStage(4, CreateLayers);

        }
        
        /// <summary>
        /// Creates all the backgrounds layers based on their date.
        /// </summary>
        public void CreateLayers()
        {
            //Creates the layers and assigns their controller inside _activeLayers by their index.

            _activeLayers = new Dictionary<int, ILayerController>
            {
                {GetMainLayerIndex(), MainBackground.CreateLayer(GetMainLayerIndex(), LayersParent, ViewType)}
            };

            if (_layersData == null)
                return;

            for (int i = 0; i < _layersData.Count; i++)
            {
                if (_layersData == null)
                    continue;

                if (_activeLayers.ContainsKey(_layersData[i].LayerIndex))
                {
                    _layersData[i].CreateLayer(LayersParent, ViewType);
                    Debug.Log("the layer index is already in used");
                }
                else
                    _activeLayers.Add(_layersData[i].LayerIndex, _layersData[i].CreateLayer(LayersParent, ViewType));

            }
        }

        /// <summary>
        /// Returns the highest index in _allIndex + 1 ,
        /// This represents the index of the main background
        /// to make sure that it will always stay as the one
        /// furthest away from the camera.
        /// </summary>
        public int GetMainLayerIndex()
        {

            if (_allIndex == null)
                return 1;

            _allIndex.Sort();

            return _allIndex[_allIndex.Count - 1] + 1;
            
        }

        /// <summary>
        /// Initialize the Main Background rectangle.
        /// </summary>
        public void InitializeMainBackground()
        {
            if (MainBackground == null)
                return;

            //Initializes the main background so that we can get its dimensions.
            MainBackground.Initialize();
            
            Rect backgroundRect = new Rect(0, 0, MainBackground.Dimension.x, MainBackground.Dimension.y);

            //Defines the background depending on the level view, that's because the view 
            //will change the position that the background will spawn from
            if (ViewType == LevelViewType.Vertical)
                backgroundRect.center = new Vector2(0, MainBackground.Dimension.y * 0.5f);

            if (ViewType == LevelViewType.Horizontal)
                backgroundRect.center = new Vector2(MainBackground.Dimension.x * 0.5f, 0);
            
            BackgroundRect = backgroundRect;
        }

        /// <summary>
        /// Submits the index for a layer.
        /// </summary>
        public void SubmitLayerIndex(int index)
        {
            if (_allIndex == null)
                _allIndex = new List<int>();
            _allIndex.Add(index);
        }

        /// <summary>
        /// Submits layers to be created.
        /// </summary>
        public void SubmitLayers(IEnumerable<ILayerCreation> Layers)
        {
            if (_layersData == null)
                _layersData = new List<ILayerCreation>();
            _layersData.AddRange(Layers);
        }

        /// <summary>
        /// Submits a layer to be created.
        /// </summary>
        public void SubmitLayer(ILayerCreation Layer)
        {
            if (_layersData == null)
                _layersData = new List<ILayerCreation>();
            if (Layer == null)
                return;
            _layersData.Add(Layer);
        }

        /// <summary>
        /// Gets the controller for the active layers in the scene.
        /// </summary>
        /// <param name="index">The index of the background layer</param>
        public ILayerController GetLayerController(int index)
        {
            return !_activeLayers.ContainsKey(index) ? null : _activeLayers[index];
        }

        #if UNITY_EDITOR

        public void OnDrawGizmos()
        {

            if (!DrawMainBGBound || MainBackground == null)
                return;

            //Initialize the main background so that we can get its dimensions.
            MainBackground.Initialize();

            Gizmos.color = Color.green;

            BackgroundForGizmo = new Rect(0, 0, MainBackground.Dimension.x, MainBackground.Dimension.y);

            //Gets the level view from the Orthographic Camera.
            OrthographicCamera orthographicCamera = FindObjectOfType<OrthographicCamera>();

            if (orthographicCamera == null)
                return;

            //sets the rectangle position to the View.
            if (orthographicCamera.ViewType == LevelViewType.Vertical)
                BackgroundForGizmo.center = new Vector2(0, MainBackground.Dimension.y * 0.5f);

            if (orthographicCamera.ViewType == LevelViewType.Horizontal)
                BackgroundForGizmo.center = new Vector2(MainBackground.Dimension.x * 0.5f, 0);

            //Draw the rectangle.
            GizmosExtension.DrawRect(BackgroundForGizmo);

        }

        #endif

    }
}