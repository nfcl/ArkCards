using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// 地图水平区块个数
        /// </summary>
        private int chunkCountX;
        /// <summary>
        /// 地图垂直区块个数
        /// </summary>
        private int chunkCountZ;
        /// <summary>
        /// 单元集合
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// 地图区块集合
        /// </summary>
        private HexGridChunk[] chunks;
        /// <summary>
        /// 寻路使用的优先队列
        /// </summary>
        private HexCellPriorityQueue searchFrontier;

        /// <summary>
        /// 随机数种子
        /// </summary>
        public int seed;
        /// <summary>
        /// 噪声纹理
        /// </summary>
        public Texture2D noiseSource;
        /// <summary>
        /// 地形细节集合
        /// </summary>
        public HexFeatureCollection[] featureCollections;
        /// <summary>
        /// 单元标签预设体
        /// </summary>
        public GameObject cellLabelPrefab;
        /// <summary>
        /// 地图区块预设体
        /// </summary>
        public HexGridChunk chunkPrefab;
        /// <summary>
        /// 单元预设体
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// 地图宽的单元数
        /// </summary>
        public int cellCountX;
        /// <summary>
        /// 地图高的单元数
        /// </summary>
        public int cellCountZ;

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
            cell.TerrainType = HexMetrics.HexTerrains[0];
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
            GameObject label = Instantiate(cellLabelPrefab);    
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x, position.z);
            //设置UI的RectTransform
            cell.uiRect = label.GetComponent<RectTransform>();
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
        /// 路径搜索可视化
        /// </summary>
        private IEnumerator SearchVisible(HexCell fromCell, HexCell toCell)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Distance = int.MaxValue;
                cells[i].DisableHighlight();
            }
            fromCell.EnableHighlight(Color.blue);
            toCell.EnableHighlight(Color.red);

            WaitForSeconds delay = new WaitForSeconds(0);

            List<HexCell> frontier = new List<HexCell>();
            fromCell.Distance = 0;
            frontier.Add(fromCell);
            int distance;
            while (frontier.Count > 0)
            {
                yield return delay;
                HexCell current = frontier[0];
                frontier.RemoveAt(0);

                if (current == toCell)
                {
                    current = current.PathFrom;
                    while (current != fromCell)
                    {
                        current.EnableHighlight(Color.white);
                        current = current.PathFrom;
                    }
                    break;
                }

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor is null
                        || neighbor.IsUnderwater == true
                        )
                    {
                        continue;
                    }

                    HexEdgeType edgeType = current.GetEdgeType(d);
                    if (edgeType == HexEdgeType.Cliff)
                    {
                        continue;
                    }

                    distance = current.Distance;
                    if (current.HasRoadThroughEdge(d))
                    {
                        distance += 1;
                    }
                    else
                    {
                        distance += edgeType == HexEdgeType.Flat ? 5 : 10;
                    }
                    if (distance < neighbor.Distance)
                    {
                        if (neighbor.Distance == int.MaxValue)
                        {
                            neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                        }
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        frontier.Add(neighbor);
                    }
                }
                frontier.Sort((x, y) => x.SearchPriority.CompareTo(y.SearchPriority));
            }
        }
        /// <summary>
        /// 路径搜索
        /// </summary>
        private void Search(HexCell fromCell, HexCell toCell)
        {
            if (searchFrontier is null)
            {
                searchFrontier = new HexCellPriorityQueue();
            }
            else
            {
                searchFrontier.Clear();
            }
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Distance = int.MaxValue;
                cells[i].DisableHighlight();
            }
            fromCell.EnableHighlight(Color.blue);
            toCell.EnableHighlight(Color.red);

            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            int distance;
            while (searchFrontier.Count > 0)
            {
                HexCell current = searchFrontier.Dequeue();

                if (current == toCell)
                {
                    current = current.PathFrom;
                    while (current != fromCell)
                    {
                        current.EnableHighlight(Color.white);
                        current = current.PathFrom;
                    }
                    break;
                }

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor is null
                        || neighbor.IsUnderwater == true
                        )
                    {
                        continue;
                    }

                    HexEdgeType edgeType = current.GetEdgeType(d);
                    if (edgeType == HexEdgeType.Cliff)
                    {
                        continue;
                    }

                    distance = current.Distance;
                    if (current.HasRoadThroughEdge(d))
                    {
                        distance += 1;
                    }
                    else
                    {
                        distance += edgeType == HexEdgeType.Flat ? 5 : 10;
                    }
                    if (neighbor.Distance == int.MaxValue)
                    {
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }
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
        /// 地图网格数据写入
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
        /// 地图网格数据读取
        /// </summary>
        public void Load(BinaryReader reader)
        {
            StopAllCoroutines();

            int x = reader.ReadInt32(), z = reader.ReadInt32();
            //判断和当前地图大小是否相同
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
        /// 创建新的地图网格
        /// </summary>
        public bool CreateMap(int x, int z)
        {
            //判断地图大小是否合法
            if (
                x <= 0 || z <= 0                    //存在负数
                || x % HexMetrics.chunkSizeX != 0   //x长度无法整除
                || z % HexMetrics.chunkSizeZ != 0   //z长度无法整除
            )
            {
#if UNITY_EDITOR
                Debug.LogError("Unsupported map size.");
#endif
                //创建地图失败
                return false;
            }
            //清除旧的数据
            if (chunks != null)
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    Destroy(chunks[i].gameObject);
                }
            }
            //计算地图总结点长宽
            cellCountX = x;
            cellCountZ = z;
            chunkCountX = cellCountX / HexMetrics.chunkSizeX;
            chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
            //创建地图区块
            CreateChunks();
            //创建地图单元
            CreateCells();
            //创建地图成功
            return true;
        }
        /// <summary>
        /// 寻找两个单元间的最短路径
        /// </summary>
        public void FindPath(HexCell fromCell, HexCell toCell)
        {
            ////停止旧的协程
            //StopAllCoroutines();
            ////开始新的协程
            //StartCoroutine(SearchVisible(fromCell, toCell));
            Search(fromCell, toCell);
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
