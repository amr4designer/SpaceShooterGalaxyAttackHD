using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Define the general methods for the 2D curve.
    /// </summary>
    public interface ICurve2D
    {
        /// <summary>
        /// Returns positions on the curve.
        /// </summary>
        /// <param name="t">Value between 0 and curve length, zero 
        /// will return the first control point position.</param>
        /// <returns>position on the curve.</returns>
        Vector2 GetPosition(float t);
        /// <summary>
        /// The length of the curve in world units.
        /// </summary>
        /// <param name="segment">higher values for accurate curve length.</param>
        /// <returns>the length of the curve in world units.</returns>
        float GetLength(int segment);
        /// <summary>
        /// Draws gizmo lines representing the curve.
        /// </summary>
        /// <param name="delta">Value between 0 and 1, lower value 
        /// for more accurate draw.</param>
        void Draw(float delta);
        /// <summary>
        /// Draws gizmo lines representing the curve.
        /// </summary>
        /// <param name="delta">Value between 0 and 1, lower values 
        /// for more accurate drawing.</param>
        /// <param name="ZPosition">the position of the curve drawn on the Z-Axis.</param>
        void Draw(float delta, float z);
    }

    /// <summary>
    /// A 2D curve on the XY-Plane based on Catmull-Rom spline equation.
    /// </summary>
	public class Curve2D : ICurve2D
    {

        #region Members

        /// <summary>
        /// The smoothness of the curve, a value of zero will 
        /// make the curve segment take the shape of a line, 
        /// higher values to be more rounded.
        /// </summary>
        public float Smooth {
            set {
                //Edits the smoothness for all the segments.
                if (_segments != null)
                    for (int i = 0; i < _segments.Length; i++) {
                        _segments[i].Smooth = value;
                    }
            }
            get {
                //Gets the smoothness of the first segment.
                if (_segments != null)
                    return _segments[0].Smooth;
                else
                    return 0;
            }
        }

        /// <summary>
        /// The segments that make up the curve.
        /// </summary>
        private CurveSegment[] _segments;

        #endregion

        #region Constructors

        /// <summary>
        /// Curve2D constructor.
        /// </summary>
        /// <param name="controllPoints">array of the curve control point.</param>
        public Curve2D(Vector2[] controllPoints) {

            int pointNum;

            if (controllPoints != null)
                pointNum = controllPoints.Length;
            else
                return;


            if (pointNum <= 1)
                _segments = null;


            //creates a curve with one segment.
            if (pointNum == 2) {
                _segments = new CurveSegment[1];
                _segments[0] = new CurveSegment(controllPoints[0], controllPoints[1]);
            }

            //creates a curve with more than one segment.
            if (pointNum > 2) {

                _segments = new CurveSegment[pointNum - 1];

                //Creates the first segment.
                _segments[0] = new CurveSegment(controllPoints[0], controllPoints[1], controllPoints[2], SegmentType.Start);
                //Creates the segments in-between.
                for (int i = 1; i <= _segments.Length - 2; i++)
                {
                    _segments[i] = new CurveSegment(controllPoints[i - 1], controllPoints[i], controllPoints[i + 1], controllPoints[i + 2]);
                }
                //Creates the last segment.
                _segments[_segments.Length - 1] = new CurveSegment(controllPoints[controllPoints.Length - 3], controllPoints[controllPoints.Length - 2], controllPoints[controllPoints.Length - 1], SegmentType.End);

            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// returns control points position.
        /// </summary>
        /// <param name="index">the index of the point.</param>
        /// <returns>control point position.</returns>
        public Vector2 GetPoint(int index) {

            if (index <= 0)
                return _segments[0].PointB;

            if (index >= _segments.Length)
                return _segments[_segments.Length - 1].PointC;

            return _segments[index].PointB;

        }

        /// <summary>
        /// Edits a curve control point.
        /// </summary>
        /// <param name="newPoint">the value for the point.</param>
        /// <param name="index">the point index.</param>
		public void EditPoint(Vector2 newPoint, int index) {

            if (index < 0 || index > _segments.Length)
                return;

            if (index <= 0) {
                _segments[0].PointB = newPoint;
                _segments[0].CalculatePointA();
                if (_segments.Length > 1) {
                    _segments[1].PointA = newPoint;
                }
                return;
            }

            if (index == _segments.Length) {
                _segments[_segments.Length - 1].PointC = newPoint;
                _segments[_segments.Length - 1].CalculatePointD();
                _segments[_segments.Length - 2].PointD = newPoint;
                return;
            }

            _segments[index].PointB = newPoint;
            _segments[index - 1].PointC = newPoint;

            if (index >= 2)
                _segments[index - 2].PointD = newPoint;

            if (index < _segments.Length - 1)
                _segments[index + 1].PointA = newPoint;

            _segments[0].CalculatePointA();
            _segments[_segments.Length - 1].CalculatePointD();

        }

        /// <summary>
        /// Returns a position on the curve.
        /// </summary>
        /// <param name="t">value between 0 and 1 on the curve, zero 
        /// will return the first control point position.</param>
        /// <returns>position on the curve.</returns>
        public Vector2 GetPosition(float t)
        {

            if (t >= 1)
                return _segments[_segments.Length - 1].PointC;

            if (t <= 0)
                return _segments[0].PointB;

            int segmentIndex = Mathf.FloorToInt(t * _segments.Length);

            float segmentT = Math2D.Remap01(t, segmentIndex / (float)_segments.Length,
                (segmentIndex+1) / (float)_segments.Length);

            return _segments[segmentIndex].GetPosition(segmentT);
        }      

        /// <summary>
        /// The length of the curve in world units.
        /// </summary>
        /// <param name="segment">higher values for accurate curve length.</param>
        /// <returns>The length of the curve in world units.</returns>
		public float GetLength ( int segment ){

			float length = 0;

			if ( _segments != null )
				for (int i = 0; i < _segments.Length; i++) {
					length += _segments [i].GetLength (segment);
				}

			return length;
		}

        /// <summary>
        /// Draws gizmo lines representing the curve.
        /// </summary>
        /// <param name="delta">value between 0 and 1, lower values 
        /// for more accurate drawing.</param>
        public void Draw ( float delta ){
			if ( _segments != null )
				for (int i = 0; i < _segments.Length; i++) {
					_segments[i].DrawSegment ( delta );
				}
		}
        /// <summary>
        /// Draws gizmo lines representing the curve.
        /// </summary>
        /// <param name="delta">values between 0 and 1, lower values 
        /// for more accurate drawings.</param>
        /// <param name="ZPosition">the position of the curve drawn on the Z-Axis.</param>
        public void Draw ( float delta , float z ){
			if ( _segments != null )
				for (int i = 0; i < _segments.Length; i++) {
					_segments[i].DrawSegment ( delta , z );
				}
		}

		#endregion

        /// <summary>
        /// Defines the deferent types of the curve segment.
        /// </summary>
	    public enum SegmentType
	    {
	        Start ,
	        End ,
	        StartAndEnd ,
	        Middle
	    } 

        /// <summary>
        /// the curve segment between two control point.
        /// </summary>
		class CurveSegment {

			#region public Members

            /// <summary>
            /// the tangent for the first control point.
            /// </summary>
			public Vector2 PointA;
            /// <summary>
            /// the first control point.
            /// </summary>
			public Vector2 PointB;
            /// <summary>
            /// the second control point.
            /// </summary>
			public Vector2 PointC;
            /// <summary>
            /// the tangent for the second control point.
            /// </summary>
			public Vector2 PointD;

            /// <summary>
            /// the smoothness of the segment, a value of zero will 
            /// make the curve segment take the shape of a line, 
            /// higher values to be more rounded.
            /// </summary>
			public float Smooth = 0.5f;

            /// <summary>
            /// defines the type of the segment,
            /// setting this value will auto calculate the average
            /// tangents if necessary. 
            /// </summary>
			public SegmentType Type {
				get {

					return _type;

				}
				set {

					_type = value;

					switch (_type) {
					case SegmentType.Start:
						{
							CalculatePointA ();
							break;
						}
					case SegmentType.End:
						{
							CalculatePointD ();
							break;
						}
					case SegmentType.StartAndEnd:
						{
							CalculatePointA ();
							CalculatePointD ();
							break;
						}
					}

				}
			}

            #endregion

            #region Private Members

            /// <summary>
            /// back-end field for Type.
            /// </summary>
            private SegmentType _type ;

            #endregion

            #region Constructors

            /// <summary>
            /// constructs a head and tail CurveSegment, for curves that contain only one segment.
            /// </summary>
            /// <param name="pointA">the tangent for the first control point.</param>
            /// <param name="pointB">the first control point.</param>
            /// <param name="pointC">the second control point.</param>
            /// <param name="pointD">the tangent for the second control point.</param>
            public CurveSegment ( Vector2 pointA , Vector2 pointB , Vector2 pointC , Vector2 pointD ){
				PointA = pointA;
				PointB = pointB;
				PointC = pointC;
				PointD = pointD;
				Type = SegmentType.Middle;
			}

            /// <summary>
            /// constructs a start or end CurveSegment.
            /// </summary>
			public CurveSegment ( Vector2 pointA , Vector2 pointB , Vector2 pointC , SegmentType type ){

				switch (type) {
				case SegmentType.Start:
					{
						PointB = pointA;
						PointC = pointB;
						PointD = pointC;
						Type = SegmentType.Start;
						break;
					}
				case SegmentType.End:
					{
						PointA = pointA;
						PointB = pointB;
						PointC = pointC;
						Type = SegmentType.End;
						break;
					}
				}
			}

            /// <summary>
            /// Constructs a middle CurveSegment.
            /// </summary>
            /// <param name="pointA">the first control point</param>
            /// <param name="pointB">the second control point</param>
			public CurveSegment ( Vector2 pointA , Vector2 pointB ){

				PointB = pointA;
				PointC = pointB;

				Type = SegmentType.StartAndEnd;

			}

			#endregion

			#region Public Methods

            /// <summary>
            /// calculates the tangent for the first point from
            /// the control point.
            /// </summary>
			public void CalculatePointA () {
				PointA = PointB + (PointB - PointC);
			}

            /// <summary>
            /// calculates the tangent for the second point from
            /// the control point.
            /// </summary>
			public void CalculatePointD () {
				PointD = PointC + (PointC - PointB);
			}

            /// <summary>
            /// The length of the segment curve in world units.
            /// </summary>
            /// <param name="segment">higher values for a more accurate curve segment length.</param>
            /// <returns>the length of the curve segment in world units.</returns>
			public float GetLength ( int segment ){

				float length = 0;
                float delta = 1 / segment;

				for (int i = 0; i < segment; i++) {

					Vector2 Start = GetPosition (i* delta);
					Vector2 End = GetPosition ((i + 1)*delta);
					length += (End - Start).magnitude;

				}

				return length;

			}

            /// <summary>
            /// Returns a position on the segment.
            /// </summary>
            /// <param name="t">value between 0 and 1, zero 
            /// will return the first control point position.</param>
            /// <returns>position on the segment.</returns>
			public Vector2 GetPosition ( float t ){

				//Caching
				float t2 = t * t;
				float t3 = t2 * t;

				float h1 =  2*t3  - 3*t2  + 1;          // calculates basis function 1
				float h2 = -2*t3 + 3*t2 ;              // calculates basis function 2
				float h3 =   t3 - 2*t2  + t;         // calculates basis function 3
				float h4 =   t3 -  t2 ;              // calculates basis function 4

				Vector2 T1 = (PointC-PointA)*Smooth;  //Defines Tangent
				Vector2 T2 = (PointD-PointB)*Smooth;  //Using Pre and Post Points

				return h1*PointB + h2*PointC + h3*T1 + h4*T2;
			}

            /// <summary>
            /// draws gizmo lines representing the curve segment.
            /// </summary>
            /// <param name="delta">values between 0 and 1, lower values 
            /// for more accurate drawings.</param>
			public void DrawSegment ( float delta ) {

				int Counter = Mathf.FloorToInt( 1 / delta);

				for (int i = 0; i < Counter; i++) {

					Gizmos.DrawLine( Math2D.Vector2ToVector3 ( GetPosition ( i*delta ) ) , Math2D.Vector2ToVector3 ( GetPosition ( (i+1)*delta ) ) );

				}

			}
            /// <summary>
            /// Draws gizmo lines representing the curve segment.
            /// </summary>
            /// <param name="delta">value between 0 and 1, a lower value 
            /// for a more accurate drawing.</param>
            /// <param name="ZPosition">the position of the curve segment drawn on the Z-Axis.</param>
			public void DrawSegment ( float delta , float ZPosition ) {

				int Counter = Mathf.FloorToInt( 1 / delta);

				for (int i = 0; i < Counter; i++) {

					Gizmos.DrawLine( Math2D.Vector2ToVector3 ( GetPosition ( i*delta ) , ZPosition ) , Math2D.Vector2ToVector3 ( GetPosition ( (i+1)*delta ) , ZPosition ) );

				}

			}

			#endregion

		}

	}

}