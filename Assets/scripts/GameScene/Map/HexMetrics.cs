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
        /// �ⶥ���ϵ��������������������������λ��
        /// </summary>
        public static Vector3[] corners_spire = new Vector3[]
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
        public static Vector3[] corners_flattened = new Vector3[]
        {
            new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
            new Vector3(outerRadius         ,0,0                ),
            new Vector3(outerRadius * 0.5f  ,0,-innerRadius     ),
            new Vector3(-outerRadius * 0.5f ,0,-innerRadius     ),
            new Vector3(-outerRadius        ,0,0                ),
            new Vector3(-outerRadius * 0.5f ,0,innerRadius      ),
            new Vector3(outerRadius * 0.5f  ,0,innerRadius      ),
        };
    }
}
