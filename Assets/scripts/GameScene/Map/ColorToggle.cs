using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map.Editor
{
    /// <summary>
    /// 地图编辑器中的单个地形编辑选项
    /// </summary>
    public class TerrainToggle : MonoBehaviour
    {
        public delegate void ColorToggleDelegate(int index);

        public Toggle checkToggle;
        public Text terrainNameText;
        public RectTransform rt;

        public void SetToggleGroup(ToggleGroup group)
        {
            checkToggle.group = group;
        }

        public void SetTerrainName(string Name)
        {
            terrainNameText.text = Name;
        }

        public void SetPos(Vector2 pos)
        {
            rt.localPosition = pos;
        }

        public void SetToggleValueChangeListener(ColorToggleDelegate dele, int index)
        {
            checkToggle.onValueChanged.AddListener((bool isChecked) => { if (isChecked) dele(index); });
        }

        public void SelectToggle()
        {
            checkToggle.isOn = true;
        }
    }
}
