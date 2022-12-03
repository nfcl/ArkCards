using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class SaveLoadItem : MonoBehaviour
    {
        /// <summary>
        /// 地图名称
        /// </summary>
        private string mapName;

        /// <summary>
        /// 地图保存加载界面
        /// </summary>
        public SaveLoadMenu menu;

        /// <summary>
        /// 地图名称属性
        /// 读 : 返回地图名称
        /// 写 : 设置地图名称
        /// </summary>
        public string MapName
        {
            get
            {
                return mapName;
            }
            set
            {
                mapName = value;
                transform.GetChild(0).GetComponent<Text>().text = value;
            }
        }

        /// <summary>
        /// 选择此地图
        /// </summary>
        public void Select()
        {
            menu.SelectItem(mapName);
        }
    }
}