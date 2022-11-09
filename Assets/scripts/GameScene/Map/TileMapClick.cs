using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Map
{
    public class TileMapClick : MonoBehaviour,IPointerClickHandler
    {
        public delegate void TileMapClickDelegate((int, int) posision2);

        /// <summary>
        /// <para/>点击事件的回调函数
        /// </summary>
        private TileMapClickDelegate _delegate;
        /// <summary>
        /// <para/>瓦片地图的grid用于坐标转换
        /// </summary> 
        [SerializeField]
        private Grid _grid;
        /// <summary>
        /// <para/>场景主相机
        /// </summary>
        [SerializeField]
        private Camera _mainCamer;
        /// <summary>
        /// <para/>存储转换后获得的瓦片地图坐标
        /// </summary>
        private Vector3Int _tilemapPos;

        /// <summary>
        /// 添加新的委托监听
        /// </summary>
        /// <param name="newEvent">新的委托</param>
        public void AddClickEvent(TileMapClickDelegate newEvent)
        {
            _delegate += newEvent;
        }

        /// <summary>
        /// 清空所有委托
        /// </summary>
        public void ClearClickEvent()
        {
            Delegate[] buf = _delegate.GetInvocationList();

            foreach (Delegate item in buf)
            {
                _delegate -= item as TileMapClickDelegate;
            }

            buf = null;
        }

        /// <summary>
        /// 鼠标点击事件监听
        /// </summary>
        /// <param name="eventData">点击信息</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            _tilemapPos = _grid.WorldToCell(_mainCamer.ScreenToWorldPoint(eventData.position));
            Debug.Log(_tilemapPos);
            _delegate?.Invoke((_tilemapPos.x, _tilemapPos.y));
        }
    }
}