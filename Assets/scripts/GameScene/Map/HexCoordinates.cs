using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// 六边形坐标
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {
        /// <summary>
        /// 节点x坐标
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// <para/>节点Y坐标
        /// <para/>用于3维表示法时
        /// </summary>
        public int Y { get { return -X - Z; } }
        /// <summary>
        /// 节点z坐标
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// 正常构造
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="z">z轴坐标</param>
        public HexCoordinates(int x, int z)
        {
            X = x;
            Z = z;
        }
        /// <summary>
        /// 将未偏移的六边形坐标转化为偏移后的六边形坐标
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="z">z轴坐标</param>
        /// <returns>返回偏移后的坐标</returns>
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }
        /// <summary>
        /// 将世界坐标转换为六边形坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static HexCoordinates FromPosition(Vector3 position)
        {
            //三维坐标构造
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;
            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;
            //取整
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);
            //如果计算的三维坐标非法还需要重新合法化
            if (iX + iY + iZ != 0)
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }
            //返回结果
            return new HexCoordinates(iX, iZ);
        }
        /// <summary>
        /// 
        /// </summary>
        public string ToStringOnSeparateLines()
        {
            return $"{X}\n{Y}\n{Z}";
        }

        public static bool operator !=(HexCoordinates lhs, HexCoordinates rhs)
        {
            return lhs.X != rhs.X || lhs.Z != rhs.Z;
        }

        public static bool operator ==(HexCoordinates lhs, HexCoordinates rhs)
        {
            return lhs.X == rhs.X && lhs.Z == rhs.Z;
        }
    }
}