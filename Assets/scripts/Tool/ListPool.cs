using System.Collections.Generic;

/// <summary>
/// 列表池
/// </summary>
/// <typeparam name="T">列表类型</typeparam>
public static class ListPool<T>
{
    /// <summary>
    /// 使用栈存储列表
    /// </summary>
    private static Stack<List<T>> stack = new Stack<List<T>>(); 
    /// <summary>
    /// 获得新的列表
    /// </summary>
    public static List<T> Get()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return new List<T>();
    }
    /// <summary>
    /// 添加使用完的列表
    /// </summary>
    public static void Add(List<T> list)
    {
        list.Clear();
        stack.Push(list);
    }
}