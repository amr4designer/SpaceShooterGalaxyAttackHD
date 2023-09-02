using UnityEngine;
using UnityEditor;
using ShmupBaby;

namespace ShmupEditor
{

    /// <summary>
    /// The editor for the curve wave, draws handles to the curve wave 
    /// control points.
    /// </summary>
    [CustomEditor(typeof(CurveWave))]
    public class CurveWaveHandle : Editor
    {
        //Serialized property to patrol mover fields
        private SerializedProperty Points;
        private SerializedProperty HandleSize;
        private SerializedProperty UsePositionHandle;
        private SerializedProperty UseCircleHandle;
        private SerializedProperty ShowLabel;

        /// <summary>
        /// The label position from its waypoint.
        /// </summary>
        [SerializeField]
        private float LablePosition = 1;
        /// <summary>
        /// circle handle snap size.
        /// </summary>
        private readonly float HandleSnap = 0.3f;
        /// <summary>
        /// the color of the handle circle.
        /// </summary>
        private readonly Color CircleHandleColor = Color.red;

        /// <summary>
        /// the game object the mover is attached to.
        /// </summary>
        [SerializeField]
        GameObject gameObject;
        /// <summary>
        /// The game object transform the mover is attached to.
        /// </summary>
        Transform transform
        {
            get
            { return gameObject.transform; }
        }
        /// <summary>
        /// the selected curve wave component.
        /// </summary>
        CurveWave Instance
        {
            get
            { return ((CurveWave)target); }
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {

            gameObject = Instance.gameObject;

            Points = serializedObject.FindProperty("Points");
            HandleSize = serializedObject.FindProperty("HandleSize");
            UsePositionHandle = serializedObject.FindProperty("UsePositionHandle");
            UseCircleHandle = serializedObject.FindProperty("UseCircleHandle");
            ShowLabel = serializedObject.FindProperty("ShowLabel");

        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            serializedObject.Update();

            DrawCurveHandle();

            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// draw handle for the curve control points.
        /// </summary>
        private void DrawCurveHandle()
        {
            for (int i = 0; i < Points.arraySize; i++)
            {
                //gets the curve control point.
                SerializedProperty currentPoint = Points.GetArrayElementAtIndex(i);
                Vector3 currentPos = Math2D.Vector2ToVector3(currentPoint.vector2Value, transform.position.z);

                //draws the labels.
                DrawLabel(i, currentPos);

                //draw the handles
                currentPos = DrawCircleHandle(currentPos);
                currentPos = DrawPositionHandle(currentPos);

                //savse the value to that point.
                currentPoint.vector2Value = currentPos;

            }
        }

        /// <summary>
        /// draws a position handle to a given point.
        /// </summary>
        /// <param name="currentPos">the point to draw a handle on.</param>
        /// <returns>the position after it gets modified by the handle.</returns>
        private Vector3 DrawPositionHandle(Vector3 currentPos)
        {
            if (UsePositionHandle.boolValue)
                currentPos = Handles.DoPositionHandle(currentPos, Quaternion.identity);
            return currentPos;
        }

        /// <summary>
        /// draws a circle handle to a given point.
        /// </summary>
        /// <param name="currentPos">The point to draw a handle on.</param>
        /// <returns>the position after it gets modified by the handle.</returns>
        private Vector3 DrawCircleHandle(Vector3 currentPos)
        {
            Color prevHandleColor = Handles.color;

            Handles.color = CircleHandleColor;

            if (UseCircleHandle.boolValue)
            {
                LablePosition = 1;
                currentPos = Handles.FreeMoveHandle(currentPos, Quaternion.identity, HandleSize.floatValue, Vector3.one * HandleSnap, WayPointCap);
            }
            else
                LablePosition = 0;

            Handles.color = prevHandleColor;

            return currentPos;
        }

        /// <summary>
        /// Draws a label for a given waypoint.
        /// </summary>
        /// <param name="index">waypoint index</param>
        /// <param name="currentPos">waypoint position.</param>
        private void DrawLabel(int index, Vector3 currentPos)
        {
            if (ShowLabel.boolValue)
                Handles.Label(currentPos - Vector3.one * HandleSize.floatValue * LablePosition, index.ToString());
        }

        /// <summary>
        /// draws a double circle handle.
        /// </summary>
        /// <param name="controlID">the control ID for the handle.</param>
        /// <param name="position">the position of the handle.</param>
        /// <param name="rotation">the rotation of the handle.</param>
        /// <param name="size">the size of the handle.</param>
        /// <param name="eventType">Event type for the handle to act upon.</param>
        public void WayPointCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {

            Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
            Handles.CircleHandleCap(controlID, position, rotation, size * 0.5f, eventType);

        }

    }


}
