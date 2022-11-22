using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// <para/>六边形度量（直译）
    /// <para/>存储了一些基础数据和一些封装后的方法
    /// </summary>
    public static class HexMetrics
    {
        /// <summary>
        /// 内切圆半径 ：外切圆半径
        /// </summary>
        public const float outerToInner = 0.866025404f;
        /// <summary>
        /// 外切圆半径 ：内切圆半径
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
        private readonly static Vector3[] corners = new Vector3[]
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
        //private readonly static Vector3[] corners_flattened = new Vector3[]
        //{
        //    new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
        //    new Vector3(outerRadius         ,0,0                ),
        //    new Vector3(outerRadius * 0.5f  ,0,-innerRadius     ),
        //    new Vector3(-outerRadius * 0.5f ,0,-innerRadius     ),
        //    new Vector3(-outerRadius        ,0,0                ),
        //    new Vector3(-outerRadius * 0.5f ,0,innerRadius      ),
        //    new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
        //};
        /// <summary>
        /// 单个区块的水平单元数
        /// </summary>
        public const int chunkSizeX = 5;
        /// <summary>
        /// 单个区块的垂直单元数
        /// </summary>
        public const int chunkSizeZ = 5;
        /// <summary>
        /// 六边形纯色区域半径占总半径的比例
        /// </summary>
        public const float solidFactor = 0.8f;
        /// <summary>
        /// 六边形混合色区域（过渡区）环半径占总半径的比例
        /// </summary>
        public const float blendFactor = 1f - solidFactor;
        /// <summary>
        /// 每单位节点高度的实际y轴大小
        /// </summary>
        public const float elevationStep = 3f;
        /// <summary>
        /// 每个斜坡的平台个数
        /// </summary>
        public const int terracesPerSlope = 2;
        /// <summary>
        /// 每个斜坡的总面数(包括水平和倾斜的)
        /// </summary>
        public const int terraceSteps = terracesPerSlope * 2 + 1;
        /// <summary>
        /// 单元连接边缘斜坡阶梯的水平均分量
        /// </summary>
        public const float horizontalTerraceStepSize = 1f / terraceSteps;
        /// <summary>
        /// 单元连接边缘斜坡阶梯的垂直均分量
        /// </summary>
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
        /// <summary>
        /// 噪声图
        /// </summary>
        public static Texture2D noiseSource; //= Resources.Load<Texture2D>("GameScene/MapNodes/sprite/noise");
        /// <summary>
        /// 噪声强度缩放
        /// </summary>
        public const float noiseScale = 0.003f;
        /// <summary>
        /// 单元噪声扰动强度
        /// </summary>
        public const float cellPerturbStrength = 4f;
        /// <summary>
        /// 垂直方向噪声扰动强度
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;
        /// <summary>
        /// 河床深度偏移量（相对于单元高度）
        /// </summary>
        public const float streamBedElevationOffset = -1.75f;
        /// <summary>
        /// 河流表面高度偏移量（相对于单元高度）
        /// </summary>
        public const float waterElevationOffset = -0.5f;
        /// <summary>
        /// 水面六边形纯色区域半径占总半径的比例
        /// </summary>
        public const float waterFactor = 0.6f;
        /// <summary>
        /// 水面六边形混合色区域（过渡区）环半径占总半径的比例
        /// </summary>
        public const float waterBlendFactor = 1f - waterFactor;
        /// <summary>
        /// 随机哈希网格大小
        /// </summary>
        public const int hashGridSize = 256;
        /// <summary>
        /// 哈希网格采样放缩
        /// </summary>
        public const float hashGridScale = 0.25f;
        /// <summary>
        /// 地形集合
        /// </summary>
        public static HexTerrainType[] HexTerrains = new HexTerrainType[]
        {
            //Grass
            new HexTerrainType{ type = 0, typeName = "Grass"},
            //Stone
            new HexTerrainType{ type = 1, typeName = "Stone"},
            //Sand
            new HexTerrainType{ type = 2, typeName = "Sand"},
            //Snow
            new HexTerrainType{ type = 3, typeName = "Snow"},
            //Mud
            new HexTerrainType{ type = 4, typeName = "Mud"}
        };

        /// <summary>
        /// 哈希网格
        /// </summary>
        private static HexHash[] hashGrid;

        /// <summary>
        /// 初始化哈希网格
        /// </summary>
        public static void InitializeHashGrid(int seed)
        {
            //保存当前的随机数生成器状态
            Random.State currentState = Random.state;
            //设置随机数种子
            Random.InitState(seed);
            //初始化哈希网格
            hashGrid = new HexHash[hashGridSize * hashGridSize];
            for (int i = 0; i < hashGrid.Length; i++)
            {
                hashGrid[i] = HexHash.Create();
            }
            //设置回状态
            Random.state = currentState;
        }
        /// <summary>
        /// 通过Vector3对哈希网格采样
        /// </summary>
        public static HexHash SampleHashGrid(Vector3 position)
        {
            int x = (int)(position.x * hashGridScale) % hashGridSize;
            if (x < 0)
            {
                x += hashGridSize;
            }
            int z = (int)(position.z * hashGridScale) % hashGridSize;
            if (z < 0)
            {
                z += hashGridSize;
            }
            return hashGrid[x + z * hashGridSize];
        }
        /// <summary>
        /// 给定方向的左侧顶点的向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners[(int)direction];
        }
        /// <summary>
        /// 给定方向的右侧顶点的向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners[(int)direction + 1];
        }
        /// <summary>
        /// 给定方向从中心到纯色区域边缘的向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners[(int)direction] * solidFactor;
        }
        /// <summary>
        /// 给定方向的下一个方向从中心到纯色区域边缘向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners[(int)direction + 1] * solidFactor;
        }
        /// <summary>
        /// 获得六边形纯色区边缘至混色区边缘的垂直向量
        /// </summary>
        /// <param name="direction">给定方向</param>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) *
                blendFactor;
        }
        /// <summary>
        /// 单元斜坡边缘阶梯插值点位置计算
        /// </summary>
        /// <param name="a">起始位置</param>
        /// <param name="b">终止位置</param>
        /// <param name="step">要求的插值点位置</param>
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
        /// 单元斜坡边缘阶梯插值点颜色计算
        /// </summary>
        /// <param name="a">起始颜色</param>
        /// <param name="b">终止颜色</param>
        /// <param name="step">要求的插值点位置</param>
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
        /// 指定方向纯色区边缘的中间位置
        /// </summary>
        /// <param name="direction">指定方向</param>
        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            //返回两边角的向量相加除以2
            return (corners[(int)direction] + corners[(int)direction + 1]) * (0.5f * solidFactor);
        }
        /// <summary>
        /// 使用三维坐标中的X轴和Z轴对噪声图进行二维噪声采样
        /// </summary>
        /// <param name="position">三维位置</param>
        /// <returns>返回一个颜色的四元数</returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }
        /// <summary>
        /// 对三维顶点位置进行噪声扰动
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
        /// 给定方向的水面六边形左侧顶点向量
        /// </summary>
        public static Vector3 GetFirstWaterCorner(HexDirection direction)
        {
            return corners[(int)direction] * waterFactor;
        }
        /// <summary>
        /// 给定方向的水面六边形右侧顶点向量
        /// </summary>
        public static Vector3 GetSecondWaterCorner(HexDirection direction)
        {
            return corners[(int)direction + 1] * waterFactor;
        }
        /// <summary>
        /// 获得水面六边形纯色区边缘至混色区边缘的垂直向量
        /// </summary>
        public static Vector3 GetWaterBridge(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) *
                waterBlendFactor;
        }
    }
}
