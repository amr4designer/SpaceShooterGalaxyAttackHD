using UnityEngine;
using System.Collections.Generic;

namespace ShmupBaby {

    /// <summary>
    /// The controller for the seamless layer in the horizontal view.
    /// </summary>
    [AddComponentMenu("")]
    public class SeamlessLayerControllerHorizontal : SeamlessLayerController
	{
        /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
		protected override void Start()
        {

            base.Start();

            mover.RepeatDistance = GetBackgroundNumbers() * Settings.Dimension.x;
            mover.ChangeDirection(Vector3.left);

        }

        /// <summary>
        /// One of Unity's Messages that gets called every frame.
        /// </summary>
        private void Update()
        {

            Distance += mover.DeltaDistance;

            if (Distance >= Settings.Dimension.x)
            {
                backgrounds[minIndex].SetActive(false);

                minIndex++;
                maxIndex++;

                if (maxIndex < backgrounds.Length)
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
            float backgroundLength = Settings.Dimension.x;

            //Creates position, scale ad rotates the background.
            GameObject background = (GameObject)Instantiate(prefab, transform);

            Rotate(background);
            Scale(background);
            background.transform.position = new Vector3(backgroundLength * 0.5f + (index) * backgroundLength,0 , transform.position.z);

            background.SetActive(false);

            return background;
        }

    }

}