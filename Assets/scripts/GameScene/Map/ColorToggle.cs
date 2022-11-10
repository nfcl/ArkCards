using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class ColorToggle : MonoBehaviour
    {
        public delegate void ColorToggleDelegate(int index);

        private Toggle _toggle;
        private Image _colorImage;
        private RectTransform _rt;

        public void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _colorImage = transform.Find("Color").GetComponent<Image>();
            _rt = GetComponent<RectTransform>();
        }

        public void SetToggleGroup(ToggleGroup group)
        {
            _toggle.group = group;
        }

        public void SetColor(Color color)
        {
            _colorImage.color = color;
        }

        public void SetPos(Vector2 pos)
        {
            _rt.localPosition = pos;
        }

        public void SetToggleValueChangeListener(ColorToggleDelegate dele, int index)
        {
            _toggle.onValueChanged.AddListener((bool isChecked) => { if (isChecked) dele(index); });
        }

        public void SelectToggle()
        {
            _toggle.isOn = true;
        }
    }
}
