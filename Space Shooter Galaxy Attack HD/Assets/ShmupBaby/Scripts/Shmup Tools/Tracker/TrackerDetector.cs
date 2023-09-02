using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// provide a track information as the tracker 
    /// with extra event and radius to check for the 
    /// target position.
    /// </summary>
    [AddComponentMenu("")]
    public class TrackerDetector : Tracker
    {
        /// <summary>
        /// the radius at which the target will be detected.
        /// </summary>
        public float DetectRadius
        {
            get;
            set;
        }


        protected override void Update()
        {
            base.Update();

            //divide the TargetFound state into TargetDetected and TargetOutOfRange state.
            if (trackState.state != TrackState.TargetFound)
            {
                return;
            }               

            if (Distance <= DetectRadius)
            {
                trackState.state = TrackState.TargetDetected;
            }
            else
            {
                trackState.state = TrackState.TargetOutOfRange;
            }              
        }

    }

}
