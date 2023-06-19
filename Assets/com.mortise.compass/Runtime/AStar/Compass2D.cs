using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {
    public class Compass2D {

        readonly PriorityQueue<Node2D> openList = new PriorityQueue<Node2D>();
        readonly List<Node2D> closedList = new List<Node2D>();
        readonly int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        readonly int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };
        readonly int mpu;
        readonly Vector2 localOffset;

        // 启发式函数
        readonly Func<Node2D, Node2D, float> heuristicFunc;

        public Compass2D(int mpu, Vector2 localOffset, HeuristicType type = HeuristicType.Euclidean) {
            this.mpu = mpu;
            this.localOffset = localOffset;
            this.heuristicFunc = HeuristicUtil.GetHeuristic(type);
        }

        public List<Node2D> FindPath(Map2D map, Vector2 startPos, Vector2 endPos, float agentsize) {

            openList.Clear();
            closedList.Clear();

            var start = MathUtil.Pos2Node(startPos, mpu, localOffset, map);
            var end = MathUtil.Pos2Node(endPos, mpu, localOffset, map);
            var agentRealSize = agentsize * mpu;

            if (start == null) {
                Debug.LogError($"start is null: {startPos}");
                return null;
            }

            openList.Enqueue(start, start.F);

            while (openList.Count > 0) {
                var currentNode = openList.Dequeue();
                if (currentNode == null) {
                    Debug.LogError("currentNode is null");
                    return null;
                }

                if (currentNode.X == end.X && currentNode.Y == end.Y) {
                    var path = GetPathFromNode(currentNode, start);
                    return path;
                }

                closedList.Add(currentNode);

                for (int i = 0; i < 8; i++) {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) {
                        continue;
                    }

                    // 通行度测试
                    if (map.Nodes[nx, ny].Capacity < agentRealSize) {
                        continue;
                    }

                    var neighbour = map.Nodes[nx, ny];

                    if (closedList.Contains(neighbour) || !neighbour.Walkable) {
                        continue;
                    }

                    if (!openList.Contains(neighbour)) {
                        openList.Enqueue(neighbour, neighbour.F);
                        neighbour.SetG(currentNode.G + 1);
                        neighbour.SetH(heuristicFunc(neighbour, end));
                        neighbour.SetF(neighbour.G + neighbour.H);
                        neighbour.SetParent(currentNode);
                    } else if (neighbour.G > currentNode.G + 1) {
                        var oldG = neighbour.G;
                        var oldF = neighbour.F;
                        neighbour.SetG(currentNode.G + 1);
                        neighbour.SetF(neighbour.G + neighbour.H);
                        neighbour.SetParent(currentNode);
                        openList.UpdatePriority(neighbour, neighbour.F);
                    }
                }
            }

            return null;
        }

        private List<Node2D> GetPathFromNode(Node2D node, Node2D startNode) {

            var path = new List<Node2D> { node };

            while (node.Parent != startNode) {
                node = node.Parent;
                path.Add(node);
            }

            path.Add(startNode);
            path.Reverse();

            return path;

        }
        
    }
}
