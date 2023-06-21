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

        float speed = 10f;

        public float agentSize = 1f;

        void Awake() {

            InitAgent(agent_01);
            InitAgent(agent_02);
            InitAgent(agent_03);

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

            if (agent.Path == null || agent.Path.Count == 0 || agent.CurrentPathIndex >= agent.Path.Count
                || (agent.LastTargetPos != null && Vector2.Distance(agent.LastTargetPos.Value, endPos) > 0.01f)) {
                var startPos = agent.transform.position;

                var path = agent.Compass.FindPath(agent.Map, startPos, endPos, agentSize);
                agent.SetPath(path);
                agent.SetLastTargetPos(endPos);
            }

            if (agent.Path == null || agent.Path.Count == 0 || agent.CurrentPathIndex >= agent.Path.Count) {
                agent.isStop = true;
                OnReach(agent);
                return;
            }

            var currentPos = agent.transform.position;
            var nextPos = agent.Path[agent.CurrentPathIndex].GetPos(model.tm.MPU, model.tm.LocalOffset);
            // float step = speed * Time.deltaTime;
            float step = speed * Time.fixedDeltaTime / 4;

            var dir = new Vector2(nextPos.x - currentPos.x, nextPos.y - currentPos.y).normalized;
            agent.transform.position = AddVector2ToPos(dir * step, currentPos);

            currentPos = agent.transform.position;
            if (Vector2.Distance(currentPos, nextPos) <= 0.05f) {
                agent.AddCurrentPathIndex();
            }

        }

        void Update() {

            TickAgent(agent_01);
            TickAgent(agent_02);
            TickAgent(agent_03);

        }

        Vector3 AddVector2ToPos(Vector2 delta, Vector3 pos) {
            var x = delta.x + pos.x;
            var y = delta.y + pos.y;
            var z = pos.z;
            return new Vector3(x, y, z);
        }

    }

}
