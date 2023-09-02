using UnityEngine;

namespace ShmupBaby
{       
    /// <summary>
    /// Sample positions and distances between positions on a 2D Curve.
    /// </summary>
    [System.Serializable]
    public class Curve2DSampler
    {
        public delegate Vector2 PositionOn2DCurve(float t);

        /// <summary>
        /// Data structure for a curve sample.
        /// </summary>
        [System.Serializable]
        public class CurveSample
        {
            /// <summary>
            /// the sample position.
            /// </summary>
            [Space]
            public Vector2 Point;
            /// <summary>
            /// the curve length from the start point to this sample.
            /// </summary>
            [Space]
            public float Distance;

            /// <summary>
            /// CurveSample constructor.
            /// </summary>
            /// <param name="point">the sample position.</param>
            /// <param name="distance">the curve length from the start point to this sample.</param>
            public CurveSample(Vector2 point, float distance)
            {
                Point = point;
                Distance = distance;
            }

        }

        /// <summary>
        /// the samples for the curve.
        /// </summary>
        public CurveSample[] Samples
        {
            get;
            private set;
        }

        /// <summary>
        /// Curve2DSampler constructor.
        /// </summary>
        /// <param name="curve">the curve to sample.</param>
        /// <param name="sampleNum">the number of samples generated from the curve.</param>
        public Curve2DSampler (ICurve2D curve, int sampleNum)
        {
            SampleCurve(curve, sampleNum);
        }

        /// <summary>
        /// populates the sample field based on a given curve.
        /// </summary>
        /// <param name="curve">The curve to sample.</param>
        /// <param name="sampleNum">The number of samples generated from the curve.</param>
        public void SampleCurve(ICurve2D curve, int sampleNum)
        {
            Samples = new CurveSample[sampleNum];

            float distance = 0;
            Vector2 prePoint = curve.GetPosition(0);

            float step = 1f / (sampleNum - 1);

            Samples[0] = new CurveSample(prePoint, 0);

            for (int i = 1; i < sampleNum; i++)
            {
                Vector2 currentPoint = curve.GetPosition(i * step);

                distance += (prePoint - currentPoint).magnitude;

                Samples[i] = new CurveSample(currentPoint, distance);

                prePoint = currentPoint;
            }

        }

        /// <summary>
        /// Draws gizmo lines representing the curve based on samples.
        /// </summary>
        /// <param name="z">the position of the curve drawn on the Z-Axis.</param>
        public void DrawSampleCurve(float z)
        {
            if (Samples.Length < 1)
                return;

            for (int i = 1; i < Samples.Length; i++)
            {
                Gizmos.DrawLine( Math2D.Vector2ToVector3( Samples[i - 1].Point , z ), Math2D.Vector2ToVector3(Samples[i].Point,z));
            }
        }
        /// <summary>
        /// Draws a gizmo sphere representing the samples position.
        /// </summary>
        /// <param name="z">The position of the curve drawn on the Z-Axis.</param>
        public void DrawSamplePoints(float pointRadius , float z)
        {
            if (Samples.Length < 1)
                return;

            for (int i = 0; i < Samples.Length; i++)
            {
                Gizmos.DrawSphere(Math2D.Vector2ToVector3(Samples[i].Point , z ), pointRadius);
            }
        }

    }

    /// <summary>
    /// Manages the position and tangent sampling for
    /// curve samples.
    /// </summary>
    public class Curve2DSamplerManger
    {
        /// <summary>
        /// the current position on the curve in world space.
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// the current tangent on the curve in world space.
        /// </summary>
        public Vector2 Tangent;
        /// <summary>
        /// the distance passed relative to the curve length.
        /// </summary>
        public float Distance;
        
        /// <summary>
        /// The curve length in world units.
        /// </summary>
        public float CurveLength
        {
            get { return _samples[_samples.Length - 1].Distance; }
        }
        
