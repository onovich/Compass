using System;
using UnityEngine;


namespace MortiseFrame.Compass {

    [Serializable]
    public class GridTM {

        [SerializeField] bool[] grid;
        public bool[] Grid => grid;
        public void SetGrid(bool[] value) => grid = value;
        public void ClearGrid() => grid = null;

        public int width;
        public int height;

        public float cellWidth;
        public float cellHeight;

        public bool GetGridValue(int x, int y) {
            return grid[x + y * width];
        }

        public void SetGridValue(int x, int y, bool value) {
            grid[x + y * width] = value;
        }

        public Vector2 GetLocalPosition(int x, int y) {
            var localX = x + cellWidth / 2;
            var localY = y + cellHeight / 2;
            return new Vector2(localX, localY);
        }

    }

}