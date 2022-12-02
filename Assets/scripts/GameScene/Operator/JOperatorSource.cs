using System.Collections.Generic;

namespace GameScene.Operator.Json.OperatorSource
{
    public class OperatorSourceItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pic_Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Agile { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OperatorSourceItem> OperatorSource { get; set; }
    }
}