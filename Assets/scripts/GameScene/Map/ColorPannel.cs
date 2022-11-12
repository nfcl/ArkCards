using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class ColorPannel : MonoBehaviour
    {
        /// <summary>
        /// ������ɫѡ��Ԥ����
        /// </summary>
        public GameObject ColorTogglePrefab;
        /// <summary>
        /// ��ɫѡ����
        /// </summary>
        public ToggleGroup toggleGroup;
        /// <summary>
        /// ��ɫѡ�
        /// </summary>
        public ColorToggle[] colorToggles;

        /// <summary>
        /// ������ɫѡ��
        /// </summary>
        /// <param name="colors">�µ���ɫѡ��</param>
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
        /// <summary>
        /// �򿪳����еĿ���
        /// </summary>
        /// <param name="index">Ҫ�򿪵Ŀ����±�</param>
        public void SelectToggle(int index)
        {
            colorToggles[index].SelectToggle();
        }
        /// <summary>
        /// ���ÿ��صĻص�����
        /// </summary>
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
