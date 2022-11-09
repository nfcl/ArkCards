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

        public void AddClickEvent(TileMapClickDelegate newEvent)
        {
            _delegate += newEvent;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _tilemapPos = _grid.WorldToCell(_mainCamer.ScreenToWorldPoint(eventData.position));
            Debug.Log(_tilemapPos);
            _delegate?.Invoke((_tilemapPos.x, _tilemapPos.y));
        }
    }
}