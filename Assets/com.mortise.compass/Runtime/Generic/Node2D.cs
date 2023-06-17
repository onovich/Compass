using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public class Node2D : IComparable<Node2D> {

        int x;
        public int X => x;
        public void SetX(int value) => x = value;

        int y;
        public int Y => y;
        public void SetY(int value) => y = value;

        float f;
        public float F => f;
        public void SetF(float value) => f = value;

        float g;
        public float G => g;
        public void SetG(float value) => g = value;

        float h;
        public float H => h;
        public void SetH(float value) => h = value;

        Node2D parent;
        public Node2D Parent => parent;
        public void SetParent(Node2D value) => parent = value;

        bool walkable;
        public bool Walkable => walkable;
        public void SetWalkable(bool value) => walkable = value;

        int capacity;
        public int Capacity => capacity;
        public void SetCapacity(int value) => capacity = value;

        public int CompareTo(Node2D other) {
            if (other.F > F) {
                return -1;
            } else if (other.F < F) {
                return 1;
            }
            return 0;
        }

        public Node2D(int x, int y, bool walkable) {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
        }

        public void Clear() {
            f = 0;
            g = 0;
            h = 0;
            x = 0;
            y = 0;
            capacity = 0;
            walkable = false;
            parent = null;
        }

        public string GetCoordinates() {
            return $"({X}, {Y})";
        }

    }

}