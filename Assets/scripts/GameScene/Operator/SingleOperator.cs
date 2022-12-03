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
            /// <summary>
            /// 干员敏捷值
            /// </summary>
            private int _agile;

            /// <summary>
            /// <para/>干员名称属性
            /// <para/>读 : 返回干员名称
            /// </summary>
            public string Name
            {
                get
                {
                    return _name;
                }
            }
            /// <summary>
            /// <para/>资源名称属性
            /// <para/>读 : 返回干员相关的资源名称
            /// </summary>
            public string Resource_Name
            {
                get
                {
                    return OperatorMetrices.OperatorInfo[_name].Pic_Name;
                }
            }
            /// <summary>
            /// <para/>干员状态属性
            /// <para/>读 : 返回干员状态
            /// <para/>写 : 设置干员状态
            /// </summary>
            public OperatorState State
            {
                get 
                {
                    return _state; 
                }
                set
                {
                    _state = value;
                }
            }
            /// <summary>
            /// <para/>干员敏捷值属性
            /// <para/>读 : 返回干员敏捷值
            /// </summary>
            public int Agile 
            {
                get 
                { 
                    return _agile;
                }
            }

            /// <summary>
            /// 从json数据构造干员数据
            /// </summary>
            /// <returns>返回干员数据</returns>
            public static SingleOperator CreateFromJsonData(Json.CurrentOperators.CurrentOperatorsItem data)
            {
                return new SingleOperator 
                {
                    _name = data.Name, 
                    State = OperatorState.Other, 
                    _agile = OperatorMetrices.OperatorInfo[data.Name].Agile 
                };
            }
        }
    }
}
