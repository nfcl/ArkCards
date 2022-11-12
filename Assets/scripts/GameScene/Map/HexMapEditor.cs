using GameScene.Map;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    /// <summary>
    /// 可选颜色
    /// </summary>
    public Color[] colors;
    /// <summary>
    /// 六边形网格
    /// </summary>
    public HexGrid hexGrid;
    /// <summary>
    /// 单元格颜色选择面板
    /// </summary>
    public ColorPannel colorPannel;

    /// <summary>
    /// 当前选择的单元格颜色
    /// </summary>
    private Color activeColor;
    /// <summary>
    /// 刷子的大小
    /// </summary>
    private int brushSize;
    /// <summary>
    /// 当前选择的单元格高度
    /// </summary>
    private int activeElevation;
    /// <summary>
    /// 是否更改单元格颜色
    /// </summary>
    private bool applyColor = true;
    /// <summary>
    /// 是否更改单元格高度
    /// </summary>
    private bool applyElevation = true;
    /// <summary>
    /// 河流更改选项
    /// </summary>
    private OptionalToggle riverEditMode;
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
    /// 选择对应下标的颜色
    /// </summary>
    /// <param name="index">要选择的颜色</param>
    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
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
    /// 设置是否启用更改高度
    /// </summary>
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
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
    /// 设置地图节点的UI是否显示
    /// </summary>
	public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
    /// <summary>
    /// 设置河流编辑选项
    /// </summary>
    public void SetRiverMode(int mode)
    {
        riverEditMode = (OptionalToggle)mode;
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
            HandleInput_inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //判断是否检测到了物体
            if (Physics.Raycast(HandleInput_inputRay, out HandleInput_hit))
            {
                //获得点击到的单元
                HexCell currentCell = hexGrid.GetCell(HandleInput_hit.point);
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
        if (cell is null) return;
        //调整颜色
        if (applyColor)
        {
            cell.Color = activeColor;
        }
        //调整高度
        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }
        //调整河流
        if (riverEditMode == OptionalToggle.No)
        {//如果编辑模式为去除则清除单元河流
            cell.RemoveRiver();
        }
        else if (isDrag && riverEditMode == OptionalToggle.Yes)
        {//如果编辑模式为添加则添加河流出口				
            HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
            //判断对应方向是否存在邻居
            if (otherCell is null) return;
            otherCell.SetOutgoingRiver(dragDirection);
        }
    }
    /// <summary>
    /// 加载脚本实例时调用Awake
    /// </summary>
    private void Awake()
    {
        //设置颜色选择器
        colorPannel.SetColors(colors);
        colorPannel.SetToggleDelegate(SelectColor);
        colorPannel.SelectToggle(0);
        //启用鼠标左键监听协程
        StartCoroutine(MouseLeftClickListener());
    }

    /// <summary>
    /// 选项
    /// </summary>
    private enum OptionalToggle
    {
        Ignore, No, Yes
    }
}