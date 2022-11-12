using UnityEngine;
using System.Collections.Generic;
using GameScene.Map;
using System;

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
    /// 网格碰撞器
    /// </summary>
    private MeshCollider meshCollider;

    /// <summary>
    /// 清除网格数据
    /// </summary>
    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        colors = ListPool<Color>.Get();
        triangles = ListPool<int>.Get();
    }
    /// <summary>
    /// 设置新的网格数据
    /// </summary>
    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        hexMesh.SetColors(colors);
        ListPool<Color>.Add(colors);
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
    /// <summary>
    /// 添加一个受噪声扰动的三角形到网格
    /// </summary>
    public void AddTrianglePerturbed(Vector3 v1, Vector3 v2, Vector3 v3)
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
    /// 添加一个四边形到网格
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
    /// 加载脚本实例时调用Awake
    /// </summary>
    void Awake()
    {
        //获得网格管理器
        hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
        //获得网格碰撞体
        meshCollider = gameObject.AddComponent<MeshCollider>();
        //命名新网格
        hexMesh.name = "Hex Mesh";
    }
}