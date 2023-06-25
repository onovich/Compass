using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public static class MathUtil {

        public static Node2D Pos2Node(Vector3 pos, int mpu, Vector2 localOffset, Map2D map) {

            var x = Mathf.FloorToInt(pos.x - localOffset.x) * mpu;
            var y = Mathf.FloorToInt(pos.y - localOffset.y) * mpu;
            var index = new Vector2Int(x, y);

            // Index2CenterPos
            if (index.x < 0 || index.x >= map.Width || index.y < 0 || index.y >= map.Height) {
                Debug.LogError($"out of range: x={x}, y={index.y}, width={map.Width}, height={map.Height}, localOffset = {localOffset}");
            }

            var node = map.Nodes[index.x, index.y];

            var nodeX = Mathf.RoundToInt(node.X);
            var nodeY = Mathf.RoundToInt(node.Y);

            var fixedNode = map.Nodes[nodeX, nodeY];

            if (fixedNode == null) {
                Debug.LogError($"node is null: {index.x}, {index.y}");
            }

            return fixedNode;

        }

        public static Vector2Int Pos2Index(Vector3 pos, int mpu, Vector2 localOffset) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu);

            return new Vector2Int(x, y);

        }

    }

}