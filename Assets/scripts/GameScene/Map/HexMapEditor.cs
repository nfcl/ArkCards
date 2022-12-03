using System.Collections;
using System.IO;
using UnityEditorInternal;
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
        public TerrainPannel terrainPannel;
        /// <summary>
        /// 地形材质
        /// </summary>
        public Material terrainMaterial;
        /// <summary>
        /// 创建新队伍的界面
        /// </summary>
        public NewTeamMenu newTeamMenu;

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
        /// 上一个单元格
        /// </summary>
        HexCell previousCell;

        /// <summary>
        /// 设置编辑模式启用
        /// </summary>
        public void SetEditMode(bool toggle)
        {
            enabled = toggle;
        }
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
        /// 是否显示网格线
        /// </summary>
        public void ShowGrid(bool visible)
        {
            if (visible)
            {
                terrainMaterial.EnableKeyword("GRID_ON");
            }
            else
            {
                terrainMaterial.DisableKeyword("GRID_ON");
            }
        }
        /// <summary>
        /// 生成新地图时初始化防止使用已destory的物体
        /// </summary>
        public void Init()
        {
            previousCell = null;
        }

        /// <summary>
        /// 输入处理
        /// </summary>
        private void HandleInput()
        {
            HexCell currentCell = GetCellUnderCursor();
            if (currentCell)
            {
                if (
                    (previousCell is null) == false
                    && previousCell != currentCell
                )
                {
                    ValidateDrag(currentCell);
                }
                else
                {
                    isDrag = false;
                }

                EditCells(currentCell);

                previousCell = currentCell;
            }
            else
            {
                previousCell = null;
            }
        }
        /// <summary>
        /// 获得鼠标碰到的第一个单元格
        /// </summary>
        private HexCell GetCellUnderCursor()
        {
            return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
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
                cell.TerrainType = activeTerrain;
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
        /// 创建单位
        /// </summary>
        private void CreateUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if ((cell is null) == false && cell.Unit is null)
            {
                newTeamMenu.Open(cell);
            }
        }
        /// <summary>
        /// 移除单位
        /// </summary>
        private void DestroyUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if ((cell is null) == false && (cell.Unit is null) == false)
            {
                hexGrid.RemoveUnit(cell.Unit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButton(0))
                {
                    HandleInput();
                    return;
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        DestroyUnit();
                    }
                    else
                    {
                        CreateUnit();
                    }
                    return;
                }
            }
            previousCell = null;
        }
        /// <summary>
        /// 加载脚本实例时调用Awake
        /// </summary>
        private void Awake()
        {
            //设置地形选择器
            terrainPannel.SetColors();
            terrainPannel.SelectToggle(0);
            SelectTerrain(0);
            terrainPannel.SetToggleDelegate(SelectTerrain);
            //关闭编辑模式
            SetEditMode(false);
            //关闭网格线显示
            terrainMaterial.DisableKeyword("GRID_ON");
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