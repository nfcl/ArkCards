using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// ��ͼˮƽ�������
        /// </summary>
        private int chunkCountX;
        /// <summary>
        /// ��ͼ��ֱ�������
        /// </summary>
        private int chunkCountZ;
        /// <summary>
        /// ��Ԫ����
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// ��ͼ���鼯��
        /// </summary>
        private HexGridChunk[] chunks;

        /// <summary>
        /// ���������
        /// </summary>
        public int seed;
        /// <summary>
        /// ��������
        /// </summary>
        public Texture2D noiseSource;
        /// <summary>
        /// ����ϸ�ڼ���
        /// </summary>
        public HexFeatureCollection[] featureCollections;
        /// <summary>
        /// ��Ԫ��ǩԤ����
        /// </summary>
        public Text cellLabelPrefab;
        /// <summary>
        /// ��ͼ����Ԥ����
        /// </summary>
        public HexGridChunk chunkPrefab;
        /// <summary>
        /// ��ԪԤ����
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// ��ͼ��ĵ�Ԫ��
        /// </summary>
        public int cellCountX;
        /// <summary>
        /// ��ͼ�ߵĵ�Ԫ��
        /// </summary>
        public int cellCountZ;

        /// <summary>
        /// ������ͼ���鼯��
        /// </summary>
        private void CreateChunks()
        {
            //ʵ�����б�
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            int x;  //�����x����
            int z;  //�����z����
            int i;  //�����±�

            //������������
            for (z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }
        /// <summary>
        /// ������ͼ��Ԫ����
        /// </summary>
        private void CreateCells()
        {
            //ʵ�����б�
            cells = new HexCell[cellCountZ * cellCountX];

            int x;  //��Ԫx����
            int z;  //��Ԫz����
            int i;  //��Ԫ�±�

            //����������Ԫ
            for (z = 0, i = 0; z < cellCountZ; z++)
            {
                for (x = 0; x < cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }
        /// <summary>
        /// �����µĵ�Ԫ
        /// </summary>
        /// <param name="x">��Ԫx��</param>
        /// <param name="z">��Ԫz��</param>
        /// <param name="i">��Ԫ�±�</param>
        private void CreateCell(int x, int z, int i)
        {
            //���㵥Ԫ����λ��
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            //�½���Ԫ
            HexCell cell = cells[i] = Instantiate(cellPrefab);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.TerrinType = HexMetrics.HexTerrains[0];
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
            //�½���Ԫ��ǩ
            Text label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            //����UI��RectTransform
            cell.uiRect = label.rectTransform;
            //���õ�Ԫ�߶�
            cell.Elevation = 0;
            //��ӵ�Ԫ����������
            AddCellToChunk(x, z, cell);
        }
        /// <summary>
        /// ��ӵ�Ԫ��ָ������
        /// </summary>
        /// <param name="x">��Ԫx����</param>
        /// <param name="z">��Ԫz����</param>
        /// <param name="cell">Ҫ��ӵĵ�Ԫ</param>
        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            //��ö�Ӧ������
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
            //��ӵ�Ԫ
            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        /// <summary>
        /// ����������������ö�Ӧ�ĵ�ͼ��Ԫ
        /// </summary>
        /// <param name="position">�������������</param>
        public HexCell GetCell(Vector3 position)
        {
            //��������ϵ����������ά����
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            return cells[coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2];
        }
        /// <summary>
        /// ���ݵ�Ԫ�����ö�Ӧ�ĵ�ͼ��Ԫ
        /// </summary>
        public HexCell GetCell(int x, int z)
        {
            if (z < 0 || z >= cellCountZ)
            {
                return null;
            }
            if (x < 0 || x >= cellCountX)
            {
                return null;
            }
            return cells[x + z * cellCountX];
        }
        /// <summary>
        /// ʹ��ƫ�ƺ�Ķ�ά�����õ�ͼ��Ԫ
        /// </summary>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= cellCountZ)
            {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX)
            {
                return null;
            }
            return cells[x + z * cellCountX];
        }
        /// <summary>
        /// ��ʾ��Ԫ��UI
        /// </summary>
        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].ShowUI(visible);
            }
        }
        /// <summary>
        /// ��ͼ��������д��
        /// </summary>
        public void Save(BinaryWriter writer)
        {
            writer.Write(cellCountX);
            writer.Write(cellCountZ);

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Save(writer);
            }
        }
        /// <summary>
        /// ��ͼ�������ݶ�ȡ
        /// </summary>
        public void Load(BinaryReader reader)
        {
            int x = reader.ReadInt32(), z = reader.ReadInt32();
            //�жϺ͵�ǰ��ͼ��С�Ƿ���ͬ
            if (x != cellCountX || z != cellCountZ)
            {
                if (!CreateMap(x, z))
                {
                    return;
                }
            }
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Load(reader);
            }
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].Refresh();
            }
        }
        /// <summary>
        /// �����µĵ�ͼ����
        /// </summary>
        public bool CreateMap(int x, int z)
        {
            //�жϵ�ͼ��С�Ƿ�Ϸ�
            if (
                x <= 0 || z <= 0                    //���ڸ���
                || x % HexMetrics.chunkSizeX != 0   //x�����޷�����
                || z % HexMetrics.chunkSizeZ != 0   //z�����޷�����
            )
            {
#if UNITY_EDITOR
                Debug.LogError("Unsupported map size.");
#endif
                //������ͼʧ��
                return false;
            }
            //����ɵ�����
            if (chunks != null)
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    Destroy(chunks[i].gameObject);
                }
            }
            //�����ͼ�ܽ�㳤��
            cellCountX = x;
            cellCountZ = z;
            chunkCountX = cellCountX / HexMetrics.chunkSizeX;
            chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
            //������ͼ����
            CreateChunks();
            //������ͼ��Ԫ
            CreateCells();
            //������ͼ�ɹ�
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            if (HexMetrics.noiseSource is null)
            {
                HexMetrics.noiseSource = noiseSource;
                HexMetrics.InitializeHashGrid(seed);
                HexFeatureManager.InitfeatureCollection(featureCollections);
                HexMetrics.InitializeHashGrid(seed);
            }
            CreateMap(cellCountX, cellCountZ);
        }
    }
}
