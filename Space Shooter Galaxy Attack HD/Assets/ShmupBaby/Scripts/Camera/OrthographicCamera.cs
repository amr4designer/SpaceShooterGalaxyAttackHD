using System;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Used to define if this level will be horizontal or vertical.
    /// </summary>
    public enum LevelViewType
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// Orthographic Camera forces the camera to be in orthographic mode,
    /// and extend the camera functionality. 
    /// </summary>
	[AddComponentMenu("Shmup Baby/Camera/Orthographic Camera")]
	[RequireComponent(typeof(Camera))]
	[SelectionBase]
	public sealed class OrthographicCamera : Singleton<OrthographicCamera>
    {
        // Only used by the OrthographicCamera script to define the area that the camera should focus on.
        public enum CameraSetOption
        {
            MainBackground,
            Input,
            None
        };

        // Only used by the OrthographicCamera script when View is set to vertical, to force the screen to vertical aspect
        public enum VerticalAspect
        {
            None,
            Aspect3By4,
            Aspect4By5,
            Aspect9By16,
            Aspect10By16
        };

        /// <summary>
		/// Specifies the view type for this Scene.
        /// </summary>
        [Space]
		[Tooltip("Specifies the view type for this level")]
        public LevelViewType View;

        /// <summary>
        /// Main camera of the scene.
        /// </summary>
        [Space]
        [Tooltip("Optional reference to the camera, if it's not set, the script will search this gameObject for the camera component")]
        public Camera MainCamera;

        /// <summary>
        /// A rectangle shape to adjust the camera according to it.
        /// </summary>
        [HelpBox("Adjusting the camera to a given rectangle, by default it's set to MainBackground that mean it will take the field that's been defined " +
			"by the BackgroundController to adjust the camera, you can also adjust the camera to custom field or leave it as it is.")]
        public CameraSetOption CameraFieldOption;

        /// <summary>
        /// The rectangle that the camera will be adjusted to if the Camera Field Option is set to input.
        /// </summary>
        [Space]
        [Tooltip("This field will only be used if Camera Field Option is set to Input")]
        [RectHandle( "Camera Field" , 2f , BasicColors.Blue , BasicColors.Green )]
        public Rect CameraField;

        /// <summary>
        /// In case the View is set to Vertical, this Aspect will be used to define the aspect of the camera.
        /// </summary>
        [HelpBox("If you are building a vertical game for Desktop, you need to set the aspect ratio here." +
                " But if you are building for mobile then select none.")]
        public VerticalAspect Aspect;

        /// <summary>
        /// This rectangle defines what the camera sees, make sure to call UpdateCameraRect before getting  CameraRect.
        /// </summary>
		public Rect CameraRect
        {
            get;
            protected set;
        }

        /// <summary>
        /// The view type for this level define by camera.
        /// </summary>
		public LevelViewType ViewType
        {
            get
            {
                return View;
            }
        }

        public float FinalAspect
        {
            get
            {
                return AspectToFloat(Aspect);
            }
        }


        protected override void Awake ()
        {
			base.Awake ();           
		    LevelInitializer.Instance.SubscribeToStage(2, InitializeCamera);
        }


        private void InitializeCamera()
        {           
            //gets a reference to the camera.
            if (MainCamera == null)
            {
                MainCamera = GetComponent<Camera>();
            }
                
            //Sets the camera to orthographic view.
            MainCamera.orthographic = true;

            UpdateCameraRect();

            #if !UNITY_IOS && !UNITY_ANDROID

            //call ForceVertical only if View is set to vertical and the platform isn't IOS or Android
            if (View == LevelViewType.Vertical && Aspect != VerticalAspect.None)
                ForceVertical(AspectToFloat(Aspect));
#else
            
            //if the device is a portable device, the Screen rotation should be locked to Level View Type

            if (View == LevelViewType.Vertical)
		    {
                Screen.orientation = ScreenOrientation.Portrait;
		        Screen.autorotateToLandscapeLeft = false;
		        Screen.autorotateToLandscapeRight = false;
		        Screen.autorotateToPortrait = true;
		        Screen.autorotateToPortraitUpsideDown = true;
		    }

		    if (View == LevelViewType.Horizontal)
		    {
                Screen.orientation = ScreenOrientation.Landscape;
		        Screen.autorotateToLandscapeLeft = true;
		        Screen.autorotateToLandscapeRight = true;
		        Screen.autorotateToPortrait = false;
		        Screen.autorotateToPortraitUpsideDown = false;
		    }

#endif

            switch (CameraFieldOption)
            {
                case CameraSetOption.MainBackground:
                    /// Set the camera view to the main background rect
                    AdjustCameraToRect(BackgroundController.Instance.BackgroundRect);
                    break;

                case CameraSetOption.Input:
                    //Adjust camera view to CameraField rect
                    AdjustCameraToRect(CameraField);
                    break;
            }

            UpdateCameraRect();
        }

        
        /// <summary>
        /// Updates the Camera rectangle to what the camera is currently seeing. 
        /// </summary>
		public void UpdateCameraRect ()
		{
            CameraRect = new Rect(0, 0, Instance.MainCamera.orthographicSize * 2 * Instance.MainCamera.aspect,
                Instance.MainCamera.orthographicSize * 2)
                {
                    center = new Vector2(Instance.transform.position.x, Instance.transform.position.y)
                };

        }


        /// <summary>
        /// Adjusts the camera position and size for a given rectangle.
        /// </summary>
        private void AdjustCameraToRect(Rect rect)
        {
            if (View == LevelViewType.Horizontal)
            {
                MainCamera.orthographicSize = rect.height * 0.5f;
				MainCamera.transform.position = new Vector3
                    ((MainCamera.orthographicSize * MainCamera.aspect) + rect.xMin, rect.center.y,
                    -LevelController.Instance.SpaceBetween*3f);
            }
            else
            {
                MainCamera.orthographicSize = (1f / MainCamera.aspect) * rect.width * 0.5f;
				MainCamera.transform.position = new Vector3
                    (rect.center.x, MainCamera.orthographicSize + rect.yMin, 
                    -LevelController.Instance.SpaceBetween*3f);
            }
            MainCamera.farClipPlane = LevelController.Instance.SpaceBetween * (105);
        }


        /// <summary>
        /// Converts VerticalAspect enum to float value that represent the value
        /// </summary>
        private float AspectToFloat ( VerticalAspect aspect )
        {
                switch (aspect)
                {
                    case VerticalAspect.Aspect3By4:
                        return 3f / 4f;

                    case VerticalAspect.Aspect4By5:
                        return 4f / 5f;

                    case VerticalAspect.Aspect9By16:
                        return 9f / 16f;

                    case VerticalAspect.Aspect10By16:
                        return 10f / 16f;

                    default:
                        return 0;
                }
        }

        /// <summary>
        /// Forces the screen to a certain vertical aspect.
        /// </summary>
        private void ForceVertical(float newAspect)
        {
            float oldAspect = MainCamera.aspect;

            float ratio = (1f / oldAspect) * newAspect;

            Rect cameraRect = MainCamera.rect;

            cameraRect.width = ratio;

            cameraRect.position = new Vector2(0.5f - cameraRect.width * 0.5f, 0);

            MainCamera.rect = cameraRect;
        }

        /// <summary>
        /// return the unused width of a given canvas when the force aspect apply.
        /// </summary>
        /// <param name="Canvas"></param>
        public float GetUnusedSpace(RectTransform Canvas)
        {
            float usedSpace = Canvas.rect.height * FinalAspect;
            return Canvas.rect.width - usedSpace;
        }

        /// <summary>
        /// Adjusts RectTransform which is set to screen space to camera aspect.
        /// </summary>
        /// <param name="target">the target RectTransform</param>
        /// <param name="unusedWidth">the unused width</param>
        public void AdujstCanvasToForceAspect(RectTransform target, float unusedWidth)
        {
            Vector2 offsetMin = target.offsetMin;
            Vector2 offsetMax = target.offsetMax;

            offsetMin.x += unusedWidth * 0.5f;
            offsetMax.x -= unusedWidth * 0.5f;

            target.offsetMin = offsetMin;
            target.offsetMax = offsetMax;
        }

    }

}
