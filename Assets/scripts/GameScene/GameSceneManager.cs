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
            //地图初始化
            MapInit();
            //干员初始化
            OperatorInit();
            //小队初始化
            TeamInit();
        }

        /// <summary>
        /// 地图初始化
        /// </summary>
        public void MapInit()
        {
            _mapNodeDrawControl = new Map.MapNodeDraw(GameObject.Find("Map/Tilemap").GetComponent<UnityEngine.Tilemaps.Tilemap>());
            _mapNodeDrawControl.AddTile("Grass", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Grass"));
            _mapNodeDrawControl.AddTile("Sand", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Sand"));
            _mapNodeDrawControl.AddTile("Water", Resources.Load<UnityEngine.Tilemaps.Tile>("GameScene/MapNodes/TileMap/Water"));
        }

        /// <summary>
        /// 干员信息初始化
        /// </summary>
        public void OperatorInit()
        {

        }

        /// <summary>
        /// 小队信息初始化
        /// </summary>
        public void TeamInit()
        {

        }
    }
}