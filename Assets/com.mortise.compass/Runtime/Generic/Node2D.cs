using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass {

    [System.Serializable]
    public class Node2D {

        int x;
        public int X => x;

        int y;
        public int Y => y;

        bool walkable;
        public bool Walkable => walkable;

        public Node2D(int x, int y, bool walkable) {
            Init(x, y, walkable);
        }

        public void Init(int x, int y, bool walkable) {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
        }

    }

}