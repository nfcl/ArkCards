using System.Collections.Generic;

namespace GameScene.Team.Json.CurrentOperators
{
    public class CurrentOperatorsItem
    {
        /// <summary>
        /// 队伍名称
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// 队伍成员
        /// </summary>
        public List<string> TeamMates { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CurrentOperatorsItem> CurrentOperators { get; set; }
    }
}