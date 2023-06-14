using ClipperLib;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MortiseFrame.Knot;
using MortiseFrame.Knot.Shape2D;


namespace MortiseFrame.Compass.Sample {

    public class GridEditor : MonoBehaviour {

        [Header("输出")] public GridSampleSO model;

        [Header("网格尺寸")] public Vector2Int gridSize = new Vector2Int(10, 10);

        [Header("格子尺寸")] public Vector2 cellSize = new Vector2(1, 1);

        [Header("可通行颜色")] public Color cellColor_walkable = Color.white;
        [Header("不可通行颜色")] public Color cellColor_unwalkable = Color.red;
        [Header("边框颜色")] public Color cellColor_border = Color.black;

        Vector2 overLapPos = Vector2.zero;

        AABB[,] cellAABBs;
        List<AABB> obstacles_box_aabb;
        List<OBB> obstacles_box_obb;
        List<Circle> obstacles_circle;

        [ContextMenu("烘焙网格")]
        void SaveGrid() {

            BakeGrid();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        [ContextMenu("烘焙障碍物")]
        void SaveObstacle() {

            BakeObstacleArray();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        [ContextMenu("烘焙碰撞信息")]
        void SaveIntersectInfo() {

            BakeIntersectInfo();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        [ContextMenu("清除网格")]
        void ClearGrid() {

            model.tm.ClearGrid();
            cellAABBs = null;
            obstacles_box_aabb = null;
            obstacles_box_obb = null;
            obstacles_circle = null;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        [ContextMenu("清除障碍物")]
        void ClearObstacle() {

            obstacles_box_aabb = null;
            obstacles_box_obb = null;
            obstacles_circle = null;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        [ContextMenu("清除碰撞信息")]
        void ClearIntersectInfo() {

            for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                for (int y = 0; y < cellAABBs.GetLength(1); y++) {
                    model.tm.SetGridValue(x, y, true);
                }
            }

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        void BakeIntersectInfo() {

            for (int i = 0; i < obstacles_box_aabb.Count; i++) {

                var obstacle_aabb = obstacles_box_aabb[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_AABB(cell_aabb, obstacle_aabb, float.Epsilon);

                        if (intersect) {
                            model.tm.SetGridValue(x, y, false);
                            Debug.Log($"AABB 碰撞: {x},{y}");
                        }

                    }
                }

            }

            for (int i = 0; i < obstacles_box_obb.Count; i++) {

                var obstacle_obb = obstacles_box_obb[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_OBB(cell_aabb, obstacle_obb, float.Epsilon);

                        if (intersect) {
                            model.tm.SetGridValue(x, y, false);
                        }
                    }
                }

            }

            for (int i = 0; i < obstacles_circle.Count; i++) {

                var obstacle_circle = obstacles_circle[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_Circle(cell_aabb, obstacle_circle, float.Epsilon);

                        if (intersect) {
                            model.tm.SetGridValue(x, y, false);
                        }
                    }
                }

            }

        }

        void BakeGrid() {

            var grid = new bool[gridSize.x * gridSize.y];
            model.tm.SetGrid(grid);
            cellAABBs = new AABB[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    model.tm.SetGridValue(x, y, true);
                    var cell_aabb = Cell2AABB(x, y);
                    cellAABBs[x, y] = cell_aabb;
                }
            }

            model.tm.width = gridSize.x;
            model.tm.height = gridSize.y;

            model.tm.cellWidth = cellSize.x;
            model.tm.cellHeight = cellSize.y;
            Debug.Log($"网格大小: {cellSize.x},{cellSize.y}");

        }

        void BakeObstacleArray() {

            var group = transform.Find("editor_obstacle_group");
            var goes = group.transform.GetComponentsInChildren<ObstacleEditor>();

            obstacles_box_aabb = new List<AABB>();
            obstacles_box_obb = new List<OBB>();
            obstacles_circle = new List<Circle>();

            if (goes == null || goes.Length == 0) {
                return;
            }

            for (int i = 0; i < goes.Length; i++) {

                var boxCol = goes[i].GetComponent<BoxCollider2D>();
                var circleCol = goes[i].GetComponent<CircleCollider2D>();

                if (boxCol == null && circleCol == null) {
                    Debug.LogError("障碍物必须有BoxCollider2D或者CircleCollider2D组件");
                }

                if (boxCol == null && circleCol != null) {
                    var circle = CircleCollider2Circle(circleCol);
                    obstacles_circle.Add(circle);
                    Debug.Log($"圆形障碍物: {circle.Center.x},{circle.Center.y},{circle.Radius}");
                }

                if (boxCol != null && circleCol == null && boxCol.transform.eulerAngles.z == 0) {
                    var aabb = BoxCollider2AABB(boxCol);
                    obstacles_box_aabb.Add(aabb);
                    Debug.Log($"矩形障碍物: {aabb.GetCenter().x},{aabb.GetCenter().y},{aabb.GetSize().x},{aabb.GetSize().y}");
                }

                if (boxCol != null && circleCol == null && boxCol.transform.eulerAngles.z != 0) {
                    var obb = BoxCollider2OBB(boxCol);
                    obstacles_box_obb.Add(obb);
                    Debug.Log($"旋转矩形障碍物: {obb.Center.x},{obb.Center.y},{obb.Size.x},{obb.Size.y},{obb.RadAngle}");
                }

            }

        }

        Circle CircleCollider2Circle(CircleCollider2D collider) {

            var center = collider.transform.position;
            var radius = collider.radius;
            var circle = new Circle(center, radius);
            return circle;

        }

        OBB BoxCollider2OBB(BoxCollider2D collider) {

            var offset = collider.offset;
            var size = collider.size;
            var center = collider.transform.position;
            var angle = collider.transform.eulerAngles.z;
            var obb = new OBB(center, size, angle);
            return obb;

        }

        AABB BoxCollider2AABB(BoxCollider2D collider) {
            // var offset = collider.offset;
            var offset = Vector2.zero;
            var size = collider.size;
            var xMin = collider.transform.position.x - size.x / 2 + offset.x;
            var xMax = collider.transform.position.x + size.x / 2 + offset.x;
            var yMin = collider.transform.position.y - size.y / 2 + offset.y;
            var yMax = collider.transform.position.y + size.y / 2 + offset.y;
            var min = new Vector2(xMin, yMin);
            var max = new Vector2(xMax, yMax);
            var aabb = new AABB(min, max);

            Debug.Log($"aabb: {collider.transform.position.x},{collider.transform.position.y},{size.x},{size.y},xmin:{xMin},xmax:{xMax},ymin:{yMin},ymax:{yMax}");
            return aabb;
        }

        AABB Cell2AABB(int x, int y) {
            var xMin = (int)(x * cellSize.x);
            var xMax = (int)((x + 1) * cellSize.x);
            var yMin = (int)(y * cellSize.y);
            var yMax = (int)((y + 1) * cellSize.y);
            var min = new Vector2(xMin, yMin);
            var max = new Vector2(xMax, yMax);
            var cell_aabb = new AABB(min, max);
            return cell_aabb;
        }

        void OnDrawGizmos() {
            if (model == null || model.tm == null || model.tm.Grid == null) {
                return;
            }
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    if (model.tm.GetGridValue(x, y)) {
                        var worldPos = model.tm.GetLocalPosition(x, y) + (Vector2)transform.position;
                        Gizmos.color = cellColor_walkable;
                        Gizmos.DrawWireCube(worldPos, cellSize);
                    } else {
                        Gizmos.color = cellColor_unwalkable;
                        var worldPos = model.tm.GetLocalPosition(x, y) + (Vector2)transform.position;
                        Gizmos.DrawWireCube(worldPos, cellSize);
                        var min = Vector2.zero - cellSize / 2;
                        var max = cellSize / 2;
                        Gizmos.DrawLine(worldPos + new Vector2(min.x, min.y), worldPos + new Vector2(max.x, max.y));
                        Gizmos.DrawLine(worldPos + new Vector2(min.x, max.y), worldPos + new Vector2(max.x, min.y));
                    }
                }
            }

        }

    }

}
