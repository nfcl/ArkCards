namespace GameScene
{
    namespace Map
    {
        public class MapNode
        {
            /// <summary>
            /// 节点类型
            /// </summary>
            private MapNodeState _state;    
            /// <summary>
            /// 节点位置
            /// </summary>
            private (int, int) _position;
            /// <summary>
            /// 节点是否已被探索
            /// </summary>
            private bool _isVisit;
            /// <summary>
            /// <para/>关注度
            /// <para/>为该节点在多少个我方单位的视野内
            /// <para/>用于判断是否需要添加战争迷雾
            /// </summary>
            private int _notice;

            /// <summary>
            /// <para/>外部访问节点状态
            /// <para/>只读
            /// </summary>
            public MapNodeState State { get { return _state; } }
            /// <summary>
            /// <para/>外部访问节点位置
            /// <para/>只读
            /// </summary>
            public (int, int) Position { get { return _position; } }
            /// <summary>
            /// <para/>外部访问是否已探索
            /// <para/>只读
            /// </summary>
            public bool IsVisit { get { return _isVisit; } }
            /// <summary>
            /// <para/>外部访问是否处于战争迷雾
            /// <para/>只读
            /// </summary>
            public bool IsInfog { get { return true == IsVisit && 0 == _notice; } }

            public MapNode()
            {
                _state = MapNodeState.Grass;
                _position= (0, 0);
                _isVisit = true;
                _notice = 0;
            }
        }
    }
}