using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Data structure for the AdvanceObjectLayer Objects 
    /// </summary>
	[System.Serializable]
	public class AdvanceObject { 

        /// <summary>
        /// Reference to the prefab GameObject.
        /// </summary>
		[Tooltip("Prefab reference to the Layer Object")]
		public Object LayerObject ;

        /// <summary>
        /// Time to wait before spawning the next Object.
        /// </summary>
		[Space]
		[Tooltip("Wait time for the next Layer Object")]
		public float TimeForNextObject ;

        /// <summary>
        /// Position of the object depending on the level view.
        /// For example it represents the position on the top edge of the
        /// game field for the vertical view and the right edge of the game
		/// field for the horisontal view.
        /// </summary>
		[Space]
		[Tooltip("Position of the Layer Object between 0 and 1 where 0.5 is the middle of the screen")]
		[Range(0,1f)]
		public float Position = 0.5f ;
        
        /// <summary>
        /// Override for the Object Scale.
        /// </summary>
		[Tooltip("Override the default scale of the prefab Layer Object")]
		public Vector3 Scale = Vector3.one ;

        /// <summary>
        /// Override for the Object Rotation.
        /// </summary>
        [Tooltip("Override the default rotation of the prefab Layer Object")]
		public Vector3 Rotation ;

	}

    /// <summary>
    /// The structure of the  Advance Object layer data, this will also handle
    /// the creation of the layer based on the data.
    /// </summary>
	[System.Serializable]
	public class AdvanceObjectLayerData : LayerCreator  {

        /// <summary>
        /// Time before layer gets created in seconds from the start of the level. 
        /// </summary>
	    [Space]
	    [Tooltip("time in Seconds before this layer gets created")]
	    public float StartTime;

        /// <summary>
        /// The layer Objects. 
        /// </summary>
        [Space]
        [Tooltip("List of Object to be spawned")]
		public AdvanceObject[] Objects;

        /// <summary>
        /// layer Objects life time in sec.
        /// </summary>
		[Space]
		[Tooltip("Lifetime for the layer Object in Seconds")]
		public float Lifetime ;

        /// <summary>
        /// layer speed.
        /// </summary>
		[Space]
		[Tooltip("Speed of the layer in world unit per Second")]
		public float Speed ;

        /// <summary>
        /// Indicates if the layer keeps looping or not.
        /// </summary>
		[Space]
		[Tooltip("Make the layer loop for the end of the game")]
		public bool Loop;

        /// <summary>
        /// Offset for the Object spawn position from the edge of the game field.
        /// </summary>
		[Space]
		[Tooltip("to prevent Object spawning at the edge of the field")]
		public float Offset;

        /// <summary>
        /// The layer Index.
        /// </summary>
        [Space]
		[Range(1,100)]
		public int Index ;

        //Override the LayerIndex to return Index, because there is no support for the properties in the inspector. 
        public override int LayerIndex { get { return Index; } }

        /// <summary>
        /// Create an advance object layer and returns a controller for it.
        /// </summary>
        /// <param name="index">The layer index (layer position on the Z axis)</param>
        /// <param name="parent">The parent for the layer</param>
        /// <param name="viewType">The view type of this level</param>
        /// <returns></returns>
		public override ILayerController CreateLayer ( int index , Transform parent , LevelViewType viewType ) { 

			return CreateLayer<AdvanceObjectLayerControllerVertical,AdvanceObjectLayerControllerHorizontal,DirectionalMover> ("Advance Object Layer", index, parent, viewType);

		}

	}

    /// <summary>
    /// Base class for the Advance Object layers controller.
    /// </summary>
    [AddComponentMenu("")]
	public class AdvanceObjectLayerController : MonoBehaviour , ILayerController
	{
	    /// <summary>
	    /// Layer data.
	    /// </summary>
	    [Tooltip("Layer settings")]
        [Space]
		public AdvanceObjectLayerData Settings ;

	    /// <summary>
	    /// Layer Mover.
	    /// </summary>
	    [Tooltip("Layer mover")]
        [Space]
		public DirectionalMover mover;

	    /// <summary>
	    /// Index of the layer.
	    /// </summary>
		public int Index { get; set; }
	    /// <summary>
	    /// The current speed of the layer.
	    /// </summary>
		public float Speed { get{ return ((Mover) mover).speed; } }
	    /// <summary>
	    /// The type of layer that this controller controls.
	    /// </summary>
		public LayerControllerType Type { get; protected set; }

        /// <summary>
        /// The time that the next object will spawn at.
        /// </summary>
		protected float NextObjectTime ;
        /// <summary>
        /// Index of the Object that's gonna be spawned next.
        /// </summary>
		protected int NextObjectIndex;
        /// <summary>
        /// The Object that's going to be spawned next.
        /// </summary>
		protected AdvanceObject NextObject { get { return Settings.Objects [NextObjectIndex]; } }

	    protected Rect GameField
	    {
	        get { return LevelController.Instance.GameField; }
	    }

        /// <summary>
        /// Indicates if the layer passes the first loop.
        /// </summary>
		private bool _firstGo ;

	    /// <summary>
	    /// The Start method is one of Unity's messages that get called when a new object is instantiated.
	    /// </summary>
        protected virtual void Start () {

			Type = LayerControllerType.AdvanceObject;

			NextObjectTime = Settings.StartTime + Time.time;

			mover.speed = Settings.Speed;
			
			_firstGo = true;

		}

	    /// <summary>
	    /// One of Unity's messages that gets called at every frame.
	    /// </summary>
        void Update () {

            //If the layer passes the first loop and it is not set to loop we stop the layer creation.
			if (!Settings.Loop && !_firstGo)
				return;

            //Checks if it's time to create the next object
	        if (!(Time.time >= NextObjectTime))
	            return;

	        //Creates the next object, sets the time for the next one and updates the index.

	        CreateNextObject ();

	        NextObjectTime += GetWaitTime();

	        if ( NextObjectIndex+1 >= Settings.Objects.Length) {
	            //Resetting the index if it surpasses the Objects array length.
	            NextObjectIndex = 0;
	            _firstGo = false;

	        }else
	            NextObjectIndex++;

	    }

        /// <summary>
        /// Creates the next Advance Object Layers object.
        /// </summary>
		private void CreateNextObject () {
            
			GameObject element = null;

			element = Instantiate (NextObject.LayerObject, transform) as GameObject;

			Position (element.transform);
			Rotate (element.transform);
			Scale (element.transform);

			Destroy (element, Settings.Lifetime);

		}

        /// <summary>
        /// Position the NextObject to its location.
        /// </summary>
        /// <param name="target">NextObject transform</param>
		protected virtual void Position ( Transform target ) {

			

		}

	    /// <summary>
	    /// Scales the NextObject.
	    /// </summary>
	    /// <param name="target">NextObject transform</param>
		protected void Scale ( Transform target ) {

			target.localScale = NextObject.Scale;

		}

	    /// <summary>
	    /// Rotates the NextObject.
	    /// </summary>
	    /// <param name="target">NextObject transform</param>
		protected void Rotate ( Transform target ) {

			target.rotation = Quaternion.Euler (NextObject.Rotation);

		}

        /// <summary>
        /// Returns the wait time for the next creation.
        /// </summary>
        /// <returns>The time between the latest created object and the next object.</returns>
		protected float GetWaitTime () {

			return Settings.Objects [NextObjectIndex].TimeForNextObject;

		}

	    /// <summary>
	    /// Changes the layer speed.
	    /// </summary>
	    /// <param name="value">new speed value</param>
		public void ChangeSpeed ( float value ) {
			mover.speed = value;
		}

	    /// <summary>
	    /// Changes the layer settings.
	    /// </summary>
	    /// <param name="settings">A new setting that will replace the old ones.</param>
	    /// <param name="layerMover">The new mover that will replace the old one.</param>
	    /// <param name="index">The new index for the layer.</param>
		public void ChangeSettings (LayerCreator settings, Mover layerMover, int index) {

			Index = index;

	        transform.position = new Vector3(transform.position.x, transform.position.y, LevelController.Instance.SpaceBetween * index);

            AdvanceObjectLayerData layerCreation = settings as AdvanceObjectLayerData;
		    if (layerCreation != null)
				Settings = layerCreation;

		    DirectionalMover mover = layerMover as DirectionalMover;
		    if (mover != null)
				this.mover = mover;

		}


	}

}