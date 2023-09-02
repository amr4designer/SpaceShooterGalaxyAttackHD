using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// A list of Mover states.
    /// </summary>
    public enum MoverState
    {
        Moving,
        Stopping
    }


    /// <summary>
    /// Defines the data to be passed when the Mover event is triggered.
    /// </summary>
    public class MoveChangeArgs : ShmupEventArgs
    {
        /// <summary>
        /// The current state of the Mover.
        /// </summary>
        public MoverState State;

        /// <summary>
        /// MoveChangeArgs constructor
        /// </summary>
        /// <param name="state">State after the change.</param>
        public MoveChangeArgs(MoverState state)
        {
            State = state;
        }
    }


    /// <summary>
    /// Defines the general properties for the moving objects in the scene.
    /// </summary>
    public interface IMove
    {
        /// <summary>
        /// This event should be triggered when there is a change in
        /// the moving state.
        /// </summary>
        event ShmupDelegate OnMovingChange;

        /// <summary>
        /// Current moving speed (World Unit/Sec).
        /// </summary>
		float speed { get ; set; }
        /// <summary>
        /// Moving direction.
        /// </summary>
        Vector2 Direction { get; }
        /// <summary>
        /// The distance this mover moves for the current frame.
        /// </summary>
        float DeltaDistance { get; }
        /// <summary>
        /// The current moving state.
        /// </summary>
        MoverState MoveState { get ; set; }

        /// <summary>
        /// Simplified mover direction. 
        /// </summary>
        FourDirection BasicDirection { get ; }
        /// <summary>
        /// Advanced mover direction. 
        /// </summary>
        EightDirection AdvanceDirection { get ; }
        /// <summary>
        /// Direction used with the gradual sprite swap.
        /// </summary>
        HorizontalDirection SideToSideDirection { get; }
        /// <summary>
        /// Direction used with the gradual sprite swap.
        /// </summary>
        VerticalDirection UpAndDownDirection { get; }
    }

    /// <summary>
    /// The base class for any mover component.
    /// </summary>
    public abstract class Mover : MonoBehaviour , IMove
	{
        /// <summary>
        /// Is triggered when the MoveState change.
        /// </summary>
        public event ShmupDelegate OnMovingChange;

        /// <summary>
        /// Current speed for the mover (World Unit/Sec).
        /// </summary>
        public virtual float speed { get ; set; }
	    /// <summary>
	    /// Direction of velocity.
	    /// </summary>
        public Vector2 Direction { get; private set; }
        /// <summary>
        /// The distance this mover moves for the current frame.
        /// </summary>
        public float DeltaDistance
        {
            get { return _deltaDisplacement.magnitude; }
        }
        /// <summary>
	    /// The current moving state.
	    /// </summary>
	    public MoverState MoveState
	    {
	        get { return _moveState.state; }
	        set { _moveState.state = value; }
	    }


        /// <summary>
        /// Gradual sprite swap direction.
        /// </summary>
        public HorizontalDirection SideToSideDirection
        {
            get
            {
                if (speed == 0) return HorizontalDirection.None;
                else return Directions.VectorToHorizontalDirection(Direction);
            }
        }


        /// <summary>
        /// Gradual sprite swap direction.
        /// </summary>
        public VerticalDirection UpAndDownDirection
        {
            get
            {
                if (speed == 0) return VerticalDirection.None;
                else return Directions.VectorToVerticalDirection(Direction);
            }
        }


        /// <summary>
        /// Simplified mover direction. 
        /// </summary>
        public FourDirection BasicDirection
        { 
			get
            {
				if (speed == 0)
					return FourDirection.None;

			    return Directions.VectorToFourDirection(Direction);
			}
		}
        /// <summary>
        /// Advanced mover direction. 
        /// </summary>
        public EightDirection AdvanceDirection
        {
			get
            {
				if (speed == 0)
					return EightDirection.None;

			    return Directions.VectorToEightDirection(Direction);               
			}
		}


        /// <summary>
        /// Reference to the attached rigidbody if it exists. 
        /// </summary>
        protected Rigidbody2D rigidbody;     

        /// <summary>
        /// Back-end field for MoveState.
        /// </summary>
	    private State<MoverState> _moveState;
        /// <summary>
        /// Mover velocity.
        /// </summary>
        private Vector2 _velocity;
        /// <summary>
        /// Displacement in the current frame.
        /// </summary>
        private Vector2 _deltaDisplacement;


        protected virtual void Awake()
	    {
	        _moveState = new State<MoverState>();
	        _moveState.OnStateChange += UpdateMover;
	    }


        private void Update()
	    {
	        Direction = UpdateDirection();
	        _velocity = Direction * speed;
            _deltaDisplacement = _velocity * Time.deltaTime;

            //If the velocity is zero then we change the MoverState.
            _moveState.state = _velocity == Vector2.zero ?
	            MoverState.Stopping : MoverState.Moving;

            //Move the gameObject by speed in the given direction 
            if (_moveState.state == MoverState.Moving && rigidbody == null)
            {
                transform.position += Math2D.Vector2ToVector3(_deltaDisplacement);
            }	            
	            
            
	        //Move by the rigid body if it exits, or by the transform component.
            if (rigidbody != null)
            {
                rigidbody.velocity = _velocity;
            }	            
        }


        /// <summary>
        /// UpdateDirection called every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
	    protected virtual Vector2 UpdateDirection()
	    {
	        return Vector2.zero;
	    }
        
        /// <summary>
        /// Called when the MoverState changes, raises the
        /// MoveChange event.
        /// </summary>
        private void UpdateMover ()
        {
            if (OnMovingChange != null)
            {
                OnMovingChange(new MoveChangeArgs(_moveState.state));
            }                           
		}

	}

}