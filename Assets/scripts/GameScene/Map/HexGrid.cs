using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// 地图宽的单元数
        /// </summary>
        private int cellCountX;
        /// <summary>
        /// 地图高的单元数
        /// </summary>
        private int cellCountZ;
        /// <summary>
        /// 单元集合
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// 地图区块集合
        /// </summary>
        private HexGridChunk[] chunks;

        /// <summary>
        /// 随机数种子
        /// </summary>
        public int seed;
        /// <summary>
        /// 噪声纹理
        /// </summary>
        public Texture2D noiseSource;
        /// <summary>
        /// 单元标签预设体
        /// </summary>
        public Text cellLabelPrefab;
        /// <summary>
        /// 地图区块预设体
        /// </summary>
        public HexGridChunk chunkPrefab;
        /// <summary>
        /// 单元预设体
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// 地图水平区块个数
        /// </summary>
        public int chunkCountX = 4;
        /// <summary>
        /// 地图垂直区块个数
        /// </summary>
        public int chunkCountZ = 3;

        /// <summary>
        /// 创建地图区块集合
        /// </summary>
        private void CreateChunks()
        {
            //实例化列表
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            int x;  //区块的x坐标
            int z;  //区块的z坐标
            int i;  //区块下标

            //遍历创建区块
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
        /// 创建地图单元集合
        /// </summary>
        private void CreateCells()
        {
            //实例化列表
            cells = new HexCell[cellCountZ * cellCountX];

            int x;  //单元x坐标
            int z;  //单元z坐标
            int i;  //单元下标

            //遍历创建单元
            for (z = 0, i = 0; z < cellCountZ; z++)
            {
                for (x = 0; x < cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }
        /// <summary>
        /// 创建新的单元
        /// </summary>
        /// <param name="x">单元x轴</param>
        /// <param name="z">单元z轴</param>
        /// <param name="i">单元下标</param>
        private void CreateCell(int x, int z, int i)
        {
            //计算单元世界位置
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            //新建单元
            HexCell cell = cells[i] = Instantiate(cellPrefab);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = Color.white;
            //设置相邻关系
            if (x > 0)
            {
                //设置自己左侧（W）的邻居
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {//非第一行
                if ((z & 1) == 0)
                {//偶数行
                    //设置自己的右下侧（SE）的邻居
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                    if (x > 0)
                    {//非第一列
                        //设置自己的左下侧（SW）的邻居
                        cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                else
                {//奇数行
                    //设置自己的左下侧（SW）的邻居
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1)
                    {//非最后一个
                        //设置自己的右下侧（SE）的邻居
                        cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }
            //新建单元标签
            Text label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            //设置UI的RectTransform
            cell.uiRect = label.rectTransform;
            //设置单元高度
            cell.Elevation = 0;
            //添加单元到所属区块
            AddCellToChunk(x, z, cell);
        }
        /// <summary>
        /// 添加单元到指定区块
        /// </summary>
        /// <param name="x">单元x坐标</param>
        /// <param name="z">单元z坐标</param>
        /// <param name="cell">要添加的单元</param>
        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            //获得对应的区块
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
            //添加单元
            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        /// <summary>
        /// 根据世界坐标计算获得对应的地图单元
        /// </summary>
        /// <param name="position">点击的世界坐标</param>
        public HexCell GetCell(Vector3 position)
        {
            //换算坐标系到六边形三维坐标
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            return cells[coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2];
        }
        /// <summary>
        /// 根据单元坐标获得对应的地图单元
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
        /// 使用偏移后的二维坐标获得地图单元
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
        /// 显示单元格UI
        /// </summary>
        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].ShowUI(visible);
            }
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
            }
            HexMetrics.InitializeHashGrid(seed);
            //计算地图总结点长宽
            cellCountX = chunkCountX * HexMetrics.chunkSizeX;
            cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
            //创建地图区块
            CreateChunks();
            //创建地图单元
            CreateCells();
        }
    }
}
