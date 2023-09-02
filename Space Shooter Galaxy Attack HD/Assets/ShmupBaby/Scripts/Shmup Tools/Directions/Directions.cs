using UnityEngine;

namespace ShmupBaby
{
    public enum VerticalDirection
    {
        None,
        Up,
        Down,
    }

    public enum HorizontalDirection
    {
        None,
        Left,
        Right
    }

    public enum FourDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public enum EightDirection
    {
        None,
        Up,
        UpLeft,
        UpRight,
        Down,
        DownLeft,
        DownRight,
        Left,
        Right
    }


    /// <summary>
    /// A collection of functions for converting between the direction enumerators, 
    /// vectors and angles.
    /// </summary>
    public static class Directions
    {
        public static VerticalDirection VectorToVerticalDirection(Vector2 direction)
        {
            if (direction.y == 0.0f)
            {
                return VerticalDirection.None;
            }

            if (direction.y > 0.0f)
            {
                return VerticalDirection.Up;
            }
            else
            {
                return VerticalDirection.Down;
            }
        }


        public static HorizontalDirection VectorToHorizontalDirection(Vector2 direction)
        {
            if (direction.x == 0.0f)
            {
                return HorizontalDirection.None;
            }

            if (direction.x > 0.0f)
            {
                return HorizontalDirection.Right;
            }
            else
            {
                return HorizontalDirection.Left;
            }
        }


        #region FourDirection Methods


        /// <summary>
        /// Rounds a direction to the nearest FourDirection value.
        /// </summary>
        /// <param name="direction">The direction that will be converted.</param>
        /// <returns>the result enum value.</returns>
        public static FourDirection VectorToFourDirection(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return FourDirection.None;
            }
                
            float angle = Math2D.VectorToDegree(direction);

            if (angle >= 45f && angle < 135f)
            {
                return FourDirection.Up;
            }
                

            if (angle >= 135f && angle < 225f)
            {
                return FourDirection.Left;
            }
                

            if (angle >= 225f && angle < 315f)
            {
                return FourDirection.Down;
            }               

            return FourDirection.Right;
        }


        /// <summary>
        /// Converts a FourDirection value into a vector2.
        /// </summary>
        /// <param name="direction">The direction that needs to be converted.</param>
        /// <returns>the result vector2.</returns>
        public static Vector2 FourDirectionToVector(FourDirection direction)
        {
            switch (direction)
            {
                case FourDirection.Up:
                    {
                        return Vector2.up;
                    }

                case FourDirection.Down:
                    {
                        return Vector2.down;
                    }

                case FourDirection.Left:
                    {
                        return Vector2.left;
                    }

                case FourDirection.Right:
                    {
                        return Vector2.right;
                    }

                default:
                    {
                        return Vector2.zero;
                    }
            }
        }


        /// <summary>
        /// converts FourDirection value to angle in degrees.
        /// </summary>
        /// <param name="direction">the direction that needs to be converted.</param>
        /// <returns>the resulting angle in degrees.</returns>
        public static float FourDirectionToAngle(FourDirection direction)
        {
            switch (direction)
            {

                case FourDirection.Down:
                    return 270;
                
                case FourDirection.Up:
                    return 90;

                case FourDirection.Left:
                    return 180;

                case FourDirection.Right:
                    return 0;
                                   
            }

            return 0;
        }


        #endregion


        #region Eight Direction Methods


        /// <summary>
        /// Convert an EightDirection value to angle in degrees.
        /// </summary>
        /// <param name="direction">The direction that needs to be converted.</param>
        /// <returns>the resulting angle in degrees.</returns>
        public static float EightDirectionToAngle(EightDirection direction)
        {
            switch (direction)
            {
                case EightDirection.Down:
                    {
                        return 270;
                    }
                    
                case EightDirection.Up:
                    {
                        return 90;
                    }
                    
                case EightDirection.Left:
                    {
                        return 180;
                    }                    

                case EightDirection.Right:
                    {
                        return 0;
                    }                   

                case EightDirection.UpRight:
                    {
                        return 45;
                    }                   

                case EightDirection.UpLeft:
                    {
                        return 135;
                    }
                    

                case EightDirection.DownLeft:
                    {
                        return 225;
                    }
                    

                case EightDirection.DownRight:
                    {
                        return 315;
                    }                   
            }

            return 0;
        }


        /// <summary>
        /// Rounds a direction to the nearest EightDirection value.
        /// </summary>
        /// <param name="direction">The direction that will be converted.</param>
        /// <returns>the resulting enumerator value.</returns>
        public static EightDirection VectorToEightDirection(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return EightDirection.None;
            }
                
            float angle = Math2D.VectorToDegree(direction);

            if (angle >= 22.5f && angle < 67.5f)
            {
                return EightDirection.UpRight;
            }
                
            if (angle >= 67.5f && angle < 112.5f)
            {
                return EightDirection.Up;
            }
                
            if (angle >= 112.5f && angle < 157.5f)
            {
                return EightDirection.UpLeft;
            }
                
            if (angle >= 157.5f && angle < 202.5f)
            {
                return EightDirection.Left;
            }
                
            if (angle >= 202.5f && angle < 247.5f)
            {
                return EightDirection.DownLeft;
            }
                
            if (angle >= 247.5f && angle < 292.5f)
            {
                return EightDirection.Down;
            }
                
            if (angle >= 292.5f && angle < 337.5f)
            {
                return EightDirection.DownRight;
            }               

            return EightDirection.Right;
        }

        /// <summary>
        /// Converts an EightDirection value to a vector2.
        /// </summary>
        /// <param name="direction">The direction that needs to be converted.</param>
        /// <returns>the result vector2.</returns>
        public static Vector2 EightDirectionToVector(EightDirection direction)
        {
            switch (direction)
            {
                case EightDirection.Up:
                    {
                        return Vector2.up;
                    }
                    
                case EightDirection.Down:
                    {
                        return Vector2.down;
                    }
                    
                case EightDirection.Left:
                    {
                        return Vector2.left;
                    }
                    
                case EightDirection.Right:
                    {
                        return Vector2.right;
                    }
                    
                case EightDirection.UpRight:
                    {
                        return new Vector2(0.5f, 0.5f);
                    }
                    
                case EightDirection.DownRight:
                    {
                        return new Vector2(0.5f, -0.5f);
                    }
                    
                case EightDirection.DownLeft:
                    {
                        return new Vector2(-0.5f, -0.5f);
                    }
                    
                case EightDirection.UpLeft:
                    {
                        return new Vector2(-0.5f, 0.5f);
                    }

                default:
                    {
                        return Vector2.zero;
                    }                    
            }
        }

        #endregion


    }


}
