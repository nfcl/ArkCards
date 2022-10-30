using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        private Map.MapNodeDraw _mapNodeDrawControl;
        private Operator.OperatorCollection _operators;
        private Team.TeamCollection _teams;

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
            _mapNodeDrawControl = new Map.MapNodeDraw(GameObject.Find("Map/Tilemap").GetComponent<UnityEngine.Tilemaps.Tilemap>());
            _mapNodeDrawControl.AddTile("Grass", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Grass"));
            _mapNodeDrawControl.AddTile("Sand", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Sand"));
            _mapNodeDrawControl.AddTile("Water", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Water"));
        }

        /// <summary>
        /// ��Ա��Ϣ��ʼ��
        /// </summary>
        public void OperatorInit()
        {

        }

        /// <summary>
        /// С����Ϣ��ʼ��
        /// </summary>
        public void TeamInit()
        {

        }
    }
}