using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct HexFeatureCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public Transform[] prefabs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public Transform Pick(float choice)
        {
            return prefabs[(int)(choice * prefabs.Length)];
        }
    }
}
