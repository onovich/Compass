using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    public class Node2DPool {

        readonly Stack<Node2D> pool;

        public Node2DPool(int capacity) {
            pool = new Stack<Node2D>(capacity);
        }

        public Node2D GetNode(int x, int y, bool walkable) {
            if (pool.Count > 0) {
                var node = pool.Pop();
                node.Clear();
                node.SetX(x);
                node.SetY(y);
                node.SetWalkable(walkable);
                return node;
            }
            return new Node2D(x, y, walkable);
        }

        public void ReturnNode(Node2D node) {
            node.Clear();
            pool.Push(node);
        }

    }

}