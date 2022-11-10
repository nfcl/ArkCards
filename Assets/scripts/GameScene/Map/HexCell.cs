using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// �ڵ���ɫ
        /// </summary>
        private Color color;
        /// <summary>
        /// �ڵ���ھ�
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;
        /// <summary>
        /// �ڵ�߶�
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// �ڵ�UI��RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// �ڵ�߶�����
        /// </summary>
        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                //��ͬ�߶Ȳ���Ҫˢ��
                if (elevation == value)
                {
                    return;
                }
                //���ø߶�
                elevation = value;
                //���ýڵ�λ��
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //��y����������Ŷ�
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //���ýڵ�UIλ��
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //���ĺ����ˢ��
                Refresh();
            }
        }
        /// <summary>
        /// �ڵ�λ��
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                {
                    return;
                }
                color = value;
                Refresh();
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public HexGridChunk chunk;

        public void Awake()
        {
            neighbors = new HexCell[6];
        }
        /// <summary>
        /// ����ˢ��
        /// </summary>
        void Refresh()
        {
            if (chunk is null) return;
            //ˢ����������
            chunk.Refresh();
            //������ڽڵ��в���ͬһ���������Ҫͬʱˢ������������
            for (int i = 0; i < neighbors.Length; i++)
            {

                HexCell neighbor = neighbors[i];
                //���ھӻ��ھ������������Լ�����������ͬ����Ҫˢ��
                if (neighbor is null || neighbor.chunk == chunk) continue;

                neighbor.chunk.Refresh();
            }
        }
        /// <summary>
        /// ���ݷ��������ڵĽڵ�
        /// </summary>
        /// <param name="direction">���ڽڵ�ķ���</param>
        /// <returns>���ض��������ڽڵ�</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// ���ݷ����������ڵĽڵ�
        /// </summary>
        /// <param name="direction">���ڽڵ�ķ���</param>
        /// <param name="cell">Ҫ���õ����ڽڵ�</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            //�����Լ����ھ�
            neighbors[(int)direction] = cell;
            //�����ھӵ��ھ�
            cell.neighbors[(int)direction.Opposite()] = this;
        }
        /// <summary>
        /// ��úͶ�Ӧ�����ھӵı�Ե��������
        /// </summary>
        /// <param name="direction">��Ӧ����</param>
        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }
    }
}