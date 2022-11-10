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
        public Color color; 
        /// <summary>
        /// �ڵ���ھ�
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;
        /// <summary>
        /// �ڵ�߶�
        /// </summary>
        private int elevation;
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

        public void Awake()
        {
            neighbors = new HexCell[6];
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