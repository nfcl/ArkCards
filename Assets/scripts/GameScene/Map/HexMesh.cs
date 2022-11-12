using UnityEngine;
using System.Collections.Generic;
using GameScene.Map;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    private Mesh hexMesh;
    /// <summary>
    /// �ڵ㼯
    /// </summary>
	[NonSerialized] 
    private List<Vector3> vertices;
    /// <summary>
    /// �ڵ���ɫ��
    /// </summary>
	[NonSerialized] 
    private List<Color> colors;
    /// <summary>
    /// �����漯
    /// </summary>
	[NonSerialized] 
    private List<int> triangles;
    /// <summary>
    /// ������ײ��
    /// </summary>
    private MeshCollider meshCollider;

    /// <summary>
    /// �����������
    /// </summary>
    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        colors = ListPool<Color>.Get();
        triangles = ListPool<int>.Get();
    }
    /// <summary>
    /// �����µ���������
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
    /// ���һ���������Ŷ��������ε�����
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
    /// ���һ�����������Ŷ��������ε�����
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
    /// ���һ������������ɫ��ͬ�������ζ�����ɫ
    /// </summary>
    public void AddTriangleColor(Color c)
    {
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
    }
    /// <summary>
    /// ���һ������������ɫ��ͬ�������ε�����������ɫ
    /// </summary>
    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    /// <summary>
    /// ���һ���ı��ε�����
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
    /// <para/>���һ���ı��ε��ĸ�������ɫ
    /// <para/>4��������ɫ��ȫ��ͬ
    /// </summary>
    public void AddQuadColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    /// <summary>
    /// <para/>���һ���ı��ε��ĸ�������ɫ
    /// <para/>4��������ɫ�����ɶ�
    /// </summary>
    public void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
    /// <summary>
    /// <para/>���һ���ı��ε��ĸ�������ɫ
    /// <para/>4��������ɫ������ͬ
    /// </summary>
    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
    
    /// <summary>
    /// ���ؽű�ʵ��ʱ����Awake
    /// </summary>
    void Awake()
    {
        //������������
        hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
        //���������ײ��
        meshCollider = gameObject.AddComponent<MeshCollider>();
        //����������
        hexMesh.name = "Hex Mesh";
    }
}