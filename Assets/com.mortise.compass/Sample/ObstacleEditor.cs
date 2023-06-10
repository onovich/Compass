using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class ObstacleEditor : MonoBehaviour {

        public Node2D GetNode() {
            var x = (int)Mathf.Round(transform.localPosition.x);
            var y = (int)Mathf.Round(transform.localPosition.y);
            var node = new Node2D(x, y, false);
            return node;
        }

        void OnDrawGizmos() {

            var color = Color.blue;
            var pos = transform.localPosition;
            // color.a = 1 / 2;
            var size = Vector3.one;
            Gizmos.DrawCube(pos, size);

        }

    }

}
