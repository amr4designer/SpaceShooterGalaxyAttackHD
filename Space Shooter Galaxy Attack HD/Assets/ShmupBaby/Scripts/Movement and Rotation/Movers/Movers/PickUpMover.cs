using System;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// A list of the Pickup mover moving states.
    /// </summary>
    public enum PickUpMoverState
    {
        Spawn ,
        Normal ,
        DetectTarget
    }

    /// <summary>
    /// Mover that handle how the pickup moves.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Mover/Pick Up Mover")]
    public sealed class PickUpMover : Mover
	{
        /// <summary>
        /// The pickup speed in normal state.
        /// </summary>
        [Tooltip("The directional speed the pickup item moves with.")]
		public float NormalSpeed ;
        /// <summary>
		/// The radius at which the pickup will change its state to the detected target .
        /// </summary>
	    [Tooltip("When the player is within this radius, the pick up item will move towards the player")]
        public float MagnetRadius = 1;
	    /// <summary>
	    /// The pickup speed in the target detected state.
	    /// </summary>
	    [Tooltip("The speed with which the pick up item moves towards the player if within the magnet radius.")]
        public float MagnetSpeed ;
	    /// <summary>
	    /// The pickup speed in spawn state.
	    /// </summary>
		[Tooltip("The speed at which the item moves from the spawning location to its new location in the pick up radius.")]
        public float SpawnSpeed ;
        /// <summary>
        /// The radius which the pickup can be picked in.
        /// </summary>
	    [Tooltip("The radius the items will be picked up in.")]
        public float PickRadius;

        /// <summary>
        /// The position the pickup will reach after it gets instantiated.
        /// </summary>
        [HideInInspector]
		public Vector3 SpawnPosition;


        [Tooltip("Draws the radius of detection")]
		public bool DrawGizmos;


        /// <summary>
        /// trigger when the pickup reach it's target.
        /// </summary>
	    public event ShmupDelegate OnReachTarget;

        /// <summary>
        /// The view for the current level.
        /// </summary>
        private LevelViewType ViewType
        {
            get
            {
                return LevelController.Instance.View;
            }
        }

        /// <summary>
        /// The current moving state of the mover.
        /// </summary>
	    private PickUpMoverState _state;
        /// <summary>
        /// The tracker attached to this mover.
        /// </summary>
        private TrackerDetector _tracker;


        private void Start ()
		{
            // Clamps the MagnetRadius
            if (MagnetRadius <= 0)
            {
                MagnetRadius = 1f;
            }		        

            // Adds a tracker and initializes it.
		    _tracker = gameObject.AddComponent<TrackerDetector>();

            // Caching the spawn position just in case the pickup was used without being spawned by an enemy drop.
            if (SpawnPosition == Vector3.zero)
            {
                SpawnPosition = transform.position;
            }
            
		    _tracker.Target = TargetOption.InputPosition;
		    _tracker.InputPosition = SpawnPosition;

            _tracker.DetectRadius = MagnetRadius;
		    _tracker.TargetReachedThreshold = 0.1f;

            _tracker.OnStateChange += UpdateTargetPosition;
            
            // Sets the mover state to spawn.
		    _state = PickUpMoverState.Spawn;
            speed = SpawnSpeed;           
		}

        /// <summary>
        /// Called by the tracker, updates the mover moving state.
        /// </summary>
	    private void UpdateTargetPosition(ShmupEventArgs args)
	    {
	        TrackStateArgs trackerArgs = (TrackStateArgs)args;
            
            switch (trackerArgs.State)
	        {
	            case TrackState.TargetDetected:
	                if (_state != PickUpMoverState.Spawn)
	                {
	                    _state = PickUpMoverState.DetectTarget;
	                    speed = MagnetSpeed;
                    }
                    break;

	            case TrackState.TargetReached:
	                if (_state == PickUpMoverState.Spawn)
	                {
	                    _state = PickUpMoverState.Normal;
	                    speed = NormalSpeed;
	                    _tracker.TargetReachedThreshold = PickRadius;
	                    _tracker.Target = TargetOption.Player;
                    }
	                else
	                {
	                    RiseOnReachTarget();
                    }
                    break;

                default:
                    if (_state != PickUpMoverState.Spawn)
                    {
                        _state = PickUpMoverState.Normal;
                        speed = NormalSpeed;
                    }
                    break;
            }

	    }

	    /// <summary>
	    /// UpdateDirection is called on every frame to update the mover direction.
	    /// </summary>
	    /// <returns>the current mover direction.</returns>
	    protected override Vector2 UpdateDirection()
	    {
            if (_state == PickUpMoverState.Normal)
            {
                return NormalDirection();
            }
            else
            {
                return _tracker.Direction;
            }	            
	    }

        /// <summary>
        /// Returns a direction depending on the current level view.
        /// </summary>
        /// <returns></returns>
	    private Vector2 NormalDirection()
	    {
	        switch (ViewType)
	        {
	            case LevelViewType.Vertical:
	                return Vector2.down;
	            case LevelViewType.Horizontal:
	                return Vector2.left;
	            default:
	                return Vector2.zero;
            }
        }

        /// <summary>
        /// Handles the rise of OnReachTarget.
        /// </summary>
        private void RiseOnReachTarget()
	    {
	        if (OnReachTarget != null)
            {
                OnReachTarget(null);
            }	            
	    }

#if UNITY_EDITOR

        void OnDrawGizmos ()
        {
			if (!DrawGizmos)
            {
                return;
            }
				
            //Draws the MagnetRadius
            Gizmos.color = Color.yellow;
			GizmosExtension.DrawCircle (transform.position, MagnetRadius);

            //Draws the PickRadius
            Gizmos.color = Color.cyan;
			GizmosExtension.DrawCircle (transform.position, PickRadius);
		}

#endif

    }

}