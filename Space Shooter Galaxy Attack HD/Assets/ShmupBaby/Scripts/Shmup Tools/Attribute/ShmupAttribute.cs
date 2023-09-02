using UnityEngine;

namespace ShmupBaby
{

    #region Help Box
    
    /// <summary>
    /// Shows a help box in the inspector above the target field.
    /// </summary>
    public sealed class HelpBox : PropertyAttribute
    {

        /// <summary>
        /// The help box message.
        /// </summary>
        public string Message;

        /// <summary>
        /// HelpBox constructor.
        /// </summary>
        /// <param name="message">The message inside the help box.</param>
        public HelpBox(string message)
        {
            Message = message;
        }

    }

    #endregion

    #region Rect Handle
    
    /// <summary>
    /// A list of basic colors.
    /// </summary>
    public enum BasicColors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        White,
        Black,
        Gray
    }

    /// <summary>
    /// Draw a handle for a Rectangle in the scene
    /// (A rectangle will be drawn in the XY plane).
    /// </summary>
    public sealed class RectHandle : PropertyAttribute
    {

        /// <summary>
        /// the label of the rect will appear in the left up corner of the rect.
        /// </summary>
        public string Label;

        /// <summary>
        /// the size of the handle of the rect.
        /// </summary>
        public float HandleSize;

        /// <summary>
        /// color of the rect.
        /// </summary>
        public Color RectColor;

        /// <summary>
        /// color of the rect handle.
        /// </summary>
        public Color HandleColor;

        /// <summary>
        /// rect position in the z axis.
        /// </summary>
        public float PositionOnZ;

        /// <summary>
        /// RectHandle constructor.
        /// </summary>
        /// <param name="label">the label of the rect will appear in the left up corner of the rect.</param>
        /// <param name="handleSize">he size of the handle of the rect.</param>
        /// <param name="rectColor">color of the rect.</param>
        /// <param name="handleColor">color of the rect handle.</param>
        public RectHandle(string label, float handleSize, BasicColors rectColor, BasicColors handleColor)
        {
            Label = label;
            HandleSize = handleSize;
            RectColor = BasicColorsToColor(rectColor);
            HandleColor = BasicColorsToColor(handleColor);
        }

        /// <summary>
        /// RectHandle constructor.
        /// </summary>
        /// <param name="label">the label of the rect will appear in the left up corner of the rect.</param>
        /// <param name="handleSize">he size of the handle of the rect.</param>
        /// <param name="rectColor">color of the rect.</param>
        /// <param name="handleColor">color of the rect handle.</param>
        /// <param name="positionOnZ">rect position in the z axis.</param>
        public RectHandle(string label, float handleSize, BasicColors rectColor, BasicColors handleColor,
            float positionOnZ)
        {
            Label = label;
            HandleSize = handleSize;
            RectColor = BasicColorsToColor(rectColor);
            HandleColor = BasicColorsToColor(handleColor);
            PositionOnZ = positionOnZ;
        }

        /// <summary>
        /// convert a BasicColor value to Color.
        /// </summary>
        private Color BasicColorsToColor(BasicColors color)
        {
            switch (color)
            {
                case BasicColors.Red:
                    return Color.red;
                case BasicColors.Blue:
                    return Color.blue;
                case BasicColors.Green:
                    return Color.green;
                case BasicColors.Black:
                    return Color.black;
                case BasicColors.White:
                    return Color.white;
                case BasicColors.Gray:
                    return Color.gray;
                case BasicColors.Yellow:
                    return Color.yellow;
                case BasicColors.Cyan:
                    return Color.cyan;
            }

            return Color.white;
        }

    }

    #endregion

}