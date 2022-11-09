using UnityEngine;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        /// <summary>
        /// ��ͼ����
        /// </summary>
        private Map.MapNodeDraw                 _mapNodeDrawControl;
        /// <summary>
        /// ��ͼ�������
        /// </summary>
        private Map.TileMapClick                _mapClickListener;
        /// <summary>
        /// ��ͼ��Ϣ
        /// </summary>
        private Map.MapInfo                     _mapInfo;
        /// <summary>
        /// ��Ա��Ϣ
        /// </summary>
        private Operator.OperatorCollection     _operators;
        /// <summary>
        /// С����Ϣ
        /// </summary>
        private Team.TeamCollection             _teams;                 

        public void Start()
        {
            //��ͼ��ʼ��
            MapInit();
            //��Ա��ʼ��
            OperatorInit();
            //С�ӳ�ʼ��
            TeamInit();
        }

        /// <summary>
        /// ��ͼ��ʼ��
        /// </summary>
        public void MapInit()
        {
         
            _mapClickListener = GameObject.Find("").GetComponent<Map.TileMapClick>();

            _mapNodeDrawControl = new Map.MapNodeDraw(GameObject.Find("Map/Tilemap").GetComponent<UnityEngine.Tilemaps.Tilemap>());
            _mapNodeDrawControl.AddTile("Grass", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Grass"));
            _mapNodeDrawControl.AddTile("Sand", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Sand"));
            _mapNodeDrawControl.AddTile("Water", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Water"));

            _mapInfo = new Map.MapInfo();
        }

        /// <summary>
        /// ��Ա��Ϣ��ʼ��
        /// </summary>
        public void OperatorInit()
        {
            //��Ա���ϳ�ʼ��
            _operators = new Operator.OperatorCollection();
            //���ļ���ȡ��Ҹ�Ա��Ϣ
            //TODO
        }

        /// <summary>
        /// С����Ϣ��ʼ��
        /// </summary>
        public void TeamInit()
        {
            //С�Ӽ��ϳ�ʼ��
            _teams = new Team.TeamCollection();
            //���ļ���ȡ���С����Ϣ
            //TODO
        }
    }
}