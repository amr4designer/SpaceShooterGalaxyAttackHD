using UnityEngine;
using UnityEditor;
using ShmupBaby;

namespace ShmupEditor
{

    /// <summary>
    /// draw gizmo for PlayerSelectUIManager.
    /// </summary>
    [CustomEditor(typeof(PlayerSelectUIManager))]
    public class PlayerSelectUIManagerEditor : Editor
    {
        /// <summary>
        /// the PlayerSelectUIManager for this editor.
        /// </summary>
        private PlayerSelectUIManager Component
        {
            get { return (PlayerSelectUIManager)target; }
        }

        //Serialized property to PlayerSelectUIManager mover fields
        private SerializedProperty DrawFirstAvatarPosition;
        private SerializedProperty DrawTurnTablePath;
        private SerializedProperty DistanceFromOrigin;
        private SerializedProperty ArcLength;
        private SerializedProperty AvatarOffsetAngle;

        /// <summary>
        /// the avatar visuals currently in the scene, instantiated by this editor.
        /// </summary>
        private GameObject[] _avatars;
        /// <summary>
        /// the parent object for the avatars.
        /// </summary>
        private Transform _parent;

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {
            DrawFirstAvatarPosition = serializedObject.FindProperty("DrawFirstAvatarPosition");
            DrawTurnTablePath = serializedObject.FindProperty("DrawTurntablePath");

            DistanceFromOrigin = serializedObject.FindProperty("DistanceFromOrigin");
            ArcLength = serializedObject.FindProperty("ArcLength");
            AvatarOffsetAngle = serializedObject.FindProperty("AvatarOffsetAngle");

            #if UNITY_2017_2_OR_NEWER

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += Update;

                //instantiates the avatar visuals in the scene.
                _parent = (new GameObject("Ships")).transform;
                _avatars = new GameObject[Component.Player.Length];
                Component.CreateVisual(_avatars, _parent);
            }

            EditorApplication.playModeStateChanged += DestroyShips;
                       
            #endif
        }

        /// <summary>
        /// This function is called when the object is unloaded.
        /// </summary>
        void OnDisable()
        {


            #if UNITY_2017_2_OR_NEWER

            EditorApplication.update -= Update;

            EditorApplication.playModeStateChanged -= DestroyShips;

            DestroyShips(PlayModeStateChange.EnteredEditMode);

            #endif

        }

        #if UNITY_2017_2_OR_NEWER

        /// <summary>
        /// destroys the _avatars.
        /// </summary>
        /// <param name="state">the play mode state for this editor.</param>
        private void DestroyShips(PlayModeStateChange state)
        {
            if (_avatars == null)
                return;

            if (_parent != null)
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                    DestroyImmediate(_parent.gameObject);
                if (state == PlayModeStateChange.EnteredEditMode)
                    DestroyImmediate(_parent.gameObject);
            }

            _avatars = null;

        }

        #endif

        /// <summary>
        /// called every frame in the editor.
        /// </summary>
        void Update()
        {
            if (_avatars == null)
                return;

            //updates the rotation and position of the avatars visuals in the scene.
            for (int i = 0; i < _avatars.Length; i++)
            {
                if (_avatars[i] == null)
                    continue;

                _avatars[i].transform.eulerAngles = new Vector3(0, AvatarOffsetAngle.floatValue, 0);

            }

            Component.PositionVisuals(_avatars, Component.GetStepAngle());
        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        void OnSceneGUI()
        {

            //draws a gizmo for the turntable path.
            if (DrawTurnTablePath.boolValue)
            {
                Handles.color = Color.yellow;
                Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, 360f, DistanceFromOrigin.floatValue);
                Handles.color = Color.white;
                Handles.DrawWireArc(Vector3.zero, Vector3.forward, Vector3.up, ArcLength.floatValue - 360f, DistanceFromOrigin.floatValue);
            }

            //draws a gizmo for the first avatar position.
            if (DrawFirstAvatarPosition.boolValue)
            {
                Handles.color = new Color(0, 1f, 0, 0.2f);
                Handles.DrawSolidDisc(Vector3.up * DistanceFromOrigin.floatValue, Vector3.forward, 1f);
            }
        }

    }

}
