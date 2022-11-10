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
        /// <param name="direction">提供的方向</param>
        /// <returns>返回相反的方向</returns>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
        /// <summary>
        /// 返回给定方向的上一个方向
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        /// <summary>
        /// 返回给定方向的下一个方向
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
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