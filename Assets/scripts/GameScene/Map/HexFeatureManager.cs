using UnityEngine;

namespace GameScene.Map
{
    public class HexFeatureManager : MonoBehaviour
    {
        /// <summary>
        /// 地形要素预设体
        /// </summary>
        public HexFeatureCollection[] featureCollections;
        /// <summary>
        /// 
        /// </summary>
        private Transform container;
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (container)
            {
                Destroy(container.gameObject);
            }
            container = new GameObject("Features Container").transform;
            container.SetParent(transform, false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Apply() { }
        /// <summary>
        /// 在指定位置添加地形细节
        /// </summary>
        public void AddFeature(HexCell cell, Vector3 position)
        {
            HexHash hash = HexMetrics.SampleHashGrid(position);
            //选取要生成的预制件
            Transform prefab = PickPrefab(cell.UrbanLevel, hash.a, hash.b);
            //如果没有则不生成
            if (prefab is null)
            {
                return;
            }
            Transform instance = Instantiate(prefab);
            position.y += instance.localScale.y * 0.5f;
            instance.localPosition = HexMetrics.Perturb(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.c, 0f);
            instance.SetParent(container, false);
        }

        /// <summary>
        /// 根据级别和哈希值选取要生成的细节
        /// </summary>
        private Transform PickPrefab(int level, float hash, float choice)
        {
            if (level > 0)
            {
                float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (hash < thresholds[i])
                    {
                        return featureCollections[i].Pick(choice);
                    }
                }
            }
            return null;
        }
    }
}