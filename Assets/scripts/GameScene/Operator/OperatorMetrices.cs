using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Operator
{
    using Json.OperatorSource;

    public static class OperatorMetrices
    {
        public static Dictionary<string, OperatorSourceItem> OperatorInfo;

        public static void InitData()
        {
            Root jsonData_OperatorSource =
                Tool.DataRead.JsonReader<Root>(
                    $"{Application.streamingAssetsPath}/OperatorSource.json"
                );
            OperatorInfo = new Dictionary<string, OperatorSourceItem>();

            foreach (OperatorSourceItem item in jsonData_OperatorSource.OperatorSource)
            {
                OperatorInfo[item.Name] = item;
            }
        }
    }
}