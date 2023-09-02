using UnityEngine;
using UnityEditor;
using ShmupBaby;


namespace ShmupEditor
{
    /// <summary>
    /// draw a help box in the inspector for the fields that 
    /// implement the help box attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(HelpBox))]
    public class HelpBoxAttribute : PropertyDrawer
    {

        /// <summary>
        /// the space above and beneath the help box.
        /// </summary>
        private const float SpaceBetween = 10;
        /// <summary>
        /// the help box icon size.
        /// </summary>
        private const float HelpBoxIconWidth = 55;

        /// <summary>
        /// the selected attribute.
        /// </summary>
        private HelpBox helpBox
        {
            get
            { return (HelpBox)attribute; }
        }

        /// <summary>
        /// draw the help box and the field.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect helpBoxRect = position;

            //shift the help box down by SpaceBetween.
            helpBoxRect.y += SpaceBetween;
            //assign the help box height.
            helpBoxRect.height = GetHelpBoXHeight(helpBox.Message);
            //draw the help box.
            EditorGUI.HelpBox(helpBoxRect, helpBox.Message, MessageType.Info);
            //shift field beneath the help box.
            position.y += helpBoxRect.height + SpaceBetween * 2;
            //remove the help box height from the field height.
            position.height -= (helpBoxRect.height + SpaceBetween * 2);
            //draw the field.
            EditorGUI.PropertyField(position, property);
        }

        /// <summary>
        /// return the height for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>the height for this property in pixel.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + GetHelpBoXHeight(helpBox.Message) + SpaceBetween * 2;
        }

        /// <summary>
        /// calculate the height for the EditorGUI.HelpBox based on the message.
        /// </summary>
        /// <param name="message">the help box message.</param>
        /// <returns>the hight of the help box.</returns>
        protected virtual float GetHelpBoXHeight(string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);

            return style.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth - HelpBoxIconWidth);

        }

    }

}