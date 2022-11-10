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
    /// ���ݵ�ͼ�ڵ㼯��������
    /// </summary>
    /// <param name="cells">�ڵ㼯</param>
    public void Triangulate(HexCell[] cells)
    {
        //��վɵ���������
        hexMesh.Clear();
        //��վɵĶ��㼯
        vertices.Clear();
        //��վɵ������μ�
        triangles.Clear();
        //��վɵĶ�����ɫ��
        colors.Clear();
        //�����ڵ㼯������µ���
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        //��������Ķ��㼯
        hexMesh.vertices = vertices.ToArray();
        //��������������μ�
        hexMesh.triangles = triangles.ToArray();
        //���¶�����ɫ��
        hexMesh.colors = colors.ToArray();
        //���¼��㷨��
        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }
    /// <summary>
    /// ���һ�������ε�����
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
    /// ���һ������������ɫ��ͬ�������ζ�����ɫ
    /// </summary>
    void AddTriangleColor(Color c)
    {
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
    }
    /// <summary>
    /// ���һ������������ɫ��ͬ�������ε�����������ɫ
    /// </summary>
    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    /// <summary>
    /// ���һ���ı��ε�����
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
    /// ���һ���ı��ε��ĸ�������ɫ
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
    /// �������ĵ��������������
    /// </summary>
    /// <param name="cell">�ڵ�</param>
    void Triangulate(HexCell cell)
    {
        //���������������������
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    /// <summary>
    /// �������ĵ�ͷ������������
    /// </summary>
    /// <param name="direction">����</param>
    /// <param name="cell">���Ľڵ�</param>
    void Triangulate(HexDirection direction, HexCell cell)
    {
        //�������ĵ�λ��
        Vector3 center = cell.transform.localPosition;
        //���㴿ɫ����������εױߵ���������
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        //��Ӵ�ɫ�������������
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);
        //��Ӷ�Ӧ��������Ӿ���
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }
    /// <summary>
    /// ��ӽڵ��Ӧ��������Ӿ���
    /// </summary>
    /// <param name="direction">��Ӧ����</param>
    /// <param name="cell">�ڵ�</param>
    /// <param name="v1">��ɫ����ױ߶���1</param>
    /// <param name="v2">��ɫ����ױ߶���2</param>
    void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
    {
        //��ö�Ӧ������ھ�
        HexCell neighbor = cell.GetNeighbor(direction);
        //���û���ھӷ���
        if (neighbor == null) return;
        //���㴿ɫ����ױߵ��������ױߴ��߷�������
        Vector3 bridge = HexMetrics.GetBridge(direction);
        //���㴿ɫ����ױߵ��������ױߴ��ߵ���������
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        //���û�ɫ���εױ߶���߶�Ϊ�ھӵĸ߶�
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;
        //��ӻ��������ζ���
        AddQuad(v1, v2, v3, v4);
        //��ӻ��������ζ�����ɫ
        AddQuadColor(cell.color, neighbor.color);
        //���˳ʱ�뷽�����һ���ھ�
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        //���������һ���ھ�����������ڵ��ཻ���Ļ����������
        //�����������������ɫʵΪ�����ڵ����ɫ
        //���Ʒ����ֹ�����ظ��Ļ������������
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            //������һ����Ͼ��ε�ͬ��ױ߶���
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            //���ø߶�Ϊ��һ���ھӵĸ߶�
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
            AddTriangle(
                //��ɫ�ױ߶���
                v2,
                //��Ͼ��εױ߶���
                v4,
                //��һ����Ͼ��ε�ͬ��ױ߶���
                v5
            );
            AddTriangleColor(
                //�ڵ���ɫ
                cell.color, 
                //�ھ���ɫ
                neighbor.color, 
                //��һ���ھ���ɫ
                nextNeighbor.color
            );
        }
    }

    void Awake()
    {
        //������������
        hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
        //���������ײ��
        meshCollider = gameObject.AddComponent<MeshCollider>();
        //����������
        hexMesh.name = "Hex Mesh";
        //���㼯
        vertices = new List<Vector3>();
        //�����μ�
        triangles = new List<int>();
        //������ɫ��
        colors = new List<Color>();
    }
}