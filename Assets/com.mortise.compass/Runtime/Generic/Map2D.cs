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

        public Map2D(int width, int height) {

            this.width = width;
            this.height = height;
            nodes = new Node2D[width, height];

        }

        public void Init() {

            for (int i = 0; i < width; i++) {

                for (int j = 0; j < height; j++) {
                    nodes[i, j] = new Node2D(i, j, true);
                }

            }

        }

    }

}

