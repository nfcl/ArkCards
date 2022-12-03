using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Map
{
    /// <summary>
    /// <para/>地图细节控制器
    /// <para/>用于管理地图细节的生成和清除
    /// </summary>
    public class HexFeatureManager : MonoBehaviour
    {
        /// <summary>
        /// 地形要素预设体
        /// </summary>
        public static Dictionary<string, HexFeatureCollection> featureCollections;
        /// <summary>
        /// 细节的容器
        /// </summary>
        private Transform container;

        /// <summary>
        /// 清除地图细节
        /// </summary>
        public void Clear()
        {
            //如果存在细节容器则清除旧的细节容器
            if ((container is null) == false)
            {
                Destroy(container.gameObject);
            }
            //生成新的细节容器
            container = new GameObject("Features Container").transform;
            container.SetParent(transform, false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Apply() 
        {

        }
        /// <summary>
        /// 初始化细节集合
        /// </summary>
        /// <param name="features"></param>
        public static void InitfeatureCollection(HexFeatureCollection[] features)
        {
            if (featureCollections is null)
            {
                featureCollections = new Dictionary<string, HexFeatureCollection>();
            }
            foreach (HexFeatureCollection item in features)
            {
                featureCollections[item.name] = item;
            }
        }
        /// <summary>
        /// 在指定位置添加地形细节
        /// </summary>
        public void AddFeature(HexCell cell, Vector3 position)
        {
            HexHash hash = HexMetrics.SampleHashGrid(position);
            //选取要生成的预制件
            Transform prefab = PickPrefab(cell, hash);
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
        /// 选取要生成的细节预制件
        /// </summary>
        private Transform PickPrefab(HexCell cell,HexHash hash)
        {
            int terrain = cell.TerrainType.type;
            bool hasRiver = cell.HasRiver;

            if(terrain == 0)
            {//Grass
                if (hash.a > 0.6)
                {
                    return featureCollections["橡树"].Pick(hash.c);
                }
            }
            else if(terrain == 1)
            {

            }
            else if(terrain == 2)
            {//Sand
                if (hasRiver == true)
                {//如果沿河则可生成椰子树
                    if (hash.a > 0.7)
                    {
                        return featureCollections["椰子树"].Pick(hash.c);
                    }
                }
                if (hash.a > 0.7)
                {
                    return featureCollections["仙人掌"].Pick(hash.c);
                }
            }
            else if(terrain == 3)
            {//Snow
                if (hash.a > 0.7)
                {
                    return featureCollections["带雪松树"].Pick(hash.c);
                }
            }
            else if (terrain == 4)
            {

            }
            return null;
        }
    }
}