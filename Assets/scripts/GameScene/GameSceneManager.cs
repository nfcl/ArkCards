using GameScene.Map;
using UnityEngine;

namespace GameScene
{
    public class GameSceneManager : MonoBehaviour
    {
        /// <summary>
        /// 干员信息
        /// </summary>
        public static Operator.OperatorCollection operators;
        /// <summary>
        /// 小队信息
        /// </summary>
        public static Team.TeamCollection teams;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
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
            GameObject.Find("HexGrid").GetComponent<HexGrid>().Init();
        }
        /// <summary>
        /// 干员信息初始化
        /// </summary>
        public void OperatorInit()
        {
            //初始化干员原始信息
            Operator.OperatorMetrices.InitData();
            //干员集合初始化
            operators = new Operator.OperatorCollection();
            operators.InitData();
        }
        /// <summary>
        /// 小队信息初始化
        /// </summary>
        public void TeamInit()
        {
            //小队集合初始化
            teams = new Team.TeamCollection();
            teams.InitData();
        }

        public void Awake()
        {
            Init();
        }

    }
}