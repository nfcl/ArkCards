using System.Runtime.CompilerServices;

namespace GameScene.Map
{
    public enum HexDirection
    {
        NE, E, SE, SW, W, NW
    }

    public static class HexDirectionExtensions
    {
        /// <summary>
        /// 返回相反的方向
        /// </summary>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
        /// <summary>
        /// 返回给定方向按顺时针方向的上一个方向
        /// </summary>
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        /// <summary>
        /// 返回给定方向按顺时针方向的下一个方向
        /// </summary>
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }
        /// <summary>
        /// 返回给定方向按顺时针方向的上上个方向
        /// </summary>
        public static HexDirection Previous2(this HexDirection direction)
        {
            direction -= 2;
            return direction >= HexDirection.NE ? direction : (direction + 6);
        }
        /// <summary>
        /// 返回给定方向按顺时针方向的下下个方向
        /// </summary>
        public static HexDirection Next2(this HexDirection direction)
        {
            direction += 2;
            return direction <= HexDirection.NW ? direction : (direction - 6);
        }
    }
}
//   / \ / \
//  |NW |NE |
// / \ / \ / \
//| W | C | E |
// \ / \ / \ / 
//  |SW |SE |
//   \ / \ /