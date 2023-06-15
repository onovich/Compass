using System;
using UnityEngine;


namespace MortiseFrame.Compass {

    [Serializable]
    public class GridTM {

        [SerializeField] bool[] walkableValue;
        public bool[] WalkableValue => walkableValue;
        public void SetWalkableValue(bool[] value) => walkableValue = value;
        public void ClearWalkableValue() => walkableValue = null;

        public int countX;
        public int countY;
        public Vector2 cellSize;
        public Vector2 stageOffset;
        public int MPU;

        public bool GetWalkableValueWithIndex(Vector2Int index) {
            var x = index.x;
            var y = index.y;
            var i = x + y * countX;
            if (i >= walkableValue.Length || i < 0) {
                Debug.LogError($"Index out of range: x = {x}; y = {y}; i = {i}; length = {walkableValue.Length}");
            }
            return walkableValue[x + y * countX];
        }

        public void SetWalkableValueWithIndex(Vector2Int index, bool value) {
            var x = index.x;
            var y = index.y;
            var i = x + y * countX;
            if (i >= walkableValue.Length || i < 0) {
                Debug.LogError($"Index out of range: x = {x}; y = {y}; i = {i}");
            }
            walkableValue[i] = value;
        }

        public void Clear() {
            walkableValue = null;
            countX = 0;
            countY = 0;
        }

    }

}