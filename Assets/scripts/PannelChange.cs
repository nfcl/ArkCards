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
    /// <summary>
    /// �޲ι���
    /// </summary>
    public PannelChange()
    {

    }

    /// <summary>
    /// ���ι���
    /// </summary>
    /// <param name="source">Ҫ���ӹ����Pannel</param>
    public PannelChange(List<KeyValuePair<string,CanvasGroup>> source)
    {

    }

    /// <summary>
    /// ����һ���µ�Pannel���й�������
    /// </summary>
    /// <param name="name">Ҫ���ӵ�Pannel����</param>
    /// <param name="source">Ҫ���ӵ�Pannel�ϵ�CanvasGroup</param>
    /// <returns>�����Ƿ����ӳɹ�(�����Pannel���Ƿ񲻴��ڶ�Ӧ���Ƶ�)</returns>
    public bool AddPannel(string name, CanvasGroup source)
    {
        return false;   //��ֹ��ⲻ������ֵ����
    }

    /// <summary>
    /// ���������Ƴ�һ��Pannel
    /// </summary>
    /// <param name="name">Ҫ�Ƴ���Pannel����</param>
    /// <returns>�����Ƿ��Ƴ��ɹ�(�����Pannel���Ƿ���ڶ�Ӧ���Ƶ�)</returns>
    public bool RemovePannel(string name)
    {
        return false;   //��ֹ��ⲻ������ֵ����
    }

    /// <summary>
    /// ��ʾ�µ�Pannel,�����ؾɵ�Pannel
    /// </summary>
    /// <param name="name">Ҫ��ʾ��Pannel����</param>
    /// <returns>�����Ƿ���ʾ�ɹ�(�����Pannel���Ƿ���ڶ�Ӧ���Ƶ�)</returns>
    public bool ChangeToPannel(string name)
    {
        return false;   //��ֹ��ⲻ������ֵ����
    }
}