        /// <summary>
        /// The curve samples.
        /// </summary>
        private Curve2DSampler.CurveSample[] _samples
        {
            get
            {
                if (_sampler != null)
                    return _sampler.Samples;
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Represents the percentage value for the current position between the closest curve sample.
        /// </summary>
        private float _t;
        /// <summary>
        /// The index of the previous sample.
        /// </summary>
        private int _sampleIndex;
        /// <summary>
        /// The sampler which contains curve samples.
        /// </summary>
        private Curve2DSampler _sampler;

        /// <summary>
        /// Curve2DSamplerManger constructor.
        /// </summary>
        /// <param name="sampler">sampler that contains the curve sample.</param>
        public Curve2DSamplerManger(Curve2DSampler sampler)
        {
            _sampler = sampler;
        }

        /// <summary>
        /// Updates the fields by moving the distance forward on the curve.
        /// </summary>
        /// <param name="deltaDistance">Change of position on the curve.</param>
        public void SampleForward(float deltaDistance)
        {
            //updates the distance
            Distance += deltaDistance;

            float sampleDistance = _samples[_sampleIndex + 1].Distance;

            //Checks at what segment the next position will be.
            if (_sampleIndex + 2 < _samples.Length)
            {

                while (Distance > sampleDistance)
                {
                    _sampleIndex++;
                    sampleDistance = _samples[_sampleIndex + 1].Distance;
                }

            }

            float distanceBetweenSamples = _samples[_sampleIndex + 1].Distance - _samples[_sampleIndex].Distance;

            float deltaDistanceBetweenSamples = Distance - _samples[_sampleIndex].Distance;

            _t = deltaDistanceBetweenSamples / distanceBetweenSamples;

            //updates the tangent for the curve
            UpdateTangentForward();

        }

        /// <summary>
        /// update the tangent by forward sampling.
        /// </summary>
        private void UpdateTangentForward()
        {
            Vector2 tangetForward;
            Vector2 tangetBackward;

            tangetForward = (_samples[_sampleIndex + 1].Point - _samples[_sampleIndex].Point).normalized;
            
            if (_sampleIndex != 0)
            {
                tangetBackward = (_samples[_sampleIndex].Point - _samples[_sampleIndex - 1].Point).normalized;
            }
            else
            {
                tangetBackward = tangetForward;
            }                

            Tangent = Vector2.Lerp(tangetBackward, tangetForward, _t);
        }

        /// <summary>
        /// updates the fields by moving the distance backward on the curve.
        /// </summary>
        /// <param name="deltaDistance">change of position on the curve.</param>
        public void SampleBackward(float deltaDistance)
        {
            //updates the distance
            Distance -= deltaDistance;

            float sampleDistance = _samples[_sampleIndex].Distance;

            //Checks at what segment the next position will be.
            if (_sampleIndex >= 1)
            {

                while (Distance < sampleDistance)
                {
                    _sampleIndex--;
                    sampleDistance = _samples[_sampleIndex].Distance;
                }

            }

            float distanceBetweenSamples = _samples[_sampleIndex + 1].Distance - _samples[_sampleIndex].Distance;

            float deltaDistanceBetweenSamples = Distance - _samples[_sampleIndex].Distance;

            _t = 1 - deltaDistanceBetweenSamples / distanceBetweenSamples;

            //updates the tangent for the curve
            UpdateTangentBackWard();
            
        }

        /// <summary>
        /// updates the tangent by backward sampling.
        /// </summary>
        private void UpdateTangentBackWard()
        {
            Vector2 tangetForward;
            Vector2 tangetBackward;

            tangetBackward = ( _samples[_sampleIndex].Point - _samples[_sampleIndex + 1].Point).normalized;

            if (_sampleIndex <= 1)
            {
                tangetForward = tangetBackward;
            }

            else
            {
                tangetForward = (_samples[_sampleIndex - 1].Point - _samples[_sampleIndex].Point).normalized;
            }
                
            Tangent = Vector2.Lerp(tangetBackward, tangetForward, _t);
        }

        /// <summary>
        /// draws gizmo lines between the samples position.
        /// </summary>
        /// <param name="z">The position of the curve drawn on the Z-Axis.</param>
        /// <param name="color">the color of gizmos</param>
        public void DrawSampleCurve(Color color, float z)
        {

            if (_sampler == null)
                return;

            Gizmos.color = color;

            _sampler.DrawSampleCurve(z);

        }
        /// <summary>
        /// Draws a gizmo sphere representing the samples position.
        /// </summary>
        /// <param name="z">The position of the curve drawn on the Z-Axis.</param>
        /// <param name="color">The color of gizmos</param>
        public void DrawSamplePoints(Color color, float radius, float z)
        {
            if (_sampler == null)
                return;

            Gizmos.color = color;

            _sampler.DrawSamplePoints(0.125f, z);
        }

    }

}