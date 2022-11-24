using UnityEngine;
using Spine.Unity;

namespace GameScene.Map
{
    using System.IO;
    using Team;
    using static UnityEditor.FilePathAttribute;

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
        /// 
        /// </summary>
        private SingleTeam team;
        /// <summary>
        /// 当前所在的单元格
        /// </summary>
        private HexCell currentCell;
        /// <summary>
        /// 移动速度
        /// </summary>
        private float speed;
        /// <summary>
        /// 新的移动位置
        /// </summary>
        private Vector3 newPosition;

        /// <summary>
        /// spine小人的动画控制器
        /// </summary>
        public SkeletonAnimation spineAnime;

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
                return team.speedPerTurn;
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
                    currentCell.Unit = null;
                }
                currentCell = value;

                value.Unit = this;

                transform.localPosition = value.Position;
            }
        }

        /// <summary>
        /// <para/>直线移动至目标单元格
        /// </summary>
        public void Move(HexCell cell, float duration)
        {
            currentCell = cell;
            speed = Vector3.Distance(currentCell.transform.position, transform.position) / duration;
            ChangeSpineAnimation("Move");
            enabled = true;
        }
        /// <summary>
        /// 更改spine动画
        /// </summary>
        public void ChangeSpineAnimation(string animeName)
        {
            if (spineAnime.AnimationName != animeName)
            {
                spineAnime.AnimationName = animeName;
            }
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
            currentCell.Unit = null;
            Destroy(gameObject);
        }
        /// <summary>
        /// 判断是否是合法的目的地
        /// </summary>
        public bool IsValidDestination(HexCell cell)
        {
            return !cell.IsUnderwater && !cell.Unit;
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
        /// 
        /// </summary>
        public void Update()
        {
            //transform.LookAt(Camera.main.transform);

            newPosition = Vector3.MoveTowards(transform.position, currentCell.transform.position, speed * Time.deltaTime);
            transform.position = newPosition;
            if(transform.position == currentCell.transform.position)
            {
                ChangeSpineAnimation("Relax");
                enabled = false;
            }
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