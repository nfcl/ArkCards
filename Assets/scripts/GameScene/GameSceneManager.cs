using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        private MapNodeDraw _mapNodeDrawControl;

        public void Start()
        {
            _mapNodeDrawControl = new MapNodeDraw(GameObject.Find("Map/Tilemap").GetComponent<Tilemap>());
            _mapNodeDrawControl.AddTile("Grass",Resources.Load<Tile>("GameScene/MapNodes/TileMap/Grass"));
            _mapNodeDrawControl.Draw("Grass", (0, 0));
            _mapNodeDrawControl.Draw("Grass", (3, 0));
            _mapNodeDrawControl.Draw("Grass", (0, 3));
            _mapNodeDrawControl.Draw("Grass", (3, 3));
        }
    }
}

