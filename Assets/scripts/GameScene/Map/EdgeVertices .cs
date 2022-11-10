using UnityEngine;
namespace GameScene.Map
{
    /// <summary>
    /// 一条线段的两个端点和两个三等分点的集合
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 v1;  // 0
        public Vector3 v2;  // 1/3
        public Vector3 v3;  // 2/3
        public Vector3 v4;  // 1

        /// <summary>
        /// 根据两个端点构造三等分点
        /// </summary>
        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
            v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
            v4 = corner2;
        }
        /// <summary>
        /// 根据两个边缘进行插值计算
        /// </summary>
        /// <returns>返回对应位置的插值边缘</returns>
        public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
        {
            EdgeVertices result;
            result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
            result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
            result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
            result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
            return result;
        }
    }
}