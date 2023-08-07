using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MortiseFrame.Compass;

namespace MortiseFrame.Compass.Sample {
    public class SATSample : MonoBehaviour {
        public Vector2[] pointsA;
        public Vector2[] pointsB;

        void Update() {
            RenderLine(pointsA, Color.red);
            RenderLine(pointsB, Color.green);

            // 按下空格出发测试
            if(Input.GetKeyDown(KeyCode.Space)){
                Test();
            }
        }

        void RenderLine(Vector2[] vector2s, Color color) {
            for (int i = 0; i < vector2s.Length; i++) {
                if (i + 1 == vector2s.Length) {
                    Debug.DrawLine(vector2s[i], vector2s[0], color);
                } else {
                    Debug.DrawLine(vector2s[i], vector2s[i + 1], color);
                }
            }
        }

        void Test() {

            SATPolygon polygonA = new SATPolygon(pointsA);
            SATPolygon polygonB = new SATPolygon(pointsB);

            // 检测两个多边形是否相交
            bool isIntersecting = polygonA.IsIntersecting(polygonB);

            if (isIntersecting)
                Debug.Log("多边形A和多边形B相交");
            else
                Debug.Log("多边形A和多边形B不相交");
        }
    }
}
