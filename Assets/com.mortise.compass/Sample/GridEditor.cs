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

        Vector2 overLapPos = Vector2.zero;
        bool overLapDone = false;

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

            overLapDone = false;

            var polygons = GetPolygonsArray();
            for (int i = 0; i < polygons.Length; i++) {

                var polygon = polygons[i];
                for (int x = 0; x < gridSize.x; x++) {
                    for (int y = 0; y < gridSize.y; y++) {
                        if (IsPointInPolygon(x, y, polygon)) {
                            grid[x, y].Init(x, y, false);
                            Debug.Log($"添加障碍物:x={x},y={y}");
                        }
                    }
                }
            }

            overLapDone = true;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        bool IsPointInPolygon(int x, int y, Vector2[] polygon) {
            bool isInside = false;
            var point = new Vector2(x * cellSize.x + transform.position.x, y * cellSize.y + transform.position.y);
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++) {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)) {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        public Vector2[][] GetPolygonsArray() {
            var group = transform.Find("editor_obstacle_group");
            var goes = group.transform.GetComponentsInChildren<ObstacleEditor>();
            if (goes == null || goes.Length == 0) {
                return null;
            }
            var arr = new Vector2[goes.Length][];
            for (int i = 0; i < goes.Length; i++) {
                var go = goes[i];
                var collider = go.GetComponent<PolygonCollider2D>();
                var points = collider.points;
                for (int j = 0; j < points.Length; j++) {
                    var offset = collider.offset;
                    points[j] = new Vector2(points[j].x + go.transform.position.x, points[j].y + go.transform.position.y) + offset;
                }
                arr[i] = points;
            }
            return arr;
        }

        void OnDrawGizmos() {
            if (model == null || model.tm == null || model.tm.grid == null) {
                return;
            }
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    Vector3 pos = new Vector3(x * cellSize.x, y * cellSize.y, 0);
                    pos += transform.position;
                    if (model.tm.grid[x, y].Walkable) {
                        Gizmos.color = cellColor_walkable;
                    } else {
                        Gizmos.color = cellColor_unwalkable;
                        Gizmos.DrawCube(pos, cellSize);
                    }
                    Gizmos.DrawWireCube(pos, cellSize);
                }
            }
            if (!overLapDone) {
                return;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(overLapPos, cellSize);
        }

    }

}
