using UnityEngine;
using System.Collections.Generic;
using System;

namespace GameScene.Map
{
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
        /// UV���꼯
        /// </summary>
        [NonSerialized]
        private List<Vector2> uvs;
        /// <summary>
        /// �ڶ���UV���꼯
        /// </summary>
        [NonSerialized]
        private List<Vector2> uv2s;
        /// <summary>
        /// ������ײ��
        /// </summary>
        private MeshCollider meshCollider;

        /// <summary>
        /// �Ƿ�������ײ��
        /// </summary>
        public bool useCollider;
        /// <summary>
        /// �Ƿ�������ɫ
        /// </summary>
        public bool useColors;
        /// <summary>
        /// �Ƿ�����UV����
        /// </summary>
        public bool useUVCoordinates;
        /// <summary>
        /// �Ƿ����õڶ���UV����
        /// </summary>
        public bool useUV2Coordinates;

        /// <summary>
        /// �����������
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
            triangles = ListPool<int>.Get();
        }
        /// <summary>
        /// �����µ���������
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
            hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(triangles);
            hexMesh.RecalculateNormals();
            if (useCollider)
            {
                meshCollider.sharedMesh = hexMesh;
            }
        }
        /// <summary>
        /// ���һ���������Ŷ��������ε�����
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
        /// ���һ���������Ŷ����ı��ε�����
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
        /// ���һ�����㲻���Ŷ����ı���
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
        /// ���������UV����
        /// </summary>
        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }
        /// <summary>
        /// ����ı���UV����
        /// </summary>
        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }
        /// <summary>
        /// ����ı���UV����
        /// </summary>
        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }
        /// <summary>
        /// ��ӵڶ���������UV����
        /// </summary>
        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
        }
        /// <summary>
        /// ��ӵڶ����ı���UV����
        /// </summary>
        public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
            uv2s.Add(uv4);
        }
        /// <summary>
        /// ��ӵڶ����ı���UV����
        /// </summary>
        public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
        {
            uv2s.Add(new Vector2(uMin, vMin));
            uv2s.Add(new Vector2(uMax, vMin));
            uv2s.Add(new Vector2(uMin, vMax));
            uv2s.Add(new Vector2(uMax, vMax));
        }

        /// <summary>
        /// ���ؽű�ʵ��ʱ����Awake
        /// </summary>
        private void Awake()
        {
            //������������
            hexMesh = GetComponent<MeshFilter>().mesh = new Mesh();
            //���������ײ��
            if (useCollider)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            //����������
            hexMesh.name = "Hex Mesh";
        }
    }
}