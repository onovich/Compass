using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class Sample : MonoBehaviour {

        Compass2D compass;
        public Transform end;
        public Transform agent;
        public GridSampleSO model;

        void Awake() {

            compass = new Compass2D(model.tm);

        }

        float currentTime = 0f;
        float interval = 0.5f;
        void ResetCurrentTime() {
            currentTime = 0f;
        }
        void SpendCurrentTime() {
            currentTime += Time.deltaTime;
        }

        void Update() {

            SpendCurrentTime();
            if (currentTime < interval) {
                return;
            }

            var startPos = agent.position;
            var startNode = Pos2Node(startPos);

            var endPos = end.position;
            var endNode = Pos2Node(endPos);

            var path = compass.FindPath(startNode, endNode);

            if (path == null || path.Count == 0) {
                Debug.LogError("No Path");
                return;
            }

            if (Vector2.Distance(startPos, endPos) < 0.1f) {
                Debug.LogError("Arrived");
                return;
            }

            var nextNode = path[0];
            var dir = Node2Dir(startNode, nextNode);
            var move = dir * Time.deltaTime * 5f;
            Debug.Log($"dir = {dir}; move = {move}");
            var targetPos = AddVector2ToPos(move, startPos);
            agent.position = targetPos;

            if (Vector2.Distance(targetPos, Node2Pos(nextNode)) < 0.1f) {
                path.RemoveAt(0);
            }

            ResetCurrentTime();

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

            var cellSize = model.tm.cellSize;
            var x = node.X * cellSize.x + model.tm.stageOffset.x * model.tm.MPU;
            var y = node.Y * cellSize.y + model.tm.stageOffset.y * model.tm.MPU;
            return new Vector2(x, y);

        }

        Node2D Pos2Node(Vector3 pos) {

            var x = Mathf.FloorToInt((pos.x - model.tm.stageOffset.x) * model.tm.MPU);
            var y = Mathf.FloorToInt((pos.y - model.tm.stageOffset.y) * model.tm.MPU);

            Debug.Log($"P2 Pos2Node: node.x = {x}; node.y = {y}; pos.x = {pos.x}; pos.y = {pos.y}; MPUI = {model.tm.MPU}; stageOffset.x = {model.tm.stageOffset.x}; stageOffset.y = {model.tm.stageOffset.y}");

            var walkable = model.tm.GetWalkableValueWithIndex(new Vector2Int(x, y));
            var node = new Node2D(x, y, walkable);

            // Debug.Log($"Pos2Node: node.x = {x}; node.y = {y}; pos.x = {pos.x}; pos.y = {pos.y}; cellSize.x = {cellSize.x}; cellSize.y = {cellSize.y}; stageOffset.x = {model.tm.stageOffset.x}; stageOffset.y = {model.tm.stageOffset.y}");

            return node;

        }
    }

}