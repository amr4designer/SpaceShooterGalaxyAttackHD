using UnityEngine;
using System.Collections.Generic;

namespace ShmupBaby {

    /// <summary>
    /// The structure of the Seamless layer data, this will also handle
    /// the creation of the layer based on the data.
    /// </summary>
    [System.Serializable]
	public class SeamlessLayerData : MainLayerData  { 

        /// <summary>
        /// The Index for this layer.
        /// </summary>
		[Range(1,100)]
        [Tooltip("The Index for this layer")]
		public int Index ;

        //Overrides the Layer Index to return the Index, this is because there is no support for these properties in the inspector. 
        public override int LayerIndex { get { return Index; } }

        public SeamlessLayerData(BackgroundObject[] backgrounds, float speed, Vector3 scale, int backgroundRepetition) 
	        : base(backgrounds, speed, scale, backgroundRepetition)
	    {

	    }
        
        /// <summary>
        /// Creates the seamless layer and returns a controller for the layer.
        /// </summary>
        /// <param name="index">the layer index (layer position on the Z axis)</param>
        /// <param name="parent">the parent for the layer</param>
        /// <param name="viewType">The view type of this level</param>
        /// <returns></returns>
		public override ILayerController CreateLayer ( int index , Transform parent , LevelViewType viewType ) { 

			return CreateLayer<SeamlessLayerControllerVertical,SeamlessLayerControllerHorizontal,RepeaterMover> ("Seamless Layer", index, parent, viewType);

		}

	}

    /// <summary>
    /// Base class for the Seamless layer controller
    /// </summary>
    [AddComponentMenu("")]
    public class SeamlessLayerController : MonoBehaviour, ILayerController
    {
        /// <summary>
        /// Layer data.
        /// </summary>
        [Tooltip("Layer settings")]
        [Space]
        public MainLayerData Settings;

        /// <summary>
        /// Layer Mover.
        /// </summary>
        [Tooltip("Layer mover")]
        [Space]
        public RepeaterMover mover;

        /// <summary>
        /// Index of the layer.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The current speed of the layer.
        /// </summary>
        public float Speed { get { return mover.speed; } }
        /// <summary>
        /// Type of layer that this controller controls.
        /// </summary>
        public LayerControllerType Type
        {
            get { return LayerControllerType.Seamless; }
        }


        //The distance that has been passed since the start.
        protected float Distance;

        protected GameObject[] backgrounds;

        protected int minIndex;
        protected int maxIndex;

        /// <summary>
        /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        protected virtual void Start()
        {
            if (!Settings.IsInitialize)
                Settings.Initialize();

            int backgroundNumber = 0;

            backgroundNumber = GetBackgroundNumbers();

            //if the BackgroundRepetition is less than the number of background then the Repetition is passed.
            CreateAllBackground(backgroundNumber + 1 + Settings.BackgroundRepetition);

            mover.speed = Settings.Speed;
            //reset the active backgrounds when the mover repeat.
            mover.OnRepeat += ResetActiveBackground;

            //active the initial backgrounds and set the minIndex and maxIndex
            ResetActiveBackground();

            Distance = mover.DeltaDistance;

        }

        /// <summary>
        /// reset the min index and the max index to their original position
        /// and set the background activation depending on the new indexes.
        /// </summary>
        protected void ResetActiveBackground(ShmupEventArgs args)
        {
            ResetActiveBackground();
        }

        /// <summary>
        /// reset the min index and the max index to their original position
        /// and set the background activation depending on the new indexes.
        /// </summary>
        protected void ResetActiveBackground ()
        {
            minIndex = 0;
            maxIndex = minIndex + Settings.BackgroundRepetition + 1;

            ChangeActiveBackground();
        }

        /// <summary>
        /// set the background activation depending on the minIndex and the maxIndex.
        /// </summary>
        private void ChangeActiveBackground()
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if ( i >= minIndex && i <= maxIndex)
                    backgrounds[i].SetActive(true);
                else
                    backgrounds[i].SetActive(false);
            }
        }

        /// <summary>
        /// return the number of background in the settings.
        /// </summary>
        /// <returns>the number of background in the settings</returns>
        protected virtual int GetBackgroundNumbers ()
        {
            int backgroundNumber = 0;

            for (int i = 0; i < Settings.Backgrounds.Length; i++)
            {
                int bgRepeate = Settings.Backgrounds[i].Repeat <= 0 ? 1 : Settings.Backgrounds[i].Repeat;

                for (int j = 0; j < bgRepeate; j++)
                {
                    backgroundNumber++;
                }
            }

            return backgroundNumber;
        }

        /// <summary>
        /// Instantiate and set the background (position,scale rotation) and deactivate them.
        /// </summary>
        /// <param name="backgroundNum">the number of background needed.</param>
        protected virtual void CreateAllBackground (int backgroundNum)
        {
            float backgroundLength = Settings.Dimension.y;

            backgrounds = new GameObject[ backgroundNum + 1];

            backgrounds[0] = CreateBackground(Settings.Backgrounds[Settings.Backgrounds.Length-1].Background, -1);

            //track the number of background needed.
            int index = 0;

            while (true)
            {
                for (int i = 0; i < Settings.Backgrounds.Length; i++)
                {
                    int bgRepeate = Settings.Backgrounds[i].Repeat <= 0 ? 1 : Settings.Backgrounds[i].Repeat;

                    for (int j = 0; j < bgRepeate; j++)
                    {
                        if (index  >= backgroundNum)
                            return;

                        index++;
                        backgrounds[index] = CreateBackground(Settings.Backgrounds[i].Background, index-1);
                        
                    }

                }

            }
        }

        protected virtual GameObject CreateBackground (Object prefab, int index)
        {
            return null;
        }

        /// <summary>
        /// Changes the layer speed.
        /// </summary>
        /// <param name="value">new speed value</param>
        public void ChangeSpeed ( float value ) {
            mover.speed = value;
        }

	    /// <summary>
	    /// Change the layer settings
	    /// </summary>
	    /// <param name="settings">A new setting that will replace the old ones</param>
	    /// <param name="layerMover">The new mover that will replace the old one</param>
	    /// <param name="index">The new index for the layer</param>
		public void ChangeSettings (LayerCreator settings, Mover layerMover, int index) {

			Index = index;

            transform.position = new Vector3(transform.position.x,transform.position.y,LevelController.Instance.SpaceBetween*index);

		    MainLayerData layerCreation = settings as MainLayerData;
		    if (layerCreation != null)
				Settings = layerCreation;

            RepeaterMover m = layerMover as RepeaterMover;

            if (m != null)
                mover = m;

		}

        /// <summary>
        /// Rotates the new instantiated background by overriding the rotation.
        /// </summary>
		protected void Rotate (GameObject background) {

			switch (Settings.OverrideRotation) {

			case OrthographicRotate.Rotate90:
                background.transform.rotation = Quaternion.Euler(0, 0, 90);
				break;
			case OrthographicRotate.Rotate270:
                background.transform.rotation = Quaternion.Euler(0, 0, 270);
				break;
			case OrthographicRotate.Rotate180:
                background.transform.rotation = Quaternion.Euler(0, 0, 180);
				break;

			}

		}
        /// <summary>
        /// Scales the new instantiated background by the override scale.
        /// </summary>
		protected void Scale (GameObject background) {

			if ( Settings.OverrideScale )
                background.transform.localScale = Settings.Scale;

		}

	}

}