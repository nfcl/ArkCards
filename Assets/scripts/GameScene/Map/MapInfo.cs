using System.Collections;
using UnityEngine;

namespace GameScene
{
    namespace Map
    {
        public class MapInfo
        {
            /// <summary>
            /// <para/>地图信息
            /// <para/>使用二维数组保存
            /// <para/>左下角为0,0
            /// </summary>
            private MapNode[,] _source;
            /// <summary>
            /// 地图初始种子
            /// </summary>
            private int _seed;

            /// <summary>
            /// 地图宽度
            /// </summary>
            public int MapWidth => _source.GetLength(0);
            /// <summary>
            /// 地图高度
            /// </summary>
            public int MapHieght => _source.GetLength(1);

            public MapInfo()
            {
                _source = null;
                _seed = -1;
            }

            /// <summary>
            /// 读取地图存档
            /// </summary>
            public void ReadMap()
            {
                //TODO
            }

            /// <summary>
            /// 创建新的地图
            /// </summary>
            /// <param name="size">要创建的地图大小</param>
            /// <param name="seed">要创建的地图初始种子,值为-1时为随机</param>
            public void CreateNewMap((int width, int height) size,int seed = -1)
            {
                if (-1 == seed)
                {
                    seed = Random.Range(int.MinValue, int.MaxValue);
                }

                _source = new MapNode[size.width, size.height];
            }

            /// <summary>
            /// 访问对应位置的节点
            /// </summary>
            /// <param name="x">横坐标</param>
            /// <param name="y">纵坐标</param>
            /// <returns>返回对应节点</returns>
            public MapNode GetNode(int x, int y)
            {
                return _source[x, y];
            }
        }
    }
}