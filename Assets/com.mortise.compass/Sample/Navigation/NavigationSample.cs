using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class NavigationSample : MonoBehaviour {

        // Input
        [Header("输入")] public CompassSampleSO model;

        Compass2D compass;
        public Transform end;
        public Transform agent;

        Map2D map;
        float speed = 10f;
        bool isStop = false;

        List<Node2D> path;

        public float agentSize = 1f;

        void Awake() {

            this.map = GetMap();

            compass = new Compass2D();
            isStop = false;

            var startPos = agent.position;
            var startNode = Pos2Node(startPos);

            var endPos = end.position;
            var endNode = Pos2Node(endPos);
            var agentRealSize = agentSize * model.tm.MPU;
            path = compass.FindPath(map, startNode, endNode, agentRealSize);

        }

        void Update() {

            if (isStop) {
                return;
            }

            if (path == null || path.Count == 0) {
                isStop = true;
                Debug.Log("没有路径");
                return;
            }

            var currentPos = agent.position;
            var nextPos = Node2Pos(path[0]);
            float step = speed * Time.deltaTime;

            var dir = new Vector2(nextPos.x - currentPos.x, nextPos.y - currentPos.y).normalized;
            agent.position = AddVector2ToPos(dir * step, currentPos);

            if (Vector2.Distance(currentPos, nextPos) <= 0.05f) {
                path.RemoveAt(0);
            }

        }

        Map2D GetMap() {
            var map = new Map2D(model.tm.CellCount.x, model.tm.CellCount.y);
            map.Init();
            for (int i = 0; i < model.tm.CellCount.x; i++) {
                for (int j = 0; j < model.tm.CellCount.y; j++) {
                    var node = map.Nodes[i, j];
                    var index = new Vector2Int(i, j);
                    node.SetWalkable(model.tm.GetPassableValue(index));
                    node.SetCapacity(model.tm.GetCapacityValue(index));
                }
            }
            return map;
        }


        Vector3 AddVector2ToPos(Vector2 delta, Vector3 pos) {
            var x = delta.x + pos.x;
            var y = delta.y + pos.y;
            var z = pos.z;
            return new Vector3(x, y, z);
        }

        Vector2 Node2Dir(Node2D a, Node2D b) {

            var dir = new Vector2(b.X - a.X, b.Y - a.Y);
            dir.Normalize();
            return dir;

        }

        Vector2 Node2Pos(Node2D node) {

            var cellSize = model.tm.CellSize;
            var x = node.X / model.tm.MPU + model.tm.LocalOffset.x;
            var y = node.Y / model.tm.MPU + model.tm.LocalOffset.y;
            return new Vector2(x, y);

        }

        Node2D Pos2Node(Vector3 pos) {

            var x = Mathf.FloorToInt((pos.x - model.tm.LocalOffset.x) * model.tm.MPU);
            var y = Mathf.FloorToInt((pos.y - model.tm.LocalOffset.y) * model.tm.MPU);

            var node = map.Nodes[x, y];

            return node;

        }


        void OnDrawGizmos() {

            DrawGrid();

        }

        void DrawGrid() {

            if (end == null || agent == null || model == null || model.tm == null || map == null) {
                return;
            }

            var cellSize = model.tm.CellSize;

            for (int i = 0; i < map.Width; i++) {

                for (int j = 0; j < map.Height; j++) {

                    var walkable = map.Nodes[i, j].Walkable;
                    var capacity = map.Nodes[i, j].Capacity;
                    var index = new Vector2Int(i, j);

                    if (walkable) {

                        Gizmos.color = Color.white;
                        var pos = Index2GizmosCenter(index);
                        Gizmos.DrawWireCube(pos, cellSize);
                        UnityEditor.Handles.Label(pos, capacity.ToString());

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
                        UnityEditor.Handles.Label(pos, capacity.ToString());

                    }


                }

            }


        }

        Vector2 Index2GizmosCenter(Vector2Int index) {

            var cellSize = model.tm.CellSize;
            var stageOffset = model.tm.LocalOffset;

            var cell_offset = cellSize / 2;
            var pos = new Vector2(index.x * cellSize.x, index.y * cellSize.y) + cell_offset + stageOffset;
            return pos;

        }

    }

}
