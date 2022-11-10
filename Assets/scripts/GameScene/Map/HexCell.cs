using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// 节点坐标
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// 节点颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 节点的邻居
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;
        /// <summary>
        /// 节点高度
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// 节点UI的RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// 节点高度属性
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
                //设置节点位置
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //对y轴进行噪声扰动
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //设置节点UI位置
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //更改后进行刷新
                Refresh();
            }
        }
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }

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

        public void Awake()
        {
            neighbors = new HexCell[6];
        }
        /// <summary>
        /// 区块刷新
        /// </summary>
        void Refresh()
        {
            if (chunk is null) return;
            //刷新所属区块
            chunk.Refresh();
            //如果相邻节点有不是同一个区块的则要同时刷新其所属区块
            for (int i = 0; i < neighbors.Length; i++)
            {

                HexCell neighbor = neighbors[i];
                //无邻居或邻居所属区块与自己所属区块相同则不需要刷新
                if (neighbor is null || neighbor.chunk == chunk) continue;

                neighbor.chunk.Refresh();
            }
        }
        /// <summary>
        /// 根据方向获得相邻的节点
        /// </summary>
        /// <param name="direction">相邻节点的方向</param>
        /// <returns>返回对饮的相邻节点</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// 根据方向设置相邻的节点
        /// </summary>
        /// <param name="direction">相邻节点的方向</param>
        /// <param name="cell">要设置的相邻节点</param>
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
    }
}