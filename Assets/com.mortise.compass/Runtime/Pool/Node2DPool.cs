using System.Collections.Generic;

namespace MortiseFrame.Compass {

    public class Node2DPool {

        Stack<Node2D> pool;

        public Node2DPool() {
            pool = new Stack<Node2D>();
        }

        public Node2D Get(int x, int y, bool walkable) {
            Node2D node;

            if (pool.Count > 0) {
                node = pool.Pop();
            } else {
                node = new Node2D(x, y, walkable);
            }

            node.Init(x, y, walkable);

            return node;
        }


        public void ReturnNode(Node2D node) {
            pool.Push(node);
        }

        public void ReturnAll() {
            foreach (var node in pool) {
                ReturnNode(node);
            }
        }

    }

}