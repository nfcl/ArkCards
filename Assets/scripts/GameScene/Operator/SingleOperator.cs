namespace GameScene
{
    namespace Operator
    {
        /// <summary>
        /// 单个干员类
        /// 存储了一个干员的信息
        /// </summary>
        public class SingleOperator
        {
            /// <summary>
            /// 干员名称
            /// </summary>
            private string _name;
            /// <summary>
            /// 干员状态
            /// </summary>
            private OperatorState _state;

            public string Name { get => _name; }
        }
    }
}
