using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public static class MathUtil {

        public static Node2D Pos2Node(Vector3 pos, int mpu, Vector2 localOffset, Map2D map) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu - 1 / (float)mpu / 2);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu - 1 / (float)mpu / 2);

            x = Mathf.Clamp(x, 0, map.Width - 1);
            y = Mathf.Clamp(y, 0, map.Height - 1);

            if (x > map.Width - 1 || x < 0 || y > map.Height - 1 || y < 0) {
                Debug.LogError($"out of range: x = {x}, y = {y}; map width = {map.Width}, map height = {map.Height}");
            }

            var node = map.Nodes[x, y];
            if (node == null) {
                Debug.LogError($"node is null: {x}, {y}");
            }

            return node;

        }

        public static Vector2Int Pos2Index(Vector3 pos, int mpu, Vector2 localOffset, Map2D map) {

            var x = Mathf.RoundToInt((pos.x - localOffset.x) * mpu - 1 / (float)mpu / 2);
            var y = Mathf.RoundToInt((pos.y - localOffset.y) * mpu - 1 / (float)mpu / 2);

            x = Mathf.Clamp(x, 0, map.Width - 1);
            y = Mathf.Clamp(y, 0, map.Height - 1);

            return new Vector2Int(x, y);

        }

    }

}