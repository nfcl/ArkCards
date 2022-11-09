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
    /// ��������λ�����������
    /// </summary>
    /// <param name="v1">λ��1</param>
    /// <param name="v2">λ��2</param>
    /// <param name="v3">λ��3</param>
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
    /// �����µ������ζ�����ɫ
    /// </summary>
    /// <param name="color">ָ���Ķ�����ɫ</param>
    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    /// <summary>
    /// �������ĵ���������ε����������ζ���
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