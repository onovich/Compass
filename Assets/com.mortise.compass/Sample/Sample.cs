using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Compass.Sample {

    public class Sample : MonoBehaviour {

        Compass2D compass;
        Transform start;
        Transform end;
        Transform agent;

        void Awake() {

            // compass = new Compass2D();

        }

        void Update() {

            // var startPos = start.position;
            // var endPos = end.position;
            // var path = compass.FindPath(startPos, endPos);

            // if (path == null) {
            //     return;
            // }

            // var nextPos = path[0];
            // var dir = nextPos - startPos;
            // var move = dir.normalized * Time.deltaTime * 5f;
            // agent.position += move;

        }
    }

}