using System;
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

        LineRenderer lineRenderer;
        Node2DPool node2DPool;

        void Awake() {

            lineRenderer = GetComponent<LineRenderer>();

            this.map = new Map2D(model.tm.CellCount.x, model.tm.CellCount.y, 1000, out node2DPool, model.tm.GetPassableValue, model.tm.GetCapacityValue);
            this.compass = new Compass2D(model.tm.MPU, node2DPool, model.tm.LocalOffset, HeuristicType.Euclidean);

            isStop = false;

            var startPos = agent.position;
            var endPos = end.position;

            if (map == null) {
                Debug.LogError("Map is null");
                return;
            }

            if (compass == null) {
                Debug.LogError("Compass is null");
                return;
            }

            path = compass.FindPath(map, startPos, endPos, agentSize);

        }

        void OnReach() {
            isStop = true;
        }

        void Update() {

            if (isStop) {
                return;
            }

            if (path == null || path.Count == 0) {
                isStop = true;
                OnReach();
                return;
            }

            var currentPos = agent.position;
            var nextPos = path[0].GetPos(model.tm.MPU, model.tm.LocalOffset);
            float step = speed * Time.deltaTime;

            var dir = new Vector2(nextPos.x - currentPos.x, nextPos.y - currentPos.y).normalized;
            agent.position = AddVector2ToPos(dir * step, currentPos);

            if (Vector2.Distance(currentPos, nextPos) <= 0.05f) {
                path.RemoveAt(0);
            }

        }

        Vector3 AddVector2ToPos(Vector2 delta, Vector3 pos) {
            var x = delta.x + pos.x;
            var y = delta.y + pos.y;
            var z = pos.z;
            return new Vector3(x, y, z);
        }

    }

}
