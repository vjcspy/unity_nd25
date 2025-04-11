using UnityEngine;
namespace ND25.Util.Common.Enum
{
    public class Direction
    {
        public static XDirection ConvertToXDirection(float velocity)
        {
            return velocity > 0 ? XDirection.Right : velocity < 0 ? XDirection.Left : XDirection.None;
        }

        public static YDirection ConvertToYDirection(float velocity)
        {
            return velocity > 0 ? YDirection.Up : velocity < 0 ? YDirection.Down : YDirection.None;
        }
    }
    public enum XDirection : sbyte
    {
        Left = -1,
        None = 0,
        Right = 1
    }

    public enum YDirection : sbyte
    {
        Down = -1,
        None = 0,
        Up = 1
    }

    public class FacingDirection
    {
        public static readonly Vector3 FacingRight = new Vector3(x: 1f, y: 1f, z: 1f);
        public static readonly Vector3 FacingLeft = new Vector3(x: -1f, y: 1f, z: 1f);
    }
}
