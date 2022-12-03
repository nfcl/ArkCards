using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameScene.Operator
{
    public class OperatorCollection
    {
        /// <summary>
        /// 当前玩家拥有的干员
        /// </summary>
        private Dictionary<string, SingleOperator> currentOperators;
        /// <summary>
        /// 所有干员的原始信息
        /// </summary>
        private Dictionary<string, Json.OperatorSource.OperatorSourceItem> operatorSources;

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            Json.CurrentOperators.Root jsonData_CurrentOperators =
                Tool.DataRead.JsonReader<Json.CurrentOperators.Root>(
                    $"{Application.streamingAssetsPath}/CurrentOperators.json"
                );
            currentOperators = new Dictionary<string, SingleOperator>();
            foreach (Json.CurrentOperators.CurrentOperatorsItem item in jsonData_CurrentOperators.CurrentOperators)
            {
                currentOperators[item.Name] = SingleOperator.CreateFromJsonData(item);
            }
        }
        /// <summary>
        /// 获得名称对应的干员
        /// </summary>
        public SingleOperator GetOperator(string name)
        {
            return currentOperators[name];
        }
        /// <summary>
        /// 添加一个新的干员到集合
        /// </summary>
        /// <param name="source">要添加的干员</param>
        /// <returns>返回是否添加成功</returns>
        public bool AddOperator(SingleOperator source)
        {
            //检查是否已存在
            if (true == currentOperators.ContainsKey(source.Name))
            {
#if UNITY_EDITOR
                Debug.Log($"已存在名称为{{{source.Name}}}的干员");
#endif
                //返回添加失败
                return false;
            }
            //增加映射关系
            currentOperators[source.Name] = source;
            //返回添加成功
            return true;
        }
        /// <summary>
        /// <para/>重新设置名称对应的干员
        /// <para/>不需要检查是否已存在
        /// </summary>
        /// <param name="source">要重设的干员</param>
        public void ReSetOperator(SingleOperator source)
        {
            //重新设置映射关系
            currentOperators[source.Name] = source;
        }
        /// <summary>
        /// 移除一个干员
        /// </summary>
        /// <param name="name">要移除的干员名称</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveOperator(string name)
        {
            //检查是否不存在
            if (false == currentOperators.ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log($"不存在名称为{{{name}}}的干员");
#endif
                //返回移除失败
                return false;
            }
            //移除映射关系
            currentOperators.Remove(name);
            //返回移除成功
            return true;
        }
        /// <summary>
        /// 获得已有的干员名称列表
        /// </summary>
        /// <returns></returns>
        public SingleOperator[] GetCurrentOperators()
        {
            return currentOperators.Values.ToArray();
        }
    }
}
