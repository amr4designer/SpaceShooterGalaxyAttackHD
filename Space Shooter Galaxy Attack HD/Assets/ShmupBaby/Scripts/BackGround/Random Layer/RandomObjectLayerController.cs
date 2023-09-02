using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Scale option for Object creation of the Random Object layer.
    /// </summary>
    public enum ScaleOption
    {
        Prefab ,
        Random ,
        Input
    }

    /// <summary>
    /// Rotation option for Object creation for the Random Object layer.
    /// </summary>
    public enum RotationOption
    {
        Prefab ,
        Random2D ,
        Random ,
        Input
    }

    /// <summary>
    /// The structure of the Random Object layer data, this will also handle
    /// the creation of the layer based on the data.
    /// </summary>
	[System.Serializable]
	public class RandomObjectLayerData : LayerCreator  { 

        /// <summary>
        /// Indicates if the Object creation should be random or by order.
        /// </summary>
		[Space]
        [Tooltip("By default, objects will be created in order," +
                 "Check this for a random pick")]
		public bool UseRandomObject;
        /// <summary>
        /// The Objects of the random layer.
        /// </summary>
        [Tooltip("A list of Objects of the random layer")]
		public Object[] Objects;
        
        /// <summary>
        /// Layer Speed.
        /// </summary>
		[Space]
		[Tooltip("Speed of the layer in world units per Seconds")]
        public float Speed ;

        /// <summary>
        /// Layer Objects lifetime in seconds.
        /// </summary>
        [Space]
        [Tooltip("Lifetime for the layer Object in Seconds")]
        public float StartTime;

        /// <summary>
        /// Wait time between elements.
        /// </summary>
		[Space]
		[Tooltip("Wait time between elements")]
		public float WaitTime;
        /// <summary>
        /// Indicates if the layer is using a random wait time.
        /// </summary>
        [Tooltip("Check if you want to use Random Time For Next Object")]
		public bool RandomWaitTime ;
        /// <summary>
        /// The minimum wait time between the objects used if Random Wait Time is set to true.
        /// </summary>
		[Tooltip("This time will be used if RandomWaitTime is checked")]
		public float MinRandomWaitTime = 1 ;
        /// <summary>
        /// The maximum wait time between the objects used if Random Wait Time is set to true.
        /// </summary>
		[Tooltip("This time will be used if Random Wait Time is checked")]
		public float MaxRandomWaitTime = 4 ;

        /// <summary>
        /// Indicates if the layer keeps looping.
        /// </summary>
        [Space]
		[Tooltip("Make the layer loop till the end of the game")]
		public bool Loop;

        /// <summary>
		/// Position of the object depending on the level view.
		/// For example it represents the position on the top edge of the
		/// game field for the vertical view and the right edge of the game
		/// field for the horizontal view.
        /// </summary>
		[Space]
		[Range(0,1)]
		public float Position = 0.5f;
        /// <summary>
        /// Indicates if the layer is using a random Position for Objects creation.
        /// </summary>
		[Tooltip("Random layer element position through the width of the screen")]
		public bool RandomPosition ;

        /// <summary>
        /// Options for the initial rotation of the object.
        /// </summary>
		[Space]
        [Tooltip("Options for Objects rotation")]
		public RotationOption rotationOption;
        /// <summary>
        /// This initial rotation of the object will be used if the rotation Option is set to input.
        /// </summary>
        [Tooltip("This rotation will be used if the Rotation option set to Input")]
		public Vector3 Rotation;

        /// <summary>
        /// Options for the initial scale of the object
        /// </summary>
		[Space]
		[Tooltip("Options for Objects scale")]
        public ScaleOption scaleOption;
        /// <summary>
        /// An initial scale of the object will be used if the scale Option is set to input.
        /// </summary>
		[Tooltip("This Scale will be used if Scale To Use is set to Input")]
		public Vector3 Scale;
        /// <summary>
        /// The minimum scale of the object what ill be used if the scale Option is set to random.
        /// </summary>
		[Tooltip("This Scale will be used if Scale To Use is set to Random")]
		public float MinRandomScale = 1 ;
        /// <summary>
        /// The maximum scale of the object will be used if the scale Option is set to random.
        /// </summary>
		[Tooltip("This Scale will be used if Scale To Use is set to Random")]
		public float MaxRandomScale = 4 ;

        /// <summary>
        /// Layer Objects lifetime in seconds.
        /// </summary>
		[Space]
		[Tooltip("The lifetime for the layer in Seconds")]
		public float ElementLifetime;

        /// <summary>
        /// Offset for the Object spawn position from the edge of the game field.
        /// </summary>
        [Space]
        [Tooltip("To prevent Objects from spawning at the edge of the field")]
        public float Offset;

        /// <summary>
        /// The layer Index.
        /// </summary>
        [Space]
		[Range(1,100)]
		public int Index ;

        //Overrides the LayerIndex to return Index, this is because there is no support for the properties in the inspector. 
        public override int LayerIndex { get { return Index; } }

        /// <summary>
        /// Creates the seamless layer and returns a controller for the layer.
        /// </summary>
        /// <param name="index">The layer index (layer position on the Z axis)</param>
        /// <param name="parent">The parent of the layer</param>
        /// <param name="viewType">The view type of this level</param>
        /// <returns></returns>
		public override ILayerController CreateLayer ( int index , Transform parent , LevelViewType viewType ) { 

			return CreateLayer<RandomObjectLayerControllerVertical,RandomObjectLayerControllerHorizontal,DirectionalMover> ("Random Object Layer", index, parent, viewType);

		}

	}

    /// <summary>
    /// Base class for the Random Object layer controller
    /// </summary>
    [AddComponentMenu("")]
    public class RandomObjectLayerController : MonoBehaviour , ILayerController
	{

	    /// <summary>
	    /// Layer data.
	    /// </summary>
	    [Tooltip("Layer settings")]
	    [Space]
        public RandomObjectLayerData Settings ;

	    /// <summary>
	    /// Layer Mover.
	    /// </summary>
	    [Tooltip("Layer mover")]
	    [Space]
        public DirectionalMover mover;

	    /// <summary>
	    /// The index of the layer.
	    /// </summary>
	    public int Index { get; set; }
	    /// <summary>
	    /// The current speed of the layer.
	    /// </summary>
	    public float Speed { get { return ((Mover) mover).speed; } }
	    /// <summary>
	    /// The type of layer that this controller controls.
	    /// </summary>
	    public LayerControllerType Type { get; protected set; }

	    /// <summary>
	    /// The time that the next object will be spawned at.
	    /// </summary>
        protected float NextObjectTime ;
	    /// <summary>
	    /// Index of the Object that's going to be spawned next.
	    /// </summary>
        protected int NextObjectIndex ;

	    protected Rect GameField { get { return LevelController.Instance.GameField; } }

        /// <summary>
        /// Indicates if the layer passes the first loop.
        /// </summary>
        protected bool _firstGo ;

	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
	    protected virtual void Start()
	    {

	        Type = LayerControllerType.AdvanceObject;

	        NextObjectTime = Settings.StartTime + Time.time;

	        mover.speed = Settings.Speed;

	        _firstGo = true;

	    }

        /// <summary>
        /// One of Unity's messages that gets called every frame.
        /// </summary>
        private void Update () {

	        //If the layer passes the first loop and it is not set to loop we stop the layer creation.
            if (!Settings.Loop && !_firstGo)
				return;

	        //Check if it's time to create the next object.
            if (!(Time.time >= NextObjectTime))
		        return;

			//Creates the next object, sets the time for the next one and updates the index.

            CreateNextObject();

		    NextObjectTime += GetWaitTime();

		    NextObjectIndex++;

		    if (NextObjectIndex + 1 >= Settings.Objects.Length)
		    {
		        //Resetting the index if it passes the Objects array length.
                NextObjectIndex = 0;
		        _firstGo = false;

		    }
		    else
		        NextObjectIndex++;

		}

	    /// <summary>
	    /// create the next Advance Object Layer object
	    /// </summary>
		protected void CreateNextObject () {

			GameObject element = null;

			if ( Settings.UseRandomObject ) 
				element = Instantiate (Settings.Objects[Random.Range(0,Settings.Objects.Length)], transform) as GameObject;
			else
				element = Instantiate (Settings.Objects[NextObjectIndex], transform) as GameObject;

			Position (element.transform);
			Rotate (element.transform);
			Scale (element.transform);

			Destroy (element, Settings.ElementLifetime);

		}

	    /// <summary>
	    /// Positions the NextObject.
	    /// </summary>
	    /// <param name="target">Next Object transform</param>
		protected virtual void Position ( Transform target ) {

			

		}

	    /// <summary>
	    /// Scales the NextObject.
	    /// </summary>
	    /// <param name="target">NextObject transform</param>
		protected void Scale ( Transform target ) {

            //Scales the object depending on the scale option.
			switch (Settings.scaleOption) {

			case ScaleOption.Prefab:
				return;
			case ScaleOption.Input:
				target.localScale = Settings.Scale;
				return;
			case ScaleOption.Random:
				target.localScale = Vector3.one * Random.Range (Settings.MinRandomScale, Settings.MaxRandomScale);
				return;
				
			}

		}

	    /// <summary>
	    /// Rotates the NextObject.
	    /// </summary>
	    /// <param name="target">Next Object transform</param>
		protected void Rotate ( Transform target ) {

	        //Rotates the object depending on the rotation option.
            switch (Settings.rotationOption) {

			case RotationOption.Prefab:
				return;
			case RotationOption.Input:
			    target.rotation = Quaternion.Euler (Settings.Rotation);
				return;
			case RotationOption.Random2D:
			
				target.rotation = Math2D.Random2DRotation;
				return;
				
			case RotationOption.Random:
				target.rotation = Random.rotation;
				return;
			}

		}

	    /// <summary>
	    /// Returns the wait time for the next object creation.
	    /// </summary>
	    /// <returns>The time between the latest created object and the next object.</returns>
		protected float GetWaitTime () {
			
			if (!Settings.RandomWaitTime)
				return Settings.WaitTime;
			else
				return Random.Range (Settings.MinRandomWaitTime, Settings.MaxRandomWaitTime);
			
		}

	    /// <summary>
	    /// Changes the layer speed.
	    /// </summary>
	    /// <param name="value">New speed value</param>
		public void ChangeSpeed ( float value )
	    {
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

            RandomObjectLayerData layerCreation = settings as RandomObjectLayerData;
		    if (layerCreation != null)
				Settings = layerCreation;

		    DirectionalMover mover = layerMover as DirectionalMover;
		    if (mover != null)
				this.mover = mover;

		}


		}

}