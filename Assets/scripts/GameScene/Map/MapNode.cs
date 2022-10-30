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
            /// 节点是否还隐藏着
            /// </summary>
            private bool _isHide;

            public MapNodeState State { get => _state; }
            public (int, int) Position { get => _position; }
            public bool IsHide { get => _isHide; }

            public MapNode()
            {
                _state = MapNodeState.Grass;
                _position= (0, 0);
                _isHide = true;
            }
        }
    }
}