using System;
using UnityEngine;

namespace GameScene.Map
{
    public static class HexMetrics
    {
        /// <summary>
        /// ����������Բ�뾶
        /// </summary>
        public const float outerRadius = 10f;
        /// <summary>
        /// ����������Բ�뾶
        /// </summary>
        public const float innerRadius = outerRadius * Tool.Math.sqrt3 / 2; 
        /// <summary>
        /// ��ɫ�������
        /// </summary>
        public const float solidFactor = 0.75f;
        /// <summary>
        /// ���ɫ�������
        /// </summary>
        public const float blendFactor = 1f - solidFactor;
        /// <summary>
        /// �ⶥ���ϵ��������������������������λ��
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
        /// ƽ�����ϵ��������������������������λ��
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
    }
}
