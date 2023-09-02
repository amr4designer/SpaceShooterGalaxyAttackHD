using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Library for 2D math calculations.
    /// </summary>
    public static class Math2D {

        /// <summary>
        /// returns a random rotation in the y axis with integer values.
        /// </summary>
        public static Quaternion Random2DRotation
        {
            get { return Quaternion.Euler(0, 0, Random.Range(0, 361)); }
        }

        /// <summary>
        /// Converts a vector 2 to a vector 3 with 0 in the z value.
        /// </summary>
        /// <param name="vector">the vector 2 that needs to be converted.</param>
        public static Vector3 Vector2ToVector3 ( Vector2 vector ) {

            return new Vector3 (vector.x, vector.y, 0);

        }

        /// <summary>
        /// Converts vector 2 to vector 3 .
        /// </summary>
        /// <param name="vector">the vector 2 that needs to be converted.</param>
        /// <param name="z">the z value for the returned vector 3.</param>
        public static Vector3 Vector2ToVector3 ( Vector2 vector , float z ) {

            return new Vector3 (vector.x, vector.y, z);

        }
        
        /// <summary>
        /// converts an angle to a vector 2.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>a normalized vector represent the direction of the angle.</returns>
        public static Vector2 DegreeToVector2 ( float angle ) {

            angle *= Mathf.Deg2Rad;

            return new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));

        }

        /// <summary>
        /// Convert an angle to a vector 3.
        /// </summary>
        /// <param name="angle">angle in degrees.</param>
        /// <param name="z">the z value for the returned vector 3.</param>
        /// <returns>a normalized vector represents the direction of the angle.</returns>
        public static Vector3 DegreeToVector3 ( float angle , float z ) {

            angle *= Mathf.Deg2Rad;

            return new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), z);

        }

        /// <summary>
        /// Convert an angle to vector 3.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>a normalized vector represents the direction of the angle.</returns>
        public static Vector3 DegreeToVector3 ( float angle ) {

            return DegreeToVector3 (angle, 0);

        }

        /// <summary>
        /// Converts a vector 2 to an angle.
        /// </summary>
        /// <param name="vector">The vector that will be converted.</param>
        /// <returns>Converted angle in degrees.</returns>
        public static float VectorToDegree ( Vector2 vector ) {

            //Checks when x equals zero to avoid the division on zero. 
            if (vector.x == 0 && vector.y >= 0)
                return 90f;

            if (vector.x == 0 && vector.y < 0)
                return 270f;

            float angle = Mathf.Rad2Deg*Mathf.Atan (vector.y / vector.x);

            //If the angle is still in the first quarter then it is returned as it is. 
            if (vector.x > 0 && vector.y >= 0)
                return angle;

            //if the angle is in the second or the third quarter returns the angle + 180. 
            if (vector.x < 0 )
                return angle + 180f;

            //if the angle in the fourth quarter return the angle + 360. 
            return angle + 360f ;

        }
                
        /// <summary>
        /// converts vector 3 to an angle.
        /// </summary>
        /// <param name="vector">the vector that will be converted.</param>
        /// <returns>converted angle in degree.</returns>
        public static float VectorToDegree ( Vector3 vector ) {

            return VectorToDegree ( new Vector2 ( vector.x , vector.y) );

        }

        /// <summary>
        /// Accelerates interpolation. 
        /// </summary>
        /// <param name="min">The minimum number for the interpolation</param>
        /// <param name="max">The maximum number for the interpolation</param>
        /// <param name="t">Interpolates coefficient</param>
        /// <returns>Interpolated value</returns>
        public static float Accp ( float min , float max , float t ) {

            if (t >= 1)
                t = 1;

            if (t <= 0)
                t = 0;

            float deference = max - min;

            return min + deference*t*t;

        }

        /// <summary>
        /// Remaps a value between 0 and 1.
        /// </summary>
        /// <param name="value">the value that needs to be remaped.</param>
        /// <param name="minValue">The minimum value domain.</param>
        /// <param name="maxValue">The maximum value domain.</param>
        /// <returns>the remap value.</returns>
        public static float Remap01(float value, float minValue, float maxValue)
        {
            if (value <= minValue)
                return minValue;
            if (value >= maxValue)
                return maxValue;

            return (value - minValue) / (maxValue - minValue);
        }

    }
}