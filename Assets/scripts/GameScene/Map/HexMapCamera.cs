using UnityEngine;

namespace GameScene.Map
{
    public class HexMapCamera : MonoBehaviour
    {
        private Transform swivel;       //旋转遥感
        private Transform stick;        //移动遥感

        public HexGrid grid;            //地图
        //相机缩放
        private float zoom = 1f;        //相机缩放速率
        public float stickMinZoom;      //最小相机缩放
        public float stickMaxZoom;      //最大相机缩放
        public float swivelMinZoom;     //最小相机上下旋转角度
        public float swivelMaxZoom;     //最大相机上下旋转角度
        //相机移动
        public float moveSpeedMinZoom;  //最小相机移动速度
        public float moveSpeedMaxZoom;  //最大相机移动速度
        //相机旋转
        private float zoomDelta;        //滚轮值
        private float xDelta;           //水平按键值
        private float zDelta;           //垂直按键值
        private float rotationAngle;    //相机旋转角度
        public float rotationSpeed;     //相机左右旋转速度

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            //监听鼠标滚轮
            zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                AdjustZoom(zoomDelta);
            }
            //监听旋转
            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                AdjustRotation(rotationDelta);
            }
            //监听方向键
            xDelta = Input.GetAxis("Horizontal");
            zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                AdjustPosition(xDelta, zDelta);
            }
        }
        /// <summary>
        /// 相机缩放
        /// </summary>
        /// <param name="delta">缩放更改值</param>
        private void AdjustZoom(float delta)
        {
            //更新缩放速率(同时限制缩放速度)
            zoom = Mathf.Clamp01(zoom + delta);
            //计算缩放后的相机位置
            float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);
            //计算缩放后的相机旋转
            float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
        /// <summary>
        /// 相机移动
        /// </summary>
        /// <param name="xDelta">水平移动增加值</param>
        /// <param name="zDelta">垂直移动增加值</param>
        private void AdjustPosition(float xDelta, float zDelta)
        {
            //标准化移动方向
            Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
            //移动速度计算
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            //计算移动距离
            float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;
            //当前位置
            Vector3 position = transform.localPosition;
            //移动
            position += direction * distance;
            //更新位置
            transform.localPosition = ClampPosition(position);
        }
        /// <summary>
        /// 限制移动位置不会移动到地图外
        /// </summary>
        /// <param name="position">新的移动位置</param>
        /// <returns>限制后的移动位置</returns>
        private Vector3 ClampPosition(Vector3 position)
        {
            //计算x轴最大移动距离
            float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
            position.x = Mathf.Clamp(position.x, 0f, xMax);
            //计算z轴最大移动距离
            float zMax = (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.outerRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);
            //返回正确的位置
            return position;
        }
        /// <summary>
        /// 相机左右旋转
        /// </summary>
        /// <param name="delta">旋转增量</param>
        void AdjustRotation(float delta)
        {
            //增加旋转角度
            rotationAngle += delta * rotationSpeed * Time.deltaTime;
            //角度小于0°时变为360°
            if (rotationAngle < 0f)
            {
                rotationAngle += 360f;
            }
            //角度大于360°时变为0°
            else if (rotationAngle >= 360f)
            {
                rotationAngle -= 360f;
            }
            //更新旋转角度
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            swivel = transform.GetChild(0);
            stick = swivel.GetChild(0);
        }
    }
}