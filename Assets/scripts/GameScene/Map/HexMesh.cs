using UnityEngine;
using System.Collections.Generic;
using GameScene.Map;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;
    MeshCollider meshCollider;

    /// <summary>
    /// 根据地图节点集绘制网格
    /// </summary>
    /// <param name="cells">节点集</param>
    public void Triangulate(HexCell[] cells)
    {
        //清空旧的网格数据
        hexMesh.Clear();
        //清空旧的顶点集
        vertices.Clear();
        //清空旧的三角形集
        triangles.Clear();
        //清空旧的顶点颜色集
        colors.Clear();
        //遍历节点集并添加新的面
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        //更新网格的顶点集
        hexMesh.vertices = vertices.ToArray();
        //更新网格的三角形集
        hexMesh.triangles = triangles.ToArray();
        //更新顶点颜色集
        hexMesh.colors = colors.ToArray();
        //重新计算法线
        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }

    /// <summary>
    /// 根据三个位置添加三角形
    /// </summary>
    /// <param name="v1">位置1</param>
    /// <param name="v2">位置2</param>
    /// <param name="v3">位置3</param>
    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
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
    /// 增加新的三角形顶点颜色
    /// </summary>
    /// <param name="color">指定的顶点颜色</param>
    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    /// <summary>
    /// 根据中心点计算六边形的六个三角形顶点
    /// </summary>
    /// <param name="cell"></param>
    void Triangulate(HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + HexMetrics.corners_spire[i],
                center + HexMetrics.corners_spire[i + 1]
            );
            AddTriangleColor(cell.color);
        }
    }

    void Awake()
    {
        //获得网格管理器
        hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
        //获得网格碰撞体
        meshCollider = gameObject.AddComponent<MeshCollider>();
        //命名新网格
        hexMesh.name = "Hex Mesh";
        //顶点集
        vertices = new List<Vector3>();
        //三角形集
        triangles = new List<int>();
        //顶点颜色集
        colors = new List<Color>();
    }
}