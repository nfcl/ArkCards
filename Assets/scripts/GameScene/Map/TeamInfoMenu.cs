using GameScene.Team;
using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// 显示地图上的队伍信息界面
    /// </summary>
    public class TeamInfoMenu : MonoBehaviour
    {
        /// <summary>
        /// 当前显示的队伍
        /// </summary>
        private SingleTeam currentTeam;

        /// <summary>
        /// 打开界面并设置当前显示的队伍
        /// </summary>
        /// <param name="team"></param>
        public void Open(SingleTeam team)
        {
            currentTeam = team;
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            currentTeam = null;
            gameObject.SetActive(false);
        }
    }
}