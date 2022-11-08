using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameScene
{
    namespace Map
    {
        public class MapNode
        {
            /// <summary>
            /// 节点类型
            /// </summary>
            private MapNodeState _state;
            /// <summary>
            /// 节点位置2维坐标
            /// </summary>
            private (int, int) _position2;
            /// <summary>
            /// 节点位置3维坐标
            /// </summary>
            private (int, int, int) _position3;
            /// <summary>
            /// 节点是否已被探索
            /// </summary>
            private bool _isVisit;
            /// <summary>
            /// <para/>关注度
            /// <para/>为该节点在多少个我方单位的视野内
            /// <para/>用于判断是否需要添加战争迷雾
            /// </summary>
            private int _notice;

            public MapNodeState     State       { get { return _state;                              } }
            public (int, int)       Position2   { get { return _position2;                          } }
            public (int, int, int)  Position3   { get { return _position3;                          } }
            public bool             IsVisit     { get { return _isVisit;                            } }
            public bool             IsInfog     { get { return true == IsVisit && 0 == _notice;     } }

            public MapNode()
            {
                _state = MapNodeState.Grass;
                _position2 = (0, 0);
                _position3 = (0, 0, 0);
                _isVisit = true;
                _notice = 0;
            }

            /// <summary>
            /// 设置节点坐标
            /// </summary>
            /// <param name="position2">2维坐标</param>
            public void SetPosition((int, int) position2)
            {
                _position2 = position2;
                _position3 = Position2To3(_position2);
            }

            /// <summary>
            /// 设置节点坐标
            /// </summary>
            /// <param name="position3">3维坐标</param>
            public void SetPosition((int, int, int) position3)
            {
                _position3 = position3;
                _position2 = Position3To2(_position3);
            }

            /// <summary>
            /// 将六边形节点的2维坐标转换维3维坐标
            /// </summary>
            /// <param name="pos2">要转换的2维坐标</param>
            /// <returns>转换后的3维坐标</returns>
            public static (int, int, int) Position2To3((int, int) pos2)
            {
                if (pos2.Item2 > 0)
                {//非负数
                    if ((pos2.Item2 & 1) == 0)
                    {//偶数
                        return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                    }
                    else
                    {//奇数
                        return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2 + 1, pos2.Item2);
                    }
                }
                else
                {//负数
                    if ((pos2.Item2 & 1) == 0)
                    {//偶数
                        return (pos2.Item1 - pos2.Item2 / 2, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                    }
                    else
                    {//奇数
                        return (pos2.Item1 - pos2.Item2 / 2 + 1, pos2.Item1 + pos2.Item2 / 2, pos2.Item2);
                    }
                }
            }

            /// <summary>
            /// 将六边形节点的3维坐标转换维2维坐标
            /// </summary>
            /// <param name="pos3">要转换的3维坐标</param>
            /// <returns>转换后的2维坐标</returns>
            public static (int, int) Position3To2((int, int, int) pos3)
            {
                if ((pos3.Item3 & 1) == 0)
                {//偶数
                    return ((pos3.Item1 + pos3.Item2) / 2, pos3.Item3);
                }
                else
                {//奇数
                    if (pos3.Item1 + pos3.Item2 > 0)
                    {//正半轴
                        return ((pos3.Item1 + pos3.Item2) / 2, pos3.Item3);
                    }
                    else
                    {//负半轴
                        return ((pos3.Item1 + pos3.Item2) / 2 - 1, pos3.Item3);
                    }
                }
            }
        }
    }
}
