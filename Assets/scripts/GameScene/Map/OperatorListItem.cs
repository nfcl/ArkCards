using GameScene.Operator;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class OperatorListItem : MonoBehaviour
    {
        public Image Image_Avatar;
        public Image Image_IsSelected;
        public string OperatorName { get; set; }

        public bool IsSelected
        {
            get
            {
                return Image_IsSelected.gameObject.activeSelf;
            }
        }

        public void Init(SingleOperator OperatorData)
        {
            Image_Avatar.sprite = Resources.Load<Sprite>($"CharPack/Avatar/char_{OperatorMetrices.OperatorInfo[OperatorData.Name].Pic_Name}_1");
            OperatorName = OperatorData.Name;
        }

        public void SetSelected(bool value)
        {
            Image_IsSelected.gameObject.SetActive(value);
        }
    }
}