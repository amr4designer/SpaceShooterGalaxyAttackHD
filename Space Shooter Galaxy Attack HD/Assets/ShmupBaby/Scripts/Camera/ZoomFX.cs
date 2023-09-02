using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Adds zooming control for the Orthographic Camera.
    /// </summary>
	[RequireComponent(typeof(OrthographicCamera))]
    [AddComponentMenu("Shmup Baby/Camera/Zoom FX")]
    public sealed class ZoomFX : Singleton<ZoomFX>
    {
        /// <summary>
        /// Interpolation choices.
        /// </summary>
        public enum SpeedType
        {
            Linear,
            Accelerate
        };

        /// <summary>
        /// The max offset for the camera when it's set to zoom in.
        /// </summary>
        [Space]
        [Tooltip("Offset for the camera from its original size to give space for zooming.")]
		public float Offset ;

        /// <summary>
        /// The speed for zooming in.
        /// </summary>
        [Space]
        [Tooltip("The speed for zooming in.")]
        public float ZoomInSpeed ;
        
        /// <summary>
        /// The speed for zooming out.
        /// </summary>
        [Tooltip("The speed for zooming out.")]
        public float ZoomOutSpeed ;

        /// <summary>
        /// Interpolation choices for zooming in and out.
        /// </summary>
        [Space]
        [Tooltip("Interpolation choices for zooming in and out")]
        public SpeedType InterpolateType ;

        /// <summary>
        /// Controls zooming in and out.
        /// </summary>
		[HideInInspector]
		public bool Zooming = true ;

        /// <summary>
        /// This value between 0 and 1 represents if the camera is zooming in or out or in-between.
        /// </summary>
		private float _zoomPosition = 1f;
        /// <summary>
        /// The minimum camera size while its zoomed in to the maximum value.
        /// </summary>
		private float _minCameraSize ;
        /// <summary>
        /// The maximum camera size while its zoomed out to the maximum value.
        /// </summary>
		private float _maxCameraSize ;


        private static Camera MainCamera
        {
            get
            {
                return OrthographicCamera.Instance.MainCamera;
            }
        }


	    protected override void Awake()
	    {
	        base.Awake();
	        LevelInitializer.Instance.SubscribeToStage(4, SetCamera);
	    }


        private void Update()
        {
            //Adjusting the zoom position to use later as an interpolation coefficient 
            if (Zooming)
            {
                _zoomPosition += ZoomOutSpeed * Time.deltaTime;
            }
            else
            {
                _zoomPosition -= ZoomInSpeed * Time.deltaTime;
            }

            _zoomPosition = Mathf.Clamp01(_zoomPosition);

            switch (InterpolateType)
            {
                //Interpolates the camera size between the minimum and maximum sizes.
                case SpeedType.Linear:
                    MainCamera.orthographicSize = Mathf.Lerp(_minCameraSize, _maxCameraSize, _zoomPosition);
                    break;

                case SpeedType.Accelerate:
                    MainCamera.orthographicSize = Math2D.Accp(_minCameraSize, _maxCameraSize, _zoomPosition);
                    break;
            }

            OrthographicCamera.Instance.UpdateCameraRect();
        }


        /// <summary>
        /// Adjust the camera for zooming.
        /// </summary>
        private void SetCamera ( )
        {
			_maxCameraSize = MainCamera.orthographicSize;
			_minCameraSize = MainCamera.orthographicSize - Offset;

			MainCamera.orthographicSize = _minCameraSize;
		}

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            GizmosExtension.DrawRect(OrthographicCamera.Instance.CameraRect);
        }

#endif

    }

}