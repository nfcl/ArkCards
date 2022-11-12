using GameScene.Map;
using System.Collections;
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
    private IEnumerator MouseLeftClickListener()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            if (true == EventSystem.current.IsPointerOverGameObject()) continue;
            HandleInput_inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(HandleInput_inputRay, out HandleInput_hit))
            {
                EditCells(hexGrid.GetCell(HandleInput_hit.point));
            }
        }
    }
    /// <summary>
    /// 编辑以center为中心，brushsize为半径的单元群
    /// </summary>
    /// <param name="center">中心</param>
	void EditCells(HexCell center)
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
}