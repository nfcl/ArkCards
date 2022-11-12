using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// ��Ԫ����
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// ��Ԫ��ɫ
        /// </summary>
        private Color color;
        /// <summary>
        /// ��Ԫ���ھ�
        /// </summary>
        private HexCell[] neighbors;
        /// <summary>
        /// ��Ԫ�߶�
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// ��Ԫ�Ƿ���ں������
        /// </summary>
        private bool hasIncomingRiver;
        /// <summary>
        /// ��Ԫ�Ƿ���ں�������
        /// </summary>
        private bool hasOutgoingRiver;
        /// <summary>
        /// ��Ԫ������ڷ���
        /// </summary>
        private HexDirection incomingRiver;
        /// <summary>
        /// ��Ԫ�������ڷ���
        /// </summary>
        private HexDirection outgoingRiver;
        /// <summary>
        /// ��ԪUI��RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// ��Ԫ�߶�����
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
                //���õ�Ԫλ��
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //��y����������Ŷ�
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //���õ�ԪUIλ��
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //������ĸ߶Ⱥ�ĺ��������������������������
                if (
                    hasOutgoingRiver &&
                    elevation < GetNeighbor(outgoingRiver).elevation
                )
                {
                    RemoveOutgoingRiver();
                }
                //������ĸ߶Ⱥ�ĺ������������������������
                if (
                    hasIncomingRiver &&
                    elevation > GetNeighbor(incomingRiver).elevation
                )
                {
                    RemoveIncomingRiver();
                }
                //���ĺ����ˢ��
                Refresh();
            }
        }
        /// <summary>
        /// ��Ԫλ�÷�����
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        /// <summary>
        /// ��Ԫ��ɫ����
        /// </summary>
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
        /// <summary>
        /// �Ƿ���ں������
        /// �� : �����Ƿ���ں������
        /// </summary>
        public bool HasIncomingRiver { get { return hasIncomingRiver; } }
        /// <summary>
        /// �Ƿ���ں�������
        /// �� : �����Ƿ���ں�������
        /// </summary>
        public bool HasOutgoingRiver { get { return hasOutgoingRiver; } }
        /// <summary>
        /// ������ڷ���
        /// �� : ���غ�����ڷ���
        /// </summary>
        public HexDirection IncomingRiver { get { return incomingRiver; } }
        /// <summary>
        /// �������ڷ���
        /// �� : ���غ������ڷ���
        /// </summary>
        public HexDirection OutgoingRiver { get { return outgoingRiver; } }
        /// <summary>
        /// ��Ԫ�Ƿ���ں���
        /// �� : �����Ƿ���ں�����ڻ����
        /// </summary>
        public bool HasRiver { get { return hasIncomingRiver || hasOutgoingRiver; } }
        /// <summary>
        /// ��Ԫ�Ƿ��Ǻ���Դͷ���յ�
        /// �� : ���غ������ڻ��������Ƿ�ͬʱ����
        /// </summary>
        public bool HasRiverBeginOrEnd { get { return hasIncomingRiver != hasOutgoingRiver; } }

        /// <summary>
        /// ����ˢ��
        /// </summary>
        private void Refresh()
        {
            if (chunk is null) return;
            //ˢ����������
            chunk.Refresh();
            //������ڵ�Ԫ�в���ͬһ���������Ҫͬʱˢ������������
            for (int i = 0; i < neighbors.Length; i++)
            {

                HexCell neighbor = neighbors[i];
                //���ھӻ��ھ������������Լ�����������ͬ����Ҫˢ��
                if (neighbor is null || neighbor.chunk == chunk) continue;

                neighbor.chunk.Refresh();
            }
        }
        /// <summary>
        /// ֻˢ�±�����
        /// </summary>
        private void RefreshSelfOnly()
        {
            chunk.Refresh();
        }
        /// <summary>
        /// ���ݷ��������ڵĵ�Ԫ
        /// </summary>
        /// <param name="direction">���ڵ�Ԫ�ķ���</param>
        /// <returns>���ض��������ڵ�Ԫ</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// ���ݷ����������ڵĵ�Ԫ
        /// </summary>
        /// <param name="direction">���ڵ�Ԫ�ķ���</param>
        /// <param name="cell">Ҫ���õ����ڵ�Ԫ</param>
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
        /// <summary>
        /// ĳ����ı�Ե�Ƿ��к�������
        /// </summary>
        /// <param name="direction">����</param>
        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return
                hasIncomingRiver && incomingRiver == direction ||
                hasOutgoingRiver && outgoingRiver == direction;
        }
        /// <summary>
        /// �Ƴ���������
        /// </summary>
        public void RemoveOutgoingRiver()
        {
            //��������ں��������򷵻�
            if (!hasOutgoingRiver) return;
            //�����������
            hasOutgoingRiver = false;
            //ˢ��
            RefreshSelfOnly();
            //�����Ӧ�����ھӵĺ������
            HexCell neighbor = GetNeighbor(outgoingRiver);
            neighbor.hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// �Ƴ��������
        /// </summary>
        public void RemoveIncomingRiver()
        {
            //��������ں�������򷵻�
            if (!hasIncomingRiver) return;
            //����������
            hasIncomingRiver = false;
            //ˢ��
            RefreshSelfOnly();
            //�����Ӧ�ھӵĺ�������
            HexCell neighbor = GetNeighbor(incomingRiver);
            neighbor.hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// �Ƴ�����
        /// </summary>
        public void RemoveRiver()
        {
            //�Ƴ���������
            RemoveOutgoingRiver();
            //�Ƴ��������
            RemoveIncomingRiver();
        }
        /// <summary>
        /// ����������������ĳ����
        /// </summary>
        /// <param name="direction">Ҫ���ú������ڵķ���</param>
        public void SetOutgoingRiver(HexDirection direction)
        {
            //�����Ҫ���õķ����Ѵ��ں��������򷵻�
            if (hasOutgoingRiver && outgoingRiver == direction) return;
            //��ö�Ӧ������ھ�
            HexCell neighbor = GetNeighbor(direction);
            //����ھӲ����ڻ����ھӵĸ߶ȴ��ڱ���Ԫ�߶ȣ��������£��򷵻�
            if (!neighbor || elevation < neighbor.elevation) return;
            //�Ƴ���������
            RemoveOutgoingRiver();
            //���Ҫ���õķ����Ѵ��ں�����������Ƴ��������
            if (hasIncomingRiver && incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }
            //���ú�������
            hasOutgoingRiver = true;
            outgoingRiver = direction;
            //ˢ��
            RefreshSelfOnly();
            //�����ھӵĺ������
            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            neighbor.RefreshSelfOnly();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            neighbors = new HexCell[6];
        }
    }
}