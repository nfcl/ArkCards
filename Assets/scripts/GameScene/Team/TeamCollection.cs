using System.Collections.Generic;

namespace GameScene.Team
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

        public TeamCollection()
        {
            //初始化映射关系集合
            _source = new Dictionary<string, SingleTeam>();
        }

        /// <summary>
        /// 添加一个新的小队
        /// </summary>
        /// <param name="source">要增加的小队</param>
        /// <return>返回是否添加成功</return>
        public bool AddTeam(SingleTeam source)
        {
            if (true == _source.ContainsKey(source.TeamName))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"已存在名称为{{{source.TeamName}}}的小队");
#endif
                return false;
            }
            _source[source.TeamName] = source;
            return true;
        }

        /// <summary>
        /// 移除一个已有的小队
        /// </summary>
        /// <param name="name">要移除的小队名称</param>
        /// <returns>返回是否移除成功</returns>
        public bool RemoveTeam(string name)
        {
            if (false == _source.ContainsKey(name))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"不存在名称为{{{name}}}的小队");
#endif
                return false;
            }
            _source.Remove(name);
            return true;
        }

        /// <summary>
        /// 根据名称映射访问对应小队实例
        /// </summary>
        /// <param name="name">要访问的小队名称</param>
        /// <returns>返回要访问的小队实例</returns>
        public SingleTeam GetTeam(string name)
        {
            return _source[name];
        }
    }
}