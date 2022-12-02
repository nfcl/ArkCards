using GameScene.Operator;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

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
            /// 小队一回合可移动的点数
            /// </summary>
            private int speedPerTurn;

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
            public int SpeedPerTurn
            {
                get
                {
                    return speedPerTurn;
                }
            }

            public SingleTeam()
            {
                _members = new List<SingleOperator>();
            }

            /// <summary>
            /// 从Json数据构造小队
            /// </summary>
            public static SingleTeam CreateFromJson(Json.CurrentOperators.CurrentOperatorsItem data)
            {
                SingleTeam newTeam = new SingleTeam();
                newTeam._teamName = data.TeamName;
                foreach(string operatorName in data.TeamMates)
                {
                    newTeam.AddOperator(GameSceneManager.operators.GetOperator(operatorName));
                }
                return newTeam;
            }

            /// <summary>
            /// 添加干员到小队
            /// </summary>
            public bool AddOperator(SingleOperator other)
            {
                if (_members.IndexOf(other) != -1)
                {
                    return false;
                }

                _members.Add(other);

                other.State = OperatorState.Team;

                RefreshSpeed();

                return true;
            }
            /// <summary>
            /// 从小队移除干员
            /// </summary>
            public bool RemoveOperator(SingleOperator other)
            {
                if (_members.IndexOf(other) == -1)
                {
                    return false;
                }

                _members.Remove(other);

                other.State = OperatorState.Other;

                RefreshSpeed();

                return true;
            }
            /// <summary>
            /// 刷新小队每回合速度
            /// </summary>
            public void RefreshSpeed()
            {
                speedPerTurn = 0;
                foreach(SingleOperator Operator in _members)
                {
                    speedPerTurn += OperatorMetrices.OperatorInfo[Operator.Name].Agile;
                }
            }
        }
    }
}