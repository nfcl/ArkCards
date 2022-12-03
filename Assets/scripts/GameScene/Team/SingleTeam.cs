using GameScene.Operator;
using System.Collections.Generic;
using System.Linq;
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
            /// <summary>
            /// <para/>队长属性
            /// <para/>读 : 返回队伍列表中的第一个干员
            /// </summary>
            public SingleOperator captain
            {
                get
                {
                    return _members[0];
                }
            }

            public SingleTeam(string teamName = "")
            {
                _teamName = teamName;
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
            public bool AddOperator(SingleOperator src)
            {
                if (_members.IndexOf(src) != -1)
                {
                    return false;
                }

                _members.Add(src);

                src.State = OperatorState.Team;

                RefreshSpeed();

                return true;
            }
            /// <summary>
            /// 添加干员到小队
            /// </summary>
            public bool AddOperator(string operatorName)
            {
                SingleOperator singleOperator = GameSceneManager.operators.GetOperator(operatorName);
                if (_members.IndexOf(singleOperator) != -1)
                {
                    return false;
                }

                _members.Add(singleOperator);

                singleOperator.State = OperatorState.Team;

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
            /// 清空干员列表
            /// </summary>
            public void ClearOperators()
            {
                _members.Clear();
                RefreshSpeed();
            }
            /// <summary>
            /// 小队成员是否为空
            /// </summary>
            public bool EmptyMembers()
            {
                return _members.Count == 0;
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