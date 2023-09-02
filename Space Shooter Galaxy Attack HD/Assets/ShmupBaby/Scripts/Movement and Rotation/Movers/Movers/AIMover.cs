using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Enemy mover that have a simple AI, trying 
    /// to evade player straight fire.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Mover/AI Mover")]
    public class AIMover : Mover
    {
        /// <summary>
        /// The state of the AI mover.
        /// </summary>
        public enum ManeuverState
        {
            /// <summary>
            /// Indicates that the mover is in the starting move.
            /// </summary>
            StartingMove,
            /// <summary>
            /// Indicates that the mover is waiting for the player to do a Maneuver Move.
            /// </summary>
            Wait,
            /// <summary>
			/// Indicates that the player is ahead (in front) the mover.
            /// </summary>
            TargetAhead,
            /// <summary>
            /// Indicates that the mover is trying to find a valid move to evade 
            /// the player bullets.
            /// </summary>
            SettingManeuverMove,
            /// <summary>
            /// Indicates that the mover is doing a Maneuver Move.
            /// </summary>
            ManeuverMove
        }


        [Header("Gizmo Settings")]
        [Space]
        [Tooltip("Draws the mover average collider width")]
        public bool DrawGizmos;


        [Space]
        public float StartingMove;
        /// <summary>
        /// Input speed by the inspector.
        /// </summary>
        [Space]
        [Tooltip("Speed in (World Unit/Sec).")]
        public float Speed;
        /// <summary>
        /// Wait time before doing the Maneuver move.
        /// </summary>
        [Space]
        [Tooltip("How much should the mover wait when the player is ahead of it" +
            "before it does the Maneuver.")]
        public float TimeBeforeManeuver;
        /// <summary>
        /// Maneuver move cool-down time.
        /// </summary>
        [Tooltip("Cool-down time for the Maneuver, how much time should the mover wait before it" +
			" does another Maneuver.")]
        public float TimeForNextManeuver;
        /// <summary>
        /// The distance passed by the Maneuver move.
        /// </summary>
        [Space]
        [Tooltip("The Maneuver distance in world unit.")]
        public float ManeuverDistance;
        /// <summary>
        /// The top edge offset of the mover field from the game field.
        /// </summary>
        [Space]
        [Range(0, 1f)]
        [Tooltip("Offsets the top edge of the field that the mover will be allowed to move in" +
            "from the game field.")]
        public float OffsetTop = 1f;
        /// <summary>
        /// The bottom edge offset of the move field from the game field.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the bottom edge of the field that the mover will be allowed to move in" +
            "from the game field.")]
        public float OffsetBottom = 0;
        /// <summary>
        /// The left edge offset of the mover field from the game field.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the left edge of the field that the mover will be allowed to move in" +
            "from the game field.")]
        public float OffsetLeft = 0;
        /// <summary>
        /// The right edge offset of the move field from the game field.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("Offsets the right edge of the field that the mover will be allowed to move in" +
            "from the game field.")]
        public float OffsetRight = 1f;
        /// <summary>
        /// The average collider width for the mover.
        /// </summary>
        [Space]
		[Tooltip("Average width of the mover collider it will help the mover to better " +
			"detect the player.")]
        public float AvgColliderWidth;

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
        /// The view type of the level.
        /// </summary>
        private LevelViewType View
        {
            get
            {
                return LevelController.Instance.View;
            }
        }
        /// <summary>
        /// Current level game field.
        /// </summary>
        private Rect gamefield
        {
            get
            {
                return LevelController.Instance.GameField;
            }
        }
        /// <summary>
        /// The current player mover.
        /// </summary>
        private PlayerMover targetMover
        {
            get
            {
                if (LevelController.Instance.PlayerComponent != null)
                    return LevelController.Instance.PlayerComponent.mover;
                else
                    return null;
            }
        }

        /// <summary>
        /// The threshold used to check if the mover reached its position.
        /// </summary>
        private float _distanceCheckTolerance ;
        /// <summary>
        /// The position that the mover wants to reach.
        /// </summary>
        private Vector2 _desirePosition;
        /// <summary>
        /// The current state of the mover.
        /// </summary>
        public ManeuverState _state;
        /// <summary>
        /// The next time that the maneuver is allowed to 
        /// move.
        /// </summary>
        private float _holdManeuverUntil;
        /// <summary>
        /// The direction of the player in the current frame.
        /// </summary>
        private FourDirection _playerDirection;
        /// <summary>
        /// The direction of the mover in the current frame.
        /// </summary>
        private FourDirection _maneuverDirection;
        /// <summary>
        /// The number of attempts to find a valid move in the current frame.
        /// </summary>
        private int _findMoveCounter;
        /// <summary>
        /// The maximum number of attempts to find a valid move 
        /// per frame.
        /// </summary>
        private const int FindMoveAttempts = 5;
        /// <summary>
        /// The field the mover is allowed to move in.
        /// </summary>
        private Rect MoverField;

        /// <summary>
        /// The direction that the mover will pick from when it's going to the right.
        /// </summary>
        private readonly Vector2[] rightDirection  = new Vector2[]
        {
            new Vector2(0.5f,0.5f),
            new Vector2(1f, 0),
            new Vector2(0.5f, -0.5f)
        };
        /// <summary>
        /// The direction that the mover will pick from when it's going to the left.
        /// </summary>
        private readonly Vector2[] leftDirection   = new Vector2[] 
        {
            new Vector2(-0.5f, 0.5f),
            new Vector2(-1f, 0),
            new Vector2(-0.5f, -0.5f)
        };
        /// <summary>
        /// The direction that the mover will pick from when it's going up.
        /// </summary>
        private readonly Vector2[] upDirection   = new Vector2[] 
        {
            new Vector2(0.5f, 0.5f),
            new Vector2(0, 1f),
            new Vector2(-0.5f, 0.5f)
        };
        /// <summary>
        /// The direction that the mover will pick from when it's going down.
        /// </summary>
        private readonly Vector2[] downDirection = new Vector2[] 
        {
            new Vector2(0.5f, -0.5f),
            new Vector2(0, -1f),
            new Vector2(-0.5f, -0.5f)
        };


        void Start()
        {
            _distanceCheckTolerance = Speed * 0.02f;

            MoverField = new Rect
            {
                xMin = gamefield.xMin + (gamefield.width * OffsetLeft),
                xMax = gamefield.xMin + (gamefield.width * OffsetRight),
                yMin = gamefield.yMin + (gamefield.height * OffsetBottom),
                yMax = gamefield.yMin + (gamefield.height * OffsetTop)
            };

            _state = ManeuverState.StartingMove;

            _desirePosition = (GetStartMoveDirection() * StartingMove) + (Vector2)transform.position;
        }


        /// <summary>
        /// UpdateDirection is called from inside the mover class
        /// update function on every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
        protected override Vector2 UpdateDirection()
        {
            switch (_state)
            {
                case ManeuverState.StartingMove:

                    if (checkIfReachedTargetPosition())
                    {
                        _state = ManeuverState.Wait;
                        break;
                    }

                    return GetStartMoveDirection();

                case ManeuverState.Wait:

                    if (targetMover == null)
                    {
                        break;
                    }
                        
                    if (View == LevelViewType.Vertical)
                    {
                        //wait for the player to be in front of the mover.
                        if (Mathf.Abs(targetMover.transform.position.x - transform.position.x) <= AvgColliderWidth)
                        {
                            _playerDirection = targetMover.BasicDirection;
                            _state = ManeuverState.TargetAhead;
                            break;
                        }
                    }

                    if (View == LevelViewType.Horizontal)
                    {
                        //wait for the player to be in front of the mover.
                        if (Mathf.Abs(targetMover.transform.position.y - transform.position.y) <= AvgColliderWidth)
                        {
                            _playerDirection = targetMover.BasicDirection;
                            _state = ManeuverState.TargetAhead;
                            break;
                        }
                    }

                    break;

                case ManeuverState.TargetAhead:

                    //wait if the mover is on hold.
                    if (_holdManeuverUntil > Time.time)
                    {
                        break;
                    }
                    
                    _holdManeuverUntil = Time.time + TimeBeforeManeuver;
                    _state = ManeuverState.SettingManeuverMove;

                    break;

                case ManeuverState.SettingManeuverMove:

                    if (_holdManeuverUntil > Time.time)
                    {
                        break;
                    }                        

                    Vector2 dir = Vector2.zero;

                    if (View == LevelViewType.Vertical)
                        dir = ManeuverMoveVerticalDirection();
                    if (View == LevelViewType.Horizontal)
                        dir = ManeuverMoveHorizontal();

                    if (dir != Vector2.zero)
                        _state = ManeuverState.ManeuverMove;

                    return dir;

                case ManeuverState.ManeuverMove:

                    if (checkIfReachedTargetPosition())
                    {
                        _holdManeuverUntil = Time.time + TimeForNextManeuver;
                        _state = ManeuverState.Wait;
                    }

                    return Direction;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Returns the direction of the start move.
        /// </summary>
        /// <returns>The direction of the start move.</returns>
        private Vector2 GetStartMoveDirection()
        {
            if (View == LevelViewType.Vertical)
            {
                _maneuverDirection = FourDirection.Left;
                return Vector2.down;
            }

            if (View == LevelViewType.Horizontal)
            {
                _maneuverDirection = FourDirection.Up;
                return Vector2.left;
            }
                        
            return Vector2.zero;
        }

        /// <summary>
        /// Returns a direction for the maneuver move for the vertical level.
        /// </summary>
        /// <returns>Direction for the maneuver move for the vertical level.</returns>
        private Vector2 ManeuverMoveVerticalDirection()
        {
            if (_playerDirection == FourDirection.Left)
            {
                if (CheckIfDirectionsInMoverField(leftDirection))
                {
                    return pickMove(leftDirection);
                }                    
                else
                {
                    return pickMove(rightDirection);
                }
            }
            else 
            {
                if (CheckIfDirectionsInMoverField(rightDirection))
                    return pickMove(rightDirection);
                else
                {
                    return pickMove(leftDirection);
                }
            }
        }

        /// <summary>
        /// Returns a direction for the maneuver move for the horizontal level.
        /// </summary>
        /// <returns>Direction for the maneuver move for the horizontal level.</returns>
        Vector2 ManeuverMoveHorizontal()
        {
            if (_playerDirection == FourDirection.Up)
            {
                if (CheckIfDirectionsInMoverField(upDirection))
                    return pickMove(upDirection);
                else
                {
                    return pickMove(downDirection);
                }

            }
            else
            {
                if (CheckIfDirectionsInMoverField(downDirection))
                    return pickMove(downDirection);
                else
                {
                    return pickMove(upDirection);
                }
            }

        }

        /// <summary>
        /// Checks if there is a maneuver move valid for a given direction.
        /// </summary>
        /// <param name="direction">Array of desired directions.</param>
        /// <returns>True if there is valid maneuver move for any given direction.</returns>
        bool CheckIfDirectionsInMoverField( Vector2[] direction )
        {
            
            for (int i = 0; i < direction.Length; i++)
            {
                if ((InsideRect(direction[i] * ManeuverDistance + (Vector2)transform.position, MoverField)))
                    return true;
            }

            return false;

        }

        /// <summary>
        /// Tries to pick a valid move in the given directions.
        /// </summary>
        /// <param name="direction">Array of desire directions.</param>
        /// <returns>True if there is a maneuver move valid for a given directions.</returns>
        private Vector2 pickMove( Vector2[] direction )
        {
            _findMoveCounter = FindMoveAttempts;

            do
            {
                Vector2 d = direction[Random.Range(0, direction.Length)];
                _desirePosition = d * ManeuverDistance + (Vector2)transform.position;
                _findMoveCounter--;

                if (InsideRect(_desirePosition, MoverField))
                {
                    return d;
                }

            } while ( _findMoveCounter > 0);

            return Vector2.zero;
        }

        /// <summary>
        /// Checks if the given position is inside the given rect.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="rect">The field to check if the position is inside it.</param>
        /// <returns>True if the position is inside the rectangle.</returns>
        bool InsideRect(Vector2 position, Rect rect)
        {
            if (position.x < rect.xMax && position.x > rect.xMin &&
                position.y < rect.yMax && position.y > rect.yMin)
                return true;
            else
                return false;
            
        }

        /// <summary>
        /// Checks if the mover reached the _desirePosition.
        /// </summary>
        /// <returns>True if the mover reach the _desirePosition.</returns>
        bool checkIfReachedTargetPosition()
        {
            if (((Vector2)transform.position - _desirePosition).magnitude <= _distanceCheckTolerance)
            {
                return true;
            }
            else
                return false;
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (!DrawGizmos)
                return;

            Gizmos.color = Color.yellow;
            GizmosExtension.DrawCircle(_desirePosition, 2f);

            Gizmos.color = new Color(0,1,0,0.5f);
            GizmosExtension.DrawCircle(transform.position , AvgColliderWidth*2f);
            Gizmos.color = Color.cyan;
            GizmosExtension.DrawCircle(transform.position , ManeuverDistance);
        }
#endif

    }

}