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
        /// ����������Բ�뾶
        /// </summary>
        public const float outerRadius = 10f;
        /// <summary>
        /// ����������Բ�뾶
        /// </summary>
        public const float innerRadius = outerRadius * outerToInner;
        /// <summary>
        /// �ⶥ���ϵ��������������������������λ��
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
        /// ƽ�����ϵ��������������������������λ��
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
        /// ���������ˮƽ����
        /// </summary>
        public const int chunkSizeX = 5;
        /// <summary>
        /// ��������Ĵ�ֱ����
        /// </summary>
        public const int chunkSizeZ = 5;
        /// <summary>
        /// ��ɫ�������
        /// </summary>
        public const float solidFactor = 0.8f;
        /// <summary>
        /// ���ɫ�������
        /// </summary>
        public const float blendFactor = 1f - solidFactor;
        /// <summary>
        /// ���ڽڵ�߶�ÿ��λʵ��y����
        /// </summary>
        public const float elevationStep = 3f;
        /// <summary>
        /// ÿ��б���ƽ̨����
        /// </summary>
        public const int terracesPerSlope = 2;
        /// <summary>
        /// ÿ��б�������
        /// </summary>
        public const int terraceSteps = terracesPerSlope * 2 + 1;
        /// <summary>
        /// б����ݵ�ˮƽ������
        /// </summary>
        public const float horizontalTerraceStepSize = 1f / terraceSteps; 
        /// <summary>
        /// б����ݵĴ�ֱ������
        /// </summary>
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
        /// <summary>
        /// ����ͼ
        /// </summary>
        public static Texture2D noiseSource = Resources.Load<Texture2D>("GameScene/MapNodes/sprite/noise");
        /// <summary>
        /// �����Ŷ�ǿ��
        /// </summary>
        public const float cellPerturbStrength = 0f;// 4f;
        /// <summary>
        /// ��������
        /// </summary>
        public const float noiseScale = 0.003f;
        /// <summary>
        /// ��ֱ���������Ŷ�ǿ��
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;
        /// <summary>
        /// �Ӵ�������ƫ����
        /// </summary>
        public const float streamBedElevationOffset = -1f;

        /// <summary>
        /// ���ݸ������������������
        /// </summary>
        /// <param name="direction">��������</param>
        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners_spire[(int)direction];
        }
        /// <summary>
        /// ���ݸ����������˳ʱ�뷽�����һ������
        /// </summary>
        /// <param name="direction">��������</param>
        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners_spire[(int)direction + 1];
        }
        /// <summary>
        /// ��ø�������Ĵ�ɫ��������
        /// </summary>
        /// <param name="direction">��������</param>
        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners_spire[(int)direction] * solidFactor;
        }
        /// <summary>
        /// ��ø����������һ������ɫ��������
        /// </summary>
        /// <param name="direction">��������</param>
        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners_spire[(int)direction + 1] * solidFactor;
        }
        /// <summary>
        /// ��ô�ɫ����ױ߶�Ӧ����ɫ�ױߵķ�������
        /// </summary>
        /// <param name="direction">��������</param>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners_spire[(int)direction] + corners_spire[(int)direction + 1]) *
                blendFactor;
        }
        /// <summary>
        /// б���ֵ��������
        /// </summary>
        /// <param name="a">��ʼλ��</param>
        /// <param name="b">��ֹλ��</param>
        /// <param name="step">λ��б��λ��</param>
        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            //����ˮƽ��ֵ
            float h = step * HexMetrics.horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            //���㴹ֱ��ֵ
            float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }
        /// <summary>
        /// б����ɫ��ֵ����
        /// </summary>
        /// <param name="a">��ʼ��ɫ</param>
        /// <param name="b">��ֹ��ɫ</param>
        /// <param name="step">λ��б��λ��</param>
        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }
        /// <summary>
        /// �ж������߶ȼ����������
        /// </summary>
        /// <param name="elevation1">�߶�1</param>
        /// <param name="elevation2">�߶�2</param>
        /// <returns>������������</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {//�߶���ͬ ƽ̹
                return HexEdgeType.Flat;
            }
            int delta = Math.Abs(elevation1 - elevation2);
            if (delta == 1)
            {//����С б��
                return HexEdgeType.Slope;
            }
            else
            {//������ ����
                return HexEdgeType.Cliff;
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="position">��άλ��</param>
        /// <returns></returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }
        /// <summary>
        /// �Զ�����������Ŷ�
        /// </summary>
        /// <param name="position">ԭ����λ��</param>
        /// <returns>�����Ŷ���Ķ���λ��</returns>
        public static Vector3 Perturb(Vector3 position)
        {
            //�����������
            Vector4 sample = SampleNoise(position);
            //��x������Ŷ�
            position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
            //��z������Ŷ�
            position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
            //�����Ŷ��������
            return position;
        }
        /// <summary>
        /// ���ָ������ɫ��Ե���м�λ��
        /// </summary>
        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (corners_spire[(int)direction] + corners_spire[(int)direction + 1]) * (0.5f * solidFactor);
        }
    }
}
