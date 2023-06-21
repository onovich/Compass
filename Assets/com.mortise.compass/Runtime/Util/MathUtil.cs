using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public static class MathUtil {

        public static Node2D Pos2Node(Vector3 pos, int mpu, Vector2 localOffset, Map2D map) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu);

            var node = map.Nodes[x, y];
            if (node == null) {
                Debug.LogError($"node is null: {x}, {y}");
            }

            return node;

        }

        public static Node2D Pos2Node(Vector2 pos, int mpu, Vector2 localOffset, Map2D map, Node2D target) {
            float x = (pos.x - localOffset.x) * mpu;
            float y = (pos.y - localOffset.y) * mpu;

            int floorX = Mathf.FloorToInt(x);
            int floorY = Mathf.FloorToInt(y);
            int ceilX = Mathf.CeilToInt(x);
            int ceilY = Mathf.CeilToInt(y);

            Node2D[] candidates = new Node2D[4];
            candidates[0] = IsValid(floorX, floorY, map) ? map.Nodes[floorX, floorY] : null;
            candidates[1] = IsValid(ceilX, floorY, map) ? map.Nodes[ceilX, floorY] : null;
            candidates[2] = IsValid(floorX, ceilY, map) ? map.Nodes[floorX, ceilY] : null;
            candidates[3] = IsValid(ceilX, ceilY, map) ? map.Nodes[ceilX, ceilY] : null;

            Node2D closest = null;
            float minF = float.MaxValue;

            foreach (var candidate in candidates) {
                if (candidate != null) {
                    candidate.SetH(Mathf.Abs(candidate.X - target.X) + Mathf.Abs(candidate.Y - target.Y));
                    candidate.SetF(candidate.G + candidate.H);

                    if (candidate.F < minF) {
                        minF = candidate.F;
                        closest = candidate;
                    }
                }
            }

            return closest;
        }

        public static bool IsValid(int x, int y, Map2D map) {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height) {
                return false;
            }
            return true;
        }

        public static Vector2Int Pos2Index(Vector3 pos, int mpu, Vector2 localOffset) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu);

            return new Vector2Int(x, y);

        }

    }

}