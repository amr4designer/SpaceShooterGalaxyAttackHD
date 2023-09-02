using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ShmupBaby
{
    /// <summary>
    /// a list of all Player Select UI Events.
    /// </summary>
    public enum PlayerSelectUIEvents
    {
        OnSwitchPlayer,
        Accept,
        Back
    }

    /// <summary>
    /// Data structure which defines what to be passed when PlayerSelectUIEvent is triggered.
    /// </summary>
    public class PlayerSelectEventArg : ShmupEventArgs
    {
        public PlayerSelectUIEvents UIEvent;

        public PlayerSelectEventArg(PlayerSelectUIEvents uIEvent)
        {
            UIEvent = uIEvent;
        }
    }

    /// <summary>
    /// Controls all the UI Canvas in the Player Select Scene.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Player Select/Player Select UI Manager")]
    public class PlayerSelectUIManager : Singleton<PlayerSelectUIManager>
    {
        
        /// <summary>
        /// defines the player rating for a given property.
        /// </summary>
        [System.Serializable]
        public class PlayerProperty
        {
            /// <summary>
            /// the name of the property that needs to be rated.
            /// </summary>
            [Space]
            [Tooltip("the name of the property, you should define a property reference with the same name.")]
            public string PropertyName;
            /// <summary>
            /// the rating for the property.
            /// </summary>
            [Space]
            [Tooltip("the player rating for the property defined above.")]
            public int Rate;
        }

        /// <summary>
        /// data structure for the UI reference of a player property.
        /// </summary>
        [System.Serializable]
        public class ShipPropertyUIReferences
        {
            /// <summary>
            /// the name of the property.
            /// </summary>
            [Space]
            [Tooltip("the name of the property, this will be used later to set a player for a property.")]
            public string PropertyName;
            /// <summary>
            /// the UI element that pre-sets the rating for this property.
            /// </summary>
            [Space]
            [Tooltip("the rating UI Objects for the property in order.")]
            public RectTransform[] RatingMark;

            /// <summary>
            /// max rating possible for this property. 
            /// </summary>
            public int MaxRate
            {
                get { return RatingMark.Length -1; }
            }

            /// <summary>
            /// updates the UI elements to match the property rate. 
            /// </summary>
            /// <param name="rate">the rate for the property</param>
            public void UpdateRate(int rate)
            {
                if (MaxRate <= rate)
                    rate = MaxRate;

                //enables UI elements equal to the rating.
                for (int i = 0; i <= MaxRate; i++)
                {
                    if ( i+1 <= rate )
                        RatingMark[i].gameObject.SetActive(true);
                    else
                        RatingMark[i].gameObject.SetActive(false);
                }

            }

        }

        /// <summary>
        /// data structure for the player avatar.
        /// </summary>
        [System.Serializable]
        public class PlayerInfo
        {

            #if UNITY_EDITOR

            /// <summary>
            /// only available in the editor, for organizational purposes. 
            /// </summary>
            [Space]
            [Tooltip("only used for organize")]
            public string Name;

            #endif

            /// <summary>
            /// the visuals for the player avatar.
            /// </summary>
            [Space]
            [Tooltip("A prefab for a player prefab visual, this could be a sprite or mesh")]
            public GameObject Visual;
            /// <summary>
            /// prefab of the player avatar, for the vertical level.
            /// </summary>
            [Space]
            [Tooltip("Prefab of the player avatar, for the vertical level.")]
            public Object VerticalPlayerShip;
            /// <summary>
            /// prefab of the player avatar, for the horizontal level.
            /// </summary>
            [Space]
            [Tooltip("Prefab of the player avatar, for the horizontal level.")]
            public Object HorizontalPlayerShip;
            /// <summary>
            /// biography of the player avatar
            /// </summary>
            [Space]
            [Multiline(5)]
            [Tooltip("Biography for the player avatar")]
            public string BIO = "Type player ship BIO in here";
            /// <summary>
            /// the rate for the avatar property
            /// </summary>
            [Space]
            [Tooltip("List of the ratings for the player property")]
            public PlayerProperty[] PropertyList;

            /// <summary>
            /// updates the UI element to match this avatar.
            /// </summary>
            /// <param name="bio">the UI element that displays the BIO text</param>
            /// <param name="ratingUI">reference to the UI elements</param>
            public void UpdateUI(Text bio, ShipPropertyUIReferences[] ratingUI)
            {
                bio.text = BIO;

                //finds the property in the reference and updates it.
                for (int i = 0; i < PropertyList.Length; i++)
                {
                    ShipPropertyUIReferences propertyUIReference =
                        ratingUI.First(x => x.PropertyName == PropertyList[i].PropertyName);

                    if (propertyUIReference != null)
                    {
                        propertyUIReference.UpdateRate(PropertyList[i].Rate);
                    }
                }

            }

        }

        /// <summary>
        /// the UI elements that represent the Player ratings
        /// </summary>
        [Header("UI Reference")]
        [Space]
        [Tooltip("Reference to the UI Objects that represent the Player ratings")]
        public ShipPropertyUIReferences[] Ratings;
        /// <summary>
        /// The UI element that represents the player BIO.
        /// </summary>
        [Tooltip("Reference to the UI Object that represent the player BIO")]
        public Text PlayerBio;

        /// <summary>
        /// Accept button UI element.
        /// </summary>
        [Space]
        [Tooltip("drag the accept button, that confirms the player selection.")]
        public Button AcceptButton;
        /// <summary>
        /// Back button UI element.
        /// </summary>
        [Tooltip("drag the back button, which returns to the previous scene.")]
        public Button BackButton;

        /// <summary>
        /// the next button UI element.
        /// </summary>
        [Space]
        [Tooltip("drag the next button, which navigates to the next player avatar")]
        public Button NextPlayerButton;
        /// <summary>
        /// the previous button UI element.
        /// </summary>
        [Tooltip("drag the previous button, which navigates to the previous player avatar")]
        public Button PreviousPlayerButton;

        /// <summary>
        /// player avatar and the info for the UI display.
        /// </summary>
        [Header("Player Ships Reference")]
        [Space]
        [Tooltip("a list of the player avatar and their properties")]
        public PlayerInfo[] Player;

        /// <summary>
        /// The distance of the turntable objects from the origin point.
        /// </summary>
        [Header("Turntable Settings")]
        [Space]
        [Tooltip("The distance of the turntable objects from the origin point")]
        public float DistanceFromOrigin;
        /// <summary>
        /// The length of the turntable in degrees.
        /// </summary>
        [Tooltip("the length of the turntable in degrees")]
        [HideInInspector]
        public float ArcLength = 360f;
        /// <summary>
        /// the time to navigate to the next element in the turntable.
        /// </summary>
        [Tooltip("the time to navigate to the next element in the turnable")]
        public float RotationTime = 1f;
        /// <summary>
        /// avatar visual offset angle on the y axis.
        /// </summary>
        [Tooltip("avatar visual offset angle on the Y axis")]
        public float AvatarOffsetAngle;
        /// <summary>
        /// the rotation speed of the avatar visual.
        /// </summary>
        [Tooltip("the rotation speed of the avatar visual")]
        public float RotationSpeed;

        #if UNITY_EDITOR

        /// <summary>
        /// only available in Unity's editor, draw a gizmo for the first avatar position".
        /// </summary>
        [Tooltip("draw a gizmo for the first avatar position")]
        [Space]
        public bool DrawFirstAvatarPosition;
        /// <summary>
        /// Only available in Unity's editor, draw a gizmo for the turntable path.
        /// </summary>
        [Tooltip("draws a gizmo for the turntable path")]
        public bool DrawTurntablePath;

        #endif

        /// <summary>
        /// trigger when one of the UI events Occur.
        /// </summary>
        public static event ShmupDelegate OnUIEvent;

        /// <summary>
        /// the ID of the selected avatar, setting this will update the UI.
        /// </summary>
        public int AvatarIndex
        {
            get
            {
                return _avatarIndex;
            }
            set
            {
                if (_avatarIndex != value)
                {
                    _avatarIndex = value;
                    UpdateUI();
                }
            }
        }

        /// <summary>
        /// back-end field for the AvatarID.
        /// </summary>
        public int _avatarIndex;
        /// <summary>
        /// the avatar visual in the scene.
        /// </summary>
        public GameObject[] _visuals;
        /// <summary>
        /// the parent of the avatar visual in the scene.
        /// </summary>
        public GameObject _visualParent;
        /// <summary>
        /// indicates if the turntable is in rotation.
        /// </summary>
        public bool _isRotate;
        /// <summary>
        /// the angle between the visual in the scene and the XY plane.
        /// </summary>
        public float _stepAngle;
        /// <summary>
        /// the rotation amount of the visual in the scene.
        /// </summary>
        public float _shipsRotation;

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
            
            InitializeUI();

            InitializeTurnTable();

            UpdateUI();

            InputManager.SetSelectedUI(BackButton.gameObject);
        }

        /// <summary>
        /// Initializes the turntable from the player avatar visual.
        /// </summary>
        void InitializeTurnTable()
        {
            
            _visuals = new GameObject[Player.Length];

            _stepAngle = GetStepAngle();

            _visualParent = new GameObject("Ships To Pick");

            CreateVisual(_visuals, _visualParent.transform);
            PositionVisuals(_visuals,_stepAngle);

            _shipsRotation = AvatarOffsetAngle;
            _avatarIndex = 0;
        }

        /// <summary>
        /// the angle between the avatar visuals in degrees.
        /// </summary>
        public float GetStepAngle()
        {
            int shipsNum = Player.Length;

            if (shipsNum != 1)
            {
                float stepAngle;

                if (Mathf.Abs(ArcLength % 360f) <= 0.1f)
                {
                    stepAngle = ArcLength / shipsNum;
                }
                else
                {
                    stepAngle = ArcLength / (shipsNum - 1);
                }
                return stepAngle * Mathf.Deg2Rad;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Creates the visuals for the player avatars.
        /// </summary>
        /// <param name="visuals">The visuals prefabs.</param>
        /// <param name="visualParent">A transform to parent the visuals underneath it.</param>
        public void CreateVisual(GameObject[] visuals, Transform visualParent)
        {
            for (int i = 0; i < visuals.Length; i++)
            {
                if (Player[i].Visual != null)
                    visuals[i] = (GameObject)Instantiate(Player[i].Visual, visualParent);
            }
        }

        /// <summary>
        /// Positions the visuals to their place on the turntable.
        /// </summary>
        /// <param name="visuals">The visuals of the avatars.</param>
        /// <param name="stepAngle">The angle in degrees between the visuals.</param>
        public void PositionVisuals(GameObject[] visuals, float stepAngle)
        {
            for (int i = 0; i < visuals.Length; i++)
            {
                if (visuals[i] != null)
                    visuals[i].transform.position = new Vector3(
                        Mathf.Cos(i * stepAngle + Mathf.PI * 0.5f) * DistanceFromOrigin,
                        Mathf.Sin(i * stepAngle + Mathf.PI * 0.5f) * DistanceFromOrigin,
                        0);
            }

        }

        /// <summary>
        /// Initializes the UI elements.
        /// </summary>
        void InitializeUI()
        {
            if (AcceptButton != null)
                AcceptButton.onClick.AddListener(Accept);

            if (BackButton != null)
                BackButton.onClick.AddListener(Back);

            if (NextPlayerButton != null)
                NextPlayerButton.onClick.AddListener(NextAvatar);

            if (PreviousPlayerButton != null)
                PreviousPlayerButton.onClick.AddListener(PreviousAvatar);
        }

        /// <summary>
        /// Assigns the selected player to the GameManager, and returns to the Main Scene.
        /// </summary>
        public void Accept()
        {

            RiseOnUIEvent(PlayerSelectUIEvents.Accept);

            Object playerSelectedShipV = Player[AvatarIndex].VerticalPlayerShip;
            Object playerSelectedShipH = Player[AvatarIndex].HorizontalPlayerShip;

            PlayerObject ship = new PlayerObject
            {
                Vertical = playerSelectedShipV,
                Horizontal = playerSelectedShipH
            };

            GameManager.Instance.AssignSelectedShip(ship);

            if (GameManager.Instance.NewGame)
            {
                GameManager.Instance.NewGame = false;
                GameManager.Instance.ToFirstLevel();
            }
            else
                GameManager.Instance.ToMainScene();

        }

        /// <summary>
        /// Returns to the Main Menu.
        /// </summary>
        public void Back()
        {

            RiseOnUIEvent(PlayerSelectUIEvents.Back);

            GameManager.Instance.ToMainScene();

        }

        /// <summary>
        /// updates the UI element with the avatar info.
        /// </summary>
        public void UpdateUI()
        {

            Player[AvatarIndex].UpdateUI(PlayerBio,Ratings);

        }

        /// <summary>
        /// one of Unity's messages that gets called every frame.
        /// </summary>
        void Update()
        {
            //rotates visual around the y axis using the rotation speed
            _shipsRotation += RotationSpeed * Time.deltaTime;

            for (int i = 0; i < _visuals.Length; i++)
            {
                _visuals[i].transform.rotation = Quaternion.identity;
                _visuals[i].transform.RotateAround(_visuals[i].transform.position,Vector3.up,_shipsRotation);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextAvatar();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousAvatar();
            }

        }

        /// <summary>
        /// checks if to the next avatar is available, to rotate the turntable
        /// and update the selected avatar index.
        /// </summary>
        public void NextAvatar()
        {
            //Checks if the switch is available.
            if (_isRotate || (AvatarIndex + 1) >= _visuals.Length)
                return;

            //Inverts the stepAngle to invert the turntable rotation.
            if (_stepAngle > 0)
                _stepAngle = -_stepAngle;

            StartCoroutine(Rotate());

            RiseOnUIEvent(PlayerSelectUIEvents.OnSwitchPlayer);

            AvatarIndex++;
        }

        /// <summary>
        /// checks if to the previous avatar is available, to rotate the turntable.
        /// and update the selected avatar index.
        /// </summary>
        public void PreviousAvatar()
        {
            //Checks if the switch is available.
            if (_isRotate || (AvatarIndex - 1) < 0)
                return;

            //Inverts the stepAngle to invert the turn table rotation.
            if (_stepAngle < 0)
                _stepAngle = -_stepAngle;

            StartCoroutine(Rotate());

            RiseOnUIEvent(PlayerSelectUIEvents.OnSwitchPlayer);

            AvatarIndex--;
        }

        /// <summary>
        /// Rotates the turn table for the step angle in RotationTime.
        /// </summary>
        IEnumerator Rotate()
        {
            //Changes the is rotate flag so there will be only one rotation at a time.
            _isRotate = true;

            //The fps for the turntable rotation.
            const float rotationFps = 30;

            //number of frames for the whole rotation.
            float frameNum = RotationTime * rotationFps;
            //time between rotation frames.
            float waiting = 1 / rotationFps;

            for (int i = 0; i < frameNum; i++)
            {
                //rotates the visual parent
                _visualParent.transform.Rotate(new Vector3(0,0, (_stepAngle * Mathf.Rad2Deg) / frameNum));
                yield return new WaitForSeconds(waiting);

            }

            _isRotate = false;

            yield return null;
        }

        /// <summary>
        /// handles the rise of the OnUIEvent.
        /// </summary>
        /// <param name="uiEvent">the type of UI event that raises this event.</param>
        public void RiseOnUIEvent(PlayerSelectUIEvents uiEvent)
        {
            if (OnUIEvent != null)
                OnUIEvent(new PlayerSelectEventArg(uiEvent));
        }

    }

}