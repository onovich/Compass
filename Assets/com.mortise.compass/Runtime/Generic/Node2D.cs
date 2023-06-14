using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    [Serializable]
    public struct Node2D {

        int x;
        public int X => x;

        int y;
        public int Y => y;

        bool walkable;
        public bool Walkable => walkable;
        public void SetWalkable(bool value) => walkable = value;

        public Node2D(int x, int y, bool walkable) {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
        }

        public void Init(int x, int y, bool walkable) {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
        }

    }

}