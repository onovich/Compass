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

        public static Vector2Int Pos2Index(Vector3 pos, int mpu, Vector2 localOffset) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu);

            return new Vector2Int(x, y);

        }

    }

}