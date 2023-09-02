using UnityEngine;
using UnityEditor;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// The editor for the patrol mover, draws handles for the patrol mover. 
    /// way points.
    /// </summary>
    [CustomEditor(typeof(PatrolMover))]
    public class PatrolMoverHandle : Editor
    {
        //Serialized property to patrol mover fields
        private SerializedProperty Mode;
        private SerializedProperty Points;
        private SerializedProperty HandleSize;
        private SerializedProperty UsePositionHandle;
        private SerializedProperty UseCircleHandle;
        private SerializedProperty ShowLabel;
        private SerializedProperty IgnorePointFormLooping;

        /// <summary>
        /// The last position of the last drawn handle.
        /// </summary>
        [SerializeField]
        private Vector3 PrePosition;
        /// <summary>
        /// The label position from its waypoint.
        /// </summary>
        [SerializeField]
        private float LablePosition = 1;

        /// <summary>
        /// The spaces between the DotLine. 
        /// </summary>
        private readonly float DotLineSpace = 5f;
        /// <summary>
        /// The color of the handle circle.
        /// </summary>
        private readonly Color CircleHandleColor = Color.red;
        /// <summary>
        /// The color of the path.
        /// </summary>
        private readonly Color PathColor = Color.yellow;
        /// <summary>
        /// The color of the looped path.
        /// </summary>
        private readonly Color LoopPathColor = Color.cyan;
        /// <summary>
        /// Circle handle snap size.
        /// </summary>
        private readonly float HandleSnap = 0.4f;

        /// <summary>
        /// The game object the mover is attached to.
        /// </summary>
        [SerializeField]
        private GameObject gameObject;
        /// <summary>
        /// The game object transform the mover is attached to.
        /// </summary>
        private Transform transform
        {
            get
            {
                return gameObject.transform;
            }
        }
        /// <summary>
        /// The selected patrol mover component.
        /// </summary>
        private PatrolMover Instance
        {
            get
            {
                return ((PatrolMover)target);
            }
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {
            gameObject = Instance.gameObject;

            //Gets the mover property.
            Points = serializedObject.FindProperty("Points");
            HandleSize = serializedObject.FindProperty("HandleSize");
            UsePositionHandle = serializedObject.FindProperty("UsePositionHandle");
            UseCircleHandle = serializedObject.FindProperty("UseCircleHandle");
            ShowLabel = serializedObject.FindProperty("ShowLabel");
            Mode = serializedObject.FindProperty("Mode");
            IgnorePointFormLooping = serializedObject.FindProperty("IgnorePointFormLooping");
        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            serializedObject.Update();

            DrawPatrolPath();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the path and handle for the patrol mover.
        /// </summary>
        private void DrawPatrolPath()
        {
            DrawCloseLoopLine();

            for (int i = 0; i < Points.arraySize; i++)
            {
                Vector3 currentPos = GetPointPosition(i);

                //assign the current handle color.
                Handles.color = GetPathColor(i);

                //draw the path line.
                if (i != 0)
                {
                    Handles.DrawDottedLine(PrePosition, currentPos, DotLineSpace);
                }                  

                PrePosition = currentPos;
                DrawLabel(i, currentPos);

                //edit the way point position based on the handle.
                currentPos = DrawCircleHandle(currentPos);
                currentPos = DrawPositionHandle(currentPos);

                //assign the new way point.
                EditPoint(currentPos, i);
            }
        }

        /// <summary>
        /// Draws the line that closes the loop.
        /// </summary>
        private void DrawCloseLoopLine()
        {
            Color prevHandleColor = Handles.color;

            if ((int)PatrolMoverMode.Loop == Mode.enumValueIndex && Points.arraySize > 1)
            {
                Vector3 lastPos = GetPointPosition(Points.arraySize - 1);

                Vector3 firstLoopPos = GetPointPosition(IgnorePointFormLooping.intValue);

                Handles.color = LoopPathColor;

                Handles.DrawDottedLine(firstLoopPos, lastPos, DotLineSpace);
            }

            Handles.color = prevHandleColor;
        }

        /// <summary>
        /// Returns the color of the path.
        /// </summary>
        /// <param name="index">The index of the starting point of the line.</param>
        /// <returns>The path color</returns>
        private Color GetPathColor(int index)
        {
            if (Mode.enumValueIndex == (int)PatrolMoverMode.Normal)
            {
                return PathColor;
            }               
            else
            {
                if (index > IgnorePointFormLooping.intValue)
                {
                    return LoopPathColor;
                }
                else
                {
                    return PathColor;
                }                    
            }
        }

        /// <summary>
        /// Draws a position handle to a given point.
        /// </summary>
        /// <param name="currentPos">The point to draw handle on.</param>
        /// <returns>The position after it gets modified by the handle.</returns>
        private Vector3 DrawPositionHandle(Vector3 currentPos)
        {
            if (UsePositionHandle.boolValue)
                currentPos = Handles.DoPositionHandle(currentPos, Quaternion.identity);
            return currentPos;
        }

        /// <summary>
        /// Draws a circle handle to a given point.
        /// </summary>
        /// <param name="currentPos">The point to draw a handle on.</param>
        /// <returns>The position after it gets modified by the handle.</returns>
        private Vector3 DrawCircleHandle(Vector3 currentPos)
        {
            Color prevHandleColor = Handles.color;

            Handles.color = CircleHandleColor;

            if (UseCircleHandle.boolValue)
            {
                LablePosition = 1;
                currentPos = Handles.FreeMoveHandle(currentPos, Quaternion.identity, HandleSize.floatValue, Vector3.one * HandleSnap, WayPointCap);
                Handles.color = prevHandleColor;
            }
            else
                LablePosition = 0;

            return currentPos;
        }

        /// <summary>
        /// Draws a label for a given waypoint.
        /// </summary>
        /// <param name="index">Way point index.</param>
        /// <param name="currentPos">Way point position.</param>
        private void DrawLabel(int index, Vector3 currentPos)
        {
            if (ShowLabel.boolValue)
            {
                Handles.Label(currentPos - Vector3.one * HandleSize.floatValue * LablePosition, (index + 1).ToString());
            }              
        }

        /// <summary>
        /// Returns a waypoint position.
        /// </summary>
        /// <param name="Index">Waypoint index.</param>
        /// <returns>Waypoint position.</returns>
        private Vector3 GetPointPosition(int Index)
        {
            SerializedProperty WayPoint = Points.GetArrayElementAtIndex(Index);
            SerializedProperty WayPointPosition = WayPoint.FindPropertyRelative("Position");

            return Math2D.Vector2ToVector3(WayPointPosition.vector2Value, transform.position.z);
        }

        /// <summary>
        /// Changes the waypoint position.
        /// </summary>
        /// <param name="Position">The new position.</param>
        /// <param name="Index">The index of the waypoint.</param>
        private void EditPoint(Vector3 Position, int Index)
        {
            SerializedProperty WayPoint = Points.GetArrayElementAtIndex(Index);
            SerializedProperty WayPointPosition = WayPoint.FindPropertyRelative("Position");
            WayPointPosition.vector2Value = (Vector2)Position;
        }

        /// <summary>
        /// Draws a double circle handle.
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle.</param>
        /// <param name="rotation">The rotation of the handle.</param>
        /// <param name="size">The size of the handle.</param>
        /// <param name="eventType">Event type for the handle to act upon.</param>
        public void WayPointCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
            Handles.CircleHandleCap(controlID, position, rotation, size * 0.5f, eventType);
        }

    }

}