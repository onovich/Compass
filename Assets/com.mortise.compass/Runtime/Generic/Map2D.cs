using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MortiseFrame.Compass {

    [Serializable]
    public class Map2D {

        int width;
        public int Width => width;

        int height;
        public int Height => height;

        Node2D[,] nodes;
        public Node2D[,] Nodes => nodes;

        public Map2D(int width, int height, Predicate<Vector2Int> predicateWalkable, Func<Vector2Int, int> predicateCapacity) {

            this.width = width;
            this.height = height;
            nodes = new Node2D[width, height];

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    nodes[i, j] = new Node2D(i, j, true);
                    var index = new Vector2Int(i, j);

                    nodes[i, j].SetWalkable(predicateWalkable(index));
                    nodes[i, j].SetCapacity(predicateCapacity(index));

                }
            }

        }

    }

}

