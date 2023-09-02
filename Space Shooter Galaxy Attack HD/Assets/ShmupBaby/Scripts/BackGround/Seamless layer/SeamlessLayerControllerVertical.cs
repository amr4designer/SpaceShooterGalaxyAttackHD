using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the seamless layer in the vertical view.
    /// </summary>
    [AddComponentMenu("")]
	public class SeamlessLayerControllerVertical : SeamlessLayerController
	{
	    /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
		protected override void Start() {

		    base.Start();

            mover.RepeatDistance = GetBackgroundNumbers() * Settings.Dimension.y;
            mover.ChangeDirection(Vector3.down);

		}

        /// <summary>
        /// One of Unity's Messages that gets called every frame.
        /// </summary>
        private void Update () {

            Distance += mover.DeltaDistance;

            if (Distance >= Settings.Dimension.y)
            {
                backgrounds[minIndex].SetActive(false);

                minIndex++;
                maxIndex++;

                backgrounds[maxIndex].SetActive(true);

                //Updates the distance.
                Distance = mover.DeltaDistance;
            }
                
		}

        /// <summary>
        /// create a background and position scale and rotate it based on the controller settings.
        /// </summary>
        /// <param name="prefab">prefab of the background</param>
        /// <param name="index">the background index (control the background position)</param>
        /// <returns>reference to the new instantiated background.</returns>
        protected override GameObject CreateBackground(Object prefab, int index)
        {
            float backgroundLength = Settings.Dimension.y;

            //Creates position, scale ad rotates the background.
            GameObject background = (GameObject)Instantiate(prefab, transform);

            Rotate(background);
            Scale(background);
            background.transform.position = new Vector3(0, backgroundLength * 0.5f + (index) * backgroundLength, transform.position.z);

            background.SetActive(false);

            return background;
        }
        

    }

}