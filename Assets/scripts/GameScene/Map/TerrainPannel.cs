using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map.Editor
{
    /// <summary>
    /// ��ɫѡ����
    /// </summary>
    public class TerrainPannel : MonoBehaviour
    {
        /// <summary>
        /// ������ɫѡ��Ԥ����
        /// </summary>
        public GameObject ColorTogglePrefab;
        /// <summary>
        /// ��ɫѡ����
        /// </summary>
        public ToggleGroup ToggleGroup;
        /// <summary>
        /// ��ɫѡ���
        /// </summary>
        public ColorToggle[] ColorToggles;

        /// <summary>
        /// ������ɫѡ��
        /// </summary>
        /// <param name="colors">�µ���ɫѡ��</param>
        public void SetColors()
        {
            int size = HexMetrics.HexTerrains.Length;
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
                ColorToggles[i].SetTerrainName(HexMetrics.HexTerrains[i].typeName);
                ColorToggles[i].SetToggleGroup(ToggleGroup);
                bufpos.y -= 40;
            }
        }
        /// <summary>
        /// �򿪳����еĿ���
        /// </summary>
        /// <param name="index">Ҫ�򿪵Ŀ����±�</param>
        public void SelectToggle(int index)
        {
            ColorToggles[index].SelectToggle();
        }
        /// <summary>
        /// ���ÿ��صĻص�����
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