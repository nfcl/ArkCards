using UnityEngine;

namespace GameScene.Map
{
    public class HexMapCamera : MonoBehaviour
    {
        static HexMapCamera instance;

        /// <summary>
        /// ��ͼ����
        /// </summary>
        public HexGrid grid;

        /// <summary>
        /// �����������
        /// д : ���������active����Ϊ�෴��״̬
        /// </summary>
        public static bool Locked
        {
            set
            {
                instance.enabled = !value;
            }
        }

        private Transform swivel;       //��תң��
        private Transform stick;        //�ƶ�ң��
        //�������
        private float zoom = 1f;        //�����������
        public float stickMinZoom;      //��С�������
        public float stickMaxZoom;      //����������
        public float swivelMinZoom;     //��С���������ת�Ƕ�
        public float swivelMaxZoom;     //������������ת�Ƕ�
        //����ƶ�
        public float moveSpeedMinZoom;  //��С����ƶ��ٶ�
        public float moveSpeedMaxZoom;  //�������ƶ��ٶ�
        //�����ת
        private float zoomDelta;        //����ֵ
        private float xDelta;           //ˮƽ����ֵ
        private float zDelta;           //��ֱ����ֵ
        private float rotationAngle;    //�����ת�Ƕ�
        public float rotationSpeed;     //���������ת�ٶ�

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="delta">���Ÿ���ֵ</param>
        private void AdjustZoom(float delta)
        {
            //������������(ͬʱ���������ٶ�)
            zoom = Mathf.Clamp01(zoom + delta);
            //�������ź�����λ��
            float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);
            //�������ź�������ת
            float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
        /// <summary>
        /// ����ƶ�
        /// </summary>
        /// <param name="xDelta">ˮƽ�ƶ�����ֵ</param>
        /// <param name="zDelta">��ֱ�ƶ�����ֵ</param>
        private void AdjustPosition(float xDelta, float zDelta)
        {
            //��׼���ƶ�����
            Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
            //�ƶ��ٶȼ���
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            //�����ƶ�����
            float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;
            //��ǰλ��
            Vector3 position = transform.localPosition;
            //�ƶ�
            position += direction * distance;
            //����λ��
            transform.localPosition = ClampPosition(position);
        }
        /// <summary>
        /// �����ƶ�λ�ò����ƶ�����ͼ��
        /// </summary>
        /// <param name="position">�µ��ƶ�λ��</param>
        /// <returns>���ƺ���ƶ�λ��</returns>
        private Vector3 ClampPosition(Vector3 position)
        {
            //����x������ƶ�����
            float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
            position.x = Mathf.Clamp(position.x, 0f, xMax);
            //����z������ƶ�����
            float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);
            //������ȷ��λ��
            return position;
        }
        /// <summary>
        /// ���������ת
        /// </summary>
        /// <param name="delta">��ת����</param>
        private void AdjustRotation(float delta)
        {
            //������ת�Ƕ�
            rotationAngle += delta * rotationSpeed * Time.deltaTime;
            //�Ƕ�С��0��ʱ��Ϊ360��
            if (rotationAngle < 0f)
            {
                rotationAngle += 360f;
            }
            //�Ƕȴ���360��ʱ��Ϊ0��
            else if (rotationAngle >= 360f)
            {
                rotationAngle -= 360f;
            }
            //������ת�Ƕ�
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        /// <summary>
        /// ������λ�úϷ���
        /// </summary>
        public static void ValidatePosition()
        {
            instance.AdjustPosition(0f, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            //����������
            zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                AdjustZoom(zoomDelta);
            }
            //������ת
            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                AdjustRotation(rotationDelta);
            }
            //���������
            xDelta = Input.GetAxis("Horizontal");
            zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                AdjustPosition(xDelta, zDelta);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            if (instance is null)
                instance = this;

            swivel = transform.GetChild(0);
            stick = swivel.GetChild(0);
        }
    }
}