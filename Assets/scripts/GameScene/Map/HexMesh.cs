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
    /// ���һ���������Ŷ��������ε�����
    /// </summary>
    private void AddTrianglePerturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    /// <summary>
    /// ���һ�����������Ŷ��������ε�����
    /// </summary>
    private void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
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
    private void AddTriangleColor(Color c)
    {
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
    }
    /// <summary>
    /// ���һ������������ɫ��ͬ�������ε�����������ɫ
    /// </summary>
    private void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    /// <summary>
    /// ���һ���ı��ε�����
    /// </summary>
    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        vertices.Add(Perturb(v4));
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
    private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
    private void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
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
    /// �������ĵ��������������
    /// </summary>
    /// <param name="cell">�ڵ�</param>
    private void Triangulate(HexCell cell)
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
    private void Triangulate(HexDirection direction, HexCell cell)
    {
        //�������ĵ�λ��
        Vector3 center = cell.Position;
        //����÷���Ĵ�ɫ���ͻ�ɫ����Ե
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );
        //�÷���ɫ�������������
        TriangulateEdgeFan(center, e, cell.color);
        //��Ӹ÷���Ļ�ɫ����������
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
    }
    /// <summary>
    /// �������ĵ�ͱ�Ե��ϸ�ֵ㼯��Ӵ�ɫ��ͬ�������������
    /// </summary>
    /// <param name="center">���ĵ�</param>
    /// <param name="edge">ϸ�ֵ㼯</param>
    /// <param name="color">��ɫ</param>
    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        AddTrianglePerturbed(center, edge.v1, edge.v2);
        AddTriangleColor(color);
        AddTrianglePerturbed(center, edge.v2, edge.v3);
        AddTriangleColor(color);
        AddTrianglePerturbed(center, edge.v3, edge.v4);
        AddTriangleColor(color);
    }
    /// <summary>
    /// ��������ϸ�ֵ�����о�����������
    /// </summary>
    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
    {
        AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        AddQuadColor(c1, c2);
        AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        AddQuadColor(c1, c2);
        AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        AddQuadColor(c1, c2);
    }
    /// <summary>
    /// ��ӽڵ��Ӧ��������Ӿ���
    /// </summary>
    /// <param name="direction">��Ӧ����</param>
    /// <param name="cell">�ڵ�</param>
    /// <param name="v1">��ɫ����ױ߶���1</param>
    /// <param name="e1">1/3�ȷֵ�</param>
    /// <param name="e2">2/3�ȷֵ�</param>
    /// <param name="v2">��ɫ����ױ߶���2</param>
    private void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
    {
        //��ö�Ӧ������ھ�
        HexCell neighbor = cell.GetNeighbor(direction);
        //���û���ھӷ���
        if (neighbor == null) return;
        //���㴿ɫ����ױߵ��������ױߴ��߷�������
        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        //�����ھӷ�����Ĵ�ɫ���ͻ�ɫ����Ե
        EdgeVertices e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v4 + bridge
        );
        //���ݱ�Ե�����������������
        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {//б�²���Ҫ����
            TriangulateEdgeTerraces(e1, cell, e2, neighbor);
        }
        else
        {//ƽ̹�������������Ӽ���
            TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);
        }
        //���˳ʱ�뷽�����һ���ھ�
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        //���������һ���ھ�����������ڵ��ཻ���Ļ����������
        //�����������������ɫʵΪ�����ڵ����ɫ
        //���Ʒ����ֹ�����ظ��Ļ������������
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            //������һ����Ͼ��ε�ͬ��ױ߶���
            Vector3 v5 = e1.v4 + HexMetrics.GetBridge(direction.Next());
            //���ø߶�Ϊ��һ���ھӵĸ߶�
            v5.y = nextNeighbor.Position.y;
            //������͵Ķ��㲢����������������
            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor
);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
            }
        }
    }
    /// <summary>
    /// ��Ͼ��α�Ե����
    /// </summary>
    /// <param name="begin">��ʼ��Ե</param>
    /// <param name="beginCell">��ʼ�ڵ�</param>
    /// <param name="end">������Ե</param>
    /// <param name="endCell">�����ڵ�</param>
    private void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell,EdgeVertices end, HexCell endCell)
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.color);
    }
    /// <summary>
    /// �����ڵ������������α�Ե����
    /// </summary>
    /// <param name="bottom">�߶���͵Ķ���λ��</param>
    /// <param name="bottomCell">�߶���͵Ľڵ�</param>
    /// <param name="left">˳ʱ�����һ������λ��</param>
    /// <param name="leftCell">˳ʱ�����һ���ڵ�</param>
    /// <param name="right">��ʱ�����һ������λ��</param>
    /// <param name="rightCell">��ʱ�����һ���ڵ�</param>
    private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        //����bottom����Ľڵ�����������ڵ�ı�Ե��������
        HexEdgeType leftEdgeType = HexMetrics.GetEdgeType(bottomCell.Elevation, leftCell.Elevation);
        HexEdgeType rightEdgeType = HexMetrics.GetEdgeType(bottomCell.Elevation, rightCell.Elevation);
        //���ݱ�Ե�������������������������
        if (leftEdgeType == HexEdgeType.Slope)
        {//���б��
            if (rightEdgeType == HexEdgeType.Slope)
            {//�Ҳ�б��
                TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if (rightEdgeType == HexEdgeType.Flat)
            {//�Ҳ�ƽ̹
                TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
            }
            else
            {//�Ҳ�����
                TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {//�Ҳ�б��
            if (leftEdgeType == HexEdgeType.Flat)
            {//���ƽ̹
                TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {//�������
                TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
        }
        //���Ҳ��Ե��Ϊ����
        else if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
        {//���Ҳ�ڵ��Ե����Ϊб��
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {
                TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
            }
        }
        else
        {
            AddTrianglePerturbed(bottom, left, right);
            AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
        }
    }
    /// <summary>
    /// �����ڵ����Ļ�������������б���ƽ�����ʹ���
    /// </summary>
    /// <param name="begin">�߶���͵Ķ���λ��</param>
    /// <param name="beginCell">�߶���͵Ľڵ�</param>
    /// <param name="left">˳ʱ�����һ������λ��</param>
    /// <param name="leftCell">˳ʱ�����һ���ڵ�</param>
    /// <param name="right">��ʱ�����һ������λ��</param>
    /// <param name="rightCell">��ʱ�����һ���ڵ�</param>
    void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        //�����һ��λ�õĲ�ֵ��
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);
        //������ʼ��͵�һ�Բ�ֵ���������
        AddTrianglePerturbed(begin, v3, v4);
        AddTriangleColor(beginCell.color, c3, c4);
        //��������ѭ����ֵ����
        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2, c3, c4);
        }
        //������ֹ������һ�Բ�ֵ��ľ���
        AddQuad(v3, v4, left, right);
        AddQuadColor(c3, c4, leftCell.color, rightCell.color);
    }
    /// <summary>
    /// �����ڵ����Ļ�������������б������´���
    /// </summary>
    /// <param name="begin">�߶���͵Ķ���λ��</param>
    /// <param name="beginCell">�߶���͵Ľڵ�</param>
    /// <param name="left">˳ʱ�����һ������λ��</param>
    /// <param name="leftCell">˳ʱ�����һ���ڵ�</param>
    /// <param name="right">��ʱ�����һ������λ��</param>
    /// <param name="rightCell">��ʱ�����һ���ڵ�</param>
    private void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        //������
        //float b = (rightCell.Elevation - beginCell.Elevation) / rightCell.Elevation;
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
        Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);
        //����б�º����µײ�����
        TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);
        //�ж����ҽڵ��ı�Ե�ж�
        if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
        {//б��
            //�������ҽڵ��б�½��ݱ�Ե����
            TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
        }
        else
        {//����
            //ֱ��������������
            AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
    }
    /// <summary>
    /// TriangulateCornerTerracesCliff�����Ҿ��淴ת
    /// </summary>
    /// <param name="begin">�߶���͵Ķ���λ��</param>
    /// <param name="beginCell">�߶���͵Ľڵ�</param>
    /// <param name="left">˳ʱ�����һ������λ��</param>
    /// <param name="leftCell">˳ʱ�����һ���ڵ�</param>
    /// <param name="right">��ʱ�����һ������λ��</param>
    /// <param name="rightCell">��ʱ�����һ���ڵ�</param>
    private void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
        Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
    }
    /// <summary>
    /// ���º�б�¼���������ǲ�ֵ����
    /// </summary>
    private void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor)
    {
        Vector3 v2 = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

        AddTriangleUnperturbed(Perturb(begin), v2, boundary);
        AddTriangleColor(beginCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            AddTriangleUnperturbed(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangleUnperturbed(v2, Perturb(left), boundary);
        AddTriangleColor(c2, leftCell.color, boundaryColor);
    }
    /// <summary>
    /// �Զ�����������Ŷ�
    /// </summary>
    /// <param name="position">ԭ����λ��</param>
    /// <returns>�����Ŷ���Ķ���λ��</returns>
    private Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
        //position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
        return position;
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