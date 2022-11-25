using UnityEngine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameScene.Team;
using static UnityEditor.FilePathAttribute;

namespace GameScene.Map
{

    /// <summary>
    /// 地图上的小队单位
    /// </summary>
    public class HexMapUnit : MonoBehaviour
    {
        /// <summary>
        /// 地图单位预制件
        /// </summary>
        public static HexMapUnit unitPrefab;
        /// <summary>
        /// 单位移动动画速度
        /// </summary>
        public static float travelSpeed = 0.8f;

        /// <summary>
        /// 
        /// </summary>
        private SingleTeam team;
        /// <summary>
        /// 当前所在的单元格
        /// </summary>
        private HexCell currentCell;
        /// <summary>
        /// 路径单元集
        /// </summary>
        private List<HexCell> pathToTravel;
        /// <summary>
        /// 可见视野半径
        /// </summary>
        public const int visionRange = 3;

        /// <summary>
        /// spine小人的动画控制器
        /// </summary>
        public SkeletonAnimation spineAnime;

        /// <summary>
        /// 
        /// </summary>
        public HexGrid Grid { get; set; }

        /// <summary>
        /// 小队名称属性
        /// 读 : 返回小队名称
        /// </summary>
        public string UnitName
        {
            get
            {
                return team.TeamName;
            }
        }
        /// <summary>
        /// 每回合可移动距离属性
        /// 读 : 返回每回合可移动距离
        /// </summary>
        public int speedPerTurn
        {
            get
            {
                //return team.speedPerTurn;
                return 200;
            }
        }
        /// <summary>
        /// 当前所在单元格属性
        /// 读 : 返回当前所在单元格
        /// 写 : 设置当前所在单元格并将自身位置与其对其
        /// </summary>
        public HexCell CurrentCell
        {
            get
            {
                return currentCell;
            }
            set
            {
                if ((currentCell is null) == false)
                {
                    Grid.DecreaseVisibility(currentCell, visionRange);
                    currentCell.Unit = null;
                }
                currentCell = value;

                value.Unit = this;

                Grid.IncreaseVisibility(value, visionRange);

                transform.localPosition = value.Position;
            }
        }
        /// <summary>
        /// 单元以屏幕为坐标系是否朝向左边
        /// </summary>
        public bool TowardsLeft
        {
            get
            {
                return transform.localEulerAngles.y > 180;
            }
        }

        /// <summary>
        /// 更改spine动画
        /// </summary>
        private void ChangeSpineAnimation(string animeName)
        {
            if (spineAnime.AnimationName != animeName)
            {
                spineAnime.AnimationName = animeName;
            }
        }
        /// <summary>
        /// 使单位朝向指定位置的协程
        /// </summary>
        IEnumerator LookAt(bool isLeft)
        {
            Quaternion fromRotation = transform.localRotation;
            Quaternion toRotation = Quaternion.LookRotation(isLeft ? Vector3.forward : Vector3.back);

            if(fromRotation == toRotation)
            {
                yield break;
            }

            for (float t = Time.deltaTime; t < 1f; t += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
            transform.localRotation = toRotation;
        }
        /// <summary>
        /// 单位移动动画协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator TravelPath()
        {
            ChangeSpineAnimation("Move");

            Vector3 a, b;

            for (int i = 1; i < pathToTravel.Count; i++)
            {
                a = pathToTravel[i - 1].Position;
                b = pathToTravel[i].Position;

                yield return LookAt(b.x > a.x);

                Grid.IncreaseVisibility(pathToTravel[i], visionRange);

                for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    transform.localPosition = Vector3.Lerp(a, b, t);
                    yield return null;
                }
                Grid.DecreaseVisibility(pathToTravel[i - 1], visionRange);
            }
            ChangeSpineAnimation("Relax");
        }

        /// <summary>
        /// 根据路径移动
        /// </summary>
        public void Travel(List<HexCell> path)
        {
            currentCell.Unit = null;
            currentCell = path[path.Count - 1];
            currentCell.Unit = this;
            pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }
        /// <summary>
        /// 调整位置
        /// </summary>
        public void ValidateLocation()
        {
            transform.localPosition = currentCell.Position;
        } 
        /// <summary>
        /// 单位死亡（移除）
        /// </summary>
        public void Die()
        {
            if (currentCell)
            {
                Grid.DecreaseVisibility(currentCell, visionRange);
            }
            currentCell.Unit = null;
            Destroy(gameObject);
        }
        /// <summary>
        /// 判断是否是合法的目的地
        /// </summary>
        public bool IsValidDestination(HexCell cell)
        {
            return cell.IsExplored && !cell.IsUnderwater && !cell.Unit;
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void Save(BinaryWriter writer)
        {
            CurrentCell.coordinates.Save(writer);
        }
        /// <summary>
        /// 加载
        /// </summary>
        public static void Load(BinaryReader reader, HexGrid grid)
        {
            HexCoordinates coordinates = HexCoordinates.Load(reader);

            grid.AddUnit(Instantiate(unitPrefab), grid.GetCell(coordinates));
        }
        /// <summary>
        /// 获得两个相邻单元间的移动距离
        /// </summary>
        public int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction)
        {
            HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
            if (edgeType == HexEdgeType.Cliff)
            {
                return -1;
            }
            int moveCost;
            if (fromCell.HasRoadThroughEdge(direction))
            {
                moveCost = 1;
            }
            else
            {
                moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            }
            return moveCost;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            ChangeSpineAnimation("Relax");
            enabled = false;
        }
    } 
}