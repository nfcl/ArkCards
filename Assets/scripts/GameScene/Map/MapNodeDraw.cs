using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ��Ƭ��ͼ�ڵ������
/// �Ե�ͼ���ƽ����˷�װ
/// </summary>
public class MapNodeDraw
{
    /// <summary>
    /// ��Ƭ�洢
    /// </summary>
    private Dictionary<string, Tile> _nodeTiles;
    /// <summary>
    /// ��Ƭ��ͼ
    /// </summary>
    private Tilemap _source;
    /// <summary>
    /// Vector3Int���͵Ļ���
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
    /// ��ָ��λ�û���ָ���ڵ�
    /// </summary>
    /// <param name="tileName">Ҫ���ƵĽڵ�����</param>
    /// <param name="tilePos">Ҫ���ƵĽڵ�λ��</param>
    /// <returns>�����Ƿ���ڶ�Ӧ���ƵĽڵ�</returns>
    public bool Draw(string tileName, (int x, int y) tilePos)
    {
        if (false == _nodeTiles.ContainsKey(tileName))
        {
#if UNITY_EDITOR
            //��������ڵ�tile����
            Debug.Log($"Ҫ���Ƶ�ָ���ڵ�{{{tileName}}}������");
#endif
            //���ػ���ʧ��
            return false;
        }
        //����λ��
        _vector3IntBuf.x = tilePos.x;
        _vector3IntBuf.y = tilePos.y;
        _vector3IntBuf.z = 0;
        //���ƽڵ�
        _source.SetTile(_vector3IntBuf, _nodeTiles[tileName]);
        //���ػ��Ƴɹ�
        return true;
    }

    /// <summary>
    /// �����ͼ���ѻ��Ƶ�������Ƭ
    /// </summary>
    public void ClearMap()
    {
        //����Tilemap.ClearAllTiles()�������
        _source.ClearAllTiles();
    }

    /// <summary>
    /// ���һ���µ���Ƭ����
    /// </summary>
    /// <param name="name">Ҫ��ӵ���Ƭ����</param>
    /// <param name="source">Ҫ��ӵ���Ƭ</param>
    /// <returns></returns>
    public bool AddTile(string name,Tile source)
    {
        if(true == _nodeTiles.ContainsKey(name))
        {
#if UNITY_EDITOR
            //����Ѵ��ڵ�tile����
            Debug.Log($"Ҫ��ӵ���Ƭ����{{{name}}}�Ѵ���");
#endif
            //�������ʧ��
            return false;
        }
        //������ƺ���Ƭӳ��
        _nodeTiles[name] = source;
        //������ӳɹ�
        return true;
    }

    /// <summary>
    /// �Ƴ����е���Ƭ
    /// </summary>
    /// <param name="name">Ҫ�Ƴ�����Ƭ����</param>
    /// <returns>����</returns>
    public bool RemoveTile(string name)
    {
        if(false == _nodeTiles.ContainsKey(name))
        {
#if UNITY_EDITOR
            //��������ڵ�tile����
            Debug.Log($"Ҫ�Ƴ�����Ƭ����{{{name}}}������");
#endif
            //�����Ƴ�ʧ��
            return false;
        }
        //�Ƴ�ӳ���ϵ
        _nodeTiles.Remove(name);
        //�����Ƴ��ɹ�
        return true;
    }

    /// <summary>
    /// <para/>�����Ӧ���Ƶ�ӳ���ϵ���µ���Ƭ
    /// <para/>��ͨ���÷������õ�ӳ���ϵ����Ҫ����Ƿ����
    /// </summary>
    /// <param name="name">Ҫ�������Ƭ����</param>
    /// <param name="source">Ҫ�������Ƭ</param>
    public void ResetTile(string name,Tile source)
    {
        _nodeTiles[name] = source;
    }

    /// <summary>
    /// ������е���Ƭӳ���ϵ
    /// </summary>
    public void ClearTile()
    {
        //����Dictionary.Clear()
        _nodeTiles.Clear();
    }
}
