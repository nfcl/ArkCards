using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map.Editor
{
    /// <summary>
    /// 颜色选择类
    /// </summary>
    public class ColorPannel : MonoBehaviour
    {
        /// <summary>
        /// 单个颜色选项预设体
        /// </summary>
        public GameObject ColorTogglePrefab;
        /// <summary>
        /// 颜色选项组
        /// </summary>
        public ToggleGroup ToggleGroup;
        /// <summary>
        /// 颜色选项集合
        /// </summary>
        public ColorToggle[] ColorToggles;

        /// <summary>
        /// 设置颜色选项
        /// </summary>
        /// <param name="colors">新的颜色选项</param>
        public void SetColors(Color[] colors)
        {
            int size = colors.Length;
            ColorToggles = new ColorToggle[size];
            RectTransform rt = GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + (10 + 30) * size);
            Vector3 bufpos = new Vector3(0, -25, 0);
            GameObject clone;
            for (int i = 0; i < size; ++i)
            {
                clone = Instantiate(ColorTogglePrefab, transform);
                clone.name = i.ToString();
                ColorToggles[i] = clone.GetComponent<ColorToggle>();
                ColorToggles[i].SetPos(bufpos);
                ColorToggles[i].SetColor(colors[i]);
                ColorToggles[i].SetToggleGroup(ToggleGroup);
                bufpos.y -= 40;
            }
        }
        /// <summary>
        /// 打开场景中的开关
        /// </summary>
        /// <param name="index">要打开的开关下标</param>
        public void SelectToggle(int index)
        {
            ColorToggles[index].SelectToggle();
        }
        /// <summary>
        /// 设置开关的回调函数
        /// </summary>
        public void SetToggleDelegate(ColorToggle.ColorToggleDelegate dele)
        {
            int size = ColorToggles.Length;
            for (int i = 0; i < size; ++i)
            {
                ColorToggles[i].SetToggleValueChangeListener(dele, i);
            }
        }
    }
}