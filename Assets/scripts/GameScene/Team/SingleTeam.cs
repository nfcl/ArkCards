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

            public string TeamName { get { return _teamName; } }
        }
    }
}