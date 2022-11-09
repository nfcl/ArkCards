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
        /// 地图宽的节点数(x轴)
        /// </summary>
        public int width = 10;
        /// <summary>
        /// 地图高的节点数(z轴)
        /// </summary>
        public int height = 6;
        /// <summary>
        /// 默认颜色
        /// </summary>
        public Color defaultColor = Color.white;
        /// <summary>
        /// 点击颜色
        /// </summary>
        public Color touchedColor = Color.magenta;
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
        /// 画布用于存放节点UI
        /// </summary>
        private Canvas gridCanvas;
        /// <summary>
        /// 节点网格
        /// </summary>
        private HexMesh hexMesh;

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
            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;
            //新建节点标签
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }

        void Start()
        {
            //节点网格重新绘制
            hexMesh.Triangulate(cells);
            //启动鼠标点击监听协程
            StartCoroutine(HandleInput());
        }

        void Awake()
        {
            //获得画布
            gridCanvas = GetComponentInChildren<Canvas>();
            //获得网格管理器
            hexMesh = GetComponentInChildren<HexMesh>();
            //设置节点数组大小
            cells = new HexCell[height * width];
            //创建节点
            for (int z = 0, i = 0; z < height; ++z)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    CreateCell(x, z, i);
                }
            }
        }

        /// <summary>
        /// HandleInput方法的Ray缓存
        /// </summary>
        private Ray HandleInput_inputRay;
        /// <summary>
        /// HandleInput方法的RaycastHit缓存
        /// </summary>
        private RaycastHit HandleInput_hit;
        /// <summary>
        /// 鼠标点击监听协程
        /// </summary>
        IEnumerator HandleInput()
        {
            while (true)
            {
                yield return new WaitUntil(() => Input.GetMouseButton(0));
                HandleInput_inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(HandleInput_inputRay, out HandleInput_hit))
                {
                    TouchCell(HandleInput_hit.point);
                }
            }
        }

        /// <summary>
        /// 点击地图节点
        /// </summary>
        /// <param name="position"></param>
        public void TouchCell(Vector3 position)
        {
            //换算坐标系到六边形三维坐标
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            //更改对应节点颜色并更新网格
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            cells[index].color = touchedColor;
            hexMesh.Triangulate(cells);
        }

    }
}
