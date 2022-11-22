using UnityEngine;
using System.Collections.Generic;
using System;

namespace GameScene.Map
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        /// <summary>
        /// 网格
        /// </summary>
        private Mesh hexMesh;
        /// <summary>
        /// 节点集
        /// </summary>
        [NonSerialized]
        private List<Vector3> vertices;
        /// <summary>
        /// 节点颜色集
        /// </summary>
        [NonSerialized]
        private List<Color> colors;
        /// <summary>
        /// 三角面集
        /// </summary>
        [NonSerialized]
        private List<int> triangles;
        /// <summary>
        /// UV坐标集
        /// </summary>
        [NonSerialized]
        private List<Vector2> uvs;
        /// <summary>
        /// 第二个UV坐标集
        /// </summary>
        [NonSerialized]
        private List<Vector2> uv2s;
        /// <summary>
        /// 地形类型列表
        /// </summary>
        [NonSerialized] List<Vector3> terrainTypes;
        /// <summary>
        /// 网格碰撞器
        /// </summary>
        private MeshCollider meshCollider;

        /// <summary>
        /// 是否启用碰撞体
        /// </summary>
        public bool useCollider;
        /// <summary>
        /// 是否启用颜色
        /// </summary>
        public bool useColors;
        /// <summary>
        /// 是否启用UV坐标
        /// </summary>
        public bool useUVCoordinates;
        /// <summary>
        /// 是否启用第二组UV坐标
        /// </summary>
        public bool useUV2Coordinates;
        /// <summary>
        /// 是否启用地形类型
        /// </summary>
        public bool useTerrainTypes;

        /// <summary>
        /// 清除网格数据
        /// </summary>
        public void Clear()
        {
            hexMesh.Clear();
            vertices = ListPool<Vector3>.Get();
            if (useColors)
            {
                colors = ListPool<Color>.Get();
            }
            if (useUVCoordinates)
            {
                uvs = ListPool<Vector2>.Get();
            }
            if (useUV2Coordinates)
            {
                uv2s = ListPool<Vector2>.Get();
            }
            if (useTerrainTypes)
            {
                terrainTypes = ListPool<Vector3>.Get();
            }
            triangles = ListPool<int>.Get();
        }
        /// <summary>
        /// 设置新的网格数据
        /// </summary>
        public void Apply()
        {
            hexMesh.SetVertices(vertices);
            ListPool<Vector3>.Add(vertices);
            if (useColors)
            {
                hexMesh.SetColors(colors);
                ListPool<Color>.Add(colors);
            }
            if (useUVCoordinates)
            {
                hexMesh.SetUVs(0, uvs);
                ListPool<Vector2>.Add(uvs);
            }
            if (useUV2Coordinates)
            {
                hexMesh.SetUVs(1, uv2s);
                ListPool<Vector2>.Add(uv2s);
            }
            if (useTerrainTypes)
            {
                hexMesh.SetUVs(2, terrainTypes);
                ListPool<Vector3>.Add(terrainTypes);
            }
            hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(triangles);
            hexMesh.RecalculateNormals();
            if (useCollider)
            {
                meshCollider.sharedMesh = hexMesh;
            }
        }
        /// <summary>
        /// 添加一个受噪声扰动的三角形到网格
        /// </summary>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
        /// <summary>
        /// 添加一个不受噪声扰动的三角形到网格
        /// </summary>
        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
        /// <summary>
        /// 添加一个三个顶点颜色相同的三角形顶点颜色
        /// </summary>
        public void AddTriangleColor(Color c)
        {
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
        }
        /// <summary>
        /// 添加一个三个顶点颜色不同的三角形的三个顶点颜色
        /// </summary>
        public void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }
        /// <summary>
        /// 添加一个顶点受扰动的四边形到网格
        /// </summary>
        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            vertices.Add(HexMetrics.Perturb(v4));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }
        /// <summary>
        /// 添加一个顶点不受扰动的四边形
        /// </summary>
        public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }
        /// <summary>
        /// <para/>添加一个四边形的四个顶点颜色
        /// <para/>4个顶点颜色完全相同
        /// </summary>
        public void AddQuadColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
        /// <summary>
        /// <para/>添加一个四边形的四个顶点颜色
        /// <para/>4个顶点颜色两两成对
        /// </summary>
        public void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }
        /// <summary>
        /// <para/>添加一个四边形的四个顶点颜色
        /// <para/>4个顶点颜色各不相同
        /// </summary>
        public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }
        /// <summary>
        /// 添加三角形UV坐标
        /// </summary>
        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }
        /// <summary>
        /// 添加四边形UV坐标
        /// </summary>
        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }
        /// <summary>
        /// 添加四边形UV坐标
        /// </summary>
        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }
        /// <summary>
        /// 添加第二对三角形UV坐标
        /// </summary>
        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
        }
        /// <summary>
        /// 添加第二对四边形UV坐标
        /// </summary>
        public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
            uv2s.Add(uv4);
        }
        /// <summary>
        /// 添加第二对四边形UV坐标
        /// </summary>
        public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
        {
            uv2s.Add(new Vector2(uMin, vMin));
            uv2s.Add(new Vector2(uMax, vMin));
            uv2s.Add(new Vector2(uMin, vMax));
            uv2s.Add(new Vector2(uMax, vMax));
        }
        /// <summary>
        /// 添加地形类型三角形
        /// </summary>
        public void AddTriangleTerrainTypes(Vector3 types)
        {
            terrainTypes.Add(types);
            terrainTypes.Add(types);
            terrainTypes.Add(types);
        }
        /// <summary>
        /// 添加地形类型四边形
        /// </summary>
        public void AddQuadTerrainTypes(Vector3 types)
        {
            terrainTypes.Add(types);
            terrainTypes.Add(types);
            terrainTypes.Add(types);
            terrainTypes.Add(types);
        }

        /// <summary>
        /// 加载脚本实例时调用Awake
        /// </summary>
        private void Awake()
        {
            //获得网格管理器
            hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
            //获得网格碰撞体
            if (useCollider)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            //命名新网格
            hexMesh.name = "Hex Mesh";
        }
    }
}