using GameScene.Operator;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// 创建新队伍的界面
    /// </summary>
    public class NewTeamMenu : MonoBehaviour
    {
        /// <summary>
        /// 创建的队伍名称
        /// </summary>
        private string teamName;
        /// <summary>
        /// 当前生成的队伍位置
        /// </summary>
        private HexCell currentCell;

        /// <summary>
        /// 六边形网格
        /// </summary>
        public HexGrid hexGrid;
        /// <summary>
        /// 干员列表显示容器
        /// </summary>
        public RectTransform OperatorListContent;
        /// <summary>
        /// 干员列表项
        /// </summary>
        public OperatorListItem OperatorListItemPrefab;

        /// <summary>
        /// 打开界面
        /// </summary>
        public void Open(HexCell cell)
        {
            currentCell = cell;
            InitOperatorList();
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 初始化干员列表
        /// </summary>
        public void InitOperatorList()
        {
            //删除旧的容器
            while (OperatorListContent.childCount != 0)
            {
                //Destroy会延迟删除导致一直检测到子物体死循环 -_-
                DestroyImmediate(OperatorListContent.GetChild(0).gameObject);
            }
            //生成新的容器
            Transform NewContent = new GameObject("OperatorListContent").transform;
            NewContent.SetParent(OperatorListContent);
            //遍历所有干员并将可编入小队的干员列出来
            SingleOperator[] CurrentOperators = GameSceneManager.operators.GetCurrentOperators();
            //列表项的坐标
            float tmpX = -757.5f, tmpY = 0;
            //列表项个数
            int fitOperatorNum = 0;
            //新生成的列表项
            OperatorListItem ListItemClone;
            //遍历查找符合条件的干员
            foreach (SingleOperator item in CurrentOperators)
            {
                if(item.State != OperatorState.Team)
                {
                    //克隆一个新的列表项
                    ListItemClone = Instantiate(OperatorListItemPrefab, NewContent);
                    //设置位置
                    ListItemClone.transform.localPosition = new Vector3(tmpX,tmpY);
                    //内部初始化
                    ListItemClone.Init(item);
                    //列表项个数增加
                    fitOperatorNum += 1;
                    //迭代下一个列表项的位置
                    if (fitOperatorNum % 7 == 0)
                    {
                        tmpX = -757.5f;
                        tmpY -= 252.5f;
                    }
                    else
                    {
                        tmpX += 252.5f;
                    }
                }
            }
            //设置列表显示框架的大小以显示全部内容
            OperatorListContent.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                (fitOperatorNum + 6) / 7 * (200 + 52.5f) + 52.5f
            );
            NewContent.transform.localPosition = new Vector3(0, -152.5f, 0);
            NewContent.transform.localScale = Vector3.one;
        }
        /// <summary>
        /// 玩家完成对队伍名称输入时调用
        /// </summary>
        /// <param name="newName">新的队伍名称</param>
        public void OnTeamNameChange(string newName)
        {
            teamName = newName;
        }
        /// <summary>
        /// <para/>根据现有的选择创建队伍
        /// <para/>会进行合法性检查
        /// </summary>
        public void CreateTeam()
        {
            //队伍名称非法不能创建
            if (teamName.Length == 0 || teamName == "")
            {
                return;
            }
            //获得干员列表
            OperatorListItem[] items = OperatorListContent.GetComponentsInChildren<OperatorListItem>();
            //选择的干员列表
            List<string> SelectOperators = new List<string>();
            //筛选出玩家选择的干员名称
            foreach(OperatorListItem item in items)
            {
                if (item.IsSelected == true)
                {
                    SelectOperators.Add(item.OperatorName);
                }
            }
            //如果没有选择则不能创建
            if (SelectOperators.Count == 0)
            {
                return;
            }
            //新的队伍
            Team.SingleTeam newTeam = new Team.SingleTeam(teamName);
            //将选择的干员加入新队伍
            foreach (string Name in SelectOperators)
            {
                newTeam.AddOperator(Name);
            }
            //添加新队伍到集合
            GameSceneManager.teams.AddTeam(newTeam);
            //创建地图上的队伍单位
            HexMapUnit newUnit = Instantiate(HexMapUnit.unitPrefab);
            //设置所属队伍
            newUnit.Team = newTeam;
            //添加单位到地图
            hexGrid.AddUnit(newUnit, currentCell);
            //关闭此界面
            Close();
        }
    }
}