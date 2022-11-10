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
    /// 
    /// </summary>
    public HexGrid hexGrid;
    /// <summary>
    /// 当前选择的节点颜色
    /// </summary>
    private Color activeColor;
    /// <summary>
    /// 当前选择的节点高度
    /// </summary>
    private int activeElevation;

    public ColorPannel colorPannel;

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
    IEnumerator MouseLeftClickListener()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            if (true == EventSystem.current.IsPointerOverGameObject()) continue;
            HandleInput_inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(HandleInput_inputRay, out HandleInput_hit))
            {
                EditCell(hexGrid.GetCell(HandleInput_hit.point));
            }
        }
    }
    /// <summary>
    /// 选择对应下标的颜色
    /// </summary>
    /// <param name="index">要选择的颜色</param>
    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
    /// <summary>
    /// 选择设置的节点高度
    /// </summary>
    /// <param name="elevation">高度</param>
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    private void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.Elevation = activeElevation;
    }

    public void Start()
    {

    }

    void Awake()
    {
        colorPannel.SetColors(colors);
        colorPannel.SetToggleDelegate(SelectColor);
        StartCoroutine(MouseLeftClickListener());
        colorPannel.SelectToggle(0);
    }
}