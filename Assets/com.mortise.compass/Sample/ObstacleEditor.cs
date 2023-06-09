using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class ObstacleEditor : MonoBehaviour {
        public Node2D GetNode2D() {
            return new Node2D((int)transform.localPosition.x, (int)transform.localPosition.y, false);
        }
    }

}
