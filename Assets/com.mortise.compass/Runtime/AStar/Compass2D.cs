using System;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {
    public class Compass2D {

        Node2DPool pool;
        PriorityQueue<Node2D> openList;
        HashSet<Node2D> closedList;
        Dictionary<Node2D, Node2D> cameFrom;
        Dictionary<Node2D, float> gScore;
        Dictionary<Node2D, float> fScore;

        Func<Node2D, Node2D, float> heuristicFunc;

        Node2D[,] grid;
        int width;
        int height;

        public void FromTM(GridTM tm) {
            this.grid = tm.grid;
            this.width = tm.width;
            this.height = tm.height;
        }

        public Compass2D(GridTM tm, HeuristicType type = HeuristicType.Euclidean) {
            heuristicFunc = HeuristicUtil.GetHeuristic(type);
            pool = new Node2DPool();
            openList = new PriorityQueue<Node2D>();
            closedList = new HashSet<Node2D>();
            cameFrom = new Dictionary<Node2D, Node2D>();
            gScore = new Dictionary<Node2D, float>();
            fScore = new Dictionary<Node2D, float>();
            FromTM(tm);
        }

        void ResetNode(Node2D node) {
            cameFrom.Remove(node);
            gScore.Remove(node);
            fScore.Remove(node);
            pool.ReturnNode(node);
        }

        void UpdateNode(Node2D current, Node2D neighbor, Node2D target, float newGScore) {
            cameFrom[neighbor] = current;
            gScore[neighbor] = newGScore;
            fScore[neighbor] = gScore[neighbor] + heuristicFunc(neighbor, target);
            if (openList.Contains(neighbor)) {
                openList.UpdatePriority(neighbor, (int)fScore[neighbor]);
            } else {
                openList.Enqueue(neighbor, (int)fScore[neighbor]);
            }
        }

        List<Node2D> ReconstructPath(Node2D current) {
            List<Node2D> totalPath = new List<Node2D>();
            while (cameFrom.ContainsKey(current)) {
                totalPath.Add(current);
                current = cameFrom[current];
            }
            totalPath.Reverse();
            return totalPath;
        }

        List<Node2D> GenerateNeighbors(Node2D node) {
            List<Node2D> neighbors = new List<Node2D>();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0) continue;
                    int checkX = node.X + x;
                    int checkY = node.Y + y;
                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
                        var walkable = grid[checkX, checkY].Walkable;
                        neighbors.Add(pool.Get(checkX, checkY, walkable));
                    }
                }
            }
            return neighbors;
        }

        public List<Node2D> FindPath(Node2D start, Node2D target) {
            openList.Clear();
            closedList.Clear();
            cameFrom.Clear();
            gScore.Clear();
            fScore.Clear();

            // Assume all nodes in the grid are returned to the pool
            pool.ReturnAll();

            var walkableOfStart = grid[start.X, start.Y].Walkable;
            var walkableOfTarget = grid[target.X, target.Y].Walkable;
            Node2D startNode = pool.Get(start.X, start.Y, walkableOfStart);
            Node2D targetNode = pool.Get(target.X, target.Y, walkableOfTarget);

            gScore[startNode] = 0;
            fScore[startNode] = heuristicFunc(startNode, targetNode);

            openList.Enqueue(startNode, (int)fScore[startNode]);

            while (openList.Count > 0) {
                Node2D current = openList.Dequeue();

                if (current.Equals(targetNode)) {
                    var path = ReconstructPath(current);
                    return path;
                }

                closedList.Add(current);

                foreach (Node2D neighbor in GenerateNeighbors(current)) {
                    if (closedList.Contains(neighbor) || !neighbor.Walkable) {
                        continue;
                    }

                    float tentativeGScore = gScore[current] + heuristicFunc(current, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                        if (!cameFrom.ContainsKey(neighbor)) {
                            cameFrom[neighbor] = pool.Get(neighbor.X, neighbor.Y, neighbor.Walkable);
                        }
                        UpdateNode(current, neighbor, targetNode, tentativeGScore);
                    }
                }
            }

            // If no path was found, return all nodes back to the pool
            pool.ReturnAll();

            return null;
        }

    }
}
