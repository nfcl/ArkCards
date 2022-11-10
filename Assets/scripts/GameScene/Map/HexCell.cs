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
        public Color color; 
        /// <summary>
        /// 节点的邻居
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;
        /// <summary>
        /// 节点高度
        /// </summary>
        private int elevation;
        /// <summary>
        /// 节点UI的RectTransform
        /// </summary>
        public RectTransform uiRect;

        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                //设置高度
                elevation = value;
                //设置节点位置
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                transform.localPosition = position;
                //设置节点UI位置
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = elevation * -HexMetrics.elevationStep;
                uiRect.localPosition = uiPosition;
            }
        }

        public void Awake()
        {
            neighbors = new HexCell[6];
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
    }
}