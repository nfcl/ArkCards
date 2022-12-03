using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// <para/>地图细节素材集合
    /// </summary>
    [System.Serializable]
    public struct HexFeatureCollection
    {
        /// <summary>
        /// 不同级别的细节
        /// </summary>
        public Transform[] prefabs;
        /// <summary>
        /// 细节类型名称
        /// </summary>
        public string name;

        /// <summary>
        /// 根据一个[0,1)的浮点值确定细节的级别并返回响应的预制件
        /// </summary>
        public Transform Pick(float choice)
        {
            return prefabs[(int)(choice * prefabs.Length)];
        }
    }
}
