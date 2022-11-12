using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// 单元坐标
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// 单元颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 单元的邻居
        /// </summary>
        private HexCell[] neighbors;
        /// <summary>
        /// 单元高度
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// 单元是否存在河流入口
        /// </summary>
        private bool hasIncomingRiver;
        /// <summary>
        /// 单元是否存在河流出口
        /// </summary>
        private bool hasOutgoingRiver;
        /// <summary>
        /// 单元河流入口方向
        /// </summary>
        private HexDirection incomingRiver;
        /// <summary>
        /// 单元河流出口方向
        /// </summary>
        private HexDirection outgoingRiver;
        /// <summary>
        /// 单元UI的RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// 单元高度属性
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
                //设置单元位置
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //对y轴进行噪声扰动
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //设置单元UI位置
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //如果更改高度后的河流出口在上坡则清除河流出口
                if (
                    hasOutgoingRiver &&
                    elevation < GetNeighbor(outgoingRiver).elevation
                )
                {
                    RemoveOutgoingRiver();
                }
                //如果更改高度后的河流入口在上坡则清除河流入口
                if (
                    hasIncomingRiver &&
                    elevation > GetNeighbor(incomingRiver).elevation
                )
                {
                    RemoveIncomingRiver();
                }
                //更改后进行刷新
                Refresh();
            }
        }
        /// <summary>
        /// 单元位置访问器
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        /// <summary>
        /// 单元颜色属性
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
        /// 所属区块
        /// </summary>
        public HexGridChunk chunk;
        /// <summary>
        /// 是否存在河流入口
        /// 读 : 返回是否存在河流入口
        /// </summary>
        public bool HasIncomingRiver { get { return hasIncomingRiver; } }
        /// <summary>
        /// 是否存在河流出口
        /// 读 : 返回是否存在河流出口
        /// </summary>
        public bool HasOutgoingRiver { get { return hasOutgoingRiver; } }
        /// <summary>
        /// 河流入口方向
        /// 读 : 返回河流入口方向
        /// </summary>
        public HexDirection IncomingRiver { get { return incomingRiver; } }
        /// <summary>
        /// 河流出口方向
        /// 读 : 返回河流出口方向
        /// </summary>
        public HexDirection OutgoingRiver { get { return outgoingRiver; } }
        /// <summary>
        /// 单元是否存在河流
        /// 读 : 返回是否存在河流入口或出口
        /// </summary>
        public bool HasRiver { get { return hasIncomingRiver || hasOutgoingRiver; } }
        /// <summary>
        /// 单元是否是河流源头或终点
        /// 读 : 返回河流出口或河流入口是否不同时存在
        /// </summary>
        public bool HasRiverBeginOrEnd { get { return hasIncomingRiver != hasOutgoingRiver; } }

        /// <summary>
        /// 区块刷新
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
        /// 根据方向获得相邻的单元
        /// </summary>
        /// <param name="direction">相邻单元的方向</param>
        /// <returns>返回对饮的相邻单元</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// 根据方向设置相邻的单元
        /// </summary>
        /// <param name="direction">相邻单元的方向</param>
        /// <param name="cell">要设置的相邻单元</param>
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
        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }
        /// <summary>
        /// 某方向的边缘是否有河流经过
        /// </summary>
        /// <param name="direction">方向</param>
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
            if (hasOutgoingRiver && outgoingRiver == direction) return;
            //获得对应方向的邻居
            HexCell neighbor = GetNeighbor(direction);
            //如果邻居不存在或者邻居的高度大于本单元高度（河流上坡）则返回
            if (!neighbor || elevation < neighbor.elevation) return;
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
            //刷新
            RefreshSelfOnly();
            //设置邻居的河流入口
            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            neighbors = new HexCell[6];
        }
    }
}