using UnityEngine ;
using UnityEditor ;
using ShmupBaby ;

namespace ShmupEditor
{

    /// <summary>
    /// used by the SceneHandleManager to draw the handle in the scene view.
    /// </summary>
    public interface IDrawHandle
    {

        /// <summary>
        /// draw the handle in the scene view.
        /// </summary>
        /// <param name="sceneView">the scene view.</param>
        void Draw(SceneView sceneView);

    }

    /// <summary>
    /// draw the rect property in the scene view and create handle to edit 
    /// the rect property in the scene view.
    /// </summary>
    [CustomPropertyDrawer(typeof(RectHandle))]
    public class RectHandleAttribute : PropertyDrawer, IDrawHandle
    {

        /// <summary>
        /// rect handle snap size.
        /// </summary>
        private const float HandleSnap = 0.1f;

        /// <summary>
        /// the property RectHandle attribute.
        /// </summary>
        private RectHandle _attr
        {
            get { return (RectHandle)attribute; }
        }

        /// <summary>
        /// the property that implement the attribute.
        /// </summary>
        private SerializedProperty _myProperty;

        /// <summary>
        /// draw the help box and the field.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //draw the property as it was drawn.
            EditorGUI.PropertyField(position, property, label);

            _myProperty = property;

            //check if the attribute is set to Rect field.
            if (property.type != "Rect")
            {
                Debug.LogWarning("this attribute only work on a field of type (Rect)");
                return;
            }

            //make sure that the object of this property is in the scene.
            if (PrefabUtility.GetPrefabType(property.serializedObject.targetObject) != PrefabType.PrefabInstance &&
                PrefabUtility.GetPrefabType(property.serializedObject.targetObject) != PrefabType.None &&
                PrefabUtility.GetPrefabType(property.serializedObject.targetObject) != PrefabType.MissingPrefabInstance)
                return;

            //subscribe to SceneHandleManager to draw the handle in the scene.
            SceneHandleManager.AddToSceneGUI(this);

        }

        /// <summary>
        /// return the height for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>the height for this property in pixel.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //double the height of the property because the rect property 
            //take double the height of the normal field.
            return base.GetPropertyHeight(property, label) * 2f;
        }

        /// <summary>
        /// draw a handle to the property in the scene.
        /// </summary>
        /// <param name="sceneView">the scene view.</param>
        public void Draw(SceneView sceneView)
        {

            _myProperty.serializedObject.Update();
            Rect rect = _myProperty.rectValue;

            Handles.color = _attr.HandleColor;

            //draw handle to edit the edges position of the rect.
            Vector3 LeftHandle = Handles.FreeMoveHandle(new Vector3(rect.xMin, rect.yMin + (rect.yMax - rect.yMin) * 0.5f, _attr.PositionOnZ), Quaternion.identity, _attr.HandleSize, Vector3.one * HandleSnap, Handles.RectangleHandleCap);
            rect.xMin = LeftHandle.x;
            Vector3 RightHandle = Handles.FreeMoveHandle(new Vector3(rect.xMax, rect.yMin + (rect.yMax - rect.yMin) * 0.5f, _attr.PositionOnZ), Quaternion.identity, _attr.HandleSize, Vector3.one * HandleSnap, Handles.RectangleHandleCap);
            rect.xMax = RightHandle.x;
            Vector3 UpHandle = Handles.FreeMoveHandle(new Vector3(rect.xMin + (rect.xMax - rect.xMin) * 0.5f, rect.yMax, _attr.PositionOnZ), Quaternion.identity, _attr.HandleSize, Vector3.one * HandleSnap, Handles.RectangleHandleCap);
            rect.yMax = UpHandle.y;
            Vector3 DownHandle = Handles.FreeMoveHandle(new Vector3(rect.xMin + (rect.xMax - rect.xMin) * 0.5f, rect.yMin, _attr.PositionOnZ), Quaternion.identity, _attr.HandleSize, Vector3.one * HandleSnap, Handles.RectangleHandleCap);
            rect.yMin = DownHandle.y;

            Handles.color = _attr.RectColor;

            //draw the rect and the label.
            Handles.Label(new Vector3(rect.xMin, rect.yMax, _attr.PositionOnZ), _attr.Label);
            DrawRect(rect, _attr.PositionOnZ);

            _myProperty.rectValue = rect;
            _myProperty.serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Draws the given rect on the XY plane at Z = depth.
        /// </summary>
        /// <param name="rect">Rect to draw.</param>
        public static void DrawRect(Rect rect, float depth)
        {
            Handles.DrawLine(new Vector3(rect.xMax, rect.yMax, depth), new Vector3(rect.xMin, rect.yMax, depth));
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMax, depth), new Vector3(rect.xMin, rect.yMin, depth));
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMin, depth), new Vector3(rect.xMax, rect.yMin, depth));
            Handles.DrawLine(new Vector3(rect.xMax, rect.yMin, depth), new Vector3(rect.xMax, rect.yMax, depth));
        }

    }

}
