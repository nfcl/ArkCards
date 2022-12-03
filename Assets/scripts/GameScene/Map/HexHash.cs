using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// <para/>哈希网格使用的哈希值
    /// <para/>保存了两个float作为采样结果
    /// </summary>
    public struct HexHash
    {

        public float a;

        public float b;

        public float c;
        /// <summary>
        /// 创建一个哈希值
        /// </summary>
        public static HexHash Create()
        {
            HexHash hash;
            hash.a = Random.value * 0.999f;
            hash.b = Random.value * 0.999f;
            hash.c = Random.value * 0.999f;
            return hash;
        }
    }
}