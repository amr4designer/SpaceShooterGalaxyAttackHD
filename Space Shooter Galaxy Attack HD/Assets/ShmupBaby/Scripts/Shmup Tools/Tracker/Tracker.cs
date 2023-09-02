using UnityEngine;
using System.Collections.Generic;

namespace ShmupBaby
{
    /// <summary>
    /// track the change of value for the defined fields.
    /// </summary>
    /// <typeparam name="T">the field type.</typeparam>
    public class State<T> where T : struct 
    {
        /// <summary>
        /// trigger when there is a change in value.
        /// </summary>
        public event System.Action OnStateChange;

        /// <summary>
        /// the value of the field.
        /// </summary>
        public T state
        {
            get
            {
                return _state;
            }
            set
            {
                if (!_state.Equals(value))
                {
                    _state = value;

                    if (OnStateChange != null)
                        OnStateChange();
                }

                _state = value;
            }
        }

        /// <summary>
        /// back-end field for state.
        /// </summary>
        private T _state;       
    }

    /// <summary>
    /// a list of tracker tracking state.
    /// </summary>
    public enum TrackState
    {
        StartState,
        TargetLost,
        TargetFound,
        TargetReached,
        TargetDetected,
        TargetOutOfRange
    }

    /// <summary>
    /// option for selecting target for the tracker.
    /// </summary>
    public enum TargetOption
    {
        InputAgent,
        InputPosition,
        Player,
        RandomEnemy
    }

    /// <summary>
    /// define what data to be passed when the Tracker tracks state change.
    /// </summary>
    public class TrackStateArgs : ShmupEventArgs
    {
        /// <summary>
        /// the state of tracker after the event trigger.
        /// </summary>
        public TrackState State;

        /// <summary>
        /// TrackStateArgs constructor.
        /// </summary>
        /// <param name="state">the state of tracker after the event trigger.</param>
        public TrackStateArgs(TrackState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// tracker is a simple component that provides track information for
    /// a given agent or position such as : direction , distance 
    /// </summary>
    [AddComponentMenu("")]
    public class Tracker : MonoBehaviour
    {
        /// <summary>
        /// a state to provide the target condition or position from the tracker.
        /// </summary>
        protected State<TrackState> trackState = new State<TrackState>();
        /// <summary>
        /// a state to provide the current target option.
        /// </summary>
        private State<TargetOption> _targetOption = new State<TargetOption>();

        /// <summary>
        /// the displacement from the tracker to the target in the current frame.
        /// </summary>
        private Vector2 _displacement;
        /// <summary>
        /// the target position.
        /// </summary>
        private Vector3 _desirePosition;

        /// <summary>
        /// trigger when the tracker state change.
        /// </summary>
        public event ShmupDelegate OnStateChange;

        /// <summary>
        /// the current tracking agent.
        /// </summary>
        public Agent CurrentTarget { get; private set; }

        /// <summary>
        /// the option for the tracking target.
        /// </summary>
        public TargetOption Target
        {
            get
            {
                return _targetOption.state;
            }
            set
            {
                _targetOption.state = value;
            }
        }

        /// <summary>
        /// a state to provide the target condition or position from the tracker.
        /// </summary>
        public TrackState TrackingState
        {
            get
            {
                return trackState.state;
            }
        }

        /// <summary>
        /// the agent that will be track if the TrackOption is set to InputAgent.
        /// </summary>
        public Agent InputTarget { get; set; }
        /// <summary>
        /// the position that will be track if the TrackOption is set to InputPosition.
        /// </summary>
        public Vector3 InputPosition { get; set; }
        
        /// <summary>
        /// the distance between the tracker and the target.
        /// </summary>
        public float Distance { get; private set; }
        /// <summary>
        /// the direction from the tracker toward the target.
        /// </summary>
        public Vector2 Direction { get; private set; }

        /// <summary>
        /// the distance at which the tracker will declare it self that it reached it's target.
        /// </summary>
        public float TargetReachedThreshold { get; set; }


        private void Awake()
        {
            trackState.OnStateChange += RiseTrackStateChange;
            _targetOption.OnStateChange += UpdateTarget;
        }


        protected virtual void Update()
        {
            //check if the target exist in the scene.
            if (CurrentTarget != null || Target == TargetOption.InputPosition)
            {
                trackState.state = TrackState.TargetFound;

                UpdateTrackingInfo();

                if (Distance <= TargetReachedThreshold)
                    trackState.state = TrackState.TargetReached;

            }
            else
            {
                trackState.state = TrackState.TargetLost;

                //request the target if it's doesn't exist.
                UpdateTarget();

                //recheck for the target again because it may change after request it.
                if (CurrentTarget != null)
                    UpdateTrackingInfo();
            }
        }
        
        /// <summary>
        /// update the fields that provide the track information about the target position.
        /// </summary>
        private void UpdateTrackingInfo()
        {
            _desirePosition = Target == TargetOption.InputPosition ? InputPosition : CurrentTarget.transform.position;

            _displacement = (Vector2)(_desirePosition - transform.position);
            Distance = _displacement.magnitude;
            Direction = _displacement.normalized;
        }

        /// <summary>
        /// depending on the Option of the target this method will update the current target.
        /// </summary>
        private void UpdateTarget()
        {

            switch (Target)
            {
                case TargetOption.Player:
                    CurrentTarget = LevelController.Instance.PlayerComponent;
                    return;

                case TargetOption.RandomEnemy:
                {
                    List<IPool> allEnemy = PoolManager.GetList<Enemy>();

                    if (allEnemy != null && allEnemy.Count >= 1)
                    {
                        CurrentTarget = allEnemy[Random.Range(0, allEnemy.Count)] as Enemy;
                        return;
                    }
                    else
                    {
                        CurrentTarget = null;
                        return;
                    }
                }

                case TargetOption.InputAgent:
                    CurrentTarget = InputTarget;
                    return;
            }

        }

        /// <summary>
        /// handle the rise of OnStateChange.
        /// </summary>
        private void RiseTrackStateChange()
        {
            if (OnStateChange != null)
                OnStateChange (new TrackStateArgs(trackState.state));
        }
    }
        
}