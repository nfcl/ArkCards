using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para/>һ��Canvas�ڵ�Pannel�����л���
/// <para/>���Զ�Ҫ�����Pannel������������
/// <para/>��ʾ����һ�������������е�
/// <para/>ͨ������CanvasGroup��alpha�������غ���ʾ
/// </summary>
public class PannelChange
{
    private Dictionary<string, CanvasGroup> _source;

    /// <summary>
    /// �޲ι���
    /// </summary>
    public PannelChange()
    {

        _source = new Dictionary<string, CanvasGroup>();
    }

    /// <summary>
    /// ���ι���
    /// </summary>
    /// <param name="source">Ҫ���ӹ����Pannel</param>
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
    /// ����һ���µ�Pannel���й�������
    /// </summary>
    /// <param name="name">Ҫ���ӵ�Pannel����</param>
    /// <param name="source">Ҫ���ӵ�Pannel�ϵ�CanvasGroup</param>
    /// <returns>�����Ƿ����ӳɹ�(�����Pannel���Ƿ񲻴��ڶ�Ӧ���Ƶ�)</returns>
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
    /// ���������Ƴ�һ��Pannel
    /// </summary>
    /// <param name="name">Ҫ�Ƴ���Pannel����</param>
    /// <returns>�����Ƿ��Ƴ��ɹ�(�����Pannel���Ƿ���ڶ�Ӧ���Ƶ�)</returns>
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
    /// ��ʾ�µ�Pannel,�����ؾɵ�Pannel
    /// </summary>
    /// <param name="name">Ҫ��ʾ��Pannel����</param>
    /// <returns>�����Ƿ���ʾ�ɹ�(�����Pannel���Ƿ���ڶ�Ӧ���Ƶ�)</returns>
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
