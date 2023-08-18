using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class NavigationSample : MonoBehaviour {

        // Input
        [Header("输入")] public CompassSampleSO model;

        public NavAgentSample agent_01;
        public NavAgentSample agent_02;
        public NavAgentSample agent_03;

        public float speed = 10f;

        public float agentSize = 1f;

        public Transform shadow_agent;
        public Transform shadow_end;

        Map2D map;
        Compass2D compass;
        bool isInit;

        void Awake() {

            isInit = true;

            InitAgent(agent_01);
            // InitAgent(agent_02);
            // InitAgent(agent_03);

            map = new Map2D(model.tm.CellCount.x, model.tm.CellCount.y, 1000, out var node2DPool, model.tm.GetPassableValue, model.tm.GetCapacityValue);
            compass = new Compass2D(model.tm.MPU, node2DPool, model.tm.LocalOffset, HeuristicType.Euclidean);

            var startPos = agent_01.transform.position;
            var endPos = agent_01.Target.position;
            var agentRealSize = agentSize * model.tm.MPU;
            var startNode = MathUtil.Pos2Node(startPos, model.tm.MPU, model.tm.LocalOffset, map);
            var endNode = MathUtil.Pos2Node(endPos, model.tm.MPU, model.tm.LocalOffset, map);

            Debug.Log($"startNode.X:{startNode.X}, startNode.Y:{startNode.Y};  endNode.X:{endNode.X}, endNode.Y:{endNode.Y}");

        }

        void InitAgent(NavAgentSample agent) {
            var map = new Map2D(model.tm.CellCount.x, model.tm.CellCount.y, 1000, out var node2DPool, model.tm.GetPassableValue, model.tm.GetCapacityValue);
            var compass = new Compass2D(model.tm.MPU, node2DPool, model.tm.LocalOffset, HeuristicType.Euclidean);
            agent.SetCompass(compass);
            agent.SetMap(map);
        }

        void OnReach(NavAgentSample agent) {
            agent.isStop = true;
        }

        void TickAgent(NavAgentSample agent) {

            if (agent.isStop) {
                return;
            }

            var endPos = agent.Target.position;
            agent.ResetPathIndex();

            {
                var startPos = agent.transform.position;
                var agentRealSize = agentSize * model.tm.MPU;
                var path = agent.Compass.FindPath(agent.Map, startPos, endPos, agentRealSize);
                agent.SetPath(path);
            }

            if (agent.Path == null || agent.Path.Count == 0 || agent.CurrentPathIndex >= agent.Path.Count) {
                agent.isStop = true;
                OnReach(agent);
                return;
            }

            var currentPos = agent.transform.position;
            var cellSize = new Vector2(1 / model.tm.MPU, 1 / model.tm.MPU);
            var nextPos = agent.Path[agent.CurrentPathIndex].GetPos(model.tm.MPU, model.tm.LocalOffset, cellSize);
            float step = speed * Time.fixedDeltaTime;

            var currentIndex = MathUtil.Pos2Index(currentPos, model.tm.MPU, model.tm.LocalOffset, agent.Map);
            var nextIndex = MathUtil.Pos2Index(nextPos, model.tm.MPU, model.tm.LocalOffset, agent.Map);

            var dir = new Vector2(nextPos.x - currentPos.x, nextPos.y - currentPos.y).normalized;
            agent.transform.position = AddVector2ToPos(dir * step, currentPos);

        }

        void FixedUpdate() {

            TickAgent(agent_01);
            // TickAgent(agent_02);
            // TickAgent(agent_03);
            TickShadow();

        }

        void TickShadow() {

            var startPos = agent_01.transform.position;
            var endPos = agent_01.Target.position;
            var agentRealSize = agentSize * model.tm.MPU;
            var startNode = MathUtil.Pos2Node(startPos, model.tm.MPU, model.tm.LocalOffset, map);
            var endNode = MathUtil.Pos2Node(endPos, model.tm.MPU, model.tm.LocalOffset, map);
            var cellSize = new Vector2(1 / model.tm.MPU, 1 / model.tm.MPU);

            var start = startNode.GetPos(model.tm.MPU, model.tm.LocalOffset, cellSize);
            var end = endNode.GetPos(model.tm.MPU, model.tm.LocalOffset, cellSize);

            shadow_agent.position = start;
            shadow_end.position = end;

        }

        Vector3 AddVector2ToPos(Vector2 delta, Vector3 pos) {
            var x = delta.x + pos.x;
            var y = delta.y + pos.y;
            var z = pos.z;
            return new Vector3(x, y, z);
        }

        void OnDrawGizmos() {

            if (!isInit) {
                return;
            }

            for (int i = 0; i < model.tm.CellCount.x; i++) {
                for (int j = 0; j < model.tm.CellCount.y; j++) {
                    var cellSize = new Vector2(1 / model.tm.MPU, 1 / model.tm.MPU);
                    var pos = map.Nodes[i, j].GetPos(model.tm.MPU, model.tm.LocalOffset, cellSize);
                    var index = new Vector2Int(i, j);
                    Gizmos.color = Color.green;
                    if (!model.tm.GetPassableValue(index)) {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawWireCube(pos, Vector2.one * model.tm.MPU);
                }
            }

            var path = compass.FindPath(map, agent_01.transform.position, agent_01.Target.position, agentSize * model.tm.MPU);
            for (int i = 1; i < path.Count; i++) {
                var current = path[i];
                var last = path[i - 1];
                Gizmos.DrawLine(current.GetPos(model.tm.MPU, model.tm.LocalOffset, new Vector2(1 / model.tm.MPU, 1 / model.tm.MPU)), last.GetPos(model.tm.MPU, model.tm.LocalOffset, new Vector2(1 / model.tm.MPU, 1 / model.tm.MPU)));
            }

        }

    }

}
