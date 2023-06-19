using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {
    public class Compass2D {

        readonly PriorityQueue<Node2D> openList = new PriorityQueue<Node2D>();
        readonly HashSet<Node2D> closedList = new HashSet<Node2D>();
        readonly int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        readonly int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };
        readonly int mpu;
        readonly Vector2 localOffset;
        readonly Node2DPool nodePool;

        // 启发式函数
        readonly Func<Node2D, Node2D, float> heuristicFunc;

        public Compass2D(int mpu, Node2DPool nodePool, Vector2 localOffset, HeuristicType type = HeuristicType.Euclidean) {
            this.mpu = mpu;
            this.localOffset = localOffset;
            this.heuristicFunc = HeuristicUtil.GetHeuristic(type);
            this.nodePool = nodePool;
        }

        public List<Node2D> FindPath(Map2D map, Vector2 startPos, Vector2 endPos, float agentsize) {
            openList.Clear();
            closedList.Clear();

            var start = MathUtil.Pos2Node(startPos, mpu, localOffset, map);
            var end = MathUtil.Pos2Node(endPos, mpu, localOffset, map);
            var agentRealSize = agentsize * mpu;

            if (start == null || end == null) {
                Debug.LogError($"start or end is null: {startPos}, {endPos}");
                return null;
            }

            start.SetG(0);
            start.SetH(heuristicFunc(start, end));
            start.SetF(start.G + start.H);
            start.SetParent(null);

            openList.Enqueue(start, start.F);

            while (openList.Count > 0) {
                var currentNode = openList.Dequeue();
                if (currentNode == null) {
                    Debug.LogError("currentNode is null");
                    return null;
                }

                if (currentNode == end) {
                    return GetPathFromNode(currentNode);
                }

                closedList.Add(currentNode);

                for (int i = 0; i < 8; i++) {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) {
                        continue;
                    }

                    var neighbour = map.Nodes[nx, ny];

                    if (closedList.Contains(neighbour) || neighbour.Capacity < agentRealSize) {
                        continue;
                    }

                    float tentativeG = currentNode.G + 1;

                    if (!openList.Contains(neighbour)) {
                        openList.Enqueue(neighbour, neighbour.F);
                    } else if (tentativeG >= neighbour.G) {
                        continue;
                    }

                    neighbour.SetG(tentativeG);
                    neighbour.SetH(heuristicFunc(neighbour, end));
                    neighbour.SetF(neighbour.G + neighbour.H);
                    neighbour.SetParent(currentNode);

                    if (openList.Contains(neighbour)) {
                        openList.UpdatePriority(neighbour, neighbour.F);
                    }
                }
            }

            return null;
        }

        private List<Node2D> GetPathFromNode(Node2D endNode) {
            var path = new List<Node2D>();
            var currentNode = endNode;

            while (currentNode != null) {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}