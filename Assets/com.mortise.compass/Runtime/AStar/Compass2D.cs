using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {
    public class Compass2D {

        readonly List<Node2D> openList = new List<Node2D>();
        readonly List<Node2D> closedList = new List<Node2D>();
        readonly int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        readonly int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

        // 启发式函数
        readonly Func<Node2D, Node2D, float> heuristicFunc;

        public Compass2D(HeuristicType type = HeuristicType.Euclidean) {
            this.heuristicFunc = HeuristicUtil.GetHeuristic(type);
        }

        public List<Node2D> FindPath(Map2D map, Node2D start, Node2D end, float agentSize) {

            openList.Clear();
            closedList.Clear();
            openList.Add(start);

            while (openList.Count > 0) {
                openList.Sort();
                var currentNode = openList[0];

                if (currentNode.X == end.X && currentNode.Y == end.Y) {
                    var path = GetPathFromNode(currentNode, start);
                    return path;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                for (int i = 0; i < 8; i++) {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) {
                        continue;
                    }

                    // 通行度测试
                    if (map.Nodes[nx, ny].Capacity < agentSize) {
                        continue;
                    }
                    Debug.Log($"通行度测试通过: {map.Nodes[nx, ny].Capacity} >= {agentSize}");

                    var neighbour = map.Nodes[nx, ny];

                    if (closedList.Contains(neighbour) || !neighbour.Walkable) {
                        continue;
                    }

                    if (!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
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
                        openList.Sort();
                    }
                }
            }

            return null;
        }


        private List<Node2D> GetPathFromNode(Node2D node, Node2D startNode) {
            var path = new List<Node2D>();
            while (node != null) {
                if (node != startNode) {
                    path.Add(node);
                }
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

    }
}
