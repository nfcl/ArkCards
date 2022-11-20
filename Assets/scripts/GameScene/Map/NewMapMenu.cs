using GameScene.Map;
using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// 新地图UI面板
    /// </summary>
    public class NewMapMenu : MonoBehaviour
    {
        /// <summary>
        /// 地图网格
        /// </summary>
        public HexGrid hexGrid;

        /// <summary>
        /// 创建指定大小的地图
        /// </summary>
        private void CreateMap(int x, int z)
        {
            hexGrid.CreateMap(x, z);
            HexMapCamera.ValidatePosition();
            Close();
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            HexMapCamera.Locked = true;
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            HexMapCamera.Locked = false;
        }
        /// <summary>
        /// 创建小型地图
        /// </summary>
        public void CreateSmallMap()
        {
            CreateMap(20, 15);
        }
        /// <summary>
        /// 创建中型地图
        /// </summary>
        public void CreateMediumMap()
        {
            CreateMap(40, 30);
        }
        /// <summary>
        /// 创建大型地图
        /// </summary>
        public void CreateLargeMap()
        {
            CreateMap(80, 60);
        }
    }
}