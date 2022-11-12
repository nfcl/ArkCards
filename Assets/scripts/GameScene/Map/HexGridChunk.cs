using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGridChunk : MonoBehaviour
    {
        /// <summary>
        /// 区块内的节点单元集
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// UI画布
        /// </summary>
        private Canvas gridCanvas;
        /// <summary>
        /// 网格
        /// </summary>
        public HexMesh terrain;

        /// <summary>
        /// 区块刷新
        /// </summary>
        public void Refresh()
        {
            //刷新后开启此脚本
            enabled = true;
        }
        /// <summary>
        /// 
        /// </summary>
        void LateUpdate()
        {
            //在LateUpdate进行刷新防止冲突
            terrain.Triangulate(cells);
            //刷新后再次关闭脚本
            enabled = false;
        }
        /// <summary>
        /// 添加节点到此区块
        /// </summary>
        /// <param name="index">要添加到的节点下标</param>
        /// <param name="cell">要添加的节点</param>
        public void AddCell(int index, HexCell cell)
        {
            //设置节点
            cells[index] = cell;
            //设置节点所属区块
            cell.chunk = this;
            //设置父物体
            cell.transform.SetParent(transform, false);
            cell.uiRect.SetParent(gridCanvas.transform, false);
        }
        /// <summary>
        /// 是否显示UI
        /// </summary>
        public void ShowUI(bool visible)
        {
            gridCanvas.gameObject.SetActive(visible);
        }
        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();

            cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

            ShowUI(false);
        }
    }
}