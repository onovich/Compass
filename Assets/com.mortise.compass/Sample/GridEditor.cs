using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class GridEditor : MonoBehaviour {

        [Header("输出")] public GridSampleSO model;

        [Header("网格尺寸")] public Vector2Int gridSize = new Vector2Int(10, 10);

        [Header("格子尺寸")] public Vector2 cellSize = new Vector2(1, 1);

        [Header("可通行颜色")] public Color cellColor_walkable = Color.white;
        [Header("不可通行颜色")] public Color cellColor_unwalkable = Color.red;
        [Header("边框颜色")] public Color cellColor_border = Color.black;

        [ContextMenu("生成网格")]
        void SaveAsset() {

            var grid = new Node2D[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    grid[x, y] = new Node2D(x, y, true);
                }
            }

            model.tm.grid = grid;
            model.tm.width = gridSize.x;
            model.tm.height = gridSize.y;

            var obstacles = GetObstacleArray();
            for (int i = 0; i < obstacles.Length; i++) {
                var obstacle = obstacles[i];
                var x = (int)obstacle.X;
                var y = (int)obstacle.Y;
                grid[x, y].Init(x, y, obstacle.Walkable);
            }

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        public Node2D[] GetObstacleArray() {
            var group = transform.Find("editor_obstacle_group");
            var goes = group.transform.GetComponentsInChildren<ObstacleEditor>();
            if (goes == null || goes.Length == 0) {
                return null;
            }

            var arr = new Node2D[goes.Length];
            for (int i = 0; i < goes.Length; i++) {
                var go = goes[i];
                var node = go.GetNode2D();
                arr[i] = node;
            }
            return arr;
        }

        void OnDrawGizmos() {
            // var gridOffset = new Vector2((float)gridSize.x * cellSize.x / 2f, (float)gridSize.y * cellSize.y / 2f);
            // var boxOffset = new Vector2(cellSize.x / 2f, cellSize.y / 2f);
            if (model == null || model.tm == null || model.tm.grid == null) {
                return;
            }
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    Vector3 pos = new Vector3(x * cellSize.x, y * cellSize.y, 0);
                    // pos += transform.position - (Vector3)gridOffset + (Vector3)boxOffset;
                    pos += transform.position;
                    if (model.tm.grid[x, y].Walkable) {
                        Gizmos.color = cellColor_walkable;
                    } else {
                        Gizmos.color = cellColor_unwalkable;
                    }
                    Gizmos.DrawCube(pos, cellSize);
                    Gizmos.color = cellColor_border;
                    Gizmos.DrawWireCube(pos, cellSize);
                }
            }
        }

    }

}
