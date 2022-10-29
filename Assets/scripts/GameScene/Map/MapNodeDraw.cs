using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 瓦片地图节点绘制类
/// 对地图绘制进行了封装
/// </summary>
public class MapNodeDraw
{
    /// <summary>
    /// 瓦片存储
    /// </summary>
    private Dictionary<string, Tile> _nodeTiles;
    /// <summary>
    /// 瓦片地图
    /// </summary>
    private Tilemap _source;
    /// <summary>
    /// Vector3Int类型的缓存
    /// </summary>
    private Vector3Int _vector3IntBuf;

    public MapNodeDraw(Tilemap source)
    {
        _nodeTiles = new Dictionary<string, Tile>();
        _source = source;
    }

    public MapNodeDraw(Dictionary<string, Tile> nodeTiles, Tilemap source)
    {
        _nodeTiles = nodeTiles;
        _source = source;
    }

    /// <summary>
    /// 在指定位置绘制指定节点
    /// </summary>
    /// <param name="tileName">要绘制的节点名称</param>
    /// <param name="tilePos">要绘制的节点位置</param>
    /// <returns>返回是否存在对应名称的节点</returns>
    public bool Draw(string tileName, (int x, int y) tilePos)
    {
        if (false == _nodeTiles.ContainsKey(tileName))
        {
#if UNITY_EDITOR
            //输出不存在的tile名称
            Debug.Log($"要绘制的指定节点{{{tileName}}}不存在");
#endif
            //返回绘制失败
            return false;
        }
        //设置位置
        _vector3IntBuf.x = tilePos.x;
        _vector3IntBuf.y = tilePos.y;
        _vector3IntBuf.z = 0;
        //绘制节点
        _source.SetTile(_vector3IntBuf, _nodeTiles[tileName]);
        //返回绘制成功
        return true;
    }

    /// <summary>
    /// 清除地图上已绘制的所有瓦片
    /// </summary>
    public void ClearMap()
    {
        //调用Tilemap.ClearAllTiles()进行清除
        _source.ClearAllTiles();
    }

    /// <summary>
    /// 添加一个新的瓦片类型
    /// </summary>
    /// <param name="name">要添加的瓦片名称</param>
    /// <param name="source">要添加的瓦片</param>
    /// <returns></returns>
    public bool AddTile(string name,Tile source)
    {
        if(true == _nodeTiles.ContainsKey(name))
        {
#if UNITY_EDITOR
            //输出已存在的tile名称
            Debug.Log($"要添加的瓦片名称{{{name}}}已存在");
#endif
            //返回添加失败
            return false;
        }
        //添加名称和瓦片映射
        _nodeTiles[name] = source;
        //返回添加成功
        return true;
    }

    /// <summary>
    /// 移除已有的瓦片
    /// </summary>
    /// <param name="name">要移除的瓦片名称</param>
    /// <returns>返回</returns>
    public bool RemoveTile(string name)
    {
        if(false == _nodeTiles.ContainsKey(name))
        {
#if UNITY_EDITOR
            //输出不存在的tile名称
            Debug.Log($"要移除的瓦片名称{{{name}}}不存在");
#endif
            //返回移除失败
            return false;
        }
        //移除映射关系
        _nodeTiles.Remove(name);
        //返回移除成功
        return true;
    }

    /// <summary>
    /// <para/>重设对应名称的映射关系到新的瓦片
    /// <para/>即通过该方法设置的映射关系不需要检查是否存在
    /// </summary>
    /// <param name="name">要重设的瓦片名称</param>
    /// <param name="source">要重设的瓦片</param>
    public void ResetTile(string name,Tile source)
    {
        _nodeTiles[name] = source;
    }

    /// <summary>
    /// 清除所有的瓦片映射关系
    /// </summary>
    public void ClearTile()
    {
        //调用Dictionary.Clear()
        _nodeTiles.Clear();
    }
}
