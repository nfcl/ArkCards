using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// ��ͼ�����
        /// </summary>
        public int chunkCountX = 4;
        /// <summary>
        /// ��ͼ�����
        /// </summary>
        public int chunkCountZ = 3; 
        /// <summary>
        /// ��ͼ��Ľڵ���
        /// </summary>
        private int cellCountX;
        /// <summary>
        /// ��ͼ�ߵĽڵ���
        /// </summary>
        private int cellCountZ;
        /// <summary>
        /// ��ͼ����Ԥ����
        /// </summary>
        public HexGridChunk chunkPrefab;
        /// <summary>
        /// �ڵ�Ԥ����
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// �ڵ㴢��
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// �ڵ��ǩԤ����
        /// </summary>
        public Text cellLabelPrefab;
        /// <summary>
        /// ��ͼ���鼯
        /// </summary>
        private HexGridChunk[] chunks;

        /// <summary>
        /// ������ͼ����
        /// </summary>
        private void CreateChunks()
        {
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            for (int z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (int x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }
        /// <summary>
        /// ������ͼ�ڵ�
        /// </summary>
        void CreateCells()
        {

            cells = new HexCell[cellCountZ * cellCountX];

            for (int z = 0, i = 0; z < cellCountZ; z++)
            {
                for (int x = 0; x < cellCountX; x++)
                {

                    CreateCell(x, z, i++);
                }
            }
        }
        /// <summary>
        /// �����µĽڵ�
        /// </summary>
        /// <param name="x">�ڵ�x��</param>
        /// <param name="z">�ڵ�z��</param>
        /// <param name="i">�ڵ����</param>
        void CreateCell(int x, int z, int i)
        {
            //����ڵ�����λ��
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            //�½��ڵ�
            HexCell cell = cells[i] = Instantiate(cellPrefab);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = Color.white;
            //�������ڹ�ϵ
            if (x > 0)
            {
                //�����Լ���ࣨW�����ھ�
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {//�ǵ�һ��
                if ((z & 1) == 0)
                {//ż����
                    //�����Լ������²ࣨSE�����ھ�
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]); 
                    if (x > 0)
                    {//�ǵ�һ��
                        //�����Լ������²ࣨSW�����ھ�
                        cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                else
                {//������
                    //�����Լ������²ࣨSW�����ھ�
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1)
                    {//�����һ��
                        //�����Լ������²ࣨSE�����ھ�
                        cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }
            //�½��ڵ��ǩ
            Text label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            cell.uiRect = label.rectTransform;
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }
        /// <summary>
        /// ��ӽڵ㵽ָ������
        /// </summary>
        /// <param name="x">�ڵ�x����</param>
        /// <param name="z">�ڵ�z����</param>
        /// <param name="cell">Ҫ��ӵĽڵ�</param>
        void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        void Awake()
        {
            //�����ͼ�ܽ�㳤��
            cellCountX = chunkCountX * HexMetrics.chunkSizeX;
            cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
            //������ͼ����
            CreateChunks();
            //������ͼ�ڵ�
            CreateCells();
        }

        /// <summary>
        /// �����ͼ�ڵ�
        /// </summary>
        /// <param name="position"></param>
        public HexCell GetCell(Vector3 position)
        {
            //��������ϵ����������ά����
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            return cells[index];
        }
    }
}
