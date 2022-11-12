using System;
using UnityEngine;

namespace GameScene.Map
{
    public static class HexMetrics
    {
        /// <summary>
        /// 
        /// </summary>
        public const float outerToInner = 0.866025404f;
        /// <summary>
        /// 
        /// </summary>
        public const float innerToOuter = 1f / outerToInner;
        /// <summary>
        /// 六边形外切圆半径
        /// </summary>
        public const float outerRadius = 10f;
        /// <summary>
        /// 六边形内切圆半径
        /// </summary>
        public const float innerRadius = outerRadius * outerToInner;
        /// <summary>
        /// 尖顶朝上的六边形六个角坐标相对于中心位置
        /// </summary>
        private static Vector3[] corners_spire = new Vector3[]
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
        private static Vector3[] corners_flattened = new Vector3[]
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
        /// 单个区块的水平长度
        /// </summary>
        public const int chunkSizeX = 5;
        /// <summary>
        /// 单个区块的垂直长度
        /// </summary>
        public const int chunkSizeZ = 5;
        /// <summary>
        /// 纯色区域比例
        /// </summary>
        public const float solidFactor = 0.8f;
        /// <summary>
        /// 混合色区域比例
        /// </summary>
        public const float blendFactor = 1f - solidFactor;
        /// <summary>
        /// 相邻节点高度每单位实际y轴差距
        /// </summary>
        public const float elevationStep = 3f;
        /// <summary>
        /// 每个斜面的平台个数
        /// </summary>
        public const int terracesPerSlope = 2;
        /// <summary>
        /// 每个斜面的面数
        /// </summary>
        public const int terraceSteps = terracesPerSlope * 2 + 1;
        /// <summary>
        /// 斜面阶梯的水平均分量
        /// </summary>
        public const float horizontalTerraceStepSize = 1f / terraceSteps; 
        /// <summary>
        /// 斜面阶梯的垂直均分量
        /// </summary>
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
        /// <summary>
        /// 噪声图
        /// </summary>
        public static Texture2D noiseSource = Resources.Load<Texture2D>("GameScene/MapNodes/sprite/noise");
        /// <summary>
        /// 噪声扰动强度
        /// </summary>
        public const float cellPerturbStrength = 0f;// 4f;
        /// <summary>
        /// 噪声缩放
        /// </summary>
        public const float noiseScale = 0.003f;
        /// <summary>
        /// 垂直方向噪声扰动强度
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;
        /// <summary>
        /// 河床深度相对偏移量
        /// </summary>
        public const float streamBedElevationOffset = -1f;

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
        /// <summary>
        /// 斜面插值向量计算
        /// </summary>
        /// <param name="a">起始位置</param>
        /// <param name="b">终止位置</param>
        /// <param name="step">位于斜面位置</param>
        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            //计算水平插值
            float h = step * HexMetrics.horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            //计算垂直插值
            float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }
        /// <summary>
        /// 斜面颜色插值计算
        /// </summary>
        /// <param name="a">起始颜色</param>
        /// <param name="b">终止颜色</param>
        /// <param name="step">位于斜面位置</param>
        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }
        /// <summary>
        /// 判断两个高度间的连接类型
        /// </summary>
        /// <param name="elevation1">高度1</param>
        /// <param name="elevation2">高度2</param>
        /// <returns>返回连接类型</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {//高度相同 平坦
                return HexEdgeType.Flat;
            }
            int delta = Math.Abs(elevation1 - elevation2);
            if (delta == 1)
            {//差距较小 斜坡
                return HexEdgeType.Slope;
            }
            else
            {//差距过大 悬崖
                return HexEdgeType.Cliff;
            }
        }
        /// <summary>
        /// 噪声采样
        /// </summary>
        /// <param name="position">三维位置</param>
        /// <returns></returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }
        /// <summary>
        /// 对顶点进行噪声扰动
        /// </summary>
        /// <param name="position">原顶点位置</param>
        /// <returns>返回扰动后的顶点位置</returns>
        public static Vector3 Perturb(Vector3 position)
        {
            //获得噪声采样
            Vector4 sample = SampleNoise(position);
            //对x轴进行扰动
            position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
            //对z轴进行扰动
            position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
            //返回扰动后的坐标
            return position;
        }
        /// <summary>
        /// 获得指定方向纯色边缘的中间位置
        /// </summary>
        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (corners_spire[(int)direction] + corners_spire[(int)direction + 1]) * (0.5f * solidFactor);
        }
    }
}
