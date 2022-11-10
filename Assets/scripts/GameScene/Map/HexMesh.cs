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
    /// 添加一个三角形到网格
    /// </summary>
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
    /// 添加一个三个顶点颜色相同的三角形顶点颜色
    /// </summary>
    void AddTriangleColor(Color c)
    {
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
    }
    /// <summary>
    /// 添加一个三个顶点颜色不同的三角形的三个顶点颜色
    /// </summary>
    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    /// <summary>
    /// 添加一个四边形到网格
    /// </summary>
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
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
    /// 添加一个四边形的四个顶点颜色
    /// </summary>
    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
    /// <summary>
    /// 根据中心点添加六个三角面
    /// </summary>
    /// <param name="cell">节点</param>
    void Triangulate(HexCell cell)
    {
        //遍历六个方向添加三角面
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    /// <summary>
    /// 根据中心点和方向添加三角面
    /// </summary>
    /// <param name="direction">方向</param>
    /// <param name="cell">中心节点</param>
    void Triangulate(HexDirection direction, HexCell cell)
    {
        //计算中心点位置
        Vector3 center = cell.transform.localPosition;
        //计算纯色区域的三角形底边的两个顶点
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        //添加纯色区域的三角形面
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);
        //添加对应方向的连接矩形
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }
    /// <summary>
    /// 添加节点对应方向的连接矩形
    /// </summary>
    /// <param name="direction">对应方向</param>
    /// <param name="cell">节点</param>
    /// <param name="v1">纯色区域底边顶点1</param>
    /// <param name="v2">纯色区域底边顶点2</param>
    void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
    {
        //获得对应方向的邻居
        HexCell neighbor = cell.GetNeighbor(direction);
        //如果没有邻居返回
        if (neighbor == null) return;
        //计算纯色区域底边到混合区域底边垂线方向向量
        Vector3 bridge = HexMetrics.GetBridge(direction);
        //计算纯色区域底边到混合区域底边垂线的两个垂心
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        //设置混色矩形底边顶点高度为邻居的高度
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;
        //添加混合区域矩形顶点
        AddQuad(v1, v2, v3, v4);
        //添加混合区域矩形顶点颜色
        AddQuadColor(cell.color, neighbor.color);
        //获得顺时针方向的下一个邻居
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        //如果存在下一个邻居则添加三个节点相交中心混合区域到网格
        //混合区域三个顶点颜色实为三个节点的颜色
        //限制方向防止产生重复的混合区域三角形
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            //计算下一个混合矩形的同侧底边顶点
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            //设置高度为下一个邻居的高度
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
            AddTriangle(
                //纯色底边顶点
                v2,
                //混合矩形底边顶点
                v4,
                //下一个混合矩形的同侧底边顶点
                v5
            );
            AddTriangleColor(
                //节点颜色
                cell.color, 
                //邻居颜色
                neighbor.color, 
                //下一个邻居颜色
                nextNeighbor.color
            );
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