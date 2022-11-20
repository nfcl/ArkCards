using UnityEngine;

namespace GameScene.Map
{
    [System.Serializable]
    public struct HexTerrainType
    {
        /// <summary>
        /// 地形类型
        /// </summary>
        public int type;
        /// <summary>
        /// 地形颜色
        /// </summary>
        public Color color;

        /// <summary>
        /// 重载!=运算符
        /// </summary>
        /// <returns>返回两者type是否不同</returns>
        public static bool operator !=(HexTerrainType lhs, HexTerrainType rhs)
        {
            return lhs.type != rhs.type;
        }
        /// <summary>
        /// 重载==运算符
        /// </summary>
        /// <returns>返回两者type是否相同</returns>
        public static bool operator ==(HexTerrainType lhs, HexTerrainType rhs)
        {
            return lhs.type == rhs.type;
        }
        /// <summary>
        /// 重写Equals
        /// </summary>
        public override bool Equals(object other)
        {
            return this == (HexTerrainType)other;
        }
        /// <summary>
        /// 重写GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}