using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Data structure that defines the patrol mover waypoints.
    /// </summary>
	[System.Serializable]
	public class WayPoints
    {

        /// <summary>
        /// Waypoint position in the XY plane.
        /// </summary>
		public Vector2 Position;
        /// <summary>
        /// Wait time in Seconds for this Waypoint.
        /// </summary>
		public float time ;

        /// <summary>
        /// Waypoint position as Vector3.
        /// </summary>
        public Vector3 PositionV3
        {
            get
            {
                return Math2D.Vector2ToVector3 (Position);
            }
        }

        /// <summary>
        /// WayPoints constructor.
        /// </summary>
        /// <param name="position">Waypoint position in the XY plane.</param>
        /// <param name="waitTime">Time to wait in this waypoint.</param>
		public WayPoints (Vector2 position, float waitTime)
        {
			Position = position;
			time = waitTime;
		}

	}

    /// <summary>
    /// A list of options for the patrol mover to loop through its waypoints.
    /// </summary>
	public enum PatrolMoverMode
	{
	    Normal ,
	    Loop ,
	    BackAndForth
	}

    /// <summary>
    /// Mover that moves the across a list of Waypoints.
    /// </summary>
	[AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/Patrol Mover")]
    [SelectionBase]
	public class PatrolMover : Mover
	{

		#if UNITY_EDITOR

        /// <summary>
        /// Only available in the editor,
        /// show label above the waypoint in the scene view. 
        /// </summary>
		[Header("Handle Settings")]
		[Space]
        [Tooltip("Shows a label above the waypoint in the scene view.")]
		public bool ShowLabel = true;
        /// <summary>
        /// Only available in the editor,
        /// shows a handle similar to the translate handle,
		/// for every waypoint in the scene view
        /// </summary>
        [Tooltip("Shows a handle similar to the translate handle, for every waypoint in the scene view")]
        public bool UsePositionHandle = false;
	    /// <summary>
	    /// Only available in the editor,
	    /// shows a label above the waypoint
	    /// in the scene view. 
	    /// </summary>
	    [Tooltip("Shows a simple circular handle for every waypoint in the scene view")]
        public bool UseCircleHandle = true;
	    /// <summary>
	    /// Only available in the editor,
	    /// shows a label above the waypoints
	    /// in the scene view. 
	    /// </summary>
        [Tooltip("The size of the circular handle.")]
		public float HandleSize = 0.5f;

        #endif

	    /// <summary>
	    /// input speed by the inspector.
	    /// </summary>
	    [Space]
	    [Tooltip("Speed in (World Unit/Seconds).")]
        public float Speed;

        /// <summary>
        /// The mover method to loop through the points.
        /// </summary>
		[Space]
		public PatrolMoverMode Mode;
        /// <summary>
        /// A number of the first waypoints that will be Ignored from looping,
        /// when the Mode is set to Loop or Back and Forth.
        /// </summary>
		public int IgnorePointFormLooping ;

        /// <summary>
        /// Option to snap the first waypoint to the game field rectangle.
        /// </summary>
	    [Space]
	    public PointSnapOptions FirstPointSnap = PointSnapOptions.None;
        /// <summary>
        /// Offsets the waypoint position after snapping to the gameField 
        /// </summary>
	    public float SnapOffset;
        /// <summary>
        /// The waypoints which the mover will loop though.
        /// </summary>
		public WayPoints[] Points ;

        /// <summary>
        /// Game field for this level.
        /// </summary>
        protected Rect GameField
        {
            get
            {
                return LevelController.Instance.GameField;
            }
        }

	    /// <summary>
	    /// Current speed for the mover in (World Unit/Seconds).
	    /// </summary>
	    public override float speed
	    {
	        get
            {
                return Speed;
            }
	        set
            {
                Speed = value;
            }
	    }

        /// <summary>
        /// The index of the next waypoint.
        /// </summary>
	    private int _pointIndex = 1 ;

        /// <summary>
        /// Is the mover going back and forth with the waypoints.
        /// </summary>
		private bool _forth = true ;
        /// <summary>
        /// Is the mover in waiting state.
        /// </summary>
		private bool _waiting = true ;
        /// <summary>
        /// The time for the mover to start moving if it's in waiting state.
        /// </summary>
		private float _nextTimeToMove;


        private void Start ()
        {
			_nextTimeToMove = Time.time + Points [0].time;
            
            //Snaps the first way point
		    if (Points.Length > 1 && FirstPointSnap != PointSnapOptions.None)
		    {
		        Points[0].Position = Wave.SnapPoint(Points[0].Position, GameField, SnapOffset, FirstPointSnap);
		    }

            //Positions the mover to its first waypoint.
	        if (Points.Length > 1)
            {
                transform.position = Math2D.Vector2ToVector3(Points[0].Position, transform.position.z);
            }                          
		}

	    /// <summary>
	    /// UpdateDirection is called on every frame to update the mover direction.
	    /// </summary>
	    /// <returns>the current mover direction.</returns>
        protected override Vector2 UpdateDirection()
	    {
            //Checks if the mover is in waiting state.
	        if (_waiting)
	        {
                //Check if the waiting state is over.
                if (_nextTimeToMove <= Time.time)
                {
                    _waiting = false;
                }
                else
                {
                    return Vector2.zero;
                }	                
	        }

            //get the distance for the next point. 
	        Vector3 displacementToTheNextPoint = (Vector2)(Points[_pointIndex].PositionV3 - transform.position);
            float distanceToNextPoint = displacementToTheNextPoint.magnitude;

            //Checks if the distance has been reached.
	        if (distanceToNextPoint <= 0.1f)
	        {
                //Updates the next point index depending on the mover Mode.
                switch (Mode)
	            {
	                case PatrolMoverMode.Normal:
	                    if (!UpdateIndexForward())
                        {
                            return Vector2.zero;
                        }
                                
	                    break;
	                
	                case PatrolMoverMode.Loop:
	                    MoveLoop();
                        break;
	                
	                case PatrolMoverMode.BackAndForth:
	                    MoveBackForth();
                        break;	                
	            }
	        }                       
            //Returns the direction to the next point.
	        return displacementToTheNextPoint.normalized;
	    }

        /// <summary>
        /// Updates the next point index for the loop move.
        /// </summary>
	    private void MoveLoop()
	    {
	        if (!UpdateIndexForward())
            {
                _pointIndex = IgnorePointFormLooping;
            }	            
	    }

        /// <summary>
        /// Updates the next point index for the back and forth move.
        /// </summary>
	    private void MoveBackForth()
	    {
	        if (_forth)
	        {
	            if (!UpdateIndexForward())
	            {
	                _forth = false;
	                _pointIndex--;
	            }
	        }
	        else
	        {
	            if (!UpdateIndexBackWard())
	            {
	                _forth = true;
	                _pointIndex++;
	            }
	        }
	    }

        /// <summary>
        /// Updates the next index point forward.
        /// </summary>
        /// <returns>True if the next point is available.</returns>
	    private bool UpdateIndexForward()
	    {
	        if (_pointIndex + 1 < Points.Length)
	        {
	            _nextTimeToMove = Time.time + Points[_pointIndex].time;
	            _waiting = true;
	            _pointIndex++;
	            return true;
	        }

	        return false;
	    }

	    /// <summary>
	    /// Updates the next index point backward.
	    /// </summary>
	    /// <returns>True if previous point is available.</returns>
	    private bool UpdateIndexBackWard()
	    {
	        if (_pointIndex - 1 >= IgnorePointFormLooping)
	        {
	            _nextTimeToMove = Time.time + Points[_pointIndex].time;
	            _waiting = true;
	            _pointIndex--;
	            return true;
	        }

	        return false;
	    }

    }

}