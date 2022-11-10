namespace GameScene.Map
{
    /// <summary>
    /// 节点边缘连接类型
    /// </summary>
    public enum HexEdgeType
    {
        Flat,       //平坦 高度相同
        Slope,      //斜坡 两者之间
        Cliff       //悬崖 高度差距过大
    }
}