using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        /// 当前寻路序号
        /// </summary>
        private int searchFrontierPhase;
        /// <summary>
        /// 当前寻路的起点和终点
        /// </summary>
        private HexCell currentPathFrom, currentPathTo;
        /// <summary>
        /// 当前寻路起点和终点之间是否存在路径
        /// </summary>
        private bool currentPathExists;
        /// <summary>
        /// 地图上存在的单位集合
        /// </summary>
        private List<HexMapUnit> units;
        /// <summary>
        /// 
        /// </summary>
        private HexCellShaderData cellShaderData;

        /// <summary>
        /// 随机数种子
        /// </summary>
        public int seed; 
        /// <summary>
        /// 噪声纹理
        /// </summary>
        public Texture2D noiseSource; 
        /// <summary>
        /// 地图单位预制件
        /// </summary>
        public HexMapUnit unitPrefab;
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
        /// 存在路径属性
        /// 读 : 返回选择的起点和目的地之间是否存在路径
        /// </summary>
        public bool HasPath
        {
            get
            {
                return currentPathExists;
            }
        }

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
            cell.Index = i;
            cell.ShaderData = cellShaderData;
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
        /// 路径搜索
        /// </summary>
        /// <param name="fromCell">搜索起点</param>
        /// <param name="toCell">搜索终点</param>
        /// <param name="speed">可使用的代价</param>
        private bool Search(HexCell fromCell, HexCell toCell, HexMapUnit unit)
        {
            int speed = unit.speedPerTurn;
            //更新寻路阶段
            searchFrontierPhase += 2;
            //初始化优先队列
            if (searchFrontier is null)
            {
                searchFrontier = new HexCellPriorityQueue();
            }
            else
            {
                searchFrontier.Clear();
            }
            //更新起点
            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            //将起点放入队列
            searchFrontier.Enqueue(fromCell);
            //当前单元至邻居的距离
            int distance;
            //旧的单元优先级，用于更改优先队列元素
            int oldPriority;
            //当前单元
            HexCell current;
            //当前单元的邻居
            HexCell neighbor;
            //开始搜索
            while (searchFrontier.Count > 0)
            {
                //获得距离最近的单元
                current = searchFrontier.Dequeue();
                //更新搜索阶段
                current.SearchPhase += 1;
                //如果已超出可移动范围则跳出
                if (current.Distance > speed) return false;
                //判断是否到达终点
                if (current == toCell)
                {
                    return true; ;
                }
                //遍历邻居
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {

                    neighbor = current.GetNeighbor(d);

                    if (
                        neighbor is null
                        || neighbor.IsUnderwater == true
                        || neighbor.SearchPhase > searchFrontierPhase
                        )
                    {
                        continue;
                    }
                    if (!unit.IsValidDestination(neighbor))
                    {
                        continue;
                    }
                    distance = current.Distance;
                    distance += unit.GetMoveCost(current, neighbor, d);
                    if (distance < 0)
                    {
                        continue;
                    }
                    //
                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {//邻居的搜索阶段小于当前的搜索阶段则为未搜索过的节点
                        //更新搜索阶段
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        //更新理想代价
                        neighbor.SearchHeuristic = 5 * neighbor.coordinates.DistanceTo(toCell.coordinates);
                        //加入队列
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {//已搜索过的节点但存在距离更小的路径
                        //保存旧的优先级
                        oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        //更改邻居的优先级
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获得指定位置视野内的单元集合
        /// </summary>
        private List<HexCell> GetVisibleCells(HexCell fromCell, int range)
        {
            List<HexCell> visibleCells = ListPool<HexCell>.Get();

            //更新寻路阶段
            searchFrontierPhase += 2;
            //初始化优先队列
            if (searchFrontier is null)
            {
                searchFrontier = new HexCellPriorityQueue();
            }
            else
            {
                searchFrontier.Clear();
            }
            //更新起点
            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            //将起点放入队列
            searchFrontier.Enqueue(fromCell);
            //当前单元至邻居的距离
            int distance;
            //旧的单元优先级，用于更改优先队列元素
            int oldPriority;
            //当前单元
            HexCell current;
            //当前单元的邻居
            HexCell neighbor;
            //开始搜索
            while (searchFrontier.Count > 0)
            {
                //获得距离最近的单元
                current = searchFrontier.Dequeue();
                //更新搜索阶段
                current.SearchPhase += 1;

                visibleCells.Add(current);
                //遍历邻居
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {

                    neighbor = current.GetNeighbor(d);

                    if (
                            neighbor is null ||
                            neighbor.SearchPhase > searchFrontierPhase
                        )
                    {
                        continue;
                    }
                    //初始距离为当前单元格的距离
                    distance = current.Distance + 1;
                    if (distance > range)
                    {
                        continue;
                    }
                    //
                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {//邻居的搜索阶段小于当前的搜索阶段则为未搜索过的节点
                        //更新搜索阶段
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        //更新理想代价
                        neighbor.SearchHeuristic = 0;
                        //加入队列
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {//已搜索过的节点但存在距离更小的路径
                        //保存旧的优先级
                        oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        //更改邻居的优先级
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }
            return visibleCells;
        }
        /// <summary>
        /// 显示路径
        /// </summary>
        private void ShowPath()
        {
            if (currentPathExists == true)
            {
                HexCell current = currentPathTo;
                while (current != currentPathFrom)
                {
                    current.SetLabel(current.Distance.ToString());
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
            }
            currentPathFrom.EnableHighlight(Color.blue);
            currentPathTo.EnableHighlight(Color.red);
        }
        /// <summary>
        /// 清除所有存在的单位
        /// </summary>
        private void ClearUnits()
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Die();
            }
            units.Clear();
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
            //保存地图大小
            writer.Write(cellCountX);
            writer.Write(cellCountZ);
            //保存单元
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Save(writer);
            }
            //保存单位
            writer.Write(units.Count);
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Save(writer);
            }
        }
        /// <summary>
        /// 地图网格数据读取
        /// </summary>
        public void Load(BinaryReader reader, int header)
        {
            ClearPath();
            ClearUnits();

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
                cells[i].Load(reader, header);
            }
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].Refresh();
            }

            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; i++)
            {
                HexMapUnit.Load(reader, this);
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
            ClearPath();
            ClearUnits();
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
            //更新贴图
            cellShaderData.Initialize(cellCountX, cellCountZ);
            //创建地图区块
            CreateChunks();
            //创建地图单元
            CreateCells();
            //创建地图成功
            return true;
        }
        /// <summary>
        /// 清除显示的路径
        /// </summary>
        public void ClearPath()
        {
            if (currentPathExists == true)
            {
                HexCell current = currentPathTo;
                while (current != currentPathFrom)
                {
                    current.SetLabel("");
                    current.DisableHighlight();
                    current = current.PathFrom;
                }
                current.DisableHighlight();
                currentPathExists = false;
            }
            else if ((currentPathFrom is null) == false && (currentPathTo is null) == false)
            {
                currentPathFrom.DisableHighlight();
                currentPathTo.DisableHighlight();
            }
            currentPathFrom = currentPathTo = null;
        }
        /// <summary>
        /// 寻找两个单元间的最短路径
        /// </summary>
        /// <param name="fromCell">搜索起点</param>
        /// <param name="toCell">搜索终点</param>
        /// <param name="speed">可使用的代价</param>
        public void FindPath(HexCell fromCell, HexCell toCell, HexMapUnit unit)
        {
            ClearPath();
            currentPathFrom = fromCell;
            currentPathTo = toCell;
            currentPathExists = Search(fromCell, toCell, unit);
            ShowPath();
        }
        /// <summary>
        /// 添加新的单位到单位集合
        /// </summary>
        public void AddUnit(HexMapUnit unit, HexCell location)
        {
            units.Add(unit);
            unit.Grid = this;
            unit.transform.SetParent(transform, false);
            unit.CurrentCell = location;
        }
        /// <summary>
        /// 从集合中移除单位
        /// </summary>
        public void RemoveUnit(HexMapUnit unit)
        {
            units.Remove(unit);
            unit.Die();
        }
        /// <summary>
        /// 射线检测获得第一个单元
        /// </summary>
        public HexCell GetCell(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return GetCell(hit.point);
            }
            return null;
        }
        /// <summary>
        /// 获得路径
        /// </summary>
        /// <returns></returns>
        public List<HexCell> GetPath()
        {
            if (!currentPathExists)
            {
                return null;
            }
            List<HexCell> path = ListPool<HexCell>.Get();
            for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom)
            {
                path.Add(c);
            }
            path.Add(currentPathFrom);
            //倒序 因为是从目的地回溯到起点
            path.Reverse();
            return path;
        }
        /// <summary>
        /// 增加视野内的单元可见度
        /// </summary>
        public void IncreaseVisibility(HexCell fromCell, int range)
        {
            List<HexCell> cells = GetVisibleCells(fromCell, range);
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].IncreaseVisibility();
            }
            ListPool<HexCell>.Add(cells);
        }
        /// <summary>
        /// 降低视野内的单元可见度
        /// </summary>
        public void DecreaseVisibility(HexCell fromCell, int range)
        {
            List<HexCell> cells = GetVisibleCells(fromCell, range);
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].DecreaseVisibility();
            }
            ListPool<HexCell>.Add(cells);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            HexMetrics.noiseSource = noiseSource;

            HexMetrics.InitializeHashGrid(seed);

            HexFeatureManager.InitfeatureCollection(featureCollections);

            HexMetrics.InitializeHashGrid(seed);

            HexMapUnit.unitPrefab = unitPrefab;

            units = ListPool<HexMapUnit>.Get();

            cellShaderData = gameObject.AddComponent<HexCellShaderData>();

            CreateMap(cellCountX, cellCountZ);
        }
    }
}
