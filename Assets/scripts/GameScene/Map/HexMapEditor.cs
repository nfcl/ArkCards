﻿using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Map.Editor
{
    public class HexMapEditor : MonoBehaviour
    {
        /// <summary>
        /// 六边形网格
        /// </summary>
        public HexGrid hexGrid;
        /// <summary>
        /// 单元格颜色选择面板
        /// </summary>
        public ColorPannel colorPannel;

        /// <summary>
        /// 是否更改单元格地形
        /// </summary>
        private bool applyTerrain = false;
        /// <summary>
        /// 是否更改单元格高度
        /// </summary>
        private bool applyElevation = false;
        /// <summary>
        /// 是否更改单元格水面高度
        /// </summary>
        private bool applyWaterLevel = false;
        /// <summary>
        /// 当前选择的单元格地形
        /// </summary>
        private HexTerrainType activeTerrain;
        /// <summary>
        /// 当前那选择的水面高度
        /// </summary>
        private int activeWaterLevel;
        /// <summary>
        /// 当前选择的单元格高度
        /// </summary>
        private int activeElevation;
        /// <summary>
        /// 河流更改选项
        /// </summary>
        private OptionalToggle riverEditMode;
        /// <summary>
        /// 道路更改选项
        /// </summary>
        private OptionalToggle roadMode;
        /// <summary>
        /// 刷子的大小
        /// </summary>
        private int brushSize;
        /// <summary>
        /// 鼠标是否拖拽
        /// </summary>
        private bool isDrag;
        /// <summary>
        /// 拖拽的方向
        /// </summary>
        private HexDirection dragDirection;
        /// <summary>
        /// 上一个拖拽的单元格
        /// </summary>
        private HexCell previousCell;

        /// <summary>
        /// 设置是否启用更改高度
        /// </summary>
        public void SetApplyElevation(bool toggle)
        {
            applyElevation = toggle;
        }
        /// <summary>
        /// 设置是否启用水面高度调整
        /// </summary>
        public void SetApplyWaterLevel(bool toggle)
        {
            applyWaterLevel = toggle;
        }
        /// <summary>
        /// 设置是否启用修改地形
        /// </summary>
        public void SetApplyTerrain(bool toggle)
        {
            applyTerrain = toggle;
        }
        /// <summary>
        /// 选择要调整的地形
        /// </summary>
        public void SelectTerrain(int index)
        {
            activeTerrain = HexMetrics.HexTerrains[index];
        }
        /// <summary>
        /// 选择设置的单元格高度
        /// </summary>
        /// <param name="elevation">高度</param>
        public void SetElevation(float elevation)
        {
            activeElevation = (int)elevation;
        }
        /// <summary>
        /// 设置地图节点的UI是否显示
        /// </summary>
        public void ShowUI(bool visible)
        {
            hexGrid.ShowUI(visible);
        }
        /// <summary>
        /// 设置刷子大小
        /// </summary>
        /// <param name="size">要设置的大小</param>
        public void SetBrushSize(float size)
        {
            brushSize = (int)size;
        }
        /// <summary>
        /// 设置河流编辑选项
        /// </summary>
        public void SetRiverMode(int mode)
        {
            riverEditMode = (OptionalToggle)mode;
        }
        /// <summary>
        /// 设置道路编辑选项
        /// </summary>
        public void SetRoadMode(int mode)
        {
            roadMode = (OptionalToggle)mode;
        }
        /// <summary>
        /// 设置当前调整的水面高度
        /// </summary>
        public void SetWaterLevel(float level)
        {
            activeWaterLevel = (int)level;
        }
        /// <summary>
        /// 保存地图
        /// </summary>
        public void Save()
        {
            string path = Path.Combine(Application.persistentDataPath, "test.map");

            Debug.Log(path);

            using (
                BinaryWriter writer =
                    new BinaryWriter(File.Open(path, FileMode.Create))
            )
            {
                hexGrid.Save(writer);
            }
        }
        /// <summary>
        /// 加载地图
        /// </summary>
        public void Load()
        {
            string path = Path.Combine(Application.persistentDataPath, "test.map");
            using (
                BinaryReader reader =
                    new BinaryReader(File.Open(path, FileMode.Open))
            )
            {
                hexGrid.Load(reader);
            }
        }

        /// <summary>
        /// 鼠标监听协程
        /// </summary>
        private IEnumerator MouseLeftClickListener()
        {
            while (true)
            {
                //直到鼠标左键按下
                yield return new WaitUntil(() => Input.GetMouseButton(0));
                //判断点击是否在UI上
                if (true == EventSystem.current.IsPointerOverGameObject())
                {
                    goto ELSE;
                }
                //射线检测
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                //判断是否检测到了物体
                if (Physics.Raycast(ray, out raycastHit))
                {
                    //获得点击到的单元
                    HexCell currentCell = hexGrid.GetCell(raycastHit.point);
                    //判断上一个单元和当前点击的单元是否不同
                    if (false == (previousCell is null) && previousCell != currentCell)
                    {//上一个单元和当前点击的单元不同
                        ValidateDrag(currentCell);
                    }
                    else
                    {
                        isDrag = false;
                    }
                    //编辑单元
                    EditCells(currentCell);
                    //上一个单元变为当前点击的单元
                    previousCell = currentCell;
                }
                else
                {
                    goto ELSE;
                }

                continue;

            ELSE:
                previousCell = null;
            }
        }
        /// <summary>
        /// 查找上一个单元是否是当前单元的邻居
        /// </summary>
        private void ValidateDrag(HexCell currentCell)
        {
            //遍历邻居
            for (
                dragDirection = HexDirection.NE;
                dragDirection <= HexDirection.NW;
                dragDirection++
            )
            {
                //判断是否相同
                if (previousCell.GetNeighbor(dragDirection) == currentCell)
                {
                    //是邻居则拖拽有效
                    isDrag = true;
                    return;
                }
            }
            //无效
            isDrag = false;
        }
        /// <summary>
        /// 编辑以center为中心，brushsize为半径的单元群
        /// </summary>
        /// <param name="center">中心</param>
        private void EditCells(HexCell center)
        {
            int centerX = center.coordinates.X;
            int centerZ = center.coordinates.Z;

            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }
        /// <summary>
        /// 编辑单元
        /// </summary>
        /// <param name="cell">要编辑的单元</param>
        private void EditCell(HexCell cell)
        {
            if (cell is null)
            {
                return;
            }
            //调整颜色
            if (applyTerrain == true)
            {
                cell.TerrinType = activeTerrain;
            }
            //调整高度
            if (applyElevation == true)
            {
                cell.Elevation = activeElevation;
            }
            //调整水面高度
            if (applyWaterLevel == true)
            {
                cell.WaterLevel = activeWaterLevel;
            }
            //移除河流
            if (riverEditMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            //移除道路
            else if (roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
            }
            else if (isDrag == true)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    //添加河流
                    if (riverEditMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }
                    //添加道路
                    else if (roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(dragDirection);
                    }
                }
            }
        }

        /// <summary>
        /// 加载脚本实例时调用Awake
        /// </summary>
        private void Awake()
        {
            //设置地形选择器
            colorPannel.SetColors();
            colorPannel.SelectToggle(0);
            SelectTerrain(0);
            colorPannel.SetToggleDelegate(SelectTerrain);
            //启用鼠标左键监听协程
            StartCoroutine(MouseLeftClickListener());
        }

        /// <summary>
        /// <para/>选项
        /// <para/>只在HexMapEditor中使用
        /// <para/>所以作为内部枚举
        /// </summary>
        private enum OptionalToggle
        {
            Ignore, No, Yes
        }
    }
}