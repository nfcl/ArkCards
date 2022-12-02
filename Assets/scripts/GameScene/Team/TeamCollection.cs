using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    namespace Team
    {
        /// <summary>
        /// 队伍的集合类
        /// </summary>
        public class TeamCollection
        {
            /// <summary>
            /// 小队名称和小队的映射关系集合
            /// </summary>
            private Dictionary<string, SingleTeam> _source;

            /// <summary>
            /// 数据初始化
            /// </summary>
            public void InitData()
            {
                _source = new Dictionary<string, SingleTeam>();
                Json.CurrentOperators.Root jsonData_CurrentOperators =
                    Tool.DataRead.JsonReader<Json.CurrentOperators.Root>(
                        $"{Application.streamingAssetsPath}/CurrentTeams.json"
                    );

                foreach(Json.CurrentOperators.CurrentOperatorsItem item in jsonData_CurrentOperators.CurrentOperators)
                {
                    AddTeam(SingleTeam.CreateFromJson(item));
                }
            }
            /// <summary>
            /// 添加一个新的小队
            /// </summary>
            public bool AddTeam(SingleTeam source)
            {
                if(true == _source.ContainsKey(source.TeamName))
                {
#if UNITY_EDITOR
                    Debug.Log($"已存在名称为{{{source.TeamName}}}的小队");
#endif
                    return false;
                }
                _source[source.TeamName] = source;
                return true;
            }
            /// <summary>
            /// 根据名称移除一个已有的小队
            /// </summary>
            public bool RemoveTeam(string name)
            {
                if (false == _source.ContainsKey(name))
                {
#if UNITY_EDITOR
                    Debug.Log($"不存在名称为{{{name}}}的小队");
#endif
                    return false;
                }
                _source.Remove(name);
                return true;
            }
            /// <summary>
            /// 根据名称获得对应小队
            /// </summary>
            public SingleTeam GetTeam(string name)
            {
                return _source[name];
            }
        }
    }
}