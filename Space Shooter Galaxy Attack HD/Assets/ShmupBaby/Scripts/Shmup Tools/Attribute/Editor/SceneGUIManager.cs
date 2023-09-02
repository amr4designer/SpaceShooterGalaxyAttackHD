using UnityEditor;

namespace ShmupEditor
{

    /// <summary>
    /// draw one handle at time at the scene view.
    /// </summary>
    public class SceneHandleManager : Editor
    {
        /// <summary>
        /// the current subscribe object.
        /// </summary>
        private static IDrawHandle _subscriber;

        /// <summary>
        /// add a object to draw it's handle in the scene view.
        /// </summary>
        /// <param name="thisObject"></param>
        public static void AddToSceneGUI(IDrawHandle thisObject)
        {

            if (!EditorApplication.isPlaying)
                _subscriber = thisObject;

        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        /// <param name="sceneView">the scene view.</param>
        private static void OnSceneGUI(SceneView sceneView)
        {

            if (_subscriber != null)
                _subscriber.Draw(sceneView);

        }

        /// <summary>
        /// static constructor for SceneHandleManager.
        /// </summary>
        static SceneHandleManager()
        {

            Selection.selectionChanged += OnSelectionChanged;

            #if UNITY_2017_2_OR_NEWER

            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            #else

            EditorApplication.playmodeStateChanged += OnPlayModeChanged;

            #endif

            SceneView.onSceneGUIDelegate += OnSceneGUI;

        }

        /// <summary>
        /// called when the scene change.
        /// </summary>
        static void OnSelectionChanged()
        {
            _subscriber = null;
        }

        #if UNITY_2017_2_OR_NEWER

        /// <summary>
        /// called when the play mode change.
        /// </summary>
        static void OnPlayModeChanged<T>(T obj)
        {
            _subscriber = null;
        }

        #else

        /// <summary>
        /// called when the play mode change.
        /// </summary>
        static void OnPlayModeChanged()
        {
            _subscriber = null;
        }

        #endif

    }

}