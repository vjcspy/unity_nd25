using UnityEngine;
namespace ND25.Util.Common.Enum
{
    public enum HorizontalDirection : sbyte
    {
        Left = -1,
        None = 0,
        Right = 1
    }

    public enum VerticalDirection : sbyte
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
