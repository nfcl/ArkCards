using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Operator
{
    public class OperatorCollection
    {
        /// <summary>
        /// <para/>干员名称和干员信息的映射
        /// </summary>
        private Dictionary<string, SingleOperator> _source;

        public OperatorCollection()
        {
            //初始化字典
            _source = new Dictionary<string, SingleOperator>();
        }

        /// <summary>
        /// GetOperator的合法性检查缓存
        /// </summary>
        private SingleOperator GetOperator_buf;
        /// <summary>
        /// <para/>根据名称获得干员信息
        /// <para/>会进行名称合法性检查
        /// </summary>
        /// <param name="name">干员名称</param>
        /// <returns>返回干员信息,不合法的名称会返回null</returns>
        public SingleOperator GetOperator(string name)
        {
            if(_source.TryGetValue(name,out GetOperator_buf))
            {
                return GetOperator_buf;
            }
#if UNITY_EDITOR
            Debug.Log($"不存在名称为{{ {name} }}的干员");
#endif
            return null;
        }

        /// <summary>
        /// 添加一个新的干员到集合
        /// </summary>
        /// <param name="source">要添加的干员</param>
        /// <returns>返回是否添加成功</returns>
        public bool AddOperator(SingleOperator source)
        {
            //检查是否已存在
            if (true == _source.ContainsKey(source.Name))
            {
#if UNITY_EDITOR
                Debug.Log($"已存在名称为{{{source.Name}}}的干员");
#endif
                //返回添加失败
                return false;
            }
            //增加映射关系
            _source[source.Name] = source;
            //返回添加成功
            return true;
        }

        /// <summary>
        /// <para/>重新设置名称与干员的映射关系
        /// <para/>不需要检查是否已存在
        /// </summary>
        /// <param name="source">要重设的干员</param>
        public void ReSetOperator(SingleOperator source)
        {
            //重新设置映射关系
            _source[source.Name] = source;
        }

        /// <summary>
        /// 移除一个干员
        /// </summary>
        /// <param name="name">要移除的干员名称</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveOperator(string name)
        {
            //检查是否不存在
            if (false == _source.ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log($"不存在名称为{{{name}}}的干员");
#endif
                //返回移除失败
                return false;
            }
            //移除映射关系
            _source.Remove(name);
            //返回移除成功
            return true;
        }
    }
}
