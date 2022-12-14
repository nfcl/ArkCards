using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
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
            //置null
            _source = null;
            //置-1
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
        public void CreateNewMap((int width, int height) size)
        {
            //设置种子
            _seed = Random.Range(0, int.MaxValue);
            //设置地图大小
            _source = new MapNode[size.width, size.height];
            //创建地图
            //TODO
        }

        /// <summary>
        /// 根据给定种子创建新的地图
        /// </summary>
        /// <param name="size">要创建的地图大小</param>
        /// <param name="seed">初始种子</param>
        public void CreateNewMap((int width, int height) size, int seed)
        {
            //设置种子
            _seed = seed;
            //设置地图大小
            _source = new MapNode[size.width, size.height];
            //创建地图
            //TODO
        }

        /// <summary>
        /// <para/>访问对应位置的节点
        /// <para/>二维重载
        /// </summary>
        /// <returns>返回对应节点</returns>
        public MapNode GetNode(int x, int y)
        {
            //返回对应位置的节点
            return _source[x, y];
        }

        /// <summary>
        /// GetNode方法的二维坐标转换缓存
        /// </summary>
        private (int, int) GetNode_buf;
        /// <summary>
        /// <para/>访问对应位置的节点
        /// <para/>三维重载
        /// </summary>
        /// <returns>返回对应节点</returns>
        public MapNode GetNode(int x,int y,int z)
        {
            GetNode_buf = MapNode.Position3To2((x, y, z));
            return _source[GetNode_buf.Item1, GetNode_buf.Item2];
        }

        public List<(int, int)> FindRoad((int, int) start, (int, int) end)
        {
            List<(int, int)> result = new List<(int, int)>();



            return result;
        }
    }
}