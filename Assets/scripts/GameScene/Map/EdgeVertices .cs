using UnityEngine;
namespace GameScene.Map
{
    /// <summary>
    /// 一条线段的两个端点和3个四等分点的集合
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 v1;  // 0
        public Vector3 v2;  // 1/4
        public Vector3 v3;  // 2/4
        public Vector3 v4;  // 3/4
        public Vector3 v5;  // 1

        /// <summary>
        /// 根据两个端点构造四等分点
        /// </summary>
        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, 0.25f);
            v3 = Vector3.Lerp(corner1, corner2, 0.5f);
            v4 = Vector3.Lerp(corner1, corner2, 0.75f);
            v5 = corner2;
        }
        /// <summary>
        /// 左右分割点按比例对称
        /// </summary>
        /// <param name="corner1">左端点</param>
        /// <param name="corner2">右端点</param>
        /// <param name="outerStep">左分割点到左端点占整条线段的比例</param>
        public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, outerStep);
            v3 = Vector3.Lerp(corner1, corner2, 0.5f);
            v4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
            v5 = corner2;
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
            result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, step);
            return result;
        }
    }
}