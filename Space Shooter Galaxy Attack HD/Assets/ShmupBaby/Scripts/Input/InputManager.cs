using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ShmupBaby
{
    /// <summary>
    /// used for multi-players.
    /// </summary>
    public enum PlayerID
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

    /// <summary>
    /// define the input from a player.
    /// </summary>
    public struct PlayerInput
    {
        /// <summary>
        /// player input direction.
        /// </summary>
        public Vector2 Direction;
        /// <summary>
        /// player input for fire button.
        /// </summary>
        public bool Fire;
        /// <summary>
        /// player input for ultimate weapon.
        /// </summary>
        public bool UltimateWeapon;
        /// <summary>
        /// player input for pause.
        /// </summary>
        public bool Pause;

        /// <summary>
        /// PlayerInput constructor
        /// </summary>
        /// <param name="direction">player input direction.</param>
        /// <param name="fire">player input for fire button.</param>
        /// <param name="ultimateWeapon">player input for ultimate weapon.</param>
        /// <param name="pause">player input for pause.</param>
        public PlayerInput(Vector2 direction, bool fire, bool ultimateWeapon, bool pause)
        {
            Direction = direction;
            Fire = fire;
            UltimateWeapon = ultimateWeapon;
            Pause = pause;
        }
    }

    /// <summary>
    /// Manages the player input for this pack.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Input/Input Manager")]
    public class InputManager : PersistentSingleton<InputManager>
    {
        /// <summary>
        /// the main event system for the game.
        /// </summary>
        [Tooltip("plug the event system associated with the input manager.")]
        public EventSystem eventSystem;
        /// <summary>
        /// the name of the Axis that control vertical movement of the player.
        /// </summary>
        [Space]
        [Tooltip("the name of the Axis that control vertical movement of the player," +
            "you can find the axis from Edit->Project Settings->Input.")]
        public string VerticalInputAxis = "Vertical";
        /// <summary>
        /// the name of the Axis that control horizontal movement of the player.
        /// </summary>
        [Tooltip("the name of the Axis that control horizontal movement of the player," +
            "you can find the axis from Edit->Project Settings->Input.")]
        public string HorizontalInputAxis = "Horizontal";
        /// <summary>
        /// the name of the Axis that control player fire.
        /// </summary>
        [Space]
        [Tooltip("the name of the Axis that control player fire," +
            "you can find the axis from Edit->Project Settings->Input.")]
        public string FireInputAxis = "Fire1";
        /// <summary>
        /// the name of the Axis that pause.
        /// </summary>
        [Space]
        [Tooltip("the name of the Axis that pause," +
            "you can find the axis from Edit->Project Settings->Input.")]
        public string PauseInputAxis = "Cancel";
        /// <summary>
        /// the name of the Axis that submit player input.
        /// </summary>
        [Tooltip("the name of the Axis that submit player input," +
            "you can find the axis from Edit->Project Settings->Input.")]
        public string SubmitInputAxis = "Submit";
        /// <summary>
        /// offset the touch position from the player position.
        /// </summary>
        [Space]
        [Tooltip("offset the touch position from the player position.")]
        public Vector2 TouchOffset;
        /// <summary>
        /// Threshold that prevents constant change at the slightest finger movement.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("This gives you a margin so that the player doesn't rotate " +
            "or change position instantly as your finger moves slightly.")]
        public float TouchThreshold = 0.1f;
        /// <summary>
        /// the main player current input.
        /// </summary>
        private PlayerInput _input;
        /// <summary>
        /// the joystick used for mobile controls.
        /// </summary>
        private JoyStick _joyStick;
        /// <summary>
        /// the fireButton used for mobile controls.
        /// </summary>
        private HoldButton _fireButton;
        /// <summary>
        /// indicate if the active scene is a level.
        /// </summary>
        private bool _activeSceneIsLevel;
        /// <summary>
        /// indicate if the player select autoFire.
        /// </summary>
        private bool _autoFire;


        /// <summary>
        /// Checks if the phone control UI element is needed or not.
        /// </summary>
        public bool RequiredPhoneControl
        {
            get
            {
#if (UNITY_ANDROID || UNITY_IOS)

                return _iMethod == InputMethod.Controls;

#endif
                return false;
            }
        }

        /// <summary>
        /// indicate if the player has selected autoFire.
        /// </summary>
        public static bool AutoFire
        {
            get
            {
                return Instance._autoFire;
            }
            set
            {
                Instance._autoFire = value;
            }
        }

        /// <summary>
        /// Indicates what kind of Input controls this player.
        /// </summary>
        private InputMethod _iMethod
        {
            get;
            set;
        }

        /// <summary>
        /// The player in the scene.
        /// </summary>
        private Player Player
        {
            get
            {
                return LevelController.Instance.PlayerComponent;
            }
        }


        protected override void Awake()
        {
            base.Awake();
            OnSceneChange(new Scene(), LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneChange;
        }

        private void Start()
        {
            if (eventSystem == null)
            {
                eventSystem = FindObjectOfType<EventSystem>();
            }
               
            if (eventSystem == null)
            {
                eventSystem = gameObject.AddComponent<EventSystem>();
            }                
        }

        void Update()
        {
            //make sure that the player inside a level.
            if (!LevelUIManager.IsInitialize)
            {
                return;
            }               

            _input.Direction = GetDirection();
            _input.Fire = GetFire();
            _input.Pause = GetPause();
        }


        private void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            if(LevelController.IsInitialize)
            {
                _activeSceneIsLevel = true;
            }
            else
            {
                _activeSceneIsLevel = false;
            }
        }

        /// <summary>
        /// return the player input.
        /// </summary>
        /// <param name="ID">The player id to return its input.</param>
        /// <returns>The player input for the current frame.</returns>
        public PlayerInput GetInput(PlayerID ID)
        {
            return _input;
        }

        /// <summary>
        /// the main event system for the game.
        /// </summary>
        public static void SetSelectedUI(GameObject selected)
        {
            if (Instance.eventSystem != null)
            {
                Instance.eventSystem.SetSelectedGameObject(selected);
            }
        }

        public static void SetInputMethod(InputMethod iMethod)
        {
            Instance._iMethod = iMethod;

#if (UNITY_ANDROID || UNITY_IOS)

            if (!Instance._activeSceneIsLevel)
                return;

            switch (iMethod)
            {
                case InputMethod.Controls:
                    LevelUIManager.Instance.SetActivePhoneControls(true,!AutoFire);
                    break;
                case InputMethod.FollowTouch:
                    LevelUIManager.Instance.SetActivePhoneControls(false,false);
                    break;
            }

#endif

        }

        /// <summary>
        /// get player input for pause.
        /// </summary>
        /// <returns>true if player pause.</returns>
        private bool GetPause()
        {
            if (Input.GetAxisRaw(PauseInputAxis) > 0.2f)
                return true;
            else
                return false;
        }

        /// <summary>
        /// get the player input for fire.
        /// </summary>
        /// <returns>true if the player fire.</returns>
        private bool GetFire ()
        {
            //Handles the weapons fire input, if the Player is set to autoFire we keep the weapons firing.
            if (_autoFire)
            {
                return true;
            }

#if ((UNITY_STANDALONE || UNITY_WEBGL) || UNITY_EDITOR)

            switch (_iMethod)
            {
                case InputMethod.Controls:
                    //test the FireInputAxis to check when it's time to fire.
                    if (Input.GetAxisRaw(FireInputAxis) > 0.2f)
                        return true;
                    else
                        return false;
                case InputMethod.FollowTouch:
                    return Input.GetMouseButton(0);
            }

#endif

#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)

            switch (_iMethod)
            {
                case InputMethod.Controls:
                    //get the fire input from the UI.
                    if (_fireButton == null)
                        _fireButton = LevelUIManager.Instance.GetFireButton();
                    if (_fireButton == null)
                    {
                        Debug.Log("you forget to assign a Fire Button in the level UI Manager.");
                        return false;
                    }
                    return _fireButton.Holding;
                case InputMethod.FollowTouch:
                    //get the fire input when the player hand touch the screen.
                    if (Input.touchCount > 0)
                    {
                        return true;
                    }
                    else
                        return false;
            }

#endif

            return false;           
        }

        /// <summary>
        /// get the player input for direction.
        /// </summary>
        /// <returns>the player input for direction</returns>
        private Vector2 GetDirection()
        {
            Vector2 direction = Vector2.zero;

#if ((UNITY_STANDALONE || UNITY_WEBGL) || UNITY_EDITOR)

            switch (_iMethod)
            {
                case InputMethod.FollowTouch:
                    //get input from the mouse position.
                    if (Camera.main == null || Player == null) 
                    return direction;
                    Vector3 mousePosition = Input.mousePosition;
                    
                    mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
                    mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);
                    mousePosition.z = 20;

                    Vector3 mouseCoord = Camera.main.ScreenToWorldPoint(mousePosition);

                    direction = new Vector2( (mouseCoord.x + TouchOffset.x) - Player.transform.position.x , (mouseCoord.y + TouchOffset.y) - Player.transform.position.y );
                    direction.x = Mathf.Abs(direction.x) < TouchThreshold ? 0 : direction.x;
                    direction.y = Mathf.Abs(direction.y) < TouchThreshold ? 0 : direction.y;
                    break;
                case InputMethod.Controls:
                    //get the input from HorizontalInputAxis and VerticalInputAxis.
                    direction = new Vector2(Input.GetAxis(HorizontalInputAxis), Input.GetAxis(VerticalInputAxis));
                    return direction;
            }
#endif

#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)

            switch (_iMethod)
            {
                case InputMethod.FollowTouch:
                    //get the direction from the player touch.
                    if (Input.touchCount > 0)
                    {
                        Vector3 touchCoord = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        direction = new Vector2((touchCoord.x + TouchOffset.x) - Player.transform.position.x, (touchCoord.y + TouchOffset.y) - Player.transform.position.y);
                        direction.x = Mathf.Abs(direction.x) < TouchThreshold ? 0 : direction.x;
                        direction.y = Mathf.Abs(direction.y) < TouchThreshold ? 0 : direction.y;
                    }
                    break;
                case InputMethod.Controls:
                    //get the input from the UI.
                    if (_joyStick == null)
                        _joyStick = LevelUIManager.Instance.GetJoyStick();
                    if (_joyStick == null)
                    {
                        Debug.Log("you forget to assign a joy stick in the level UI Manager.");
                        return direction;
                    }
                    direction = _joyStick.Direction;
                    break;
            }

#endif

            return direction;
        }

    }


}