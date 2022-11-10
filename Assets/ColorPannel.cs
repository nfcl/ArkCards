using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class ColorPannel : MonoBehaviour
    {

        public GameObject ColorTogglePrefab;

        public ToggleGroup toggleGroup;

        public ColorToggle[] colorToggles;

        public void SetColors(Color[] colors)
        {
            int size = colors.Length;
            colorToggles = new ColorToggle[size];
            RectTransform rt = GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + (10 + 30) * size);
            Vector3 bufpos = new Vector3(0, -25, 0);
            GameObject clone;
            for (int i = 0; i < size; ++i)
            {
                clone = Instantiate(ColorTogglePrefab, transform);
                clone.name = i.ToString();
                colorToggles[i] = clone.GetComponent<ColorToggle>();
                colorToggles[i].SetPos(bufpos);
                colorToggles[i].SetColor(colors[i]);
                colorToggles[i].SetToggleGroup(toggleGroup);
                bufpos.y -= 40;
            }
        }

        public void SelectToggle(int index)
        {
            colorToggles[index].SelectToggle();
        }

        public void SetToggleDelegate(ColorToggle.ColorToggleDelegate dele)
        {
            int size = colorToggles.Length;
            for (int i = 0; i < size; ++i)
            {
                colorToggles[i].SetToggleValueChangeListener(dele, i);
            }
        }
    }
}
