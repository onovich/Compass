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
        [Header("覆盖单位数")] public Vector2Int UnitCount = new Vector2Int(10, 10);
        [Header("每单位网格数")] public int MPU = 1;

        Vector2Int cellCount;
        Vector2 cellSize;

        AABB[,] cellAABBs;
        List<AABB> obstacles_box_aabb;
        List<OBB> obstacles_box_obb;
        List<Circle> obstacles_circle;

        bool isBaked = false;

        [ContextMenu("一键烘焙")]
        void SaveAsset() {

            isBaked = false;

            ClearGrid();
            ClearObstacle();
            ClearIntersectInfo();

            BakeGrid();
            BakeObstacleArray();
            BakeIntersectInfo();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

            isBaked = true;

        }

        void ClearGrid() {

            model.tm.ClearWalkableValue();
            model.tm.Clear();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        void ClearObstacle() {

            obstacles_box_aabb?.Clear();
            obstacles_box_aabb = null;
            obstacles_box_obb = null;
            obstacles_circle = null;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        void ClearIntersectInfo() {

            if (cellAABBs == null) {
                return;
            }

            cellAABBs = null;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(model);

        }

        void BakeAABB() {

            if (obstacles_box_aabb == null || obstacles_box_aabb.Count == 0) {
                return;
            }

            for (int i = 0; i < obstacles_box_aabb.Count; i++) {

                var obstacle_aabb = obstacles_box_aabb[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_AABB(cell_aabb, obstacle_aabb, float.Epsilon);

                        if (intersect) {
                            var index = new Vector2Int(x, y);
                            model.tm.SetWalkableValueWithIndex(index, false);
                            EditorUtility.SetDirty(model);

                            Debug.Log($"AABB 碰撞: index: {x},{y} aabb:{cell_aabb.GetCenter().x},{cell_aabb.GetCenter().y} obstacle:{obstacle_aabb.GetCenter().x},{obstacle_aabb.GetCenter().y}s");
                        }

                    }
                }

            }

        }

        void BakeOBB() {

            if (obstacles_box_obb == null || obstacles_box_obb.Count == 0) {
                return;
            }

            for (int i = 0; i < obstacles_box_obb.Count; i++) {

                var obstacle_obb = obstacles_box_obb[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_OBB(cell_aabb, obstacle_obb, float.Epsilon);

                        if (intersect) {
                            var index = new Vector2Int(x, y);
                            Debug.Log($"OBB 碰撞: index: {x},{y} aabb:{cell_aabb.GetCenter().x},{cell_aabb.GetCenter().y} obstacle:{obstacle_obb.Center.x},{obstacle_obb.Center.y}");
                            model.tm.SetWalkableValueWithIndex(index, false);
                        } else {
                            Debug.Log($"OBB 未碰撞: index: {x},{y} aabb.Min:{cell_aabb.Min.x},{cell_aabb.Min.y}, aabb.Max:{cell_aabb.Max.x},{cell_aabb.Max.y} obstacle:{obstacle_obb.Center.x},{obstacle_obb.Center.y},angle:{obstacle_obb.RadAngle}");
                        }
                    }
                }

            }

        }

        void BakeCircle() {

            if (obstacles_circle == null || obstacles_circle.Count == 0) {
                return;
            }

            for (int i = 0; i < obstacles_circle.Count; i++) {

                var obstacle_circle = obstacles_circle[i];

                for (int x = 0; x < cellAABBs.GetLength(0); x++) {
                    for (int y = 0; y < cellAABBs.GetLength(1); y++) {

                        var cell_aabb = cellAABBs[x, y];
                        var intersect = Intersect2DUtil.IsIntersectAABB_Circle(cell_aabb, obstacle_circle, float.Epsilon);

                        if (intersect) {
                            var index = new Vector2Int(x, y);
                            model.tm.SetWalkableValueWithIndex(index, false);
                        }
                    }
                }

            }

        }

        void BakeIntersectInfo() {

            BakeAABB();
            BakeOBB();
            BakeCircle();

        }

        void BakeGrid() {

            cellSize = new Vector2(1 / (float)MPU, 1 / (float)MPU);
            cellCount = new Vector2Int(UnitCount.x * MPU, UnitCount.y * MPU);

            model.tm.countX = cellCount.x;
            model.tm.countY = cellCount.y;

            var grid = new bool[cellCount.x * cellCount.y];
            Debug.Log($"网格大小: {cellCount.x},{cellCount.y}");
            model.tm.SetWalkableValue(grid);
            cellAABBs = new AABB[cellCount.x, cellCount.y];

            for (int x = 0; x < cellCount.x; x++) {
                for (int y = 0; y < cellCount.y; y++) {
                    var index = new Vector2Int(x, y);
                    var pos = Index2GizmosCenter(index);
                    model.tm.SetWalkableValueWithIndex(index, true);
                    var cell_aabb = Index2AABB(x, y);
                    cellAABBs[x, y] = cell_aabb;
                }
            }

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
            var angle = collider.transform.eulerAngles.z * Mathf.Deg2Rad;
            var obb = new OBB(center, size, angle);
            return obb;

        }

        AABB BoxCollider2AABB(BoxCollider2D collider) {

            // var offset = transform.position;
            var offset = collider.offset;
            var size = collider.size;
            var xMin = collider.transform.position.x - size.x / 2 + offset.x;
            var xMax = collider.transform.position.x + size.x / 2 + offset.x;
            var yMin = collider.transform.position.y - size.y / 2 + offset.y;
            var yMax = collider.transform.position.y + size.y / 2 + offset.y;
            var min = new Vector2(xMin, yMin);
            var max = new Vector2(xMax, yMax);
            var aabb = new AABB(min, max);

            // Debug.Log($"aabb: {collider.transform.position.x},{collider.transform.position.y},{size.x},{size.y},xmin:{xMin},xmax:{xMax},ymin:{yMin},ymax:{yMax}");
            return aabb;

        }

        AABB Index2AABB(int x, int y) {
            var offset = cellSize / 2;
            var xMin = x * cellSize.x;
            var xMax = x * cellSize.x + offset.x * 2;
            var yMin = y * cellSize.y;
            var yMax = y * cellSize.y + offset.y * 2;
            var min = new Vector2(xMin, yMin);
            var max = new Vector2(xMax, yMax);
            var cell_aabb = new AABB(min, max);
            return cell_aabb;
        }

        void OnDrawGizmos() {

            if (!isBaked || model == null || model.tm == null || model.tm.WalkableValue == null) {
                return;
            }

            DrawGrid();

        }

        void DrawGrid() {

            for (int i = 0; i < cellCount.x; i++) {

                for (int j = 0; j < cellCount.y; j++) {

                    var index = new Vector2Int(i, j);
                    var walkable = model.tm.GetWalkableValueWithIndex(index);

                    if (walkable) {

                        Gizmos.color = Color.white;
                        var pos = Index2GizmosCenter(index);
                        Gizmos.DrawWireCube(pos, cellSize);

                    } else {

                        Gizmos.color = Color.red;
                        var pos = Index2GizmosCenter(index);
                        Gizmos.DrawWireCube(pos, cellSize);

                        var localMin = Vector2.zero - cellSize / 2;
                        var localMax = cellSize / 2;

                        var min = pos + localMin;
                        var max = pos + localMax;

                        Gizmos.DrawLine(min, max);
                        Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, min.y));

                    }

                }

            }


        }

        Vector2 Index2GizmosCenter(Vector2Int index) {

            var cell_offset = cellSize / 2;
            var pos = new Vector2(index.x * cellSize.x, index.y * cellSize.y) + cell_offset;
            return pos;

        }

    }

}
