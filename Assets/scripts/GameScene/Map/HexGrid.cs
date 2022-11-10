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
        /// 地图区块宽
        /// </summary>
        public int chunkCountX = 4;
        /// <summary>
        /// 地图区块高
        /// </summary>
        public int chunkCountZ = 3; 
        /// <summary>
        /// 地图宽的节点数
        /// </summary>
        private int cellCountX;
        /// <summary>
        /// 地图高的节点数
        /// </summary>
        private int cellCountZ;
        /// <summary>
        /// 地图区块预设体
        /// </summary>
        public HexGridChunk chunkPrefab;
        /// <summary>
        /// 节点预设体
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// 节点储存
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// 节点标签预设体
        /// </summary>
        public Text cellLabelPrefab;
        /// <summary>
        /// 地图区块集
        /// </summary>
        private HexGridChunk[] chunks;

        /// <summary>
        /// 创建地图区块
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
        /// 创建地图节点
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
        /// 创建新的节点
        /// </summary>
        /// <param name="x">节点x轴</param>
        /// <param name="z">节点z轴</param>
        /// <param name="i">节点序号</param>
        void CreateCell(int x, int z, int i)
        {
            //计算节点世界位置
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            //新建节点
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
            //新建节点标签
            Text label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            cell.uiRect = label.rectTransform;
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }
        /// <summary>
        /// 添加节点到指定区块
        /// </summary>
        /// <param name="x">节点x坐标</param>
        /// <param name="z">节点z坐标</param>
        /// <param name="cell">要添加的节点</param>
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
            //计算地图总结点长宽
            cellCountX = chunkCountX * HexMetrics.chunkSizeX;
            cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
            //创建地图区块
            CreateChunks();
            //创建地图节点
            CreateCells();
        }

        /// <summary>
        /// 点击地图节点
        /// </summary>
        /// <param name="position"></param>
        public HexCell GetCell(Vector3 position)
        {
            //换算坐标系到六边形三维坐标
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            return cells[index];
        }
    }
}
