namespace GameScene.Map
{
    public class MapNode
    {
        /// <summary>
        /// <para/>节点类型
        /// </summary>
        private MapNodeKind _nodeKind;
        /// <summary>
        /// <para/>节点位置2维坐标
        /// </summary>
        private (int, int) _position2;
        /// <summary>
        /// <para/>节点位置3维坐标
        /// </summary>
        private (int, int, int) _position3;
        /// <summary>
        /// <para/>节点是否已被探索
        /// </summary>
        private bool _isVisit;
        /// <summary>
        /// <para/>关注度
        /// <para/>为该节点在多少个我方单位的视野内
        /// <para/>用于判断是否需要添加战争迷雾
        /// </summary>
        private int _notice;
        /// <summary>
        /// <para/>从相邻节点移动到这一格需要的步数
        /// </summary>
        private int _step;

        /// <summary>
        /// <para/>节点类型的访问接口
        /// <para/>读 : 返回节点类型
        /// </summary>
        public MapNodeKind NodeKind
        {
            get
            {
                return _nodeKind;
            }
        }
        /// <summary>
        /// <para/>是否探索的访问接口
        /// <para/>读 : 返回是否探索
        /// </summary>
        public bool IsVisit
        {
            get
            {
                return _isVisit;
            }
        }
        /// <summary>
        /// <para/>是否处于战争迷雾的访问接口
        /// <para/>读 : 返回已探索并不处于我方单位视野内
        /// </summary>
        public bool IsInfog
        {
            get
            {
                return true == IsVisit && 0 == _notice;
            }
        }

        /// <summary>
        /// <para/>节点二维坐标的访问接口
        /// <para/>读 : 返回二维坐标
        /// <para/>写 : 设置二维和对应的三维坐标
        /// </summary>
        public (int, int) Position2
        {
            get
            {
                return _position2;
            }
            set
            {
                _position2 = value;
                _position3 = Position2To3(_position2);
            }
        }
        /// <summary>
        /// <para/>节点三维坐标的访问接口
        /// <para/>读 : 返回三维坐标
        /// <para/>写 : 设置三维和对应的二维坐标
        /// </summary>
        public (int, int, int) Position3
        {
            get
            {
                return _position3;
            }
            set
            {
                _position3 = value;
                _position2 = Position3To2(_position3);
            }
        }

        public MapNode()
        {
            _nodeKind = MapNodeKind.Grass;
            _position2 = (0, 0);
            _position3 = (0, 0, 0);
            _isVisit = true;
            _notice = 0;
            _step = 0;
        }

        /// <summary>
        /// <para/>将六边形节点的2维坐标转换维3维坐标
        /// </summary>
        /// <param name="pos2">要转换的2维坐标</param>
        /// <returns>转换后的3维坐标</returns>
        public static (int, int, int) Position2To3((int, int) pos2)
        {
            if (pos2.Item2 > 0)
            {//非负数
                if ((pos2.Item2 & 1) == 0)
                {//偶数
                    return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                }
                else
                {//奇数
                    return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2 + 1, pos2.Item2);
                }
            }
            else
            {//负数
                if ((pos2.Item2 & 1) == 0)
                {//偶数
                    return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                }
                else
                {//奇数
                    return (pos2.Item1 - pos2.Item2 / 2 + 1, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                }
            }
        }

        /// <summary>
        /// <para/>将六边形节点的3维坐标转换维2维坐标
        /// </summary>
        /// <param name="pos3">要转换的3维坐标</param>
        /// <returns>转换后的2维坐标</returns>
        public static (int, int) Position3To2((int, int, int) pos3)
        {
            if ((pos3.Item3 & 1) == 0)
            {//偶数
                return ((pos3.Item1 + pos3.Item2) / 2, pos3.Item3);
            }
            else
            {//奇数
                if (pos3.Item1 + pos3.Item2 > 0)
                {//正半轴
                    return ((pos3.Item1 + pos3.Item2) / 2, pos3.Item3);
                }
                else
                {//负半轴
                    return ((pos3.Item1 + pos3.Item2) / 2 - 1, pos3.Item3);
                }
            }
        }
        
        /// <summary>
        /// 计算两个三维坐标的曼哈顿距离
        /// </summary>
        /// <param name="pos1">坐标1</param>
        /// <param name="pos2">坐标2</param>
        /// <returns>返回距离</returns>
        public static int PositionDistance((int,int,int) pos1, (int, int, int) pos2)
        {
            //返回三个维度差距的绝对值的最大值
            return Tool.Math.max(
                Tool.Math.max(
                    Tool.Math.abs(pos1.Item1 - pos1.Item1),
                    Tool.Math.abs(pos1.Item2 - pos1.Item2)
                    ), 
                Tool.Math.abs(pos1.Item3 - pos2.Item3));
        }
    }
}
