using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// 地形颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 邻居
        /// </summary>
        private HexCell[] neighbors;
        /// <summary>
        /// 地形高度
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// 是否存在河流入口
        /// </summary>
        private bool hasIncomingRiver;
        /// <summary>
        /// 是否存在河流出口
        /// </summary>
        private bool hasOutgoingRiver;
        /// <summary>
        /// 河流入口方向
        /// </summary>
        private HexDirection incomingRiver;
        /// <summary>
        /// 河流出口方向
        /// </summary>
        private HexDirection outgoingRiver;
        /// <summary>
        /// 六个方向是否存在道路
        /// </summary>
        [SerializeField]
        private bool[] roads;
        /// <summary>
        /// 水面高度
        /// </summary>
        private int waterLevel;
        /// <summary>
        /// 细节密度级别
        /// </summary>
        private int urbanLevel;

        /// <summary>
        /// 坐标
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// <para/>单元所属的区块
        /// </summary>
        public HexGridChunk chunk;
        /// <summary>
        /// UI的RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// <para/>高度属性
        /// <para/>读 ：返回高度
        /// <para/>写 ：写入高度，设置实体和UI的Y轴位置，并对河流的合法性进行检测，刷新区块
        /// </summary>
        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                //相同高度不需要刷新
                if (elevation == value)
                {
                    return;
                }
                //设置高度
                elevation = value;
                //设置单元y轴位置
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //对y轴进行噪声扰动
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //设置单元UI位置
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //移除非法的河流出入口
                ValidateRivers();
                //判断各方向道路的合法性
                for (int i = 0; i < roads.Length; i++)
                {
                    //判断此方向道路合法性
                    if
                    (
                        //存在道路
                        roads[i] == true
                        //与对应方向邻居的高度差过大
                        && GetElevationDifference((HexDirection)i) > 1
                    )
                    {
                        //清除此方向道路
                        SetRoad(i, false);
                    }
                }

                //更改后进行刷新
                Refresh();
            }
        }
        /// <summary>
        /// <para/>单元位置属性
        /// <para/>读 ：返回实体以父物体为坐标系的位置
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        /// <summary>
        /// <para/>单元颜色属性
        /// <para/>读 ：返回地形颜色
        /// <para/>写 ：设置颜色并刷新区块
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                {
                    return;
                }
                color = value;
                Refresh();
            }
        }
        /// <summary>
        /// <para/>是否存在河流入口属性
        /// <para/>读 : 返回是否存在河流入口
        /// </summary>
        public bool HasIncomingRiver
        {
            get
            {
                return hasIncomingRiver;
            }
        }
        /// <summary>
        /// <para/>是否存在河流出口属性
        /// <para/>读 : 返回是否存在河流出口
        /// </summary>
        public bool HasOutgoingRiver
        {
            get
            {
                return hasOutgoingRiver;
            }
        }
        /// <summary>
        /// <para/>河流入口方向属性
        /// <para/>读 : 返回河流入口方向
        /// </summary>
        public HexDirection IncomingRiver
        {
            get
            {
                return incomingRiver;
            }
        }
        /// <summary>
        /// <para/>河流出口方向属性
        /// <para/>读 : 返回河流出口方向
        /// </summary>
        public HexDirection OutgoingRiver
        {
            get
            {
                return outgoingRiver;
            }
        }
        /// <summary>
        /// <para/>单元是否存在河流属性
        /// <para/>读 : 返回是否存在河流入口或出口
        /// </summary>
        public bool HasRiver
        {
            get
            {
                return hasIncomingRiver || hasOutgoingRiver;
            }
        }
        /// <summary>
        /// <para/>单元是否是河流源头或终点属性
        /// <para/>读 : 返回河流出口或河流入口是否不同时存在
        /// </summary>
        public bool HasRiverBeginOrEnd
        {
            get
            {
                return hasIncomingRiver != hasOutgoingRiver;
            }
        }
        /// <summary>
        /// 单元河流入口或出口方向
        /// 读 : 存在河流入口则返回入口方向，否则返回出口方向
        /// </summary>
        public HexDirection RiverBeginOrEndDirection
        {
            get
            {
                return hasIncomingRiver ? incomingRiver : outgoingRiver;
            }
        }
        /// <summary>
        /// <para/>河床底的Y轴坐标属性
        /// <para/>读 : 返回计算后的河床底Y轴坐标
        /// </summary>
        public float StreamBedY
        {
            get
            {
                return
                    (elevation + HexMetrics.streamBedElevationOffset) *
                    HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>河流表面Y轴坐标属性
        /// <para/>读 : 返回计算后的河流表面Y轴坐标
        /// </summary>
        public float RiverSurfaceY
        {
            get
            {
                return
                    (elevation + HexMetrics.waterElevationOffset) *
                    HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>该单元是否存在道路属性
        /// <para/>读 : 返回六个方向是否有任意方向存在道路
        /// </summary>
        public bool HasRoads
        {
            get
            {
                for (int i = 0; i < roads.Length; i++)
                {
                    if (roads[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// <para/>水面高度属性
        /// <para/>读 : 返回当前单元的水面高度
        /// <para/>写 : 设置当前单元的水面高度并刷新档期那区块
        /// </summary>
        public int WaterLevel
        {
            get
            {
                return waterLevel;
            }
            set
            {
                if (waterLevel == value)
                {
                    return;
                }
                waterLevel = value;
                ValidateRivers();
                Refresh();
            }
        }
        /// <summary>
        /// <para/>单元是否被水面淹没属性
        /// <para/>读 : 返回水面高度是否高于地形高度
        /// </summary>
        public bool IsUnderwater
        {
            get
            {
                return waterLevel > elevation;
            }
        }
        /// <summary>
        /// <para/>被淹没的单元的水面Y轴坐标
        /// <para/>读 : 返回计算后的Y轴坐标
        /// </summary>
        public float WaterSurfaceY
        {
            get
            {
                return
                    (waterLevel + HexMetrics.waterElevationOffset)
                    * HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>细节密度级别属性
        /// <para/>读 : 获得当前单元的密度级别
        /// <para/>写 : 设置密度级别并刷新当前区块
        /// </summary>
        public int UrbanLevel
        {
            get
            {
                return urbanLevel;
            }
            set
            {
                if (urbanLevel != value)
                {
                    urbanLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        /// <summary>
        /// 刷新本区块和邻居所属的不同区块
        /// </summary>
        private void Refresh()
        {
            if (chunk is null) return;
            //刷新所属区块
            chunk.Refresh();
            //如果相邻单元有不是同一个区块的则要同时刷新其所属区块
            for (int i = 0; i < neighbors.Length; i++)
            {

                HexCell neighbor = neighbors[i];
                //无邻居或邻居所属区块与自己所属区块相同则不需要刷新
                if (neighbor is null || neighbor.chunk == chunk) continue;

                neighbor.chunk.Refresh();
            }
        }
        /// <summary>
        /// 只刷新本区块
        /// </summary>
        private void RefreshSelfOnly()
        {
            chunk.Refresh();
        }
        /// <summary>
        /// 设置对应方向的道路
        /// </summary>
        /// <param name="index">道路下标</param>
        /// <param name="state">道路是否存在</param>
        private void SetRoad(int index, bool state)
        {
            //设置对应方向的道路
            roads[index] = state;
            //设置邻居对应方向的道路
            neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
            //刷新
            neighbors[index].RefreshSelfOnly();
            RefreshSelfOnly();
        }

        /// <summary>
        /// 获得对应方向的邻居
        /// </summary>
        /// <param name="direction">指定的方向</param>
        /// <returns>返回对应方向的邻居</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// 设置对应方向的邻居
        /// </summary>
        /// <param name="direction">指定的方向</param>
        /// <param name="cell">要设置成的邻居</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            //设置自己的邻居
            neighbors[(int)direction] = cell;
            //设置邻居的邻居
            cell.neighbors[(int)direction.Opposite()] = this;
        }
        /// <summary>
        /// 获得和对应方向邻居的边缘连接类型
        /// </summary>
        /// <param name="direction">对应方向</param>
        /// <returns>返回边缘连接类型</returns>
        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }
        /// <summary>
        /// 检测某方向的边缘是否有河流经过
        /// </summary>
        /// <param name="direction">指定方向</param>
        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return
                hasIncomingRiver && incomingRiver == direction ||
                hasOutgoingRiver && outgoingRiver == direction;
        }
        /// <summary>
        /// 移除河流出口
        /// </summary>
        public void RemoveOutgoingRiver()
        {
            //如果不存在河流出口则返回
            if (!hasOutgoingRiver) return;
            //清除河流出口
            hasOutgoingRiver = false;
            //刷新
            RefreshSelfOnly();
            //清除对应方向邻居的河流入口
            HexCell neighbor = GetNeighbor(outgoingRiver);
            neighbor.hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// 移除河流入口
        /// </summary>
        public void RemoveIncomingRiver()
        {
            //如果不存在河流入口则返回
            if (!hasIncomingRiver) return;
            //清除河流入口
            hasIncomingRiver = false;
            //刷新
            RefreshSelfOnly();
            //清除对应邻居的河流出口
            HexCell neighbor = GetNeighbor(incomingRiver);
            neighbor.hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// 移除河流
        /// </summary>
        public void RemoveRiver()
        {
            //移除河流出口
            RemoveOutgoingRiver();
            //移除河流入口
            RemoveIncomingRiver();
        }
        /// <summary>
        /// 将河流出口设置在某方向
        /// </summary>
        /// <param name="direction">要设置河流出口的方向</param>
        public void SetOutgoingRiver(HexDirection direction)
        {
            //如果在要设置的方向已存在河流出口则返回
            if (hasOutgoingRiver && outgoingRiver == direction)
            {
                return;
            }
            //获得对应方向的邻居
            HexCell neighbor = GetNeighbor(direction);
            //如果邻居不存在或者邻居的高度大于本单元高度（河流上坡）则返回

            if (!IsValidRiverDestination(neighbor))
            {
                return;
            }
            //移除河流出口
            RemoveOutgoingRiver();
            //如果要设置的方向已存在河流入口则先移除河流入口
            if (hasIncomingRiver && incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }
            //设置河流出口
            hasOutgoingRiver = true;
            outgoingRiver = direction;
            //设置邻居的河流入口
            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            //清除此方向的道路
            SetRoad((int)direction, false);
        }
        /// <summary>
        /// 指定方向是否存在道路
        /// </summary>
        public bool HasRoadThroughEdge(HexDirection direction)
        {
            return roads[(int)direction];
        }
        /// <summary>
        /// 添加对应方向的道路
        /// </summary>
        public void AddRoad(HexDirection direction)
        {
            //判断是否可以在此方向设置道路
            if (roads[(int)direction] == false                          //此方向不存在道路
                && HasRiverThroughEdge(direction) == false     //此方向不存在河流
                && GetElevationDifference(direction) <= 1)     //此方向的与邻居的高度差较小
            {
                SetRoad((int)direction, true);
            }
        }
        /// <summary>
        /// 清除所有道路
        /// </summary>
        public void RemoveRoads()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (true == roads[i])
                {
                    //清除道路
                    SetRoad(i, false);
                }
            }
        }
        /// <summary>
        /// 获得自己和对应方向邻居的高度差
        /// </summary>
        public int GetElevationDifference(HexDirection direction)
        {
            int difference = elevation - GetNeighbor(direction).elevation;
            return difference >= 0 ? difference : -difference;
        }
        /// <summary>
        /// 检查邻居是否是河流出口的有效目的地
        /// </summary>
        private bool IsValidRiverDestination(HexCell neighbor)
        {
            return
                neighbor
                && (
                        elevation >= neighbor.elevation
                        || waterLevel == neighbor.elevation
                    )
                ;
        }
        /// <summary>
        /// 移除非法的河流出入口
        /// </summary>
        private void ValidateRivers()
        {
            if (
                hasOutgoingRiver == true
                && IsValidRiverDestination(GetNeighbor(outgoingRiver)) == false
            )
            {
                RemoveOutgoingRiver();
            }
            if (
                hasIncomingRiver == true
                && GetNeighbor(incomingRiver).IsValidRiverDestination(this) == false
            )
            {
                RemoveIncomingRiver();
            }
        }

        /// <summary>
        /// 加载脚本实例时调用 Awake
        /// </summary>
        public void Awake()
        {
            neighbors = new HexCell[6];
        }
    }
}