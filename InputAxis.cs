using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MGInput
{
    public enum AxisType
    {
        None,
        MouseX,
        MouseY,
        LeftThumbStickX,
        LeftThumbStickY,
        RightThumbStickX,
        RightThumbStickY
    }

    /// <summary>
    /// Represents joystick or mouse control axis
    /// </summary>
    class InputAxis
    {
        private AxisType axisType;
        private List<Keys> boundKeys;
        private List<Buttons> boundButtons;
        
        public InputAxis(AxisType axisType = AxisType.None)
        {
            this.axisType = axisType;
        }

        public void SetAxisType(AxisType axisType)
        {
            this.axisType = axisType;
        }

        public float GetValue(Vector2 mouseMovementDelta, GamePadState gamePadState)
        {
            switch (axisType)
            {
                case AxisType.LeftThumbStickX:
                    return gamePadState.ThumbSticks.Left.X;
                case AxisType.LeftThumbStickY:
                    return -gamePadState.ThumbSticks.Left.Y;
                case AxisType.RightThumbStickX:
                    return gamePadState.ThumbSticks.Right.X;
                case AxisType.RightThumbStickY:
                    return -gamePadState.ThumbSticks.Right.Y;
                case AxisType.MouseX:
                    return mouseMovementDelta.X;
                case AxisType.MouseY:
                    return mouseMovementDelta.Y;
                default:
                    return 0f;
            }
        }
    }
}
