using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        private Operator.OperatorCollection     _operators;             //干员信息
        private Team.TeamCollection             _teams;                 //小队信息

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

        }

        /// <summary>
        /// 干员信息初始化
        /// </summary>
        public void OperatorInit()
        {
            //干员集合初始化
            _operators = new Operator.OperatorCollection();
            //从文件读取玩家干员信息
            //TODO
        }

        /// <summary>
        /// 小队信息初始化
        /// </summary>
        public void TeamInit()
        {
            //小队集合初始化
            _teams = new Team.TeamCollection();
            //从文件读取玩家小队信息
            //TODO
        }
    }
}