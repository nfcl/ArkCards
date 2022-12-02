using System.Collections.Generic;

namespace GameScene.Operator.Json.CurrentOperators
{
    public class CurrentOperatorsItem
    {
        /// <summary>
        /// 干员名称
        /// </summary>
        public string Name { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CurrentOperatorsItem> CurrentOperators { get; set; }
    }
}