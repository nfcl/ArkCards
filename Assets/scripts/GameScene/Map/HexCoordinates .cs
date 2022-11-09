using System.Collections;
using UnityEngine;

namespace GameScene.Map
{
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

        public HexCoordinates(int x, int z)
        {
            X = x;
            Z = z;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            //if (z < 0)
            //    return new HexCoordinates(x - (z - 1) / 2, z);
            //else
                return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;
            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset; 
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

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

            return new HexCoordinates(iX, iZ);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({X} , {Y} , {Z})";             
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToStringOnSeparateLines()
        {
            return $"{X}\n{Y}\n{Z}";
        }
    }
}