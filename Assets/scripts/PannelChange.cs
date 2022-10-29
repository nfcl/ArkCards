using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para/>一个Canvas内的Pannel场景切换类
/// <para/>可以对要管理的Pannel进行增减操作
/// <para/>显示其中一个隐藏其他所有的
/// <para/>通过设置CanvasGroup的alpha进行隐藏和显示
/// </summary>
public class PannelChange
{
    private Dictionary<string, CanvasGroup> _source;

    /// <summary>
    /// 无参构造
    /// </summary>
    public PannelChange()
    {

        _source = new Dictionary<string, CanvasGroup>();
    }

    /// <summary>
    /// 带参构造
    /// </summary>
    /// <param name="source">要增加管理的Pannel</param>
    public PannelChange(List<KeyValuePair<string,CanvasGroup>> source)
    {
        _source = new Dictionary<string, CanvasGroup>();

        foreach(KeyValuePair<string,CanvasGroup> item in source)
        {
            if (false == _source.ContainsKey(item.Key))
            {
                _source.Add(item.Key, item.Value);
            }
        }
    }

    /// <summary>
    /// 增加一个新的Pannel进行管理并隐藏
    /// </summary>
    /// <param name="name">要增加的Pannel名称</param>
    /// <param name="source">要增加的Pannel上的CanvasGroup</param>
    /// <returns>返回是否增加成功(管理的Pannel中是否不存在对应名称的)</returns>
    public bool AddPannel(string name, CanvasGroup source)
    {
        if (false == _source.ContainsKey(name))
        {
            _source.Add(name, source);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 根据名称移除一个Pannel
    /// </summary>
    /// <param name="name">要移除的Pannel名称</param>
    /// <returns>返回是否移除成功(管理的Pannel中是否存在对应名称的)</returns>
    public bool RemovePannel(string name)
    {
        if(true == _source.ContainsKey(name))
        {
            _source.Remove(name);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 显示新的Pannel,并隐藏旧的Pannel
    /// </summary>
    /// <param name="name">要显示的Pannel名称</param>
    /// <returns>返回是否显示成功(管理的Pannel中是否存在对应名称的)</returns>
    public bool ChangeToPannel(string name)
    {
        if (true == _source.ContainsKey(name))
        {
            foreach(CanvasGroup item in _source.Values)
            {
                item.alpha = 0;
                item.blocksRaycasts = false;
            }
            _source[name].alpha = 1;
            _source[name].blocksRaycasts = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
