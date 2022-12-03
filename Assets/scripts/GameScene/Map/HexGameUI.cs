using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Map
{
    /// <summary>
    /// 六边形地图的游戏UI
    /// </summary>
    public class HexGameUI : MonoBehaviour
    {
        /// <summary>
        /// 当前选择的单元格
        /// </summary>
        private HexCell currentCell;
        /// <summary>
        /// 当前选择的单位
        /// </summary>
        private HexMapUnit selectedUnit;
        /// <summary>
        /// 场景主相机
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// 地图网格
        /// </summary>
        public HexGrid grid;
        /// <summary>
        /// 小队信息界面
        /// </summary>
        public TeamInfoMenu teamMenu;

        /// <summary>
        /// 更新当前选择的单元格
        /// </summary>
        private bool UpdateCurrentCell()
        {
            HexCell cell = grid.GetCell(mainCamera.ScreenPointToRay(Input.mousePosition));
            if ((cell is null) == false)
            {
                currentCell = cell;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据选中的单元格判断选中的物体
        /// </summary>
        private void DoSelection()
        {
            grid.ClearPath();
            UpdateCurrentCell();
            if ((currentCell is null) == false)
            {
                if(selectedUnit == currentCell.Unit)
                {
                    teamMenu.Open(currentCell.Unit.Team);
                }
                else
                {
                    selectedUnit = currentCell.Unit;
                }
            }
        }
        /// <summary>
        /// 根据选择的单位和单元格进行寻路
        /// </summary>
        private void DoPathfinding()
        {
            if (UpdateCurrentCell())
            {
                if (
                    currentCell is null == false
                    && selectedUnit.IsValidDestination(currentCell) == true
                )
                {
                    grid.FindPath(selectedUnit.CurrentCell, currentCell, selectedUnit);
                }
                else
                {
                    grid.ClearPath();
                }
            }
        }
        /// <summary>
        /// 移动单位
        /// </summary>
        private void DoMove()
        {
            if (grid.HasPath == true)
            {
                selectedUnit.Travel(grid.GetPath());
                grid.ClearPath();
            }
        }

        /// <summary>
        /// 设置编辑模式是否启用
        /// </summary>
        public void SetEditMode(bool toggle)
        {
            enabled = !toggle;
            grid.ShowUI(!toggle);
            grid.ClearPath();
            if (toggle == true)
            {
                Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
            }
            else
            {
                Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
            }
        }

        /// <summary>
        /// 如果MonoBehaviour已启用，则在每一帧都调用Update
        /// </summary>
        public void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DoSelection();
                }
                else if ((selectedUnit is null) == false)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        DoMove();
                    }
                    else
                    {
                        DoPathfinding();
                    }
                }
            }
        }
        /// <summary>
        /// 加载脚本实例时调用Awake
        /// </summary>
        public void Awake()
        {
            mainCamera = Camera.main;
        }
    }
}