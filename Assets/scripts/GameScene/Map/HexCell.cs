using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameScene.Map
{
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// ������ɫ
        /// </summary>
        private Color color;
        /// <summary>
        /// �ھ�
        /// </summary>
        private HexCell[] neighbors;
        /// <summary>
        /// ���θ߶�
        /// </summary>
        private int elevation = int.MinValue;
        /// <summary>
        /// �Ƿ���ں������
        /// </summary>
        private bool hasIncomingRiver;
        /// <summary>
        /// �Ƿ���ں�������
        /// </summary>
        private bool hasOutgoingRiver;
        /// <summary>
        /// ������ڷ���
        /// </summary>
        private HexDirection incomingRiver;
        /// <summary>
        /// �������ڷ���
        /// </summary>
        private HexDirection outgoingRiver;
        /// <summary>
        /// ���������Ƿ���ڵ�·
        /// </summary>
        [SerializeField]
        private bool[] roads;
        /// <summary>
        /// ˮ��߶�
        /// </summary>
        private int waterLevel;
        /// <summary>
        /// ϸ���ܶȼ���
        /// </summary>
        private int urbanLevel;

        /// <summary>
        /// ����
        /// </summary>
        public HexCoordinates coordinates;
        /// <summary>
        /// <para/>��Ԫ����������
        /// </summary>
        public HexGridChunk chunk;
        /// <summary>
        /// UI��RectTransform
        /// </summary>
        public RectTransform uiRect;
        /// <summary>
        /// <para/>�߶�����
        /// <para/>�� �����ظ߶�
        /// <para/>д ��д��߶ȣ�����ʵ���UI��Y��λ�ã����Ժ����ĺϷ��Խ��м�⣬ˢ������
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
                //���õ�Ԫy��λ��
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                //��y����������Ŷ�
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;
                //���õ�ԪUIλ��
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                //�Ƴ��Ƿ��ĺ��������
                ValidateRivers();
                //�жϸ������·�ĺϷ���
                for (int i = 0; i < roads.Length; i++)
                {
                    //�жϴ˷����·�Ϸ���
                    if
                    (
                        //���ڵ�·
                        roads[i] == true
                        //���Ӧ�����ھӵĸ߶Ȳ����
                        && GetElevationDifference((HexDirection)i) > 1
                    )
                    {
                        //����˷����·
                        SetRoad(i, false);
                    }
                }

                //���ĺ����ˢ��
                Refresh();
            }
        }
        /// <summary>
        /// <para/>��Ԫλ������
        /// <para/>�� ������ʵ���Ը�����Ϊ����ϵ��λ��
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        /// <summary>
        /// <para/>��Ԫ��ɫ����
        /// <para/>�� �����ص�����ɫ
        /// <para/>д ��������ɫ��ˢ������
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
        /// <para/>�Ƿ���ں����������
        /// <para/>�� : �����Ƿ���ں������
        /// </summary>
        public bool HasIncomingRiver
        {
            get
            {
                return hasIncomingRiver;
            }
        }
        /// <summary>
        /// <para/>�Ƿ���ں�����������
        /// <para/>�� : �����Ƿ���ں�������
        /// </summary>
        public bool HasOutgoingRiver
        {
            get
            {
                return hasOutgoingRiver;
            }
        }
        /// <summary>
        /// <para/>������ڷ�������
        /// <para/>�� : ���غ�����ڷ���
        /// </summary>
        public HexDirection IncomingRiver
        {
            get
            {
                return incomingRiver;
            }
        }
        /// <summary>
        /// <para/>�������ڷ�������
        /// <para/>�� : ���غ������ڷ���
        /// </summary>
        public HexDirection OutgoingRiver
        {
            get
            {
                return outgoingRiver;
            }
        }
        /// <summary>
        /// <para/>��Ԫ�Ƿ���ں�������
        /// <para/>�� : �����Ƿ���ں�����ڻ����
        /// </summary>
        public bool HasRiver
        {
            get
            {
                return hasIncomingRiver || hasOutgoingRiver;
            }
        }
        /// <summary>
        /// <para/>��Ԫ�Ƿ��Ǻ���Դͷ���յ�����
        /// <para/>�� : ���غ������ڻ��������Ƿ�ͬʱ����
        /// </summary>
        public bool HasRiverBeginOrEnd
        {
            get
            {
                return hasIncomingRiver != hasOutgoingRiver;
            }
        }
        /// <summary>
        /// ��Ԫ������ڻ���ڷ���
        /// �� : ���ں�������򷵻���ڷ��򣬷��򷵻س��ڷ���
        /// </summary>
        public HexDirection RiverBeginOrEndDirection
        {
            get
            {
                return hasIncomingRiver ? incomingRiver : outgoingRiver;
            }
        }
        /// <summary>
        /// <para/>�Ӵ��׵�Y����������
        /// <para/>�� : ���ؼ����ĺӴ���Y������
        /// </summary>
        public float StreamBedY
        {
            get
            {
                return
                    (elevation + HexMetrics.streamBedElevationOffset) *
                    HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>��������Y����������
        /// <para/>�� : ���ؼ����ĺ�������Y������
        /// </summary>
        public float RiverSurfaceY
        {
            get
            {
                return
                    (elevation + HexMetrics.waterElevationOffset) *
                    HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>�õ�Ԫ�Ƿ���ڵ�·����
        /// <para/>�� : �������������Ƿ������ⷽ����ڵ�·
        /// </summary>
        public bool HasRoads
        {
            get
            {
                for (int i = 0; i < roads.Length; i++)
                {
                    if (roads[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// <para/>ˮ��߶�����
        /// <para/>�� : ���ص�ǰ��Ԫ��ˮ��߶�
        /// <para/>д : ���õ�ǰ��Ԫ��ˮ��߶Ȳ�ˢ�µ���������
        /// </summary>
        public int WaterLevel
        {
            get
            {
                return waterLevel;
            }
            set
            {
                if (waterLevel == value)
                {
                    return;
                }
                waterLevel = value;
                ValidateRivers();
                Refresh();
            }
        }
        /// <summary>
        /// <para/>��Ԫ�Ƿ�ˮ����û����
        /// <para/>�� : ����ˮ��߶��Ƿ���ڵ��θ߶�
        /// </summary>
        public bool IsUnderwater
        {
            get
            {
                return waterLevel > elevation;
            }
        }
        /// <summary>
        /// <para/>����û�ĵ�Ԫ��ˮ��Y������
        /// <para/>�� : ���ؼ�����Y������
        /// </summary>
        public float WaterSurfaceY
        {
            get
            {
                return
                    (waterLevel + HexMetrics.waterElevationOffset)
                    * HexMetrics.elevationStep;
            }
        }
        /// <summary>
        /// <para/>ϸ���ܶȼ�������
        /// <para/>�� : ��õ�ǰ��Ԫ���ܶȼ���
        /// <para/>д : �����ܶȼ���ˢ�µ�ǰ����
        /// </summary>
        public int UrbanLevel
        {
            get
            {
                return urbanLevel;
            }
            set
            {
                if (urbanLevel != value)
                {
                    urbanLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        /// <summary>
        /// ˢ�±�������ھ������Ĳ�ͬ����
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
        /// ���ö�Ӧ����ĵ�·
        /// </summary>
        /// <param name="index">��·�±�</param>
        /// <param name="state">��·�Ƿ����</param>
        private void SetRoad(int index, bool state)
        {
            //���ö�Ӧ����ĵ�·
            roads[index] = state;
            //�����ھӶ�Ӧ����ĵ�·
            neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
            //ˢ��
            neighbors[index].RefreshSelfOnly();
            RefreshSelfOnly();
        }

        /// <summary>
        /// ��ö�Ӧ������ھ�
        /// </summary>
        /// <param name="direction">ָ���ķ���</param>
        /// <returns>���ض�Ӧ������ھ�</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        /// <summary>
        /// ���ö�Ӧ������ھ�
        /// </summary>
        /// <param name="direction">ָ���ķ���</param>
        /// <param name="cell">Ҫ���óɵ��ھ�</param>
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
        /// <returns>���ر�Ե��������</returns>
        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }
        /// <summary>
        /// ���ĳ����ı�Ե�Ƿ��к�������
        /// </summary>
        /// <param name="direction">ָ������</param>
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
            if (hasOutgoingRiver && outgoingRiver == direction)
            {
                return;
            }
            //��ö�Ӧ������ھ�
            HexCell neighbor = GetNeighbor(direction);
            //����ھӲ����ڻ����ھӵĸ߶ȴ��ڱ���Ԫ�߶ȣ��������£��򷵻�

            if (!IsValidRiverDestination(neighbor))
            {
                return;
            }
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
            //�����ھӵĺ������
            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            //����˷���ĵ�·
            SetRoad((int)direction, false);
        }
        /// <summary>
        /// ָ�������Ƿ���ڵ�·
        /// </summary>
        public bool HasRoadThroughEdge(HexDirection direction)
        {
            return roads[(int)direction];
        }
        /// <summary>
        /// ��Ӷ�Ӧ����ĵ�·
        /// </summary>
        public void AddRoad(HexDirection direction)
        {
            //�ж��Ƿ�����ڴ˷������õ�·
            if (roads[(int)direction] == false                          //�˷��򲻴��ڵ�·
                && HasRiverThroughEdge(direction) == false     //�˷��򲻴��ں���
                && GetElevationDifference(direction) <= 1)     //�˷�������ھӵĸ߶Ȳ��С
            {
                SetRoad((int)direction, true);
            }
        }
        /// <summary>
        /// ������е�·
        /// </summary>
        public void RemoveRoads()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (true == roads[i])
                {
                    //�����·
                    SetRoad(i, false);
                }
            }
        }
        /// <summary>
        /// ����Լ��Ͷ�Ӧ�����ھӵĸ߶Ȳ�
        /// </summary>
        public int GetElevationDifference(HexDirection direction)
        {
            int difference = elevation - GetNeighbor(direction).elevation;
            return difference >= 0 ? difference : -difference;
        }
        /// <summary>
        /// ����ھ��Ƿ��Ǻ������ڵ���ЧĿ�ĵ�
        /// </summary>
        private bool IsValidRiverDestination(HexCell neighbor)
        {
            return
                neighbor
                && (
                        elevation >= neighbor.elevation
                        || waterLevel == neighbor.elevation
                    )
                ;
        }
        /// <summary>
        /// �Ƴ��Ƿ��ĺ��������
        /// </summary>
        private void ValidateRivers()
        {
            if (
                hasOutgoingRiver == true
                && IsValidRiverDestination(GetNeighbor(outgoingRiver)) == false
            )
            {
                RemoveOutgoingRiver();
            }
            if (
                hasIncomingRiver == true
                && GetNeighbor(incomingRiver).IsValidRiverDestination(this) == false
            )
            {
                RemoveIncomingRiver();
            }
        }

        /// <summary>
        /// ���ؽű�ʵ��ʱ���� Awake
        /// </summary>
        public void Awake()
        {
            neighbors = new HexCell[6];
        }
    }
}