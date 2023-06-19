using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public static class MathUtil {

        public static Node2D Pos2Node(Vector3 pos, int mpu, Vector2 localOffset, Map2D map) {

            var x = Mathf.FloorToInt((pos.x - localOffset.x) * mpu);
            var y = Mathf.FloorToInt((pos.y - localOffset.y) * mpu);

            var node = map.Nodes[x, y];
            if (node == null) {
                Debug.LogError($"node is null: {x}, {y}");
            }

            return node;

        }

    }

}