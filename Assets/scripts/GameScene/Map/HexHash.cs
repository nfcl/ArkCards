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
        /// <summary>
        /// 创建一个哈希值
        /// </summary>
        public static HexHash Create()
        {
            HexHash hash;
            hash.a = Random.value;
            hash.b = Random.value;
            return hash;
        }
    }
}