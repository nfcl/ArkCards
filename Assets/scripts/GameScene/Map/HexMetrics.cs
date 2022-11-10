using System;
using UnityEngine;

namespace GameScene.Map
{
    public static class HexMetrics
    {
        /// <summary>
        /// 六边形外切圆半径
        /// </summary>
        public const float outerRadius = 10f;
        /// <summary>
        /// 六边形内切圆半径
        /// </summary>
        public const float innerRadius = outerRadius * Tool.Math.sqrt3 / 2; 
        /// <summary>
        /// 纯色区域比例
        /// </summary>
        public const float solidFactor = 0.75f;
        /// <summary>
        /// 混合色区域比例
        /// </summary>
        public const float blendFactor = 1f - solidFactor;
        /// <summary>
        /// 尖顶朝上的六边形六个角坐标相对于中心位置
        /// </summary>
        static Vector3[] corners_spire = new Vector3[]
        {
            new Vector3(0f          , 0f, outerRadius           ),
            new Vector3(innerRadius , 0f, outerRadius * 0.5f    ),
            new Vector3(innerRadius , 0f, outerRadius * -0.5f   ),
            new Vector3(0f          , 0f, -outerRadius          ),
            new Vector3(-innerRadius, 0f, outerRadius * -0.5f   ),
            new Vector3(-innerRadius, 0f, outerRadius *0.5f     ),
            new Vector3(0f          , 0f, outerRadius           )
        };
        /// <summary>
        /// 平顶朝上的六边形六个角坐标相对于中心位置
        /// </summary>
        static Vector3[] corners_flattened = new Vector3[]
        {
            new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
            new Vector3(outerRadius         ,0,0                ),
            new Vector3(outerRadius * 0.5f  ,0,-innerRadius     ),
            new Vector3(-outerRadius * 0.5f ,0,-innerRadius     ),
            new Vector3(-outerRadius        ,0,0                ),
            new Vector3(-outerRadius * 0.5f ,0,innerRadius      ),
            new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
        };

        /// <summary>
        /// 根据给定方向给出方向向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners_spire[(int)direction];
        }
        /// <summary>
        /// 根据给定方向给出顺时针方向的下一个方向
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners_spire[(int)direction + 1];
        }
        /// <summary>
        /// 获得给定方向的纯色区域向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners_spire[(int)direction] * solidFactor;
        }
        /// <summary>
        /// 获得给定方向的下一个方向纯色区域向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners_spire[(int)direction + 1] * solidFactor;
        }
        /// <summary>
        /// 获得纯色区域底边对应到混色底边的方向向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners_spire[(int)direction] + corners_spire[(int)direction + 1]) *
                blendFactor;
        }
    }
}
