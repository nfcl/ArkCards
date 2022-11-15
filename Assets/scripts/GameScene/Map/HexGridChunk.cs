using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Map
{
    public class HexGridChunk : MonoBehaviour
    {
        /// <summary>
        /// 区块内的单元单元集
        /// </summary>
        private HexCell[] cells;
        /// <summary>
        /// UI画布
        /// </summary>
        private Canvas gridCanvas;
        /// <summary>
        /// 地形网格
        /// </summary>
        public HexMesh terrain;
        /// <summary>
        /// 河流网格
        /// </summary>
        public HexMesh rivers;
        /// <summary>
        /// 道路网格
        /// </summary>
        public HexMesh roads;

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            //在LateUpdate进行刷新防止冲突
            Triangulate();
            //刷新后再次关闭脚本
            enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();

            cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

            ShowUI(false);

            Refresh();
        }

        /// <summary>
        /// 根据地图单元集合绘制网格
        /// </summary>
        /// <param name="cells">单元集合</param>
        private void Triangulate()
        {
            //清除就的网格数据
            terrain.Clear();
            rivers.Clear();
            roads.Clear();
            //创建网格
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }
            //使用新的网格数据
            terrain.Apply();
            rivers.Apply();
            roads.Apply();
        }
        /// <summary>
        /// 根据中心点添加六边形的六个三角面
        /// </summary>
        /// <param name="cell">中心单元</param>
        private void Triangulate(HexCell cell)
        {
            //遍历六个方向添加三角面
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Triangulate(d, cell);
            }
        }
        /// <summary>
        /// 根据中心点和方向添加三角面
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="cell">中心单元</param>
        private void Triangulate(HexDirection direction, HexCell cell)
        {
            //计算中心点位置
            Vector3 center = cell.Position;
            //构造该方向的纯色区边缘
            EdgeVertices e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );
            //根据是否有河流来分类纯色区三角化方式
            if (cell.HasRiver)
            {//有河流
             //判断河流是否流经这个方向
                if (cell.HasRiverThroughEdge(direction))
                {
                    //更新河床底的Y轴坐标
                    e.v3.y = cell.StreamBedY;
                    //分类河流端点和河流流经两种情况
                    if (cell.HasRiverBeginOrEnd)
                    {
                        TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
                    }
                    else
                    {
                        TriangulateWithRiver(direction, cell, center, e);
                    }
                }
                else
                {
                    TriangulateAdjacentToRiver(direction, cell, center, e);
                }
            }
            else
            {//无河流
                TriangulateWithoutRiver(direction, cell, center, e);
            }
            //添加该方向的混色区矩形网格
            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, e);
            }
        }
        /// <summary>
        /// 无河流纯色区扇形三角化
        /// </summary>
        private void TriangulateWithoutRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            TriangulateEdgeFan(center, e, cell.Color);

            if (cell.HasRoads)
            {
                Vector2 interpolators = GetRoadInterpolators(direction, cell);
                TriangulateRoad(
                    center,
                    Vector3.Lerp(center, e.v1, interpolators.x),
                    Vector3.Lerp(center, e.v5, interpolators.y),
                    e,
                    cell.HasRoadThroughEdge(direction)
                );
            }
        }
        /// <summary>
        /// 三角化中心点到边缘组成的单一颜色扇形
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="edge">边缘</param>
        /// <param name="color">颜色</param>
        private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
        {
            terrain.AddTrianglePerturbed(center, edge.v1, edge.v2);
            terrain.AddTriangleColor(color);
            terrain.AddTrianglePerturbed(center, edge.v2, edge.v3);
            terrain.AddTriangleColor(color);
            terrain.AddTrianglePerturbed(center, edge.v3, edge.v4);
            terrain.AddTriangleColor(color);
            terrain.AddTrianglePerturbed(center, edge.v4, edge.v5);
            terrain.AddTriangleColor(color);
        }
        /// <summary>
        /// 有河流的纯色区三角化
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="cell">单元</param>
        /// <param name="center">中心坐标</param>
        /// <param name="e">纯色区底边</param>
        private void TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            Vector3 centerL, centerR;
            if (cell.HasRiverThroughEdge(direction.Opposite()))
            {//河流直流
                centerL = center +
                    HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
                centerR = center +
                    HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
            }
            else if (cell.HasRiverThroughEdge(direction.Next()))
            {//60°夹角且是顺时针方向的下一个方向
                centerL = center;
                centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
            }
            else if (cell.HasRiverThroughEdge(direction.Previous()))
            {//60°夹角且是顺时针方向的上一个方向
                centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
                centerR = center;
            }
            else if (cell.HasRiverThroughEdge(direction.Next2()))
            {//120°夹角且是顺时针方向的下下个方向
                centerL = center;
                centerR = center + HexMetrics.GetSolidEdgeMiddle(direction.Next()) * (0.5f * HexMetrics.innerToOuter);
            }
            else
            {//120°夹角且是顺时针方向的上上个方向
                centerL = center + HexMetrics.GetSolidEdgeMiddle(direction.Previous()) * (0.5f * HexMetrics.innerToOuter);
                centerR = center;
            }
            //重新定位中心点位置位于转折点的河床中心
            //否则中心点位置会偏向于单元的中心点
            center = Vector3.Lerp(centerL, centerR, 0.5f);
            //计算梯形底边到顶边的中间分割线
            EdgeVertices m = new EdgeVertices(
                Vector3.Lerp(centerL, e.v1, 0.5f),
                Vector3.Lerp(centerR, e.v5, 0.5f),
                1f / 6f
            );
            //设置河床底深度
            m.v3.y = center.y = e.v3.y;
            //中间线到底边的梯形做三角化
            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
            //顶边到中间线的梯形做三角化
            terrain.AddTrianglePerturbed(centerL, m.v1, m.v2);
            terrain.AddTriangleColor(cell.Color);
            terrain.AddQuad(centerL, center, m.v2, m.v3);
            terrain.AddQuadColor(cell.Color);
            terrain.AddQuad(center, centerR, m.v3, m.v4);
            terrain.AddQuadColor(cell.Color);
            terrain.AddTrianglePerturbed(centerR, m.v4, m.v5);
            terrain.AddTriangleColor(cell.Color);
            //根据出入河口判断是否需要倒转UV方向使水流同向
            bool reversed = cell.IncomingRiver == direction;
            TriangulateRiverQuad(centerL, centerR, m.v2, m.v4, cell.RiverSurfaceY, 0.4f, reversed);
            TriangulateRiverQuad(m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, 0.6f, reversed);
        }
        /// <summary>
        /// 有河流起点或终点的纯色区三角化
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="cell">单元</param>
        /// <param name="center">单元中心位置</param>
        /// <param name="e">纯色区底边</param>
        private void TriangulateWithRiverBeginOrEnd(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            //计算纯色区梯形的中间线
            EdgeVertices m = new EdgeVertices(
                Vector3.Lerp(center, e.v1, 0.5f),
                Vector3.Lerp(center, e.v5, 0.5f)
            );
            //设置河床底部Y坐标
            m.v3.y = e.v3.y;
            //三角化中间线到纯色区底边的梯形
            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
            //三角化中心点到中间线的扇形
            TriangulateEdgeFan(center, m, cell.Color);
            //三角化河水
            bool reversed = cell.HasIncomingRiver;
            //重定向中心和底边的Y轴为河流表面
            center.y = m.v2.y = m.v4.y = cell.RiverSurfaceY;
            //添加中点至底边的四边形
            TriangulateRiverQuad(m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, 0.6f, reversed);
            //添加中心至终点的三角形
            rivers.AddTrianglePerturbed(center, m.v2, m.v4);
            //添加UV坐标
            if (reversed)
            {
                rivers.AddTriangleUV(
                    new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
                );
            }
            else
            {
                rivers.AddTriangleUV(
                    new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
                );
            }
        }
        /// <summary>
        /// 存在河流的单元的剩余扇形的三角化
        /// </summary>
        private void TriangulateAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e) 
        {
            if (cell.HasRoads)
            {
                TriangulateRoadAdjacentToRiver(direction, cell, center, e);
            }
            //判断河流流经方向
            if (cell.HasRiverThroughEdge(direction.Next()))
            {
                //检查是不是在出入口之间
                if (cell.HasRiverThroughEdge(direction.Previous()))
                {
                    //如果临近河流
                    center += HexMetrics.GetSolidEdgeMiddle(direction) *
                        (HexMetrics.innerToOuter * 0.5f);
                }
                //检查是不是直流
                else if (cell.HasRiverThroughEdge(direction.Previous2()))
                {
                    center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
                }
            }
            //判断直流的另一种情况
            else if (
                cell.HasRiverThroughEdge(direction.Previous()) &&
                cell.HasRiverThroughEdge(direction.Next2())
            )
            {
                center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
            }

            EdgeVertices m = new EdgeVertices(
                Vector3.Lerp(center, e.v1, 0.5f),
                Vector3.Lerp(center, e.v5, 0.5f)
            );

            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
            TriangulateEdgeFan(center, m, cell.Color);
        }
        /// <summary>
        /// 路和河处于同一单元时的纯色区处理
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        /// <param name="center"></param>
        /// <param name="e"></param>
        private void TriangulateRoadAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            //有河流流经这个方向
            bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
            //有河流流经上一个方向
            bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
            //有河流流经下一个方向
            bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
            //根据道路分布情况选用插值
            Vector2 interpolators = GetRoadInterpolators(direction, cell);
            //道路中心
            Vector3 roadCenter = center;
            //如果只存在河流出入口则对道路中心进行偏移
            if (cell.HasRiverBeginOrEnd)
            {
                roadCenter +=
                    HexMetrics.GetSolidEdgeMiddle(
                        cell.RiverBeginOrEndDirection.Opposite())
                    * (1f / 3f);
            }
            //如果河流出口方向在河流入口的对面，即直流，需要断开道路向外偏移
            else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite())
            {
                Vector3 corner;
                if (previousHasRiver)
                {
                    corner = HexMetrics.GetSecondSolidCorner(direction);
                }
                else
                {
                    corner = HexMetrics.GetFirstSolidCorner(direction);
                }
                //对道路中心和单元中心进行偏移
                roadCenter += corner * 0.5f;
                center += corner * 0.25f;
            }
            //入口在出口的上一个方向
            else if (cell.IncomingRiver == cell.OutgoingRiver.Previous())
            {
                //道路中心向外偏移
                roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingRiver) * 0.2f;
            }
            //出口在入口的上一个方向
            else if (cell.IncomingRiver == cell.OutgoingRiver.Next())
            {
                //道路中心向外偏移
                roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingRiver) * 0.2f;
            }
            //出口与入口相隔一个方向
            else if (previousHasRiver && nextHasRiver)
            {
                //如果并没有道路经过这个方向则不需要
                if (false == hasRoadThroughEdge)
                {
                    return;
                }
                //使道路中心偏移至河流边缘
                Vector3 offset =
                    HexMetrics.GetSolidEdgeMiddle(direction)
                    * HexMetrics.innerToOuter;
                roadCenter += offset * 0.7f;
                center += offset * 0.5f;
            }
            //对道路中心进行偏移使道路连接平滑
            else
            {
                HexDirection middle;
                if (previousHasRiver)
                {
                    middle = direction.Next();
                }
                else if (nextHasRiver)
                {
                    middle = direction.Previous();
                }
                else
                {
                    middle = direction;
                }
                if (
                    false == cell.HasRoadThroughEdge(middle)
                    && false == cell.HasRoadThroughEdge(middle.Previous())
                    && false == cell.HasRoadThroughEdge(middle.Next())
                )
                {
                    return;
                }
                roadCenter += HexMetrics.GetSolidEdgeMiddle(middle) * 0.25f;
            }
            //计算中点，中心偏移的同时中点也会向外偏移
            Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
            Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
            TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
            //如果此方向存在河流则需要补全中心（对面方向的道路中心发生了偏移）
            if (previousHasRiver)
            {
                TriangulateRoadEdge(roadCenter, center, mL);
            }
            if (nextHasRiver)
            {
                TriangulateRoadEdge(roadCenter, mR, center);
            }
        }
        /// <summary>
        /// 添加单元对应方向的连接矩形
        /// </summary>
        /// <param name="direction">对应方向</param>
        /// <param name="cell">单元</param>
        /// <param name="v1">纯色区域底边顶点1</param>
        /// <param name="e1">1/3等分点</param>
        /// <param name="e2">2/3等分点</param>
        /// <param name="v2">纯色区域底边顶点2</param>
        private void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
        {
            //获得对应方向的邻居
            HexCell neighbor = cell.GetNeighbor(direction);
            //如果没有邻居返回
            if (neighbor == null) return;
            //计算纯色区域底边到混合区域底边垂线方向向量
            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            //构造邻居反方向的纯色区和混色区边缘
            EdgeVertices e2 = new EdgeVertices(
                e1.v1 + bridge,
                e1.v5 + bridge
            );
            //判断此方向是否有河流经过
            if (cell.HasRiverThroughEdge(direction))
            {//如果此方向有河流经过
                //中间点的Y轴坐标应为河床底坐标
                e2.v3.y = neighbor.StreamBedY;
                //添加单元间河流的连接四边形
                TriangulateRiverQuad(
                    e1.v2, e1.v4, e2.v2, e2.v4,
                    cell.RiverSurfaceY, neighbor.RiverSurfaceY,0.8f,
                    cell.HasIncomingRiver && cell.IncomingRiver == direction
                );
            }
            //根据边缘连接类型添加连接面
            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            {//斜坡才需要处理
                TriangulateEdgeTerraces(e1, cell, e2, neighbor,cell.HasRoadThroughEdge(direction));
            }
            else
            {//平坦和悬崖正常连接即可
                TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color, cell.HasRoadThroughEdge(direction));
            }
            //获得顺时针方向的下一个邻居
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            //如果存在下一个邻居则添加三个单元相交中心混合区域到网格
            //混合区域三个顶点颜色实为三个单元的颜色
            //限制方向防止产生重复的混合区域三角形
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                //计算下一个混合矩形的同侧底边顶点
                Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
                //设置高度为下一个邻居的高度
                v5.y = nextNeighbor.Position.y;
                //计算最低的顶点并进行三角形面的添加
                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                    {
                        TriangulateCorner(e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor);
                    }
                    else
                    {
                        TriangulateCorner(v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor);
                    }
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor);
                }
            }
        }
        /// <summary>
        /// 根据两个细分点组进行矩形网格的添加
        /// </summary>
        private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2, bool hasRoad = false)
        {
            terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
            terrain.AddQuadColor(c1, c2);
            //如果这条边存在道路则需要使用道路三角化
            if (hasRoad)
            {
                TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);
            }
        }
        /// <summary>
        /// 混合矩形边缘处理
        /// </summary>
        /// <param name="begin">起始边缘</param>
        /// <param name="beginCell">起始单元</param>
        /// <param name="end">结束边缘</param>
        /// <param name="endCell">结束单元</param>
        /// <param name="hasRoad">是否存在道路</param>
        private void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell, EdgeVertices end, HexCell endCell, bool hasRoad = false)
        {
            EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

            TriangulateEdgeStrip(begin, beginCell.Color, e2, c2, hasRoad);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                EdgeVertices e1 = e2;
                Color c1 = c2;
                e2 = EdgeVertices.TerraceLerp(begin, end, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
                TriangulateEdgeStrip(e1, c1, e2, c2, hasRoad);
            }

            TriangulateEdgeStrip(e2, c2, end, endCell.Color, hasRoad);
        }
        /// <summary>
        /// 三个单元间的中心三角形边缘处理
        /// </summary>
        /// <param name="bottom">高度最低的顶点位置</param>
        /// <param name="bottomCell">高度最低的单元</param>
        /// <param name="left">顺时针的下一个顶点位置</param>
        /// <param name="leftCell">顺时针的下一个单元</param>
        /// <param name="right">逆时针的下一个顶点位置</param>
        /// <param name="rightCell">逆时针的下一个单元</param>
        private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            //计算bottom顶点的单元和另外两个单元的边缘连接类型
            HexEdgeType leftEdgeType = HexMetrics.GetEdgeType(bottomCell.Elevation, leftCell.Elevation);
            HexEdgeType rightEdgeType = HexMetrics.GetEdgeType(bottomCell.Elevation, rightCell.Elevation);
            //根据边缘连接类型添加中心三角形区域
            if (leftEdgeType == HexEdgeType.Slope)
            {//左侧斜面
                if (rightEdgeType == HexEdgeType.Slope)
                {//右侧斜面
                    TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                }
                else if (rightEdgeType == HexEdgeType.Flat)
                {//右侧平坦
                    TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                }
                else
                {//右侧悬崖
                    TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                }
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {//右侧斜面
                if (leftEdgeType == HexEdgeType.Flat)
                {//左侧平坦
                    TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {//左侧悬崖
                    TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                }
            }
            //左右侧边缘都为悬崖
            else if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
            {//左右侧单元边缘连接为斜面
                if (leftCell.Elevation < rightCell.Elevation)
                {
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
                }
            }
            else
            {
                terrain.AddTrianglePerturbed(bottom, left, right);
                terrain.AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
            }
        }
        /// <summary>
        /// 三个单元中心混合三角形区域的斜面加平面类型处理
        /// </summary>
        /// <param name="begin">高度最低的顶点位置</param>
        /// <param name="beginCell">高度最低的单元</param>
        /// <param name="left">顺时针的下一个顶点位置</param>
        /// <param name="leftCell">顺时针的下一个单元</param>
        /// <param name="right">逆时针的下一个顶点位置</param>
        /// <param name="rightCell">逆时针的下一个单元</param>
        private void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            //计算第一个位置的插值点
            Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
            Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
            Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);
            //加入起始点和第一对插值点的三角形
            terrain.AddTrianglePerturbed(begin, v3, v4);
            terrain.AddTriangleColor(beginCell.Color, c3, c4);
            //遍历进行循环插值计算
            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color c1 = c3;
                Color c2 = c4;
                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
                terrain.AddQuad(v1, v2, v3, v4);
                terrain.AddQuadColor(c1, c2, c3, c4);
            }
            //加入终止点和最后一对插值点的矩形
            terrain.AddQuad(v3, v4, left, right);
            terrain.AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
        }
        /// <summary>
        /// 三个单元中心混合三角形区域的斜面加悬崖处理
        /// </summary>
        /// <param name="begin">高度最低的顶点位置</param>
        /// <param name="beginCell">高度最低的单元</param>
        /// <param name="left">顺时针的下一个顶点位置</param>
        /// <param name="leftCell">顺时针的下一个单元</param>
        /// <param name="right">逆时针的下一个顶点位置</param>
        /// <param name="rightCell">逆时针的下一个单元</param>
        private void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            float b = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0)
            {
                b = -b;
            }
            Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);
            //进行斜坡和悬崖底部连接
            TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);
            //判断左右单元间的边缘判断
            if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
            {//斜坡
             //进行左右单元的斜坡阶梯边缘连接
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {//悬崖
             //直接用三角面连接
                terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
                terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }
        /// <summary>
        /// TriangulateCornerTerracesCliff的左右镜面反转
        /// </summary>
        /// <param name="begin">高度最低的顶点位置</param>
        /// <param name="beginCell">高度最低的单元</param>
        /// <param name="left">顺时针的下一个顶点位置</param>
        /// <param name="leftCell">顺时针的下一个单元</param>
        /// <param name="right">逆时针的下一个顶点位置</param>
        /// <param name="rightCell">逆时针的下一个单元</param>
        private void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
        {
            float b = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0)
            {
                b = -b;
            }
            Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

            TriangulateBoundaryTriangle(
                right, rightCell, begin, beginCell, boundary, boundaryColor
            );

            if (HexMetrics.GetEdgeType(leftCell.Elevation, rightCell.Elevation) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, leftCell, right, rightCell, boundary, boundaryColor
                );
            }
            else
            {
                terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
                terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }
        /// <summary>
        /// 悬崖和斜坡间的中心三角插值补面
        /// </summary>
        private void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor)
        {
            Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
            terrain.AddTriangleColor(beginCell.Color, c2, boundaryColor);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v2;
                Color c1 = c2;
                v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
                c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                terrain.AddTriangleUnperturbed(v1, v2, boundary);
                terrain.AddTriangleColor(c1, c2, boundaryColor);
            }

            terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
            terrain.AddTriangleColor(c2, leftCell.Color, boundaryColor);
        }
        /// <summary>
        /// 同高度的河流表面四边形
        /// </summary>
        private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y, float v, bool reversed)
        {
            TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed);
        }
        /// <summary>
        /// 前后两端高度不同的河流四边形三角化
        /// </summary>
        private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float v, bool reversed)
        {
            v1.y = v2.y = y1;
            v3.y = v4.y = y2;
            rivers.AddQuad(v1, v2, v3, v4);
            if (reversed)
            {
                rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
            }
            else
            {
                rivers.AddQuadUV(0f, 1f, v, v+0.2f);
            }
        }
        /// <summary>
        /// 三角化路面(纯色区中心扇形)
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="mL">中心点到边缘左端点的中点</param>
        /// <param name="mR">中心点到边缘右端点的中点</param>
        /// <param name="e">边缘</param>
        private void TriangulateRoad(Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge)
        {
            //
            if (hasRoadThroughCellEdge)
            {
                //计算两个左右中点的中点
                Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
                //三角化左右中点和边缘左右两个分割点组成的四边形
                TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
                //三角化中点和左右两个中点组成的三角形
                roads.AddTrianglePerturbed(center, mL, mC);
                roads.AddTrianglePerturbed(center, mC, mR);
                roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f));
                roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f));
            }
            else
            {
                TriangulateRoadEdge(center, mL, mR);
            }
        }
        /// <summary>
        /// 三角化路面(边缘)
        /// </summary>
        private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6)
        {
            roads.AddQuad(v1, v2, v4, v5);
            roads.AddQuad(v2, v3, v5, v6);
            roads.AddQuadUV(0f, 1f, 0f, 0f);
            roads.AddQuadUV(1f, 0f, 0f, 0f);
        }
        /// <summary>
        /// 三角化道路连接点的两侧缺失三角形
        /// </summary>
        /// <param name="center">中心点位置</param>
        /// <param name="mL">中心点至纯色区边缘的左中点</param>
        /// <param name="mR">中心点至纯色区边缘的右中点</param>
        private void TriangulateRoadEdge(Vector3 center, Vector3 mL, Vector3 mR)
        {
            roads.AddTrianglePerturbed(center, mL, mR);
            roads.AddTriangleUV(new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));

        }
        /// <summary>
        /// 区分纯色区道路连接中枢的每个扇区的两个顶点的插值
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        /// <returns>返回一个vector2,Vector2.x代表左侧点的插值,Vector2.y代表右侧点的插值</returns>
        private Vector2 GetRoadInterpolators(HexDirection direction, HexCell cell)
        {
            Vector2 interpolators;
            if (cell.HasRoadThroughEdge(direction))
            {
                interpolators.x = interpolators.y = 0.5f;
            }
            else
            {
                interpolators.x = cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
                interpolators.y = cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
            }
            return interpolators;
        }

        /// <summary>
        /// 区块刷新
        /// </summary>
        public void Refresh()
        {
            //刷新后开启此脚本
            enabled = true;
        }
        /// <summary>
        /// 添加单元到此区块
        /// </summary>
        /// <param name="index">要添加到的单元下标</param>
        /// <param name="cell">要添加的单元</param>
        public void AddCell(int index, HexCell cell)
        {
            //设置单元
            cells[index] = cell;
            //设置单元所属区块
            cell.chunk = this;
            //设置父物体
            cell.transform.SetParent(transform, false);
            cell.uiRect.SetParent(gridCanvas.transform, false);
        }
        /// <summary>
        /// 是否显示UI
        /// </summary>
        public void ShowUI(bool visible)
        {
            gridCanvas.gameObject.SetActive(visible);
        }
    }
}