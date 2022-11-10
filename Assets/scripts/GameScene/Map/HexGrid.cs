using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// ��ͼ��Ľڵ���(x��)
        /// </summary>
        public int width = 10;
        /// <summary>
        /// ��ͼ�ߵĽڵ���(z��)
        /// </summary>
        public int height = 6;
        /// <summary>
        /// �ڵ�Ԥ����
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// �ڵ㴢��
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// �ڵ��ǩԤ����
        /// </summary>
        public Text cellLabelPrefab;
        /// <summary>
        /// �������ڴ�Žڵ�UI
        /// </summary>
        private Canvas gridCanvas;
        /// <summary>
        /// �ڵ�����
        /// </summary>
        private HexMesh hexMesh;

        /// <summary>
        /// �����µĽڵ�
        /// </summary>
        /// <param name="x">�ڵ�x��</param>
        /// <param name="z">�ڵ�z��</param>
        /// <param name="i">�ڵ����</param>
        void CreateCell(int x, int z, int i)
        {
            //����ڵ�����λ��
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            //�½��ڵ�
            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = Color.white;
            //�������ڹ�ϵ
            if (x > 0)
            {
                //�����Լ���ࣨW�����ھ�
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {//�ǵ�һ��
                if ((z & 1) == 0)
                {//ż����
                    //�����Լ������²ࣨSE�����ھ�
                    cell.SetNeighbor(HexDirection.SE, cells[i - width]); 
                    if (x > 0)
                    {//�ǵ�һ��
                        //�����Լ������²ࣨSW�����ھ�
                        cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                    }
                }
                else
                {//������
                    //�����Լ������²ࣨSW�����ھ�
                    cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                    if (x < width - 1)
                    {//�����һ��
                        //�����Լ������²ࣨSE�����ھ�
                        cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                    }
                }
            }
            //�½��ڵ��ǩ
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            cell.uiRect = label.rectTransform;
        }

        public void Refresh()
        {
            hexMesh.Triangulate(cells);
        }

        void Start()
        {
            //�ڵ��������»���
            Refresh();
        }

        void Awake()
        {
            //��û���
            gridCanvas = GetComponentInChildren<Canvas>();
            //������������
            hexMesh = GetComponentInChildren<HexMesh>();
            //���ýڵ������С
            cells = new HexCell[height * width];
            //�����ڵ�
            for (int z = 0, i = 0; z < height; ++z)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    CreateCell(x, z, i);
                }
            }
        }

        /// <summary>
        /// �����ͼ�ڵ�
        /// </summary>
        /// <param name="position"></param>
        public HexCell GetCell(Vector3 position)
        {
            //��������ϵ����������ά����
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            return cells[index];
        }
    }
}
