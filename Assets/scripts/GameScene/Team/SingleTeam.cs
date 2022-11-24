using GameScene.Operator;
using System.Collections.Generic;

namespace GameScene
{
    namespace Team
    {
        /// <summary>
        /// 单个队伍类
        /// 存储了一支小队的信息
        /// </summary>
        public class SingleTeam
        {
            /// <summary>
            /// 小队名称
            /// </summary>
            private string _teamName;
            /// <summary>
            /// <para/>小队成员
            /// </summary>
            private List<SingleOperator> _members;

            /// <summary>
            /// 小队名称属性
            /// 读 : 返回小队名称
            /// </summary>
            public string TeamName 
            { 
                get 
                {
                    return _teamName; 
                }
            }
            /// <summary>
            /// 每回合可移动距离属性
            /// 读 : 返回每回合可移动距离
            /// </summary>
            public int speedPerTurn
            {
                get
                {
                    return 1000;
                }
            }
        }
    }
}